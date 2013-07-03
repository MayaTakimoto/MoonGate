using System.Collections.ObjectModel;
using System.Windows;

namespace MoonGate.Component
{
    /// <summary>
    /// MessageとActionの関連付けの介助を行う
    /// </summary>
    class ActionList : Collection<BaseAction>
    {
        /// <summary>
        /// 自クラス内に保持されたActionを一括で関連付ける
        /// </summary>
        /// <param name="receiver"></param>
        public void Regist(FrameworkElement receiver)
        {
            foreach (var action in this)
            {
                action.RegistAction(receiver);
            }
        }
    }
}
