using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DJH.BenefitPipe
{
    public class RestApiHelper
    {
        private AccountConfig Account { get; }
        private Environment Environment { get; }
        private string BaseUrl { get; }
        public RestApiHelper(AccountConfig account, Environment environment)
        {
            Account = account;
            Environment = environment;
            BaseUrl = Environment == Environment.Test ? "https://www.test.benefit-gateway.bh" : "https://www.test.benefit-gateway.bh";
        }

        public async Task<ResponseModel> MakeRequest(string trandata)
        {
            var result = new ResponseModel();
            try
            {
                var request = new RestRequest("payment/API/hosted.htm", Method.POST);
                request.AddJsonBody(new List<dynamic>(){ new
                {
                    id = Account.TranportalId,
                    trandata
                }
                });


                IRestResponse resp = await ExecuteAsync(request);
                if (resp.ErrorException == null)
                {
                    if (resp != null && resp.IsSuccessful && resp.ResponseStatus == ResponseStatus.Completed && !String.IsNullOrEmpty(resp.Content))
                    {
                        var responseList = JsonConvert.DeserializeObject<List<ResponseModel>>(resp.Content);
                        if (responseList != null && responseList.Count > 0)
                        {
                            result = responseList.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    throw resp.ErrorException;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public async Task<IRestResponse> ExecuteAsync(RestRequest request)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(BaseUrl)
            };
            IRestResponse response = await client.ExecuteAsync(request);
            return response;
        }

        public class ResponseModel
        {
            [JsonProperty("errorText")]
            public string ErrorText { get; set; }
            [JsonProperty("error")]
            public string ErrorCode { get; set; }
            [JsonProperty("status")]
            public string ResponseStatus { get; set; }
            [JsonProperty("result")]
            public string PaymentUrl { get; set; }
            public bool Success
            {
                get
                {
                    return ResponseStatus == "1";
                }
            }

            public string GetPaymentID()
            {
                if (String.IsNullOrEmpty(PaymentUrl))
                    return "";

                return PaymentUrl.Split('=').LastOrDefault();
            }
        }
    }
}
