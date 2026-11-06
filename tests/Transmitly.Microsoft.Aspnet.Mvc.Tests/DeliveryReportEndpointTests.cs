// Copyright (c) Code Impressions, LLC. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Transmitly.ChannelProvider;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Delivery;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Transmitly.Microsoft.Aspnet.Mvc.Tests
{
	/// <summary>
	/// Runs delivery-report requests through the real model binder and base controller, wired the way MVC wires them.
	/// </summary>
	[TestClass]
	public sealed class DeliveryReportEndpointTests
	{
		private const string Url = "https://example.com:8443/delivery?tlyc=sms&tlycp=test";

		[TestMethod]
		public void AcceptedRequest_Returns200AndDispatchesReports()
		{
			var (status, communications) = Post([Accept()]);

			Assert.AreEqual((int)HttpStatusCode.OK, status);
			communications.Verify(c => c.DispatchAsync(It.Is<IReadOnlyCollection<DeliveryReport>>(r => r.Count == 1)), Times.Once);
		}

		[TestMethod]
		public void FailedValidation_Returns403AndDoesNotDispatch()
		{
			var (status, communications) = Post([Reject()]);

			Assert.AreEqual((int)HttpStatusCode.Forbidden, status);
			communications.Verify(c => c.DispatchAsync(It.IsAny<IReadOnlyCollection<DeliveryReport>>()), Times.Never);
		}

		[TestMethod]
		public void RejectedByOneRegistration_AcceptedByAnother_Returns200()
		{
			// e.g. two accounts of the same provider: the first adaptor's secret doesn't match, the second's does.
			var (status, _) = Post([Reject(), Accept()]);

			Assert.AreEqual((int)HttpStatusCode.OK, status);
		}

		[TestMethod]
		public void NotAddressedToAnyAdaptor_IsNotReportedAsForbidden()
		{
			var (status, _) = Post([new FakeAdaptor(_ => null)]);

			Assert.AreEqual((int)HttpStatusCode.InternalServerError, status);
		}

		[TestMethod]
		public void FormPost_AdaptorSeesBodyFormFieldsAndTheRequestUrl()
		{
			const string body = "MessageSid=SM1&MessageStatus=delivered+now";
			string? content = null, requestUrl = null, formValue = null, queryAsForm = null, formAsQuery = null;
			Post([new FakeAdaptor(ctx =>
			{
				content = ctx.Content;
				requestUrl = ctx.RequestUrl;
				formValue = ctx.GetFormValue("MessageStatus");
				queryAsForm = ctx.GetFormValue(DeliveryUtil.ChannelIdKey);
				formAsQuery = ctx.GetQueryValue("MessageStatus");
				return [Report()];
			})], body, form: HttpUtility.ParseQueryString(body));

			Assert.AreEqual(body, content);
			Assert.AreEqual("delivered now", formValue);
			Assert.IsNull(queryAsForm);
			Assert.IsNull(formAsQuery);
			Assert.AreEqual(Url, requestUrl);
		}

		[TestMethod]
		public void JsonPost_AdaptorSeesRawBodyAndHeaders()
		{
			const string body = "[{\"event\":\"delivered\"}]";
			string? content = null, header = null;
			Post([new FakeAdaptor(ctx =>
			{
				content = ctx.Content;
				header = ctx.GetHeaderValue("X-Test-Signature");
				return [Report()];
			})], body, headers: new NameValueCollection { ["X-Test-Signature"] = "sig" });

			Assert.AreEqual(body, content);
			Assert.AreEqual("sig", header);
		}

		[TestMethod]
		public void PipelineContext_ComesFromQueryOnly()
		{
			// Unsigned headers must not be able to supply pipeline context.
			string? fromQuery = null, fromHeader = "unset";
			Post([new FakeAdaptor(ctx => { fromQuery = ctx.PipelineIntent; return [Report()]; })], url: $"{Url}&{DeliveryUtil.PipelineIntentKey}=order-created");
			Post([new FakeAdaptor(ctx => { fromHeader = ctx.PipelineIntent; return [Report()]; })], headers: new NameValueCollection { [DeliveryUtil.PipelineIntentKey] = "injected" });

			Assert.AreEqual("order-created", fromQuery);
			Assert.IsNull(fromHeader);
		}

		private static (int Status, Mock<ICommunicationsClient> Communications) Post(IChannelProviderDeliveryReportRequestAdaptor[] adaptors, string body = "{}", NameValueCollection? form = null, NameValueCollection? headers = null, string url = Url)
		{
			var uri = new Uri(url);
			// Left at the end, as if something earlier in the pipeline already read the body.
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
			stream.Seek(0, SeekOrigin.End);
			var request = new Mock<HttpRequestBase>();
			request.Setup(r => r.Url).Returns(uri);
			request.Setup(r => r.RawUrl).Returns(uri.PathAndQuery);
			request.Setup(r => r.QueryString).Returns(HttpUtility.ParseQueryString(uri.Query));
			request.Setup(r => r.Form).Returns(form ?? new NameValueCollection());
			request.Setup(r => r.Headers).Returns(headers ?? new NameValueCollection());
			request.Setup(r => r.InputStream).Returns(stream);
			var httpContext = new Mock<HttpContextBase>();
			httpContext.Setup(c => c.Request).Returns(request.Object);

			var communications = new Mock<ICommunicationsClient>();
			var controller = new DeliveryController(communications.Object);
			controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);

			var binders = new ModelBinderProviderCollection();
			binders.AddChannelProviderDeliveryReportModelBinders(new FakeChannelProviderFactory(adaptors));
			var binder = binders.Select(p => p.GetBinder(typeof(ChannelProviderDeliveryReportRequest))).Single(b => b != null);
			// MVC binds action parameters into the controller's own ModelState, so the action sees binder errors.
			var model = (ChannelProviderDeliveryReportRequest)binder.BindModel(controller.ControllerContext, new ModelBindingContext { ModelName = "request", ModelState = controller.ModelState });

			var result = (HttpStatusCodeResult)controller.HandleDeliveryReport(model);
			return (result.StatusCode, communications);
		}

		private static DeliveryReport Report() =>
			new(DeliveryReport.Event.StatusChanged(), "sms", "test", null, null, "id-1", CommunicationsStatus.Success("test", "Delivered"), null, null, null);

		private static FakeAdaptor Accept() => new(_ => [Report()]);

		private static FakeAdaptor Reject() => new(_ => throw new DeliveryReportRequestValidationException("signature is invalid"));

		private sealed class FakeAdaptor(Func<IRequestAdaptorContext, IReadOnlyCollection<DeliveryReport>?> adapt) : IChannelProviderDeliveryReportRequestAdaptor
		{
			public Task<IReadOnlyCollection<DeliveryReport>?> AdaptAsync(IRequestAdaptorContext adaptorContext) => Task.FromResult(adapt(adaptorContext));
		}

		private sealed class FakeChannelProviderFactory(IChannelProviderDeliveryReportRequestAdaptor[] adaptors) : IChannelProviderFactory
		{
			private sealed class Registration(int index) : IDeliveryReportRequestAdaptorRegistration
			{
				public int Index => index;
				public Type Type => typeof(FakeAdaptor);
				public object? Configuration => null;
			}

			public Task<IReadOnlyCollection<IDeliveryReportRequestAdaptorRegistration>> GetAllDeliveryReportRequestAdaptorsAsync() =>
				Task.FromResult<IReadOnlyCollection<IDeliveryReportRequestAdaptorRegistration>>(adaptors.Select((_, i) => new Registration(i)).ToArray());

			public Task<IChannelProviderDeliveryReportRequestAdaptor> ResolveDeliveryReportRequestAdaptorAsync(IDeliveryReportRequestAdaptorRegistration registration) =>
				Task.FromResult(adaptors[((Registration)registration).Index]);

			public Task<IReadOnlyCollection<IChannelProviderRegistration>> GetAllAsync() => throw new NotSupportedException();

			public Task<IChannelProviderDispatcher?> ResolveDispatcherAsync(IChannelProviderRegistration channelProvider, IChannelProviderDispatcherRegistration channelProviderDispatcherRegistration) => throw new NotSupportedException();
		}
	}

	public sealed class DeliveryController(ICommunicationsClient communicationsClient) : ChannelProviderDeliveryReportController(communicationsClient);
}
