/**************************************************************************************
 *                                                                                    *
 * Program Name : Encryptor.cs                                                        *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System;
using System.IO;
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
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        /*定数*/
        protected const string sType = @".mgen";    // 暗号化ファイルの拡張子
        protected const int KEY_LENGTH = 256;
        protected const int BLOCK_SIZE = 128;

        /*************************************************
         *  プロパティ                                   *
         *************************************************/

        // 暗号化の対象ファイルのプロパティ
        protected string Target { get; set; }

        // ファイルに書き込む復号用情報のプロパティ
        protected byte[] DecInfo { get; set; }

        // キージェネレータのプロパティ
        internal KeyGenerator KeyGen { get; set; }


        /// <summary>
        /// 暗号化プロバイダの呼び出し
        /// </summary>
        /// <param name="iKeyLength">鍵長</param>
        /// <param name="iBlockSize">ブロックサイズ</param>
        /// <param name="btKey">鍵</param>
        /// <param name="btIv">IV</param>
        /// <returns></returns>
        protected abstract int GetProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv);


        /// <summary>
        /// 暗号化を実際に行うメソッド
        /// </summary>
        /// <returns></returns>
        protected abstract int EncryptMain(byte[] decInfo, out byte[] encData);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTarget"></param>
        public void InitEncrypt(string strTarget)
        {
            Target = strTarget;

            // キージェネレータの生成
            KeyGen = new KeyGenerator();
        }


        /// <summary>
        /// 復号用情報の取得
        /// </summary>
        /// <returns></returns>
        private byte[] GetDecInfo()
        {
            // ファイル拡張子の情報
            StringBuilder sbDecInfo = new StringBuilder(16);
            sbDecInfo.Append(Path.GetExtension(Target));

            // 半角スペースで埋める
            while (sbDecInfo.Length < 16)
            {
                sbDecInfo.Append(" ");
            }

            //byte[] decInfo = new byte[32];

            //.CopyTo(decInfo, 0);

            return Encoding.Unicode.GetBytes(sbDecInfo.ToString());
        }


        /// <summary>
        /// 暗号化処理メソッド（パスワード）
        /// </summary>
        /// <param name="sStrPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(SecureString sStrPass, out byte[] encData)
        {
            int iRet = -99;
            encData = null;

            char[] cPassInf = null;
            IntPtr ptrPass = IntPtr.Zero;
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
                sStrPass.Dispose();

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

            return Encrypt(out encData);
        }


        /// <summary>
        /// 暗号化処理メソッド（パスファイル）
        /// </summary>
        /// <param name="fiPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(FileInfo fiPass, out byte[] encData)
        {
            int iRet = -99;
            encData = null;

            iRet = KeyGen.GenerateKey(fiPass);
            if (iRet < 0)
            {
                fiPass = null;
                return iRet;
            }

            fiPass = null;
            return Encrypt(out encData);
        }


        /// <summary>
        /// 暗号化処理メソッド（ハードウェアキー）
        /// </summary>
        /// <param name="sPass"></param>
        /// <param name="encData"></param>
        /// <returns></returns>
        public int Encrypt(string sPass, out byte[] encData)
        {
            int iRet = -99;
            encData = null;

            iRet = KeyGen.GenerateKey(sPass);
            if (iRet < 0)
            {
                sPass = null;
                return iRet;
            }

            sPass = null;
            return Encrypt(out encData);
        }


        /// <summary>
        /// 暗号化処理メソッド（コア）
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">暗号化モード</param>
        /// <returns></returns>
        private int Encrypt(out byte[] encData)
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
            byte[] btDecInfo = GetDecInfo();

            // 暗号化開始
            iRet = EncryptMain(btDecInfo, out encData);
            if (iRet < 0)
            {
                encData = null;
                return iRet;
            }

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            KeyGen.ClearKeyInfo();
            KeyGen = null;
        }
    }
}
