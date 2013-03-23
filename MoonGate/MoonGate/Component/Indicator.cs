using MoonGate.Component.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace MoonGate.Component
{
    /// <summary>
    /// VMとViewを仲介するMessengerクラス
    /// </summary>
    class Indicator
    {
        /// <summary>
        /// Messengerクラスのインスタンス
        /// </summary>
        private static Indicator instance = new Indicator();

        /// <summary>
        /// 
        /// </summary>
        private List<ActionInfoEntity> ActionMap = new List<ActionInfoEntity>();

        /// <summary>
        /// Messengerクラスのインスタンスのプロパティ
        /// </summary>
        public static Indicator Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// MessageとAction
        /// </summary>
        private class ActionInfoEntity
        {
            public Type Type { get; set; }                      // Messageの型
            public INotifyPropertyChanged Sender { get; set; }  // Message発信元（VM）
            public Delegate Action { get; set; }                // Message受信時に行う処理
        }


        /// <summary>
        /// MessageとActionを関連付ける
        /// </summary>
        /// <typeparam name="MesType"></typeparam>
        /// <param name="receiver">Messageの受け手（View）</param>
        /// <param name="action">実行すべきAction</param>
        public void Instruct<MesType>(FrameworkElement receiver, Action<MesType> action)
        {
            ActionInfoEntity actInfo = new ActionInfoEntity();

            actInfo.Type = typeof(MesType);
            actInfo.Sender = receiver.DataContext as INotifyPropertyChanged;
            actInfo.Action = action;

            ActionMap.Add(actInfo);
        }


        /// <summary>
        /// MessageをViewへ送り、Actionを実行させる
        /// </summary>
        /// <typeparam name="MesType"></typeparam>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void Order<MesType>(INotifyPropertyChanged sender, MesType message)
        {
            // Messageの型から実行するActionを検索
            var actionList = ActionMap.Where(param => param.Sender == sender && param.Type == message.GetType()).Select(param => param.Action as Action<MesType>);

            foreach (var action in actionList)
            {
                action(message);
            }
        }
    }
}
