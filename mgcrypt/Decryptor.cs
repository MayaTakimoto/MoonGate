using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mgcrypt
{
    public abstract class Decryptor
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        /*定数*/
        protected const string sType = @".mgen";    // 復号ファイルの拡張子

        // 進捗
        public int iProgress;


        /*************************************************
         *  プロパティ                                   *
         *************************************************/

        // 復号の対象ファイルのリストのプロパティ
        protected string Target { get; set; }

        // キージェネレータのプロパティ
        internal KeyGenerator KeyGen { get; set; }

        
        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        public Decryptor(string strTarget)
        {
            Target = strTarget;

            // キージェネレータの生成
            KeyGen = new KeyGenerator();
        }


        /*************************************************
         *  メソッド                                     *
         *************************************************/

        /// <summary>
        /// 復号プロバイダの呼び出し
        /// </summary>
        /// <param name="iKeyLength">鍵長</param>
        /// <param name="iBlockSize">ブロックサイズ</param>
        /// <param name="btKey">鍵</param>
        /// <param name="btIv">IV</param>
        /// <returns></returns>
        protected abstract int getProvider(int iKeyLength, int iBlockSize, byte[] btKey, byte[] btIv);


        /// <summary>
        /// 復号を実際に行うメソッド
        /// </summary>
        /// <returns></returns>
        protected abstract int decryptMain(string strExtention);


        /// <summary>
        /// 復号ファイルから復号用情報を抜き出す
        /// </summary>
        /// <returns></returns>
        private byte[] getDecInfo()
        {
            byte[] btDecInfo = new byte[40];

            using (FileStream fsSalt = new FileStream(Target, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    if (fsSalt.Read(btDecInfo, 0, btDecInfo.Length) != btDecInfo.Length)
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }

            return btDecInfo;
        }


        /// <summary>
        /// ファイル拡張子取得
        /// </summary>
        /// <param name="btDecInfo"></param>
        /// <returns></returns>
        private string getExtention(byte[] btDecInfo)
        {
            byte[] btExt = new byte[16];
            Buffer.BlockCopy(btDecInfo, 0, btExt, 0, 16);

            string strExtention = Encoding.Unicode.GetString(btExt);

            return strExtention.Trim();
        }


        /// <summary>
        /// Salt取得
        /// </summary>
        /// <param name="btDecInfo"></param>
        /// <returns></returns>
        private byte[] getSalt(byte[] btDecInfo)
        {
            byte[] btSalt = new byte[16];
            Buffer.BlockCopy(btDecInfo, 16, btSalt, 0, 16);

            return btSalt;
        }


        /// <summary>
        /// 復号処理メソッド
        /// </summary>
        /// <param name="sPassInf">秘密鍵の生成に使う情報</param>
        /// <param name="iMode">復号モード</param>
        /// <returns></returns>
        public int decrypt(string sPassInf, int iMode, int iKeyLength, int iBlockSize)
        {
            // メソッドの戻り値
            int iRet = -99;

            // Saltを取得する
            byte[] btDecInfo = getDecInfo();
            if (btDecInfo == null || btDecInfo.Length == 0)
            {
                return iRet;
            }
            else
            {
                KeyGen.Salt = getSalt(btDecInfo);
            }

            // モードで場合分け
            switch (iMode)
            {
                case 0: // パスワードによる復号の場合
                    char[] cPassword = sPassInf.ToCharArray();

                    iRet = KeyGen.generateKey(cPassword);
                    break;

                case 1: // 鍵ファイルによる復号の場合
                    FileInfo fiPass = new FileInfo(sPassInf);

                    iRet = KeyGen.generateKey(fiPass);
                    break;

                case 2: // 鍵ドライブによる復号の場合
                    
                    iRet = KeyGen.generateKey(sPassInf);
                    break;
            }
            
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }

            // 復号プロバイダの生成
            iRet = getProvider(iKeyLength, iBlockSize, KeyGen.SecKey, KeyGen.InitVec);
            if (iRet < 0)
            {
                // 戻り値が負の場合はエラーコードを返す
                return iRet;
            }

            // ファイル拡張子を取得する
            string strExtention = getExtention(btDecInfo);

            // 復号開始
            iRet = decryptMain(strExtention);
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
