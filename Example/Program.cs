using DJH.BenefitPipe;
using System;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        public static string TerminalID { get; } = "";
        public static string TranportalId { get; } = "";
        public static string TranportalPassword { get; } = "";
        public static string TermResourceKey { get; } = "";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var baseURL = "https://localhost";
            var account = new AccountConfig(TranportalId, TranportalPassword, TermResourceKey);
            var respUrl = $"{baseURL}/api/benefit/v2/validate";
            var respUrlError = $"{baseURL}/api/benefit/v2/error";

            var paymentRequest = new PaymentRequest("", respUrl, respUrlError, Convert.ToDecimal(12),
                udf1: "123",
                udf3: "",
                udf4: "",
                udf5: "",
                environment:  DJH.BenefitPipe.Environment.Test);

            var benefit = new Payment(account, paymentRequest);
            var respo = await benefit.MakeARequest();

            Console.WriteLine(respo.Success);

        }
    }
}
