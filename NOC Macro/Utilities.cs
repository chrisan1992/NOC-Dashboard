using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NOC_Macro
{
    public static class Utilities
    {
        private const string Key = "AOF1J2GN-O4K6-P6MW-L0R2-LE6N2V9G1P5A";

        public static Boolean IsUserLogged()
        {
            if (HttpContext.Current.Session["user_logged"] != null &&
                (Boolean)HttpContext.Current.Session["user_logged"] == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Encrypt(string data, string key = null)
        {
            if (key == null)
                key = Key;
            RijndaelManaged rijndaelCipher = GetRijndaelManaged(key);
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(data);
            var base64String = Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
            return HttpUtility.UrlEncode(base64String);
        }

        public static string Decrypt(string data, string key = null)
        {
            if (key == null)
                key = Key;
            RijndaelManaged rijndaelCipher = GetRijndaelManaged(key);
            byte[] encryptedData = Convert.FromBase64String(data);
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        private static RijndaelManaged GetRijndaelManaged(String key)
        {
            RijndaelManaged objRijndaelManaged = new RijndaelManaged();
            objRijndaelManaged.Mode = CipherMode.CBC; //remember this parameter
            objRijndaelManaged.Padding = PaddingMode.PKCS7; //remember this parameter
            objRijndaelManaged.KeySize = 128;
            objRijndaelManaged.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[0x10];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            objRijndaelManaged.Key = keyBytes;
            objRijndaelManaged.IV = keyBytes;
            return objRijndaelManaged;
        }

        public static DataTable ExecuteOracleQuery(String query)
        {
            DataTable result = null;
            OracleConnection oraConnection2 = null;
            try {
                oraConnection2 = new OracleConnection("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)"
                             + "(HOST=15.224.209.169)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)"
                             + "(SERVICE_NAME=PPMPRD9)));"
                             + "User Id=ITG118PRD;Password=MMS3CA3DEFB;");

                oraConnection2.Open();


                OracleDataAdapter cmd2 = new OracleDataAdapter(query, oraConnection2);

                OracleCommandBuilder commandBuilder2 = new OracleCommandBuilder(cmd2);
                result = new DataTable();
                cmd2.Fill(result);
            }

            catch (Exception ex)
            {
                result = null;
            }
            finally
            {//close the connection for further querys
                oraConnection2.Close();
            }
            return result;
        }
    }
}