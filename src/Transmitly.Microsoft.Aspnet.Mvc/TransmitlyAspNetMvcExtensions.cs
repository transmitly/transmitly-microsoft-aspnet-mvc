using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using System.Web.Mvc;
using Transmitly.Microsoft.Aspnet.Mvc;
using System.Net;
using Transmitly.Delivery;
using System;

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
