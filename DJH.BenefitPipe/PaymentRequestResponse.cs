using System;
using System.Collections.Generic;
using System.Text;

namespace DJH.BenefitPipe
{
    public class PaymentRequestResponse
    {
        public string RedirectLink { get; set; }
        public string PaymentID { get; set; }
        public string TrackID { get; set; }
        public string ErrorText { get; set; }
        public string ErrorCode { get; set; }
        public bool Success { get => RedirectLink != null || RedirectLink != ""; }
    }
}
