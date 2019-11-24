using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace HNProject.Service
{
    class JWTManagement
    {
        // API_KEY = "ec2caad1fd8e420294e9a62794020652"; // dat's baokim account
        // API_SECRET = "b60db64ff91e4e8a98c2be82f04c0847";// dat's baokim account
        protected string API_KEY = "a18ff78e7a9e44f38de372e093d87ca1";// sandbox account
        protected string API_SECRET = "9623ac03057e433f95d86cf4f3bef5cc";// sandbox account


        protected Int32 TOKEN_EXPIRE = 36000;
        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }




        private Object genData()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return new
            {
                iat = unixTimestamp,         // Issued at: time when the token was generated
                jti = Guid.NewGuid(),     // Json Token Id: an unique identifier for the token
                iss = this.API_KEY,     // Issuer
                nbf = unixTimestamp.ToString(),        // Not before
                exp = (unixTimestamp + this.TOKEN_EXPIRE),           // Expire
                form_params = new
                {                  // request body (dữ liệu post)
                                   //'a' => 'value a',
                                   //'b' => 'value b',
                }
            };
        }
        public string getToken()
        {
            var jsonData = new JavaScriptSerializer().Serialize(genData());
            Console.WriteLine(jsonData);
            string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            char[] padding = { '=' };
            token += System.Convert.ToBase64String(plainTextBytes).TrimEnd(padding).Replace('+', '-').Replace('/', '_'); 
            Console.WriteLine(token);
            byte[] byteKey = HashHMAC(Encoding.ASCII.GetBytes(this.API_SECRET), Encoding.ASCII.GetBytes(token));
            string stringKey = System.Convert.ToBase64String(byteKey).TrimEnd(padding).Replace('+', '-').Replace('/', '_'); 
            token += "." + stringKey;
            return token;
        }

        public Boolean verifyPaymentUrl(string id, string mrc_order_id, string txn_id, string total_amount, string stat, string created_at, string updated_at, string checksum)
        {

            NameValueCollection order_params = new NameValueCollection();
            order_params.Add("created_at", created_at);
            order_params.Add("id", id);
            order_params.Add("mrc_order_id", mrc_order_id);
            order_params.Add("stat", stat);
            order_params.Add("total_amount", total_amount);
            order_params.Add("txn_id", txn_id);
            order_params.Add("updated_at", updated_at);

            //2.Sắp xếp các phần tử trong mảng tham số trả về theo key để mã hóa
        
            string s_key = API_SECRET;

            



            //3. Tạo string dữ liệu để ký từ array tham số đã sắp xếp
            // theo định dạng key1=value1&key2=value2&...
            var array = (
                from key in order_params.AllKeys
                 from value in order_params.GetValues(key)
                select string.Format(
                         "{0}={1}",
                    HttpUtility.UrlEncode(key),
                     HttpUtility.UrlEncode(value))
                    
               ).ToArray();
            var str_nvc = string.Join("&", array).Replace("%2b","+").Replace("%25","%").Replace("%3a", "%3A");

            //Mã hóa tạo check sum, so sánh với checksum gửi về từ BaoKim.vn
            
            byte[] byteKey = HashHMAC(Encoding.ASCII.GetBytes(this.API_SECRET), Encoding.ASCII.GetBytes(str_nvc));
            string hex = BitConverter.ToString(byteKey).Replace("-", string.Empty).ToLower();

            if (checksum.CompareTo(hex) == 0)
            {
                return true;
            }

            return false;
        }
        private  byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private string GetMD5Hash(String input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            String md5String = s.ToString();
            return md5String;
        }

    }
}
