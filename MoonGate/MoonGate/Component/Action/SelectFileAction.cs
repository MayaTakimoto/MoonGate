using Microsoft.Win32;
using MoonGate.Component;
using MoonGate.Component.Message;
using System.Windows;

namespace MoonGate.Component.Action
{
    class SelectFileAction : BaseAction
    {
        /// <summary>
        /// MessageにActionを関連付ける
        /// </summary>
        /// <param name="receiver"></param>
        public override void RegistAction(FrameworkElement receiver)
        {
            Indicator.Instance.Instruct<SelectFileMessage>(receiver, ShowSelectFileDialog);
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

            message.FileNames = dlgGetFiles.FileNames;
            message.Result = (bool)resDlg;
        }
    }
}
