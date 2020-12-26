using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MVP.Helpers
{
    /// <summary>
    /// Ensures that a valid token is always sent up to the API service.
    /// </summary>
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        readonly Func<Task<string>> getToken;

        public AuthenticatedHttpClientHandler(Func<Task<string>> getToken)
            => this.getToken = getToken ?? throw new ArgumentNullException(nameof(getToken));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // See if the request has an authorize header
            var auth = request.Headers.Authorization;

            if (auth != null)
            {
                var token = await getToken().ConfigureAwait(false);

                if (token.StartsWith(Constants.AuthType))
                    token = token.Replace($"{Constants.AuthType} ", string.Empty);

                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.AuthType, token);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
