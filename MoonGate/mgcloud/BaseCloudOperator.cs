//-----------------------------------------------------------------------
// <summary>クラウドストレージとの通信を行う基底クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-01 14:00:00  +9:00 $</date>
// <copyright file="$Name: BaseCloudOperator.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using mgcloud.Config;
using System;
using System.Collections.Specialized;

namespace mgcloud
{
    /// <summary>
    /// クラウドストレージとの通信を担う基底クラス
    /// </summary>
    public abstract class BaseCloudOperator : IDisposable
    {
        /// <summary>
        /// ダウンロード対象ファイルの名前とダウンロードURLの一覧
        /// </summary>
        public HybridDictionary DownloadFileList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string authPath;

        /// <summary>
        /// 初回認証フラグ
        /// </summary>
        internal bool FirstAuthFlg { get; set; }

        /// <summary>
        /// クラウド接続済みフラグ
        /// </summary>
        internal bool ReadyConFlg { get; set; }
                
        /// <summary>
        /// 認証情報保持エンティティ
        /// </summary>
        internal AuthInfoEntity EntAuth { get; set; }

        /// <summary>
        /// コンシューマキー
        /// </summary>
        internal string ConsumerKey { get; set; }

        /// <summary>
        /// コンシューマシークレット
        /// </summary>
        internal string ConsumerSecret { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseCloudOperator(string cKey, string cSec)
        {
            ConsumerKey = cKey;
            ConsumerSecret = cSec;

            FirstAuthFlg = false;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileList"></param>
        ///// <returns></returns>
        //public abstract int UploadFiles(string[] fileList);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract int GetFileList();


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileList"></param>
        ///// <returns></returns>
        //public abstract int DownloadFiles(string[] fileList);


        /// <summary>
        /// クラウドストレージにアップロード
        /// </summary>
        /// <returns></returns>
        public abstract int UploadFile(string filePath, byte[] data);


        /// <summary>
        /// クラウドストレージからダウンロード
        /// </summary>
        /// <returns></returns>
        public abstract int DownloadFile(string downloadUrl, out byte[] data);


        /// <summary>
        /// 認証情報をエンティティに格納
        /// </summary>
        protected abstract void SetAuthInfo();


        /// <summary>
        /// 認証情報の読み込み
        /// </summary>
        /// <param name="authPath"></param>
        public void LoadAuthInfo()
        {
            AuthInfoEntity entAuth = new AuthInfoEntity();

            AuthInfoSerializer aiSerializer = new AuthInfoSerializer(authPath);
            if (!aiSerializer.TryDeserialize(out entAuth))
            {
                FirstAuthFlg = true;
            }

            EntAuth = entAuth;
        }


        /// <summary>
        /// 認証情報の保存
        /// </summary>
        /// <param name="authPath"></param>
        /// <param name="entAuthInfo"></param>
        /// <returns></returns>
        public bool SaveAuthInfo()
        {
            bool bSerialize = true;

            SetAuthInfo();

            AuthInfoSerializer aiSerializer = new AuthInfoSerializer(authPath);
            if (!aiSerializer.TrySerialize(EntAuth))
            {
                bSerialize = false;
            }

            return bSerialize;
        }


        /// <summary>
        /// 後始末メソッド
        /// </summary>
        public virtual void Dispose()
        {
            EntAuth = null;
        }
    }
}
