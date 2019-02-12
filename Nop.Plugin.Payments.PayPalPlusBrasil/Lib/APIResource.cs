using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalPlusBrasil.Lib
{
    /// <summary>
    /// Inteface básica de um recurso de API
    /// </summary>
    public interface IApiResources : IDisposable
    {
        string BaseURI { get; set; }
        Task<T> GetAsync<T>();
        Task<T> GetAsync<T>(string id);
        Task<T> GetAsync<T>(string id, string apiUserToken);
        Task<T> GetAsync<T>(string id, string partOfUrl, string apiUserToken);

        Task<T> PostAsync<T>(object data);
        Task<T> PostAsync<T>(object data, string partOfUrl);
        Task<T> PostAsync<T>(object data, string partOfUrl, string apiUserToken, string username = null, string password = null, FormUrlEncodedContent encodedContent = null);

        Task<T> PutAsync<T>(string id, object data);

        Task<T> DeleteAsync<T>(string id);
    }

    /// <summary>
    /// Implementação da inteface básica de acesso a recursos básicos da API da IUGU
    /// </summary>
    public class APIResource : IApiResources
    {
        private readonly IHttpClientWrapper client;
        private readonly JsonSerializerSettings JsonSettings;
        //private readonly string _version;
        private readonly string _endpoint;
        private readonly string _apiVersion;
        private string _baseURI;

        public string BaseURI
        {
            get { return _baseURI; }
            set { _baseURI = _endpoint + "/" + _apiVersion + value; }
        }

        /// <summary>
        /// Construtor customizado que permite total controle sobre as configurações do client
        /// </summary>
        public APIResource(bool sandBox, IHttpClientWrapper customClient, JsonSerializerSettings customJsonSerializerSettings = null)
        {
            client = customClient;
            JsonSettings = customJsonSerializerSettings ?? new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            _apiVersion = "v1";
            _endpoint = sandBox ? "https://api.sandbox.paypal.com" : "https://api.paypal.com";
            _baseURI = _endpoint + "/" + _apiVersion;
        }

        /// <summary>
        /// Construtor default que usa as configurações padrão do httpClient e do JsonSerializer
        /// </summary>
        public APIResource(bool sandBox) : this(sandBox, new StandardHttpClient(),
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
        {
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<T> GetAsync<T>()
        {
            var response = await SendRequestAsync(HttpMethod.Get, BaseURI).ConfigureAwait(false);
            return await ProcessResponse<T>(response).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string id)
        {
            var response = await GetAsync<T>(id, null, null).ConfigureAwait(false);
            return response;
        }

        public async Task<T> GetAsync<T>(string id, string apiUserToken)
        {
            var response = await GetAsync<T>(id, null, apiUserToken).ConfigureAwait(false);
            return response;
        }

        public async Task<T> GetAsync<T>(string id, string partOfUrl, string apiUserToken)
        {
            var completeUrl = GetCompleteUrl(partOfUrl, id);
            var response = await SendRequestAsync(HttpMethod.Get, completeUrl, null, apiUserToken).ConfigureAwait(false);
            return await ProcessResponse<T>(response).ConfigureAwait(false);
        }

        public async Task<T> PostAsync<T>(object data)
        {
            var response = await PostAsync<T>(data, null, null).ConfigureAwait(false);
            return response;
        }

        public async Task<T> PostAsync<T>(object data, string partOfUrl)
        {
            var response = await PostAsync<T>(data, partOfUrl, null).ConfigureAwait(false);
            return response;
        }

        public async Task<T> PostAsync<T>(object data, string partOfUrl, string customApiToken = null, string username = null, string password = null, FormUrlEncodedContent encodedContent = null)
        {
            var completeUrl = GetCompleteUrl(partOfUrl, null);
            var response = await SendRequestAsync(HttpMethod.Post, completeUrl, data, customApiToken, username, password, encodedContent).ConfigureAwait(false);
            return await ProcessResponse<T>(response).ConfigureAwait(false);
        }

        public async Task<T> PutAsync<T>(string id, object data)
        {
            return await PutAsync<T>(data, id, null).ConfigureAwait(false);
        }

        public async Task<T> PutAsync<T>(object data, string partOfUrl)
        {
            return await PutAsync<T>(data, partOfUrl, null).ConfigureAwait(false);
        }

        public async Task<T> PutAsync<T>(object data, string partOfUrl, string customApiToken)
        {
            var completeUrl = GetCompleteUrl(partOfUrl, null);
            var response = await SendRequestAsync(HttpMethod.Put, completeUrl, data, customApiToken).ConfigureAwait(false);
            return await ProcessResponse<T>(response).ConfigureAwait(false);
        }
        
        public async Task<T> DeleteAsync<T>(string id)
        {
            return await DeleteAsync<T>(id, null).ConfigureAwait(false);
        }

        public async Task<T> DeleteAsync<T>(string id, string customApiToken)
        {
            var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseURI}/{id}", null, customApiToken).ConfigureAwait(false);
            return await ProcessResponse<T>(response).ConfigureAwait(false);
        }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(JsonConvert.DeserializeObject<T>(data, JsonSettings)).ConfigureAwait(false);
            }

            var errorMessage = await GetCompleteErrorResponseAsync(data, response).ConfigureAwait(false);
            throw new Exception(errorMessage);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string url, object data = null, string customToken = null, string username = null, string password = null, FormUrlEncodedContent encodedContent = null)
        {
            using (var requestMessage = new HttpRequestMessage(method, url))
            {
                SetAutorizationHeader(requestMessage, customToken, username, password);

                await SetContent(data, requestMessage, encodedContent);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var response = await client.SendAsync(requestMessage).ConfigureAwait(false);
                return response;
            }
        }

        private async Task SetContent(object data, HttpRequestMessage requestMessage, FormUrlEncodedContent encodedContent = null)
        {
            if (data != null)
            {
                var content = await Task.FromResult(JsonConvert.SerializeObject(data, JsonSettings)).ConfigureAwait(false);
                requestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            if (encodedContent != null)
            {
                requestMessage.Content = encodedContent;
            }

        }

        private void SetAutorizationHeader(HttpRequestMessage requestMessage, string customToken = null, string username = null, string password = null)
        {
            if ((!string.IsNullOrEmpty(username)) && (!string.IsNullOrEmpty(password)))
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
            
            if ((!string.IsNullOrEmpty(username)) && (string.IsNullOrEmpty(password)))
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(username)));

            if (!string.IsNullOrEmpty(customToken))
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", customToken);
        }

        private string GetCompleteUrl(string partOfUrl, string id)
        {
            var url = string.IsNullOrEmpty(partOfUrl) ? $"{BaseURI}/{id}" : $"{BaseURI}/{partOfUrl}/{id}";

            if (url.Last().Equals('/'))
                url = url.Remove(url.Length - 1);

            return url;
        }

        private static async Task<string> GetCompleteErrorResponseAsync(string data, HttpResponseMessage response)
        {
            try
            {
                var jsonMessage = JsonConvert.DeserializeObject<PayPalPlusComplexErrorResponse>(data);

                return await Task.FromResult(JsonConvert.SerializeObject(new
                {
                    StatusCode = response.StatusCode,
                    ReasonPhase = response.ReasonPhrase,
                    Message = jsonMessage
                })).ConfigureAwait(false);

            }
            catch (Exception)
            {
                try
                {
                    var jsonMessage = JsonConvert.DeserializeObject<PayPalErrorResponse>(data).Errors;
                    return await Task.FromResult(JsonConvert.SerializeObject(new
                    {
                        StatusCode = response.StatusCode,
                        ReasonPhase = response.ReasonPhrase,
                        Message = jsonMessage
                    })).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return string.Empty;
                }

            }
        }


    }

    internal sealed class PayPalPlusComplexErrorResponse
    {
        public Dictionary<string, JArray> Errors { get; set; }
    }


    internal sealed class PayPalErrorResponse
    {
        public string Errors { get; set; }
    }
}
