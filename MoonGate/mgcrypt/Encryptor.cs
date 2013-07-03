//-----------------------------------------------------------------------
// <summary>暗号化処理クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: Encryptor.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace mgcrypt
{
    /// <summary>
    /// すべての暗号化処理クラスの基底となる抽象クラス
    /// </summary>
    public abstract class Encryptor : IDisposable
    {
        #region フィールド
                
        /// <summary>
        /// 鍵長
        /// </summary>
        protected const int KEY_LENGTH = 256;

        /// <summary>
        /// ブロック長
        /// </summary>
        protected const int BLOCK_SIZE = 128;

        /// <summary>
        /// キージェネレータ
        /// </summary>
        internal KeyGenerator KeyGen { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// 暗号化プロバイダの呼び出し
        /// </summary>
        /// <param name="iKeyLength">鍵長</param>
        /// <param name="iBlockSize">ブロックサイズ</param>
        /// <param name="btKey">鍵</param>
        /// <param name="btIv">IV</param>
        /// <returns>0：正常終了 負数：異常終了</returns>
        protected abstract int GetProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv);


        /// <summary>
        /// 暗号化を実際に行うメソッド
        /// </summary>
        /// <param name="extInfo">拡張子のバイト情報</param>
        /// <param name="bt">暗号化データ</param>
        /// <returns>0：正常終了 負数：異常終了</returns>
        protected abstract int EncryptMain(string strTarget, byte[] extInfo, out byte[] bt);


        /// <summary>
        /// 復号用情報の取得
        /// </summary>
        /// <returns></returns>
        private byte[] GetDecInfo(string strTarget)
        {
            // ファイル拡張子の情報
            StringBuilder sbDecInfo = new StringBuilder(16);
            sbDecInfo.Append(Path.GetExtension(strTarget));

            // 半角スペースで埋める
            while (sbDecInfo.Length < 16)
            {
                sbDecInfo.Append(" ");
            }

            return Encoding.UTF8.GetBytes(sbDecInfo.ToString());
        }


        /// <summary>
        /// 暗号化処理メソッド（パスワード）
        /// </summary>
        /// <param name="sStrPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(string strTarget, SecureString sStrPass, out byte[] encData)
        {
            int iRet = -99;
            char[] cPassInf = null;
            IntPtr ptrPass = IntPtr.Zero;
            
            KeyGen = new KeyGenerator();
            encData = null;

            try
            {
                cPassInf = new char[sStrPass.Length];
                ptrPass = Marshal.SecureStringToCoTaskMemUnicode(sStrPass);

                Marshal.Copy(ptrPass, cPassInf, 0, cPassInf.Length);
            }
            catch
            {
                return iRet;
            }
            finally
            {
                if (ptrPass != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(ptrPass);
                }
            }

            iRet = KeyGen.GenerateKey(cPassInf);
            if (iRet < 0)
            {
                return iRet;
            }

            iRet = Encrypt(strTarget, out encData);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 暗号化処理メソッド（パスファイル）
        /// </summary>
        /// <param name="fiPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(string strTarget, FileInfo fiPass, out byte[] encData)
        {
            int iRet = -99;
            
            KeyGen = new KeyGenerator();
            encData = null;

            iRet = KeyGen.GenerateKey(fiPass);
            if (iRet < 0)
            {
                fiPass = null;
                return iRet;
            }

            iRet = Encrypt(strTarget, out encData);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 暗号化処理メソッド（ハードウェアキー）
        /// </summary>
        /// <param name="sPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(string strTarget, string sPass, out byte[] encData)
        {
            int iRet = -99;
            
            KeyGen = new KeyGenerator();
            encData = null;

            iRet = KeyGen.GenerateKey(sPass);
            if (iRet < 0)
            {
                sPass = null;
                return iRet;
            }

            iRet = Encrypt(strTarget, out encData);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 暗号化処理メソッド（コア）
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">暗号化モード</param>
        /// <returns></returns>
        private int Encrypt(string strTarget, out byte[] encData)
        {
            int iRet = -99;
            
            // 暗号化プロバイダの生成
            iRet = GetProvider(KEY_LENGTH, BLOCK_SIZE, KeyGen.SecKey, KeyGen.InitVec);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                encData = null;
                return iRet;
            }

            // 復号用情報取得
            byte[] extInfo = GetDecInfo(strTarget);

            // 暗号化開始
            byte[] bt = null;
            iRet = EncryptMain(strTarget, extInfo, out bt);
            if (iRet < 0)
            {
                encData = null;
                return iRet;
            }

            encData = KeyGen.Salt.Concat(bt).ToArray();

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (KeyGen != null)
            {
                KeyGen.ClearKeyInfo();
                KeyGen = null;
            }
        }

        #endregion
    }
}
