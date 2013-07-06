//-----------------------------------------------------------------------
// <summary>フォルダ選択ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013‎/0‎3/‎‎23‎ ‏‎10:25:37  +9:00 $</date>
// <copyright file="$Name: SelectFolderAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// FolderBrowserWindow表示Actionクラス
    /// </summary>
    class SelectFolderAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// 選択されたフォルダの一覧
        /// </summary>
        public string[] FolderNames { get; set; }

        /// <summary>
        /// 実行するコマンド(依存関係プロパティ)
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 依存関係プロパティの登録
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SelectFolderAction), new UIPropertyMetadata());


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="message"></param>
        //private static void ShowSelectFolderDialog(SelectFolderMessage message)
        //{
        //    FolderBrowseWindow DialogGetFolder = new FolderBrowseWindow();
        //    var resDlg = DialogGetFolder.ShowDialog();

        //    if ((bool)resDlg == false)
        //    {
        //        message.Result = false;

        //        DialogGetFolder = null;
        //        return;
        //    }
        //    if (DialogGetFolder.SelectedFolder.Items.Count == 0)
        //    {
        //        message.FolderNames = null;
        //        message.Result = false;

        //        DialogGetFolder = null;
        //        return;
        //    }
            
        //    message.FolderNames = DialogGetFolder.SelectedFolder.Items.Cast<string>().ToArray();
        //    message.Result = true;

        //    DialogGetFolder = null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        protected override void Invoke(object param)
        {
            FolderBrowseWindow DialogGetFolder = new FolderBrowseWindow();
            var resDlg = DialogGetFolder.ShowDialog();

            if ((bool)resDlg == false)
            {
                DialogGetFolder = null;
                return;
            }
            if (DialogGetFolder.SelectedFolder.Items.Count == 0)
            {
                this.FolderNames = null;
                DialogGetFolder = null;
                return;
            }

            this.FolderNames = DialogGetFolder.SelectedFolder.Items.Cast<string>().ToArray();
            DialogGetFolder = null;

            foreach (string folderName in this.FolderNames)
            {
                Command.Execute(folderName);
            }
        }
    }
}
