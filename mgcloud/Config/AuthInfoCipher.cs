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
        /// <param name="authInf"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static string EncryptRsa(string authInf, string containerName)
        {
            string PublicKey = GetPublicKey(containerName);

            var rsaProv = new RSACryptoServiceProvider();

            rsaProv.FromXmlString(PublicKey);

            byte[] data = Encoding.UTF8.GetBytes(authInf);
            byte[] encryptedData = rsaProv.Encrypt(data, false);

            return Convert.ToBase64String(encryptedData);
        }


        /// <summary>
        /// RSA復号
        /// </summary>
        /// <param name="str"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static string DecryptRsa(string str, string containerName)
        {
            var rsaProv = InitRsa(containerName);

            byte[] data = System.Convert.FromBase64String(str);
            byte[] decryptedData = rsaProv.Decrypt(data, false);

            return Encoding.UTF8.GetString(decryptedData);
        }


        /// <summary>
        /// 鍵を処分する
        /// </summary>
        /// <param name="containerName"></param>
        /// <remarks>復号完了後に呼ぶ</remarks>
        public static void DeleteKeys(string containerName)
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
        private static RSACryptoServiceProvider InitRsa(string containerName)
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
        private static string GetPublicKey(string containerName)
        {
            var rsaProv = InitRsa(containerName);

            return rsaProv.ToXmlString(false);
        }
    }
}
