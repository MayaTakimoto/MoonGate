using MoonGate.Component.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// 
    /// </summary>
    class InputPassAction : BaseAction
    {
        /// <summary>
        /// MessageにActionを関連付ける
        /// </summary>
        /// <param name="receiver"></param>
        public override void RegistAction(FrameworkElement receiver)
        {
            Indicator.Instance.Instruct<InputPassMessage>(receiver, ShowInputPassWindow);
        }


        /// <summary>
        /// 鍵情報入力ダイアログ表示
        /// </summary>
        /// <param name="message"></param>
        private static void ShowInputPassWindow(InputPassMessage message)
        {
            InputPassWindow wnd = new InputPassWindow();
            var resDlg = wnd.ShowDialog();

            message.SelectedIndex = wnd.TabMain.SelectedIndex;
            message.PassWord = wnd.pswdInput.SecurePassword;

            //if (File.Exists(wnd.FilePath.Tag.ToString()))
            //{
            //    message.PassFile = new FileInfo(wnd.FilePath.Tag.ToString());
            //}

            message.PassDrive = wnd.ListDrive.SelectedItem.ToString();
            message.Result = (bool)resDlg;

            wnd = null;
        }
    }
}
