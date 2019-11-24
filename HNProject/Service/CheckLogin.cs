using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HNProject.Service
{
    public class CheckLogin
    {
        static bool CheckLoginToken(string token)
        {
            var arr = token.Split('.');
            string A = arr[0];
            string B = arr[1];

            var key = "teamsida";
            //key = BuildHash256.HexDecode(key);

            var BB = BuildHash256.CalcHMACSHA256Hash(A.ToString(), key);
            if(B == BB)
            {
                return true;
            }
            return false;
        }
    }
}