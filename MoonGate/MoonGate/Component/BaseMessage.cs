
namespace MoonGate.Component
{
    /// <summary>
    /// VMからViewを操作するためのAgentの基底クラス
    /// </summary>
    abstract class BaseMessage
    {
        /// <summary>
        /// 呼出元
        /// </summary>
        public object Sender { get; protected set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender"></param>
        public BaseMessage(object sender)
        {
            Sender = sender;
        }
    }
}
