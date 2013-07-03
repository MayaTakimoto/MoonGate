//-----------------------------------------------------------------------
// <summary>フォルダ選択ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013‎/0‎3/‎‎23‎ ‏‎10:25:37  +9:00 $</date>
// <copyright file="$Name: SelectFolderAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using MoonGate.Component.Message;
using System.Linq;
using System.Windows;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// FolderBrowserWindow表示Actionクラス
    /// </summary>
    class SelectFolderAction : BaseAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiver"></param>
        public override void RegistAction(FrameworkElement receiver)
        {
            Messenger.Instance.Instruct<SelectFolderMessage>(receiver, ShowSelectFolderDialog);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private static void ShowSelectFolderDialog(SelectFolderMessage message)
        {
            FolderBrowseWindow DialogGetFolder = new FolderBrowseWindow();
            var resDlg = DialogGetFolder.ShowDialog();

            if ((bool)resDlg == false)
            {
                message.Result = false;

                DialogGetFolder = null;
                return;
            }
            if (DialogGetFolder.SelectedFolder.Items.Count == 0)
            {
                message.FolderNames = null;
                message.Result = false;

                DialogGetFolder = null;
                return;
            }
            
            message.FolderNames = DialogGetFolder.SelectedFolder.Items.Cast<string>().ToArray();
            message.Result = true;

            DialogGetFolder = null;
        }
    }
}
