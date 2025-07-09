using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorTool.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly UserState _userState; 

        public AuthHeaderHandler(UserState userState) 
        {
            _userState = userState;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Wait for the UserState to be initialized and load data from local storage
            await _userState.InitializationTask;
            if (_userState != null && !_userState.PersonID.HasValue)
            {
                //try load identity data if UserState is not initialized
                await _userState.LoadIdentityDataAsync();
            }

            if (_userState.PersonID.HasValue)
            {
                request.Headers.Add("X-Person-ID", _userState.PersonID.Value.ToString());
            }
            else
            {
                Console.WriteLine("AuthHeaderHandler: UserState.PersonID is null after initialization. Request sent without X-Person-ID header.");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
