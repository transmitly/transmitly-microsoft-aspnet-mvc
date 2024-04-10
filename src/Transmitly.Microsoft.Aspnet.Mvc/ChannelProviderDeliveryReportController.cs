using System.Linq;
using System.Web.Mvc;
using System.Net;
using Transmitly.Delivery;

namespace Transmitly
{
	/// <summary>
	/// Base class for registering a single route for handling incoming channel provider delivery reports.
	/// </summary>
	/// <param name="communicationsClient">Communications client.</param>
	public abstract class ChannelProviderDeliveryReportController(ICommunicationsClient communicationsClient) : Controller
	{
		private readonly ICommunicationsClient _communicationsClient = Guard.AgainstNull(communicationsClient);

		/// <summary>
		/// Handle an incoming channel provider delivery report. Default behavior triggers delivery report handlers.
		/// </summary>
		/// <param name="request">Channel provider delivery report.</param>
		/// <returns>OK; Otherwise BadRequest with errors.</returns>
		[HttpPost]
		public virtual ActionResult HandleDeliveryReport(ChannelProviderDeliveryReportRequest request)
		{
			if (ModelState.IsValid && request != null && request.DeliveryReports != null)
			{
				_communicationsClient.DeliverReports(request.DeliveryReports);
				return new HttpStatusCodeResult(HttpStatusCode.OK);
			}
			return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors)));
		}
	}
}
