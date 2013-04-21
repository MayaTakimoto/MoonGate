//-----------------------------------------------------------------------
// <summary>鍵情報入力ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-04-06 ‏‎23:09:37  +9:00 $</date>
// <copyright file="$Name: InputPassAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using MoonGate.Component.Message;
using System.IO;
using System.Windows;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// InputPassWindow表示Actionクラス
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
            InputPassWindow dialogInputPass = new InputPassWindow();
            var resDlg = dialogInputPass.ShowDialog();

            message.SelectedIndex = dialogInputPass.TabMain.SelectedIndex;
            message.PassWord = dialogInputPass.pswdInput.SecurePassword;
            message.PassFile = new FileInfo(dialogInputPass.ConKeyFile.Tag.ToString());
            message.PassDrive = dialogInputPass.DriveList.SelectedItem.ToString();
            message.Result = (bool)resDlg;
        }
    }
}
