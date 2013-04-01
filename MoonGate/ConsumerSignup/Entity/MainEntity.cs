using MoonGate.Component;
using MoonGate.Component.Entity;
using MoonGate.utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace ConsumerSignup.Entity
{
    /// <summary>
    /// 
    /// </summary>
    class MainEntity : INotifyPropertyChanged
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
        private string consumerKey;

        /// <summary>
        /// 
        /// </summary>
        private string consumerSecret;

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
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


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
        /// OKボタン押下時処理
        /// </summary>
        /// <param name="param"></param>
        private void OKProcess(object param)
        {
            if (!Directory.Exists(Path.GetDirectoryName(USERFILE_PATH)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(USERFILE_PATH));
            }

            DataCipher dc = new DataCipher();
            string ck = dc.EncryptRsa(ConsumerKey, param.ToString());
            string cs = dc.EncryptRsa(ConsumerSecret, param.ToString());

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


        /// <summary>
        /// プロパティ変更イベントのコール
        /// </summary>
        /// <param name="strPropName">プロパティ名</param>
        private void OnPropertyChanged(string strPropName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropName));
            }
        }
    }
}
