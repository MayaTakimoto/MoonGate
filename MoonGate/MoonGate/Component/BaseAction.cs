using System.Windows;

namespace MoonGate.Component
{
    /// <summary>
    /// Actionクラスが実装するインターフェース
    /// </summary>
    abstract class BaseAction
    {
        /// <summary>
        /// MessageとActionを関連付ける
        /// </summary>
        /// <param name="receiver">Messageの受け手（View）</param>
        public abstract void RegistAction(FrameworkElement receiver);
    }
}
