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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Transmitly.Delivery;

namespace Transmitly.Microsoft.Aspnet.Mvc
{
	/// <summary>
	/// Wraps the provided request and request body into the expected adaptor context.
	/// </summary>
	/// <param name="request">Incoming request.</param>
	/// <param name="requestBody">Request string body.</param>
	internal sealed class DefaultRequestAdaptorContext(HttpRequestBase request, string requestBody) : IRequestAdaptorContext
	{
		private readonly Dictionary<string, string> _queryString = request.QueryString.AllKeys.ToDictionary(k => k, k => request.QueryString[k]);


		public string? GetQueryValue(string key)
		{
			return request.QueryString[key];
		}

		public string? GetFormValue(string key)
		{
			return request.Form[key];
		}

		public string? GetHeaderValue(string key)
		{
			return request.Headers[key];
		}


		[Obsolete("Use GetQueryValue, GetFormValue, or GetHeaderValue instead.", true)]
		string? IRequestAdaptorContext.GetValue(string key)
		{
			throw new NotSupportedException("Use GetQueryValue, GetFormValue, or GetHeaderValue instead.");
		}

		public string Content => requestBody;

		// RawUrl keeps the path and query exactly as received, which URL-signing providers (Twilio) need.
		public string? RequestUrl => request.Url == null ? null : request.Url.GetLeftPart(UriPartial.Authority) + request.RawUrl;


		public string? ResourceId => GetQueryValue(DeliveryUtil.ResourceIdKey);

		public IDictionary<string, string> QueryString => _queryString;

		public string? PipelineIntent => GetQueryValue(DeliveryUtil.PipelineIntentKey);

		public string? PipelineId => GetQueryValue(DeliveryUtil.PipelineIdKey);
	}
}
