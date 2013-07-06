using mgcloud;
using mgcloud.CloudOperator;
using mgcrypt;
using mgcrypt.Rijndael;
using MoonGate.Component.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security;

namespace MoonGate.Model
{
    class DataTransceiverModel
    {
        /// <summary>
        /// 
        /// </summary>
        private List<ConsumerInfoViewModel> listConsumerInfo;

        /// <summary>
        /// 
        /// </summary>
        private const string USERFILE_PATH = "./user/consumer.xml";

        /// <summary>
        /// パスワード
        /// </summary>
        internal SecureString PassWord { get; set; }

        /// <summary>
        /// パスファイル
        /// </summary>
        internal FileInfo PassFile { get; set; }

        /// <summary>
        /// パスドライブ
        /// </summary>
        internal string PassDrive { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DataTransceiverModel()
        {
            PassWord = null;
            PassFile = null;
            PassDrive = null;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cloudId"></param>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //internal int Upload(object cloudId, List<ListItemViewModel> list)
        //{
        //    int iRes = 0;
        //    char[] cKey = null;
        //    char[] cSec = null;
        //    byte[] encryptedData = null;

        //    LoadConsumerInfo(cloudId, out cKey, out cSec);
        //    if (cKey == null || cKey.Length == 0)
        //    {
        //        return -101;
        //    }
        //    if (cSec == null || cSec.Length == 0)
        //    {
        //        return -101;
        //    }

        //    using (Encryptor encryptor = new RijndaelEncryptor())
        //    {
        //        using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
        //        {
        //            try
        //            {
        //                // 認証情報のロード
        //                oprCld.LoadAuthInfo();

        //                foreach (var item in list)
        //                {
        //                    // 暗号化
        //                    encryptedData = null;

        //                    if (PassWord != null)
        //                    {
        //                        iRes = encryptor.Encrypt(item.FilePath, PassWord, out encryptedData);
        //                    }
        //                    else if (PassFile != null)
        //                    {
        //                        iRes = encryptor.Encrypt(item.FilePath, PassFile, out encryptedData);
        //                    }
        //                    else if (!string.IsNullOrEmpty(PassDrive))
        //                    {
        //                        iRes = encryptor.Encrypt(item.FilePath, PassDrive, out encryptedData);
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }

        //                    if (iRes < 0)
        //                    {
        //                        break;
        //                    }

        //                    // アップロード
        //                    iRes = oprCld.UploadFile(item.FilePath, encryptedData);
        //                    if (iRes < 0)
        //                    {
        //                        break;
        //                    }

        //                    // アップロード正常終了時はフラグオン
        //                    item.IsTransceived = true;
        //                }

        //                // 認証情報のセーブ
        //                oprCld.SaveAuthInfo();
        //            }
        //            catch (Exception e)
        //            {
        //                string s = e.StackTrace;
        //            }
        //        }
        //    }

        //    if (!SaveConsumerInfo(cloudId, cKey, cSec))
        //    {
        //        cKey = null;
        //        cSec = null;

        //        return -121;
        //    }

        //    cKey = null;
        //    cSec = null;

        //    return iRes;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="oprCld"></param>
        /// <param name="item"></param>
        internal int Upload(Encryptor encryptor, BaseCloudOperator oprCld, ListItemViewModel item)
        {
            int iRes = -1;
            byte[] cryptionData = null;

            if (PassWord != null)
            {
                iRes = encryptor.Encrypt(item.FilePath, PassWord, out cryptionData);
            }
            else if (PassFile != null)
            {
                iRes = encryptor.Encrypt(item.FilePath, PassFile, out cryptionData);
            }
            else if (!string.IsNullOrEmpty(PassDrive))
            {
                iRes = encryptor.Encrypt(item.FilePath, PassDrive, out cryptionData);
            }
            else
            {
                return iRes;
            }

            if (iRes < 0)
            {
                return iRes;
            }

            // アップロード
            iRes = oprCld.UploadFile(item.FilePath, cryptionData);
            if (iRes < 0)
            {
                return iRes;
            }

            return iRes;
        }

        //// <summary>
         
        //// </summary>
        //// <param name="cloudId"></param>
        //// <param name="listItem"></param>
        //// <returns></returns>
        ////internal int Upload(object cloudId, ListItemViewModel listItem)
        ////{
        ////    int iRes = 0;
        ////    char[] cKey = null;
        ////    char[] cSec = null;
        ////    byte[] encryptedData = null;

        ////    LoadConsumerInfo(cloudId, out cKey, out cSec);
        ////    if (cKey == null || cKey.Length == 0)
        ////    {
        ////        return -101;
        ////    }
        ////    if (cSec == null || cSec.Length == 0)
        ////    {
        ////        return -101;
        ////    }

        ////    using (Encryptor encryptor = new RijndaelEncryptor())
        ////    {
        ////        using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
        ////        {
        ////            try
        ////            {
        ////                 認証情報のロード
        ////                oprCld.LoadAuthInfo();

        ////                 暗号化
        ////                encryptedData = null;

        ////                if (PassWord != null)
        ////                {
        ////                    iRes = encryptor.Encrypt(listItem.FilePath, PassWord, out encryptedData);
        ////                }
        ////                else if (PassFile != null)
        ////                {
        ////                    iRes = encryptor.Encrypt(listItem.FilePath, PassFile, out encryptedData);
        ////                }
        ////                else if (!string.IsNullOrEmpty(PassDrive))
        ////                {
        ////                    iRes = encryptor.Encrypt(listItem.FilePath, PassDrive, out encryptedData);
        ////                }
        ////                else
        ////                {
        ////                    return -111;
        ////                }

        ////                if (iRes < 0)
        ////                {
        ////                    return iRes;
        ////                }

        ////                 アップロード
        ////                iRes = oprCld.UploadFile(listItem.FilePath, encryptedData);
        ////                if (iRes < 0)
        ////                {
        ////                    return iRes;
        ////                }

        ////                 アップロード正常終了時はフラグオン
        ////                listItem.IsTransceived = true;

        ////                 認証情報のセーブ
        ////                oprCld.SaveAuthInfo();
        ////            }
        ////            catch (Exception e)
        ////            {
        ////                string s = e.StackTrace;
        ////            }
        ////        }
        ////    }

        ////    if (!SaveConsumerInfo(cloudId, cKey, cSec))
        ////    {
        ////        cKey = null;
        ////        cSec = null;

        ////        return -121;
        ////    }

        ////    cKey = null;
        ////    cSec = null;

        ////    return iRes;
        ////}


        //// <summary>
         
        //// </summary>
        //// <param name="cloudId"></param>
        //// <param name="list"></param>
        //// <returns></returns>
        ////internal int Download(object cloudId, List<ListItemViewModel> list)
        ////{
        ////    int iRes = 0;
        ////    char[] cKey;
        ////    char[] cSec;
        ////    byte[] downloadData = null;

        ////    LoadConsumerInfo(cloudId, out cKey, out cSec);
        ////    if (cKey == null || cKey.Length == 0)
        ////    {
        ////        return -101;
        ////    }
        ////    if (cSec == null || cSec.Length == 0)
        ////    {
        ////        return -101;
        ////    }
            
        ////    using (Decryptor decryptor = new RijndaelDecryptor())
        ////    {
        ////        using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
        ////        {
        ////            try
        ////            {
        ////                 認証情報のロード
        ////                oprCld.LoadAuthInfo();

        ////                foreach (var item in list)
        ////                {
        ////                     ダウンロード
        ////                    downloadData = null;
        ////                    iRes = oprCld.DownloadFile(item.FilePath, out downloadData);
        ////                    if (iRes < 0)
        ////                    {
        ////                        break;
        ////                    }

                            
        ////                    if (PassWord != null)
        ////                    {
        ////                        iRes = decryptor.Decrypt(item.FileName, PassWord, downloadData);
        ////                    }
        ////                    else if (PassFile != null)
        ////                    {
        ////                        iRes = decryptor.Decrypt(item.FileName, PassFile, downloadData);
        ////                    }
        ////                    else if (!string.IsNullOrEmpty(PassDrive))
        ////                    {
        ////                        iRes = decryptor.Decrypt(item.FileName, PassDrive, downloadData);
        ////                    }
        ////                    else
        ////                    {
        ////                        break;
        ////                    }

        ////                    if (iRes < 0)
        ////                    {
        ////                        break;
        ////                    }

        ////                    item.IsTransceived = true;
        ////                }

        ////                 認証情報のセーブ
        ////                oprCld.SaveAuthInfo();
        ////            }
        ////            catch (Exception e)
        ////            {
        ////                string s = e.StackTrace;
        ////            }
        ////        }
        ////    }

        ////    if (!SaveConsumerInfo(cloudId, cKey, cSec))
        ////    {
        ////        cKey = null;
        ////        cSec = null;

        ////        return -121;
        ////    }

        ////    cKey = null;
        ////    cSec = null;

        ////    return iRes;
        ////}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="decryptor"></param>
        /// <param name="oprCld"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal int Download(Decryptor decryptor, BaseCloudOperator oprCld, ListItemViewModel item)
        {
            // ダウンロード
            int iRes = -1;
            byte[] downloadData = null;

            iRes = oprCld.DownloadFile(item.FilePath, out downloadData);
            if (iRes < 0)
            {
                return iRes;
            }

            // 復号
            if (PassWord != null)
            {
                iRes = decryptor.Decrypt(item.FileName, PassWord, downloadData);
            }
            else if (PassFile != null)
            {
                iRes = decryptor.Decrypt(item.FileName, PassFile, downloadData);
            }
            else if (!string.IsNullOrEmpty(PassDrive))
            {
                iRes = decryptor.Decrypt(item.FileName, PassDrive, downloadData);
            }
            else
            {
                return iRes;
            }

            if (iRes < 0)
            {
                return iRes;
            }

            return iRes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudId"></param>
        /// <returns></returns>
        internal HybridDictionary GetCloudFileList(object cloudId)
        {
            char[] cKey;
            char[] cSec;
            HybridDictionary hDic = new HybridDictionary(true);

            LoadConsumerInfo(cloudId, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return null;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return null;
            }

            using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
            {
                try
                {
                    oprCld.LoadAuthInfo();
                    hDic = oprCld.GetFileList();
                    oprCld.SaveAuthInfo();
                }
                catch (Exception e)
                {
                    string s = e.StackTrace;

                    oprCld.SaveAuthInfo();

                    if (!SaveConsumerInfo(cloudId, cKey, cSec))
                    {
                        cKey = null;
                        cSec = null;
                    }

                    return null;
                }
            }


            if (!SaveConsumerInfo(cloudId, cKey, cSec))
            {
                cKey = null;
                cSec = null;
            }

            return hDic;
        }


        /// <summary>
        /// コンシューマ情報の読み込み
        /// </summary>
        /// <param name="cloudId"></param>
        /// <param name="cKey"></param>
        /// <param name="cSec"></param>
        internal void LoadConsumerInfo(object cloudId, out char[] cKey, out char[] cSec)
        {
            cKey = null;
            cSec = null;
            DataCipherModel dcp = new DataCipherModel();

            object objConsumerInfo = new List<ConsumerInfoViewModel>();
            if (!DataSerializerModel.TryDeserialize(USERFILE_PATH, ref objConsumerInfo))
            {
                return;
            }

            listConsumerInfo = objConsumerInfo as List<ConsumerInfoViewModel>;
            ConsumerInfoViewModel conInfo = new ConsumerInfoViewModel();
            foreach (var item in listConsumerInfo)
            {
                if (cloudId.ToString() == item.StorageKey)
                {
                    conInfo = item;
                    listConsumerInfo.Remove(item);
                    break;
                }
            }

            cKey = dcp.DecryptRsa(conInfo.ConsumerKey, cloudId.ToString());
            cSec = dcp.DecryptRsa(conInfo.ConsumerSecret, cloudId.ToString());

            // 秘密鍵の削除
            dcp.DeleteKeys(cloudId.ToString());
        }


        /// <summary>
        /// コンシューマ情報リスト保存
        /// </summary>
        /// <param name="conInfo"></param>
        internal bool SaveConsumerInfo(object cloudId, char[] cKey, char[] cSec)
        {
            DataCipherModel dcp = new DataCipherModel();

            ConsumerInfoViewModel conInfo = new ConsumerInfoViewModel();
            conInfo.StorageKey = cloudId.ToString();
            conInfo.ConsumerKey = dcp.EncryptRsa(cKey, cloudId.ToString());
            conInfo.ConsumerSecret = dcp.EncryptRsa(cSec, cloudId.ToString());

            listConsumerInfo.Add(conInfo);

            if (!DataSerializerModel.TrySerialize(USERFILE_PATH, listConsumerInfo))
            {
                return false;
            }

            return true;
        }
    }
}
