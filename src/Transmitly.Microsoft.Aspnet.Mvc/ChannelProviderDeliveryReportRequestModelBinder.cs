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

using System;
using System.Collections.Generic;
using System.IO;
using Transmitly.Delivery;
using System.Web.Mvc;

namespace Transmitly.Microsoft.Aspnet.Mvc
{
	/// <summary>
	/// Wraps model binding for the registered channel providers.
	/// </summary>
	/// <param name="adaptors"></param>
	internal sealed class ChannelProviderDeliveryReportRequestModelBinder(List<Lazy<IChannelProviderDeliveryReportRequestAdaptor>> adaptors) : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			controllerContext.HttpContext.Request.InputStream.Position = 0;
			var str = new StreamReader(controllerContext.HttpContext.Request.InputStream).ReadToEnd();
			foreach (var adaptor in adaptors)
			{
				var context = new DefaultRequestAdaptorContext(controllerContext.HttpContext.Request.QueryString, str);
				var handled = AsyncHelper.RunSync(() => adaptor.Value.AdaptAsync(context));
				if (handled != null)
				{
					foreach (var result in handled)
					{
						foreach (var kvp in context.QueryString)
							result.ExtendedProperties.AddOrUpdate(kvp.Key, kvp.Value);
					}
					return new ChannelProviderDeliveryReportRequest(handled);
				}
			}
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}
}
