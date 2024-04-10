using System;

namespace Transmitly.Microsoft.Aspnet.Mvc
{
	/// <summary>
	/// Represents an error when the <see cref="ChannelProviderModelBinderProvider"/> has already been registered.
	/// </summary>
	public sealed class DuplicateModelBinderRegistrationException : Exception
	{
		internal DuplicateModelBinderRegistrationException() :
			base($"Duplicate {nameof(ChannelProviderModelBinderProvider)} ModelBinders registered. Ensure you've only called {nameof(TransmitlyAspNetMvcExtensions.AddChannelProviderDeliveryReportModelBinders)} once.")
		{

		}
	}
}