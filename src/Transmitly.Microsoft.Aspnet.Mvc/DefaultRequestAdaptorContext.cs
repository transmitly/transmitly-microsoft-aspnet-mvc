using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Transmitly.Delivery;

namespace Transmitly.Microsoft.Aspnet.Mvc
{
	/// <summary>
	/// Wraps the provided value provider and request body into the expected adaptor context.
	/// </summary>
	/// <param name="valueProvider">Querystring parameters.</param>
	/// <param name="requestBody">Request string body.</param>
	internal sealed class DefaultRequestAdaptorContext(NameValueCollection valueProvider, string requestBody) : IRequestAdaptorContext
	{
		private readonly NameValueCollection _valueProvider = valueProvider;

		public string GetValue(string key)
		{
			return _valueProvider.GetValues(key).FirstOrDefault();
		}

		public string Content { get; } = requestBody;

		public string PipelineName => GetValue(DeliveryUtil.PipelineNameKey);

		public string ResourceId => GetValue(DeliveryUtil.ResourceIdKey);

		public IDictionary<string, string> QueryString => _valueProvider.AllKeys.ToDictionary(k => k, k => _valueProvider[k]);
	}
}
