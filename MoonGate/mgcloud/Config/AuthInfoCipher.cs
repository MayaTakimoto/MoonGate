using System;
using System.Security.Cryptography;
using System.Text;

namespace mgcloud.Config
{
    /// <summary>
    /// 認証情報保存ファイルの暗号化・復号
    /// </summary>
    class AuthInfoCipher
    {
        /// <summary>
        /// RSA暗号化
        /// </summary>
        /// <param name="str"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public char[] EncryptRsa(char[] authInf, string containerName)
        {
            byte[] encryptedData = null;
            try
            {
                string PublicKey = GetPublicKey(containerName);

                var rsaProv = new RSACryptoServiceProvider();
                rsaProv.FromXmlString(PublicKey);

                byte[] data = Encoding.UTF8.GetBytes(authInf);
                encryptedData = rsaProv.Encrypt(data, false);
            }
            catch (CryptographicException)
            {
                return null;
            }

            long arrayLength = (long)((4.0d / 3.0d) * encryptedData.Length);
            if (arrayLength % 4 != 0)
            {
                arrayLength += 4 - arrayLength % 4;
            }

            char[] result = new char[arrayLength];
            Convert.ToBase64CharArray(encryptedData, 0, encryptedData.Length, result, 0);

            return result;
        }


        /// <summary>
        /// RSA復号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public char[] DecryptRsa(char[] str, string containerName)
        { 
            byte[] decryptedData;
            try
            {
                var rsaProv = InitRsa(containerName);

                byte[] data = Convert.FromBase64CharArray(str, 0, str.Length);
                decryptedData = rsaProv.Decrypt(data, false);
            }
            catch (CryptographicException)
            {
                return null;
            }

            return Encoding.UTF8.GetChars(decryptedData);
        }


        /// <summary>
        /// 鍵を処分する
        /// </summary>
        /// <param name="containerName"></param>
        /// <remarks>復号完了後に呼ぶ</remarks>
        public void DeleteKeys(string containerName)
        {
            var rsaProv = InitRsa(containerName);
            rsaProv.PersistKeyInCsp = false;
            rsaProv.Clear();
        }


        /// <summary>
        /// キーペアを作成する
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private RSACryptoServiceProvider InitRsa(string containerName)
        {
            CspParameters cspParam = new CspParameters();
            cspParam.KeyContainerName = containerName;

            return new RSACryptoServiceProvider(cspParam);
        }


        /// <summary>
        /// 公開鍵を返す
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        private string GetPublicKey(string containerName)
        {
            var rsaProv = InitRsa(containerName);

            return rsaProv.ToXmlString(false);
        }
    }
}
