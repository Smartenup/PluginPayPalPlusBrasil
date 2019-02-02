using Nop.Plugin.Payments.PayPalPlusBrasil.Models.Message.Response;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Lib
{
    public class Token : APIResource
    {
        public Token(bool sandBox) : base(sandBox)
        {
            BaseURI = "/oauth2/token";
        }

        public async Task<TokenResponse> CreateAsync(string username, string password)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var content = new FormUrlEncodedContent(dictionary);

            var retorno = await PostAsync<TokenResponse>(null, null, null, username, password, content).ConfigureAwait(false);
            return retorno;
        }
    }
}
