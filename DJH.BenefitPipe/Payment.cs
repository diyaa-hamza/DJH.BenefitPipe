using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DJH.BenefitPipe
{
    public class Payment
    {
        public Payment(AccountConfig account, PaymentRequest paymentRequest)
        {
            Account = account ?? throw new ArgumentNullException("The benefit account details is required.", nameof(account));
            PaymentRequest = paymentRequest ?? throw new ArgumentNullException("The payment request payload is required.", nameof(paymentRequest));
        }

        public Payment(AccountConfig account)
        {
            Account = account ?? throw new ArgumentNullException("The benefit account details is required.", nameof(account));
        }


        private AccountConfig Account { get; }
        private PaymentRequest PaymentRequest { get; }
        public async Task<PaymentRequestResponse> MakeARequest()
        {
            try
            {
                var generatedLink = new PaymentRequestResponse();
                var trackId = (new Random().Next(10000000) + 1).ToString();

                var requestPayLoadJson = new
                {
                    amt = Math.Round(PaymentRequest.Amount, 3).ToString(),
                    action = "1",
                    password = Account.TranportalPassword,
                    id = Account.TranportalId,
                    currencycode = "048",
                    trackId = trackId,
                    udf1 = PaymentRequest.Udf1,
                    udf2 = PaymentRequest.Udf2,
                    udf3 = PaymentRequest.Udf3,
                    udf4 = PaymentRequest.Udf4,
                    udf5 = PaymentRequest.Udf5,
                    responseURL = PaymentRequest.ResponseURL,
                    errorURL = PaymentRequest.ErrorURL
                };


                var listOfRequests = new List<dynamic>() { requestPayLoadJson };

                var trandata = EncryptAES(JsonConvert.SerializeObject(listOfRequests), Account.TermResourceKey);

                var apiHelper = new RestApiHelper(Account, PaymentRequest.Environment);
                var apiRespo = await apiHelper.MakeRequest(trandata);

                if (apiRespo.Success)
                {
                    generatedLink.RedirectLink = apiRespo.PaymentUrl;
                    generatedLink.PaymentID = apiRespo.GetPaymentID();
                    generatedLink.TrackID = trackId;
                }
                else
                {
                    generatedLink.ErrorCode = apiRespo.ErrorCode;
                    generatedLink.ErrorText = apiRespo.ErrorText;
                }

                return generatedLink;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public PaymentResponseModel DecryptResponse(string trandata)
        {
            try
            {

                var stringJson = Decrypt(trandata);

                if (String.IsNullOrEmpty(stringJson))
                    return null;

                var responseList = JsonConvert.DeserializeObject<List<PaymentResponseModel>>(stringJson);
                if (responseList == null || responseList.Count == 0)
                    return null;

                return responseList.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string EncryptAES(string encryptString, string key)
        {
            var keybytes = Encoding.UTF8.GetBytes(key);
            var iv = Encoding.UTF8.GetBytes("PGKEYENCDECIVSPC");
            string hexString;
            try
            {
                var encrypted = EncryptStringToBytes(encryptString, keybytes, iv);
                hexString = byteArrayToHexString(encrypted);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return hexString.ToUpper();
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }


        public static string byteArrayToHexString(byte[] data)
        {
            return byteArrayToHexString(data, data.Length);
        }
        public static string byteArrayToHexString(byte[] data, int length)
        {
            string HEX_DIGITS = "0123456789abcdef";
            var buf = new StringBuilder();
            for (int i = 0; i != length; i++)
            {
                int v = data[i] & 0xff;
                buf.Append(HEX_DIGITS[v >> 4]);
                buf.Append(HEX_DIGITS[v & 0xf]);
            }

            return buf.ToString();
        }


        /// <summary>
        /// decrypt using AES
        /// </summary>
        /// <param name="cypher"></param>
        /// <returns></returns>
        public string Decrypt(string cypher)
        {
            try
            {
                var key = Account.TermResourceKey;
                var keybytes = Encoding.UTF8.GetBytes(key);
                var iv = Encoding.UTF8.GetBytes("PGKEYENCDECIVSPC");
                var encrypted = StringToByteArray(cypher);
                var back = DecryptStringFromBytes(encrypted, keybytes, iv);
                return back;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.None;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }


    }

}
