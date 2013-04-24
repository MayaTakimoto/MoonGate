/**************************************************************************************
 *                                                                                    *
 * Program Name : KeyGenerator.cs                                                     *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace mgcrypt
{
    /// <summary>
    /// 秘密鍵生成クラス
    /// </summary>
    internal class KeyGenerator
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        /*定数*/
        private const string PROPERTY_SERIAL_NO = "VolumeSerialNumber"; // プロパティ名（ボリュームシリアルナンバー）


        /*************************************************
         *  プロパティ                                   *
         *************************************************/

        // Salt
        public byte[] Salt { get; set; }

        // Initialization Vector
        public byte[] InitVec { get; set; }

        // Secret Key
        public byte[] SecKey { get; set; }


        /*************************************************
         *  メソッド                                     *
         *************************************************/

        /// <summary>
        /// Saltを生成する
        /// </summary>
        /// <returns></returns>
        private void GenerateSalt()
        {
            Salt = new byte[16];

            // RNGCryptoServiceProviderを利用してSaltを生成する
            RNGCryptoServiceProvider rngProv = new RNGCryptoServiceProvider();
            rngProv.GetBytes(Salt);
        }

        /// <summary>
        /// 秘密鍵を生成する（パスワード）
        /// </summary>
        /// <returns></returns>
        public int GenerateKey(char[] password)
        {
            // 変数定義
            byte[] pass = null;

            // パスワードがNULLまたはブランクの場合は偽
            if (password == null || password.Length == 0)
            {
                return -1;
            }

            // パスワードが有効な値の場合はバイト配列に変換
            try
            {
                pass = Encoding.Unicode.GetBytes(password);
            }
            catch (EncoderFallbackException)
            {
                // 変換に失敗したら偽を返す
                return -2;
            }

            // 秘密鍵を生成
            return GenerateKey(pass);
        }

        /// <summary>
        /// 秘密鍵を生成する（鍵ファイル）
        /// </summary>
        /// <returns></returns>
        public int GenerateKey(FileInfo fiKey)
        {
            // 変数定義
            byte[] pass = null;

            // ファイルの存在が確認できない場合は偽
            if (!File.Exists(fiKey.FullName))
            {
                return -3;
            }

            // 鍵ファイルを読み込む
            using (FileStream fsKey = fiKey.Open(FileMode.Open))
            {
                // ストリームの開始位置をセット
                fsKey.Position = 0;
                try
                {
                    // SHA-256メッセージダイジェストを生成
                    SHA256 sha256 = SHA256.Create();
                    pass = sha256.ComputeHash(fsKey);
                }
                catch (ObjectDisposedException)
                {
                    // 例外発生時は偽を返す
                    return -4;
                }
            }

            // 秘密鍵を生成
            return GenerateKey(pass);
        }

        /// <summary>
        /// 秘密鍵を生成する（ハードウェアキー）
        /// </summary>
        /// <param name="sKeyDrive"></param>
        /// <returns></returns>
        public int GenerateKey(string sKeyDrive)
        {
            // 変数定義
            byte[] pass = null;

            if (string.IsNullOrEmpty(sKeyDrive) || string.IsNullOrWhiteSpace(sKeyDrive))
            {
                return -5;
            }

            // 取得した鍵ドライブに対してManagementObjectインスタンスを生成
            StringBuilder sbKeyDrv = new StringBuilder(64);
            sbKeyDrv.Append("win32_logicaldisk.deviceid=\"").Append(sKeyDrive.Remove(sKeyDrive.Length - 1)).Append("\"");
            ManagementObject moKeyDrive = new ManagementObject(sbKeyDrv.ToString());

            // 鍵ドライブのボリュームシリアル情報を取得する
            moKeyDrive.Get();
            string sVolumeSerial = moKeyDrive[PROPERTY_SERIAL_NO].ToString();

            if (string.IsNullOrEmpty(sVolumeSerial) || string.IsNullOrWhiteSpace(sVolumeSerial))
            {
                return -6;
            }

            // ボリュームシリアルをバイト配列に変換
            try
            {
                byte[] btVolumeSerial = Encoding.Unicode.GetBytes(sVolumeSerial);

                SHA256 sha256 = SHA256.Create();
                pass = sha256.ComputeHash(btVolumeSerial);

            }
            catch (EncoderFallbackException)
            {
                return -2;
            }
            catch (ObjectDisposedException)
            {
                return -4;
            }
            catch (Exception)
            {
                return -99;
            }

            // 秘密鍵を生成
            return GenerateKey(pass);
        }

        /// <summary>
        /// 秘密鍵を生成する（コア）
        /// </summary>
        /// <param name="btPass">鍵の素</param>
        /// <param name="keySize">鍵長</param>
        /// <param name="blockSize">ブロックサイズ</param>
        /// <returns></returns>
        private int GenerateKey(byte[] btPass)
        {
            try
            {
                // Salt生成
                if (Salt == null || Salt.Length == 0)
                {
                    GenerateSalt();
                }

                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(btPass, Salt, 1024);

                SecKey = deriveBytes.GetBytes(256 / 8);
                InitVec = deriveBytes.GetBytes(128 / 8);
            }
            catch
            {
                // 例外発生時は偽を返す
                return -99;
            }

            // 正常終了時は真を返す
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void ClearKeyInfo()
        {
            Salt = null;
            InitVec = null;
            SecKey = null;
        }
    }
}
