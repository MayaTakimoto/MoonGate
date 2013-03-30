using MoonGate.Component;
using MoonGate.Component.Entity;
using MoonGate.utility;
using System.Collections.Generic;
using System.Windows.Input;

namespace ConsumerSignup.Entity
{
    /// <summary>
    /// 
    /// </summary>
    class MainEntity
    {
        /// <summary>
        /// マスタファイルパス
        /// </summary>
        private const string MASTERFILE_PATH = "./mst/CSFile.xml";

        /// <summary>
        /// 
        /// </summary>
        private const string USERFILE_PATH = "";

        /// <summary>
        /// 
        /// </summary>
        public List<CloudInfoEntity> ListCloudInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsumerSecret { get; set; }

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
                    if (param == null || string.IsNullOrEmpty(ConsumerKey) || string.IsNullOrEmpty(ConsumerSecret))
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
        /// 
        /// </summary>
        /// <param name="param"></param>
        private void OKProcess(object param)
        {
            DataCipher dc = new DataCipher();

            string ck = dc.EncryptRsa(ConsumerKey, param.ToString());
            string cs = dc.EncryptRsa(ConsumerSecret, param.ToString());
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
