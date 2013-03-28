/**************************************************************************************
 *                                                                                    *
 * Program Name : RijndaelEncryptor.cs                                                *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace mgcrypt.Rijndael
{
    /// <summary>
    /// Rijndael暗号化処理クラス
    /// </summary>
    public class RijndaelEncryptor : Encryptor
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        // Rijndaelプロバイダ
        private RijndaelManaged rijnProvider;


        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        public RijndaelEncryptor(string strTarget)
            : base(strTarget) { }


        /*************************************************
         *  メソッド                                     *
         *************************************************/

        /// <summary>
        /// Rijndaelプロバイダ取得
        /// </summary>
        /// <param name="iKeyLength"></param>
        /// <param name="iBlockSize"></param>
        /// <param name="btKey"></param>
        /// <param name="btIv"></param>
        /// <returns></returns>
        protected override int getProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv)
        {
            try
            {
                rijnProvider = new RijndaelManaged();
            }
            catch
            {
                // 呼び出しに失敗したら偽を返す
                return -80;
            }

            rijnProvider.BlockSize = iBlockSize;     // 暗号化ブロックサイズ
            rijnProvider.KeySize = iKeyLength;       // 鍵長
            rijnProvider.Key = btKey;                // 鍵
            rijnProvider.IV = btIv;                  // 初期化ベクトル

            // 正常終了
            return 0;
        }


        /// <summary>
        /// Rijndaelアルゴリズムによる暗号化
        /// </summary>
        /// <returns></returns>
        protected override int encryptMain(byte[] decInfo, out byte[] encData)
        {
            using (ICryptoTransform iEncryptor = rijnProvider.CreateEncryptor())
            {
                using (MemoryStream outMs = new MemoryStream())
                {
                    using (FileStream inFs = new FileStream(Target, FileMode.Open, FileAccess.Read))
                    {
                        using (CryptoStream cryptStrm = new CryptoStream(outMs, iEncryptor, CryptoStreamMode.Write))
                        {
                            byte[] btTmp = new byte[1024];  // 一時バッファ
                            int iReadLength = 0;            // 読込データサイズ
                            //int iPosition = 0;           

                            try
                            {
                                outMs.Write(KeyGen.Salt, 0, KeyGen.Salt.Length);
                                cryptStrm.Position = KeyGen.Salt.Length;

                                cryptStrm.Write(decInfo, 0, decInfo.Length);
                                while ((iReadLength = inFs.Read(btTmp, 0, btTmp.Length)) > 0)
                                {
                                    cryptStrm.Write(btTmp, 0, iReadLength);
                                }
                                
                                encData = outMs.ToArray();
                            }
                            catch
                            {
                                // 例外発生時は偽を返す
                                encData = null;
                                return -90;
                            }
                        }
                    }
                }
            }

            //// 進捗の更新
            //Interlocked.Exchange(ref iProgress, Interlocked.Add(ref iProgress, 1));

            // 正常終了
            return 0;
        }
    }
}
