using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace mgcrypt.Rijndael
{
    public class RijndaelDecryptor : Decryptor
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        // Rijndaelプロバイダ
        private RijndaelManaged rijnProvider;


        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        public RijndaelDecryptor(string strTarget)
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
        /// 復号メインメソッド
        /// </summary>
        /// <returns></returns>
        protected override int decryptMain(string strExtention)
        {
            using (ICryptoTransform iDecryptor = rijnProvider.CreateDecryptor())
            {
                using (FileStream inFs = new FileStream(Target, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream outFs = new FileStream(Path.ChangeExtension(Target, strExtention), FileMode.Create, FileAccess.Write))
                    {
                        using (CryptoStream cryptStrm = new CryptoStream(inFs, iDecryptor, CryptoStreamMode.Read))
                        {
                            byte[] bs = new byte[1024];

                            try
                            {
                                int readLen;
                                while ((readLen = cryptStrm.Read(bs, 0, bs.Length)) > 0)
                                {
                                    outFs.Write(bs, 0, readLen);
                                }
                            }
                            catch
                            {
                                return -99;
                            }
                        }
                    }
                }
            }

            return 0;
        }
    }
}
