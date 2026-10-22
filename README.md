# Transmitly.Microsoft.Aspnet.Mvc

A [Transmitly](https://github.com/transmitly/transmitly) utility package for handling registration and controllers for channel provider delivery reports.

### Getting started

To use install the [NuGet package](https://www.nuget.org/packages/Transmitly.Microsoft.Aspnet.Mvc):

```shell
dotnet add package Transmitly.Microsoft.Aspnet.Mvc
```

Then add the model binders using `AddChannelProviderDeliveryReportModelBinders()`:

```csharp
using Transmitly;
using Transmitly.ChannelProvider.Configuration;
...
protected void Application_Start()
{
	AreaRegistration.RegisterAllAreas();
	GlobalConfiguration.Configure(WebApiConfig.Register);
	FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
	RouteConfig.RegisterRoutes(RouteTable.Routes);
	BundleConfig.RegisterBundles(BundleTable.Bundles);

	ModelBinderProviders.BinderProviders.AddChannelProviderDeliveryReportModelBinders(ServiceLocator.GetService<IChannelProviderFactory>());
}
```
-or-
```csharp
using Transmitly;
using Transmitly.ChannelProvider.Configuration;
...
public ICommunicationsClient CreateCommunicationsClient()
{
	var builder = new CommunicationsClientBuilder();
	...
	builder.AddChannelProviderDeliveryReportModelBinders(ServiceLocator.GetService<IChannelProviderFactory>());
	return builder.BuildClient();
}
```



# Using Default Delivery Report Controller
Inheriting the ChannelProviderDeliveryReportController will setup an `POST` route named, `HandleDeliveryReport` (example: `https://yourapp.com/Communications/HandleDeliveryReport`) that will automatically trigger
your registered delivery report handlers for the provided ICommunicationsClient.

The `HandleDeliveryReport` method can be overridden. Allowing you to customize behaviors and set route specifics.

MyDeliveryReportsController.cs
```csharp 
using System;
using System.Web.Mvc;
using Transmitly;
using Transmitly.Delivery;

namespace Transmitly.Aspnet.Mvc.Examples
{
	[AllowAnonymous]
	public class CommunicationsController : ChannelProviderDeliveryReportController
	{
		public CommunicationsController(ICommunicationsClient communicationsClient) : base(communicationsClient)
		{
		}
	}
}
```

* See the [Transmitly™](https://github.com/transmitly/transmitly) project for more details on how use and configure the library.

---
_Copyright © Code Impressions, LLC.  This open-source project is sponsored and maintained by Code Impressions
and is licensed under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
