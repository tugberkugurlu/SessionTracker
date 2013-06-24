using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SessionTracker.Web.MessageHandlers
{
    public class CorrelationHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            response.Headers.Add("X-CorrelationId", request.GetCorrelationId().ToString());

            return response;
        }
    }
}