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

using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using System.Web.Mvc;
using Transmitly.Microsoft.Aspnet.Mvc;

namespace Transmitly
{
	/// <summary>
	/// Extension methods related to Aspnet Mvc projects.
	/// </summary>
	public static class TransmitlyAspNetMvcExtensions
	{
		/// <summary>
		/// Adds the transmitly channel provider delivery report model binder to the model binders collection.
		/// </summary>
		/// <param name="modelBinderProviders">Model binding collection.</param>
		/// <param name="channelProviderFactory">Channel provider factory.</param>
		/// <exception cref="DuplicateModelBinderRegistrationException">When the model binder has been registered already.</exception>
		public static void AddChannelProviderDeliveryReportModelBinders(this ModelBinderProviderCollection modelBinderProviders, IChannelProviderFactory channelProviderFactory)
		{
			if (modelBinderProviders.Any(a => a.GetBinder(typeof(ChannelProviderModelBinderProvider)) != null))
				throw new DuplicateModelBinderRegistrationException();
			modelBinderProviders.Add(new ChannelProviderModelBinderProvider(channelProviderFactory));
		}

		/// <summary>
		/// Adds the transmitly channel provider delivery report model binder to the model binders collection.
		/// </summary>
		/// <param name="communicationsClientBuilder"></param>
		/// <param name="channelProviderFactory"></param>
		/// <exception cref="DuplicateModelBinderRegistrationException">When the model binder has been registered already.</exception>
		public static void AddChannelProviderDeliveryReportModelBinders(this CommunicationsClientBuilder communicationsClientBuilder, IChannelProviderFactory channelProviderFactory)
		{
			if (ModelBinderProviders.BinderProviders.Any(a => a.GetBinder(typeof(ChannelProviderModelBinderProvider)) != null))
				throw new DuplicateModelBinderRegistrationException();

			ModelBinderProviders.BinderProviders.Add(new ChannelProviderModelBinderProvider(channelProviderFactory));
		}
	}
}
