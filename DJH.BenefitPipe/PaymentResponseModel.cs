using System;
using System.Collections.Generic;
using System.Text;

namespace DJH.BenefitPipe
{
    public class PaymentResponseModel
    {
        public string authRespCode { get; set; }
        public string trackId { get; set; }
        public long transId { get; set; }
        public string udf5 { get; set; }
        public string udf6 { get; set; }
        public string udf10 { get; set; }
        public string amt { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string result { get; set; }
        public string udf9 { get; set; }
        public long paymentId { get; set; }
        public string udf7 { get; set; }
        public string udf8 { get; set; }

    }
}
