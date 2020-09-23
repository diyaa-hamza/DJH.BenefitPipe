using System;
using System.Collections.Generic;
using System.Text;

namespace DJH.BenefitPipe
{
    public class PaymentRequest
    {
        /// <summary>
        /// Your unique track Id (can't be duplicate)
        /// </summary>
        public string Trackid { get; set; }
        /// <summary>
        /// Respose page in case of success payment
        /// </summary>
        public string ResponseURL { get; set; }
        /// <summary>
        /// Error page in case of failed payment (cancel or error)
        /// </summary>
        public string ErrorURL { get; set; }
        /// <summary>
        /// Your Udf1
        /// </summary>
        public string Udf1 { get; set; }
        /// <summary>
        /// Your Udf2
        /// </summary>
        public string Udf2 { get; set; }
        /// <summary>
        /// Your Udf3
        /// </summary>
        public string Udf3 { get; set; }
        /// <summary>
        /// Your Udf4
        /// </summary>
        public string Udf4 { get; set; }
        /// <summary>
        /// Your Udf5
        /// </summary>
        public string Udf5 { get; set; }
        /// <summary>
        /// The transaction amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Knet page test or live
        /// </summary>
        public Environment Environment { get; set; }

        public PaymentRequest(string trackid, string responseURL, string errorURL, decimal amount,
            string udf1 = "", string udf2 = "", string udf3 = "", string udf4 = "", string udf5 = "",
             Environment environment = Environment.Test)
        {
            Trackid = trackid;
            ResponseURL = responseURL;
            ErrorURL = errorURL;
            Udf1 = udf1;
            Udf2 = udf2;
            Udf3 = udf3;
            Udf4 = udf4;
            Udf5 = udf5;
            Amount = amount;
            Environment = environment;
        }
    }
    public enum Environment
    {
        Test,
        Live
    }
}
