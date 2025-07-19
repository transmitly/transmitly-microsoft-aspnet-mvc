// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
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
		private readonly Dictionary<string, string> _queryString = valueProvider.AllKeys.ToDictionary(k => k, k => valueProvider[k]);

		public string GetValue(string key)
		{
			return _valueProvider.GetValues(key).FirstOrDefault();
		}

		public string? GetQueryValue(string key)
		{
			return GetValue(key);
		}

		public string? GetFormValue(string key)
		{
			return GetValue(key);
		}

		public string? GetHeaderValue(string key)
		{
			return GetValue(key);
		}

		public string Content { get; } = requestBody;


		public string ResourceId => GetValue(DeliveryUtil.ResourceIdKey);

		public IDictionary<string, string> QueryString => _queryString;

		public string? PipelineIntent => GetValue(DeliveryUtil.PipelineIntentKey);

		public string? PipelineId => GetValue(DeliveryUtil.PipelineIdKey);
	}
}
