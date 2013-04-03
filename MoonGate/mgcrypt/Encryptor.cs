/**************************************************************************************
 *                                                                                    *
 * Program Name : Encryptor.cs                                                        *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System;
using System.Collections.Specialized;
using System.IO;
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

        // 進捗
        public int iProgress;


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
        /// 暗号化プロバイダの呼び出し
        /// </summary>
        /// <param name="iKeyLength">鍵長</param>
        /// <param name="iBlockSize">ブロックサイズ</param>
        /// <param name="btKey">鍵</param>
        /// <param name="btIv">IV</param>
        /// <returns></returns>
        protected abstract int getProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv);


        /// <summary>
        /// 暗号化を実際に行うメソッド
        /// </summary>
        /// <returns></returns>
        protected abstract int encryptMain(byte[] decInfo, out byte[] encData);


        /// <summary>
        /// 復号用情報の取得
        /// </summary>
        /// <returns></returns>
        private byte[] getDecInfo()
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
        /// 暗号化処理メソッド
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">暗号化モード</param>
        /// <returns></returns>
        public int encrypt(string sPassInf, int iMode, int iKeyLength, int iBlockSize, out byte[] encData)
        {
            int iRet = -99;
            encData = null;

            // モードで場合分け
            switch (iMode)
            {
                case 0: 
                    char[] cPassword = sPassInf.ToCharArray();

                    iRet = KeyGen.generateKey(cPassword);
                    break;

                case 1: 
                    FileInfo fiPass = new FileInfo(sPassInf);

                    iRet = KeyGen.generateKey(fiPass);
                    break;

                case 2: 
                    iRet = KeyGen.generateKey(sPassInf);
                    break;
            }
            
            if (iRet < 0)
            {
                return iRet;
            }

            // 暗号化プロバイダの生成
            iRet = getProvider(iKeyLength, iBlockSize, KeyGen.SecKey, KeyGen.InitVec);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }

            // 復号用情報取得
            byte[] btDecInfo = getDecInfo();

            // 暗号化開始
            iRet = encryptMain(btDecInfo, out encData);
            if (iRet < 0)
            {
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
