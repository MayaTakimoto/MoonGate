
using System.Runtime.Serialization;

namespace MoonGate.Component.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CloudInfoViewModel
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
