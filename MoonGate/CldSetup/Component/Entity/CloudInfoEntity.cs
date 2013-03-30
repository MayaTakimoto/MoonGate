
using System.Runtime.Serialization;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    class CloudInfoEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Key { get; set; }
    }
}
