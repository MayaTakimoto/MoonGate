using System.Windows;

namespace MoonGate.Component
{
    class ActionRegistHelper
    {
        /// <summary>
        /// ActionListを保持するプロパティ
        /// </summary>
        public static readonly DependencyProperty ListActionProperty = DependencyProperty.RegisterAttached(
            "ListAction",
            typeof(ActionList),
            typeof(ActionRegistHelper),
            new UIPropertyMetadata(null, OnActionPropertyChanged));


        /// <summary>
        /// ActionListオブジェクトの取得
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public static ActionList GetListAction(Window wnd)
        {
            return wnd.GetValue(ListActionProperty) as ActionList;
        }


        /// <summary>
        /// ActionListオブジェクトの設定
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="list"></param>
        public static void SetListAction(Window wnd, ActionList list)
        {
            wnd.SetValue(ListActionProperty, list);
        }


        /// <summary>
        /// ActionProperty変更時イベント
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnActionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window wnd = d as Window;
            ActionList list = e.NewValue as ActionList;

            if (wnd == null || list == null)
            {
                return;
            }

            list.Regist(wnd);
        }
    }
}
