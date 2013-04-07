using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MoonGate.utility
{
    /// <summary>
    /// コンシューマ情報のRSA暗号化・復号クラス
    /// </summary>
    public class DataCipher
    {
        /// <summary>
        /// RSA暗号化
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public char[] EncryptRsa(char[] cData, string containerName)
        {
            if (cData == null || cData.Length == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(containerName))
            {
                return null;
            }

            byte[] encryptedData = null;

            // RSA暗号化を行う
            try
            {
                string PublicKey = GetPublicKey(containerName);

                var rsaProv = new RSACryptoServiceProvider();
                rsaProv.FromXmlString(PublicKey);

                byte[] data = Encoding.UTF8.GetBytes(cData);
                encryptedData = rsaProv.Encrypt(data, false);
            }
            catch (CryptographicException)
            {
                return null;
            }

            // Base64エンコード後のデータ長を計算する
            long arrayLength = (long)((4.0d / 3.0d) * encryptedData.Length);
            if (arrayLength % 4 != 0)
            {
                arrayLength += 4 - arrayLength % 4;
            }

            // Base64エンコードを行う
            char[] encChar = new char[arrayLength];
            Convert.ToBase64CharArray(encryptedData, 0, encryptedData.Length, encChar, 0);

            return encChar;
        }


        /// <summary>
        /// RSA復号
        /// </summary>
        /// <param name="cData"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public char[] DecryptRsa(char[] cData, string containerName)
        {
            if (cData == null || cData.Length == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(containerName))
            {
                return null;
            }

            byte[] decryptedData = null;

            // RSA復号を行う
            try
            {
                var rsaProv = InitRsa(containerName);

                byte[] data = Encoding.UTF8.GetBytes(cData);
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
