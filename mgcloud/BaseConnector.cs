/**************************************************************************************
 *                                                                                    *
 * Program Name : BaseConnector.cs                                                    *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System.Collections.Generic;
namespace mgcloud
{
    /// <summary>
    /// クラウドストレージとの通信を担う基底クラス
    /// </summary>
    public abstract class BaseConnector
    {
        /// <summary>
        /// 初回認証フラグ
        /// </summary>
        public bool FirstAuthFlg { get; set; }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileList"></param>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //public abstract int OperateFiles(string[] fileList, int mode);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // protected abstract int InitConnection();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public abstract int UploadFiles(string[] fileList);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetFileList();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public abstract int DownloadFiles(string[] fileList);


        /// <summary>
        /// クラウドストレージにアップロード
        /// </summary>
        /// <returns></returns>
        protected abstract int UploadFile(string filePath);


        /// <summary>
        /// クラウドストレージからダウンロード
        /// </summary>
        /// <returns></returns>
        protected abstract int DownloadFile(string filePath);
    }
}
