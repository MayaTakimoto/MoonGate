using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// 
    /// </summary>
    class ConsumerInfoListEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ConsumerInfoEntity> ListCloudInfo { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public ConsumerInfoListEntity()
        {
            ListCloudInfo = new List<ConsumerInfoEntity>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cldInf"></param>
        public void UpdateList(ConsumerInfoEntity cldInf)
        {
            ListCloudInfo.Add(cldInf);
        }
    }
}
