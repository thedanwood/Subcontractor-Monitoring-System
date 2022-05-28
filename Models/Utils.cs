using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildQualityChecklist.Models
{
    public static class Utils
    {
        public static string Decrypt(string cipherText)
        {
            string decrypt = "";
            for (int iChar = 0; iChar < cipherText.Length; iChar++)
            {
                decrypt += (char)(cipherText[iChar] - 3);
            }
            return decrypt;
        }

        public static string Encrypt(string plainText)
        {

            string encrypt = "";
            for (int iChar = 0; iChar < plainText.Length; iChar++)
            {
                encrypt += (char)(plainText[iChar] + 3);
            }


            return encrypt;
        }
    }
}