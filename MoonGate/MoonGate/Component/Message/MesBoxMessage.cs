using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonGate.Component.Message
{
    /// <summary>
    /// 
    /// </summary>
    class MesBoxMessage : BaseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string MesTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MesBody { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender"></param>
        public MesBoxMessage(object sender) 
            : base(sender) { }
    }
}
