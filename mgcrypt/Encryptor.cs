/**************************************************************************************
 *                                                                                    *
 * Program Name : Encryptor.cs                                                        *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace mgcrypt
{
    /// <summary>
    /// すべての暗号化処理クラスの基底となる抽象クラス
    /// </summary>
    public abstract class Encryptor
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

        
        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        public Encryptor(string strTarget)
        {
            Target = strTarget;

            // キージェネレータの生成
            KeyGen = new KeyGenerator();
        }


        /*************************************************
         *  メソッド                                     *
         *************************************************/

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
        protected abstract int encryptMain(byte[] decInfo);


        /// <summary>
        /// 復号用情報の取得
        /// </summary>
        /// <returns></returns>
        private byte[] getDecInfo()
        {
            // ファイル拡張子の情報
            StringBuilder sbDecInfo = new StringBuilder(8);
            sbDecInfo.Append(Path.GetExtension(Target));

            // 半角スペースで埋める
            while (sbDecInfo.Length < 8)
            {
                sbDecInfo.Append(" ");
            }

            byte[] decInfo = new byte[32];

            Encoding.Unicode.GetBytes(sbDecInfo.ToString()).CopyTo(decInfo, 0);
            KeyGen.Salt.CopyTo(decInfo, 16);

            return decInfo;
        }


        /// <summary>
        /// 暗号化処理メソッド
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">暗号化モード</param>
        /// <returns></returns>
        public int encrypt(string sPassInf, int iMode, int iKeyLength, int iBlockSize)
        {
            // メソッドの戻り値
            int iRet = -99;

            // モードで場合分け
            switch (iMode)
            {
                case 0: // パスワードによる暗号化の場合
                    char[] cPassword = sPassInf.ToCharArray();

                    // 秘密鍵生成メソッド（パスワード用）を呼び出す
                    iRet = KeyGen.generateKey(cPassword);
                    break;

                case 1: // 鍵ファイルによる暗号化の場合
                    FileInfo fiPass = new FileInfo(sPassInf);

                    // 秘密鍵生成メソッド（鍵ファイル用）を呼び出す
                    iRet = KeyGen.generateKey(fiPass);
                    break;

                case 2: // 鍵ドライブによる暗号化の場合
                    // 秘密鍵生成メソッド（鍵ドライブ用）を呼び出す
                    iRet = KeyGen.generateKey(sPassInf);
                    break;
            }
            
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
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
            iRet = encryptMain(btDecInfo);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }

            // 正常に終了
            return 0;
        }
    }
}
