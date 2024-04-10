using System;
using System.Collections.Generic;
using System.Linq;
using Transmitly.ChannelProvider.Configuration;
using Transmitly.Delivery;
using System.Web.Mvc;

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
			_adaptorInstances = adaptors.Select(s =>
				new Lazy<IChannelProviderDeliveryReportRequestAdaptor>(
					() => AsyncHelper.RunSync(
						() => channelProviderFactory.ResolveDeliveryReportRequestAdaptorAsync(s)
					)
				)
			).ToList();
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
