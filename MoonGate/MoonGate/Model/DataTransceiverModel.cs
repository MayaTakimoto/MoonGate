using mgcloud;
using mgcloud.CloudOperator;
using mgcrypt;
using mgcrypt.Rijndael;
using MoonGate.Component;
using MoonGate.Component.Message;
using MoonGate.Component.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

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
        /// 
        /// </summary>
        /// <param name="cloudId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal int Upload(object cloudId, InputPassMessage im, List<ListItemViewModel> list)
        {
            int iRes = 0;
            char[] cKey = null;
            char[] cSec = null;
            byte[] encryptedData = null;

            LoadConsumerInfo(cloudId, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return -101;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return -101;
            }

            using (Encryptor encryptor = new RijndaelEncryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    try
                    {
                        // 認証情報のロード
                        oprCld.LoadAuthInfo();

                        foreach (var item in list)
                        {
                            // 暗号化
                            encryptedData = null;
                            switch (im.SelectedIndex)
                            {
                                case 0:
                                    iRes = encryptor.Encrypt(item.FilePath, im.PassWord, out encryptedData);
                                    break;
                                case 1:
                                    iRes = encryptor.Encrypt(item.FilePath, im.PassFile, out encryptedData);
                                    break;
                                case 2:
                                    iRes = encryptor.Encrypt(item.FilePath, im.PassDrive, out encryptedData);
                                    break;
                                default:
                                    break;
                            }

                            if (iRes < 0)
                            {
                                break;
                            }

                            // アップロード
                            iRes = oprCld.UploadFile(item.FilePath, encryptedData);
                            if (iRes < 0)
                            {
                                break;
                            }

                            // アップロード正常終了時はフラグオン
                            item.IsTransceived = true;
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch (Exception e)
                    {
                        string s = e.StackTrace;
                    }
                }
            }

            if (!SaveConsumerInfo(cloudId, cKey, cSec))
            {
                cKey = null;
                cSec = null;

                return -121;
            }

            cKey = null;
            cSec = null;

            return iRes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal int Download(object cloudId, InputPassMessage im, List<ListItemViewModel> list)
        {
            int iRes = 0;
            char[] cKey;
            char[] cSec;
            byte[] downloadData = null;

            LoadConsumerInfo(cloudId, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return -101;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return -101;
            }
            
            using (Decryptor decryptor = new RijndaelDecryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    try
                    {
                        // 認証情報のロード
                        oprCld.LoadAuthInfo();

                        foreach (var item in list)
                        {
                            // ダウンロード
                            downloadData = null;
                            iRes = oprCld.DownloadFile(item.FilePath, out downloadData);
                            if (iRes < 0)
                            {
                                break;
                            }

                            // 復号
                            switch (im.SelectedIndex)
                            {
                                case 0:
                                    iRes = decryptor.Decrypt(item.FileName, im.PassWord, downloadData);
                                    break;
                                case 1:
                                    iRes = decryptor.Decrypt(item.FileName, im.PassFile, downloadData);
                                    break;
                                case 2:
                                    iRes = decryptor.Decrypt(item.FileName, im.PassDrive, downloadData);
                                    break;
                                default:
                                    break;
                            }

                            if (iRes < 0)
                            {
                                break;
                            }

                            item.IsTransceived = true;
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch (Exception e)
                    {
                        string s = e.StackTrace;
                    }
                }
            }

            if (!SaveConsumerInfo(cloudId, cKey, cSec))
            {
                cKey = null;
                cSec = null;

                return -121;
            }

            cKey = null;
            cSec = null;

            return iRes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        internal HybridDictionary GetCloudFileList(object param)
        {
            char[] cKey;
            char[] cSec;
            HybridDictionary hDic = new HybridDictionary(true);

            LoadConsumerInfo(param, out cKey, out cSec);
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

                    if (!SaveConsumerInfo(param, cKey, cSec))
                    {
                        cKey = null;
                        cSec = null;
                    }

                    return null;
                }
            }


            if (!SaveConsumerInfo(param, cKey, cSec))
            {
                cKey = null;
                cSec = null;
            }

            return hDic;
        }


        /// <summary>
        /// コンシューマ情報の読み込み
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cKey"></param>
        /// <param name="cSec"></param>
        private void LoadConsumerInfo(object param, out char[] cKey, out char[] cSec)
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
                if (param.ToString() == item.StorageKey)
                {
                    conInfo = item;
                    listConsumerInfo.Remove(item);
                    break;
                }
            }

            cKey = dcp.DecryptRsa(conInfo.ConsumerKey, param.ToString());
            cSec = dcp.DecryptRsa(conInfo.ConsumerSecret, param.ToString());

            // 秘密鍵の削除
            dcp.DeleteKeys(param.ToString());
        }


        /// <summary>
        /// コンシューマ情報リスト保存
        /// </summary>
        /// <param name="conInfo"></param>
        private bool SaveConsumerInfo(object param, char[] cKey, char[] cSec)
        {
            DataCipherModel dcp = new DataCipherModel();

            ConsumerInfoViewModel conInfo = new ConsumerInfoViewModel();
            conInfo.StorageKey = param.ToString();
            conInfo.ConsumerKey = dcp.EncryptRsa(cKey, param.ToString());
            conInfo.ConsumerSecret = dcp.EncryptRsa(cSec, param.ToString());

            listConsumerInfo.Add(conInfo);

            if (!DataSerializerModel.TrySerialize(USERFILE_PATH, listConsumerInfo))
            {
                return false;
            }

            return true;
        }
    }
}
