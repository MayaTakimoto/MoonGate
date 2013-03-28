using MoonGate.utility;
using System.Collections.Generic;

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
        private string CSLIST_PATH = "mst/CSList.xml";

        /// <summary>
        /// 
        /// </summary>
        public List<ComboItemEntity> ListCloudInfo { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComboItemListEntity()
        {
            object list = new List<ComboItemEntity>();
            
            if (DataSerializer.TryDeserialize(CSLIST_PATH, ref list))
            {
                ListCloudInfo = list as List<ComboItemEntity>;
            }

            SetList();
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetList()
        {
            ComboItemEntity c = new ComboItemEntity();
            c.Value = "SkyDrive";
            c.Key = "CS02";
            ListCloudInfo.Add(c);
        }
    }
}
