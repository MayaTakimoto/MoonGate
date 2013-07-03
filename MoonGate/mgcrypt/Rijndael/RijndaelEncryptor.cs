/**************************************************************************************
 *                                                                                    *
 * Program Name : RijndaelEncryptor.cs                                                *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System;
using System.IO;
using System.Security.Cryptography;

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
        protected override int GetProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv)
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

            rijnProvider.BlockSize = iBlockSize;            // 暗号化ブロックサイズ
            rijnProvider.KeySize = iKeyLength;              // 鍵長
            rijnProvider.Key = btKey;                       // 鍵
            rijnProvider.IV = btIv;                         // 初期化ベクトル
            rijnProvider.Padding = PaddingMode.ANSIX923;    // パディング

            // 正常終了
            return 0;
        }


        /// <summary>
        /// Rijndaelアルゴリズムによる暗号化
        /// </summary>
        /// <param name="extInfo">拡張子のバイト情報</param>
        /// <param name="bt">暗号化データ</param>
        /// <returns>0：正常終了 負数：異常終了</returns>
        protected override int EncryptMain(string strTarget, byte[] extInfo, out byte[] bt)
        {
            using (ICryptoTransform iEncryptor = rijnProvider.CreateEncryptor())
            {
                using (MemoryStream outMs = new MemoryStream())
                {
                    using (FileStream inFs = new FileStream(strTarget, FileMode.Open, FileAccess.Read))
                    {
                        using (CryptoStream cryptStrm = new CryptoStream(outMs, iEncryptor, CryptoStreamMode.Write))
                        {
                            byte[] btTmp = new byte[1024];  // 一時バッファ
                            int iReadLength = 0;            // 読込データサイズ
                            
                            try
                            {
                                while ((iReadLength = inFs.Read(btTmp, 0, btTmp.Length)) > 0)
                                {
                                    cryptStrm.Write(btTmp, 0, iReadLength);
                                }

                                cryptStrm.Write(extInfo, 0, extInfo.Length);
                            }
                            catch(Exception e)
                            {
                                // 例外発生時は偽を返す
                                bt = null;

                                e.StackTrace.ToString();
                                return -90;
                            }
                        }
                    }

                    bt = outMs.ToArray();
                }
            }

            // 正常終了
            return 0;
        }
    }
}
