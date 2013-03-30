using MoonGate.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// 
    /// </summary>
    class MainWindowInfoEntity
    {
        /// <summary>
        /// マスタファイルパス
        /// </summary>
        private const string CSLIST_PATH = "./mst/CSList.xml";

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
        public MainWindowInfoEntity()
        {
            object list = new List<CloudInfoEntity>();
            if (DataSerializer.TryDeserialize(CSLIST_PATH, ref list))
            {
                ListCloudInfo = list as List<CloudInfoEntity>;
            }
        }
    }
}
