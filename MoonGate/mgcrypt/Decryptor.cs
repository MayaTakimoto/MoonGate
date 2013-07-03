using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security;
using System.Runtime.InteropServices;

namespace mgcrypt
{
    public abstract class Decryptor : IDisposable
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        /*定数*/
        protected const string sType = @".mgen";    // 復号ファイルの拡張子
        protected const int KEY_LENGTH = 256;
        protected const int BLOCK_SIZE = 128;

        // 進捗
        public int iProgress;


        /*************************************************
         *  プロパティ                                   *
         *************************************************/

        /// <summary>
        /// 復号したファイルの保存先のプロパティ
        /// </summary>
        protected string OutPath { get; private set; }

        /// <summary>
        /// キージェネレータのプロパティ
        /// </summary>
        internal KeyGenerator KeyGen { get; set; }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strOutPath"></param>
        //public void InitDecrypt(string strOutPath)
        //{
        //    OutPath = strOutPath;

        //    // キージェネレータの生成
        //    KeyGen = new KeyGenerator();
        //}


        /// <summary>
        /// 復号プロバイダの呼び出し
        /// </summary>
        /// <param name="iKeyLength">鍵長</param>
        /// <param name="iBlockSize">ブロックサイズ</param>
        /// <param name="btKey">鍵</param>
        /// <param name="btIv">IV</param>
        /// <returns></returns>
        protected abstract int GetProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv);


        /// <summary>
        /// 復号を実際に行うメソッド
        /// </summary>
        /// <returns></returns>
        protected abstract int DecryptMain(byte[] decTarget, out byte[] decResult);


        ///// <summary>
        ///// 復号ファイルから復号用情報を抜き出す
        ///// </summary>
        ///// <returns></returns>
        //private byte[] getDecInfo()
        //{
        //    byte[] btDecInfo = DecTarget.

        //    using (FileStream fsSalt = new FileStream(OutPath, FileMode.Open, FileAccess.Read))
        //    {
        //        try
        //        {
        //            if (fsSalt.Read(btDecInfo, 0, btDecInfo.Length) != btDecInfo.Length)
        //            {
        //                return null;
        //            }
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }

        //    return btDecInfo;
        //}


        ///// <summary>
        ///// ファイル拡張子取得
        ///// </summary>
        ///// <param name="btDecInfo"></param>
        ///// <returns></returns>
        //private string GetExtention(byte[] decResult)
        //{
        //    byte[] btExt = decResult.Skip(decResult.Length - 16).Take(16).ToArray();
        //    //Buffer.BlockCopy(decResult, decResult.Length - 16, btExt, 0, 16);

        //    string strExtention = Encoding.UTF8.GetString(btExt);

        //    return Encoding.UTF8.GetString(btExt).Trim();
        //}


        ///// <summary>
        ///// Salt取得
        ///// </summary>
        ///// <param name="btDecInfo"></param>
        ///// <returns></returns>
        //private byte[] GetSalt(byte[] decTarget)
        //{
        //    byte[] btSalt = decTarget.Take(16).ToArray();

        //    return btSalt;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sStrPass"></param>
        /// <param name="decTarget"></param>
        /// <returns></returns>
        public int Decrypt(string strOutPath, SecureString sStrPass, byte[] decTarget)
        {
            int iRet = -99;
            char[] cPassInf = null;
            IntPtr ptrPass = IntPtr.Zero;

            KeyGen = new KeyGenerator();

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
                //sStrPass.Dispose();

                if (ptrPass != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(ptrPass);
                }
            }

            KeyGen.Salt = decTarget.Take(16).ToArray();
            iRet = KeyGen.GenerateKey(cPassInf);
            if (iRet < 0)
            {
                return iRet;
            }

            iRet = Decrypt(strOutPath, decTarget);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiPass"></param>
        /// <param name="decTarget"></param>
        /// <returns></returns>
        public int Decrypt(string strOutPath, FileInfo fiPass, byte[] decTarget)
        {
            int iRet = -99;

            KeyGen = new KeyGenerator();

            KeyGen.Salt = decTarget.Take(16).ToArray();
            iRet = KeyGen.GenerateKey(fiPass);
            if (iRet < 0)
            {
                fiPass = null;
                return iRet;
            }

            iRet = Decrypt(strOutPath, decTarget);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sPass"></param>
        /// <param name="decTarget"></param>
        /// <returns></returns>
        public int Decrypt(string strOutPath, string sPass, byte[] decTarget)
        {
            int iRet = -99;

            KeyGen = new KeyGenerator();

            KeyGen.Salt = decTarget.Take(16).ToArray();
            iRet = KeyGen.GenerateKey(sPass);
            if (iRet < 0)
            {
                sPass = null;
                return iRet;
            }

            iRet = Decrypt(strOutPath, decTarget);

            KeyGen.ClearKeyInfo();
            KeyGen = null;

            return iRet;
        }


        /// <summary>
        /// 復号処理メソッド（コア）
        /// </summary>
        /// <param name="decTarget">復号対象データ</param>
        /// <returns></returns>
        private int Decrypt(string strOutPath, byte[] decTarget)
        {
            int iRet = -99;

            // 復号プロバイダの生成
            iRet = GetProvider(KEY_LENGTH, BLOCK_SIZE, KeyGen.SecKey, KeyGen.InitVec);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }
            
            // 復号開始
            byte[] decResult = null;
            byte[] decRealTgt = decTarget.Skip(KeyGen.Salt.Length).ToArray();

            iRet = DecryptMain(decRealTgt, out decResult);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }

            byte[] writeData = decResult.Take(decResult.Length - 16).ToArray();
            
            byte[] btExt = decResult.Skip(decResult.Length - 16).ToArray();
            string strExtention = Encoding.UTF8.GetString(btExt);

            File.WriteAllBytes(Path.ChangeExtension(strOutPath, strExtention), writeData);

            // 正常に終了
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
    }
}
