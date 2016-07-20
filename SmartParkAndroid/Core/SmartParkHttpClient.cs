using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;

namespace SmartParkAndroid.Core
{
    public class SmartParkHttpClient
    {
        public async Task Post<TResponse, TRequest>(Uri url, TRequest request = null, Func<TResponse, bool> funcSuccess = null,
            Action funcBefore = null, Func<TResponse, bool> funcError = null)
            where TResponse : SmartJsonResult, new()
            where TRequest : class
        {
            funcBefore?.Invoke();
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                TResponse response;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("HashHeader", StaticManager.UserHash ?? "");

                var requestContent = JsonConvert.SerializeObject(request);
                var postResult = await client.PostAsync(url, new StringContent(requestContent, Encoding.UTF8, "application/json"));
                if (postResult.IsSuccessStatusCode)
                {
                    var json = await postResult.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<TResponse>(json);
                    funcSuccess?.Invoke(response);
                }
                else
                {
                    ErrorResponse(out response, funcError);
                }
            }
        }

        private static void ErrorResponse<TResponse>(out TResponse response, Func<TResponse, bool> funcError)
            where TResponse : SmartJsonResult, new()
        {
            response = new TResponse
            {
                ValidationErrors = new List<string>
                {
                    "Wyst¹pi³ b³¹d podczas ³¹czenia siê z serwerem."
                }
            };
            funcError?.Invoke(response);
        }
    }
}