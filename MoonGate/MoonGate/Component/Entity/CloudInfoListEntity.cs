using System;
using System.Collections.Generic;
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
        public List<CloudInfoEntity> ListCloudInfo { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public CloudInfoListEntity()
        {
            ListCloudInfo = new List<CloudInfoEntity>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cldInf"></param>
        public void UpdateList(CloudInfoEntity cldInf)
        {
            ListCloudInfo.Add(cldInf);
        }
    }
}
