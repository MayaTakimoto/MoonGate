//-----------------------------------------------------------------------
// <summary>ファイル選択ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013‎/0‎3/‎‎23‎ 10:25:37  +9:00 $</date>
// <copyright file="$Name: SelectFolderAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Win32;
using MoonGate.Component.Message;
using System.Windows;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// OpenFileDialog表示Actionクラス
    /// </summary>
    class SelectFileAction : BaseAction
    {
        /// <summary>
        /// MessageにActionを関連付ける
        /// </summary>
        /// <param name="receiver"></param>
        public override void RegistAction(FrameworkElement receiver)
        {
            Messenger.Instance.Instruct<SelectFileMessage>(receiver, ShowSelectFileDialog);
        }


        /// <summary>
        /// ファイル選択ダイアログ表示
        /// </summary>
        /// <param name="obj"></param>
        private static void ShowSelectFileDialog(SelectFileMessage message)
        {
            OpenFileDialog dlgGetFiles = new OpenFileDialog();

            dlgGetFiles.Title = message.Title;
            dlgGetFiles.InitialDirectory = message.InitialDirectory;
            dlgGetFiles.FileName = message.FileName;
            dlgGetFiles.DefaultExt = message.DefaultExt;
            dlgGetFiles.CheckPathExists = message.CheckPathExists;
            dlgGetFiles.Multiselect = message.Multiselect;

            var resDlg = dlgGetFiles.ShowDialog();

            if ((bool)resDlg == true)
            {
                message.FileNames = dlgGetFiles.FileNames;
                message.Result = (bool)resDlg;
            }

            dlgGetFiles = null;
        }
    }
}
