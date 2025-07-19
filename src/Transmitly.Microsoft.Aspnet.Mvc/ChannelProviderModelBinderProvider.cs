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
using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Delivery;
using System.Web.Mvc;
using Transmitly.Util;

namespace Transmitly.Microsoft.Aspnet.Mvc
{
	/// <summary>
	/// Provides model binding for the <see cref="ChannelProviderDeliveryReportRequest"/> object.
	/// </summary>
	internal sealed class ChannelProviderModelBinderProvider : IModelBinderProvider
	{
		private readonly List<Lazy<IChannelProviderDeliveryReportRequestAdaptor>> _adaptorInstances;

		public ChannelProviderModelBinderProvider(IChannelProviderFactory channelProviderFactory)
		{
			Guard.AgainstNull(channelProviderFactory);
			var adaptors = AsyncHelper.RunSync(channelProviderFactory.GetAllDeliveryReportRequestAdaptorsAsync);
			_adaptorInstances = [.. adaptors.Select(s =>
				new Lazy<IChannelProviderDeliveryReportRequestAdaptor>(
					() => AsyncHelper.RunSync(
						() => channelProviderFactory.ResolveDeliveryReportRequestAdaptorAsync(s)
					)
				)
			)];
		}

		public IModelBinder? GetBinder(Type modelType)
		{
			if (modelType == typeof(ChannelProviderDeliveryReportRequest))
			{
				return new ChannelProviderDeliveryReportRequestModelBinder(_adaptorInstances);
			}
			return null;
		}
	}
}
