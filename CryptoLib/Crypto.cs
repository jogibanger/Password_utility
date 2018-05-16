using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Collections;

namespace CryptoLib
{
    public static class Crypto
    {
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";


        //public static string EncryptStringUsingRijndael(string plainText)
        /// <summary>
        /// This method uses Rijndael algo to Encrypt the string
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string EncryptString(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        //public static string DecryptStringUsingRijndael(string plainText)
        /// <summary>
        /// This method uses Rijndael algo to Decrypt the string
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
                
        //public static string EncryptStringUsingRSA(string inputString, int dwKeySize, string xmlString)
        /// <summary>
        /// This method uses RSA algo to encrypt the input string
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="dwKeySize"></param>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static string EncryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider =  new RSACryptoServiceProvider( dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString );
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes( inputString );
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for( int i = 0; i <= iterations; i++ )
            {
                byte[] tempBytes = new byte[ 
                        ( dataLength - maxLength * i > maxLength ) ? maxLength : 
                                                      dataLength - maxLength * i ];
                Buffer.BlockCopy( bytes, maxLength * i, tempBytes, 0, 
                                  tempBytes.Length );
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt( tempBytes,
                                                                          true );
                        stringBuilder.Append( Convert.ToBase64String( encryptedBytes ) );
            }
            return stringBuilder.ToString();
        }

        //public static string DecryptStringUsingRSA(string inputString, int dwKeySize, string xmlString)
        /// <summary>
        /// This method for string decryption uses RSA algo
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="dwKeySize"></param>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static string DecryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
              (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                     inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                                    encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(
                                      Type.GetType("System.Byte")) as byte[]);
        }
    }

}
