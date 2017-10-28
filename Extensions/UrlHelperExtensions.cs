using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvcIpsa.Controllers;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string getPasswordHashed(string password) {
            string hashPassword = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hashPassword = GetMd5Hash(md5Hash, password);
            }
            return hashPassword;
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {         
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
         
            StringBuilder sBuilder = new StringBuilder();
           
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }          
            return sBuilder.ToString();
        }

        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
           
            string hashOfInput = GetMd5Hash(md5Hash, input);
            
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))            
                return true;
            
            else            
                return false;

            
        }
    }
}
