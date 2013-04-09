using MoonGate.Component;
using MoonGate.Component.Entity;
using MoonGate.utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Input;

namespace ConsumerSignup.Entity
{
    /// <summary>
    /// ConsumerSignUpのVMクラス
    /// </summary>
    class MainEntity
    {
        /// <summary>
        /// マスタファイルパス
        /// </summary>
        private const string MASTERFILE_PATH = "./mst.xml";

        /// <summary>
        /// 
        /// </summary>
        private const string USERFILE_PATH = "./user/consumer.xml";

        /// <summary>
        /// 
        /// </summary>
        public List<CloudInfoEntity> ListCloudInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public SecureString ConsumerKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SecureString ConsumerSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ResOKCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ResCancelCommand { get; private set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainEntity()
        {
            object list = new List<CloudInfoEntity>();
            if (DataSerializer.TryDeserialize(MASTERFILE_PATH, ref list))
            {
                ListCloudInfo = list as List<CloudInfoEntity>;
            }

            SetCommand();
        }


        /// <summary>
        /// コマンドのセット
        /// </summary>
        private void SetCommand()
        {
            ResOKCommand = new CommandSetter(
                param => this.OKProcess(param),
                param =>
                {
                    if (ConsumerKey == null ||
                        ConsumerKey.Length == 0 ||
                        ConsumerSecret == null ||
                        ConsumerSecret.Length == 0 ||
                        param == null)
                    {
                        return false;
                    }

                    return true;
                }
            );

            ResCancelCommand = new CommandSetter(
                param => this.CancelProcess()
            );
        }


        /// <summary>
        /// OKボタン押下時処理
        /// </summary>
        /// <param name="param"></param>
        private void OKProcess(object param)
        {
            if (!Directory.Exists(Path.GetDirectoryName(USERFILE_PATH)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(USERFILE_PATH));
            }

            char[] cKey, cSec;
            IntPtr ptrKey = IntPtr.Zero;
            IntPtr ptrSec = IntPtr.Zero;
            try
            {
                cKey = new char[ConsumerKey.Length];
                cSec = new char[ConsumerSecret.Length];

                ptrKey = Marshal.SecureStringToCoTaskMemUnicode(ConsumerKey);
                Marshal.Copy(ptrKey, cKey, 0, cKey.Length);

                ptrSec = Marshal.SecureStringToCoTaskMemUnicode(ConsumerSecret);
                Marshal.Copy(ptrSec, cSec, 0, cSec.Length);
            }
            catch
            {
                return;
            }
            finally
            {
                if (ptrKey != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(ptrKey);
                    Marshal.ZeroFreeCoTaskMemUnicode(ptrSec);
                }
            }


            DataCipher dc = new DataCipher();
            char[] ck = dc.EncryptRsa(cKey, param.ToString());
            char[] cs = dc.EncryptRsa(cSec, param.ToString());

            // コンシューマ情報セット
            ConsumerInfoEntity conInfo = new ConsumerInfoEntity();
            conInfo.StorageKey = param.ToString();
            conInfo.ConsumerKey = ck;
            conInfo.ConsumerSecret = cs;

            object objConList = new List<ConsumerInfoEntity>();

            DataSerializer.TryDeserialize(USERFILE_PATH, ref objConList);
            var conList = objConList as List<ConsumerInfoEntity>;

            foreach (var item in conList)
            {
                if (param.ToString() == item.StorageKey)
                {
                    conList.Remove(item);
                    break;
                }
            }

            conList.Add(conInfo);

            DataSerializer.TrySerialize(USERFILE_PATH, conList);

            this.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        private void CancelProcess()
        {
            this.Close();
        }


        /// <summary>
        /// 終了メソッド
        /// </summary>
        private void Close()
        {
            App.Current.Shutdown();
        }
    }
}
