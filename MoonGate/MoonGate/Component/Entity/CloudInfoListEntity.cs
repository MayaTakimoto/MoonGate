using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// 
    /// </summary>
    class CloudInfoListEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<CloudInfoEntity> ListCloudInfo { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CloudInfoListEntity()
        {
            ListCloudInfo = new ObservableCollection<CloudInfoEntity>();

            CloudInfoEntity c = new CloudInfoEntity();
            c.StorageName = "Google Drive";
            c.StorageKey = "CS01";
            ListCloudInfo.Add(c);
        }
    }
}
