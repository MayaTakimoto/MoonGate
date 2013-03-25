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
    class ComboItemListEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ComboItemEntity> ListCloudInfo { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComboItemListEntity()
        {
            ListCloudInfo = new ObservableCollection<ComboItemEntity>();

            SetList();
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetList()
        {
            ComboItemEntity c = new ComboItemEntity();
            c.Value = "Google Drive";
            c.Key = "CS01";
            ListCloudInfo.Add(c);
        }
    }
}
