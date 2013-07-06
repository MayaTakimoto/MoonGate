//-----------------------------------------------------------------------
// <summary>ファイル選択ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013‎/0‎3/‎‎23‎ 10:25:37  +9:00 $</date>
// <copyright file="$Name: SelectFileAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// OpenFileDialog表示Actionクラス
    /// </summary>
    class SelectFileAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// OpenFileDialog.Titleプロパティ
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// OpenFileDialog.MultiSelectプロパティ
        /// </summary>
        public bool Multiselect { get; set; }

        /// <summary>
        /// OpenFileDialog.CheckPathExistsプロパティ
        /// </summary>
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// OpenFileDialog.DefaultExtプロパティ
        /// </summary>
        public string DefaultExt { get; set; }

        /// <summary>
        /// OpenFileDialog.FileNameプロパティ
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// OpenFileDialog.FileNamesプロパティ
        /// </summary>
        public string[] FileNames { get; set; }

        /// <summary>
        /// OpenFileDialog.InitialDirectoryプロパティ
        /// </summary>
        public string InitialDirectory { get; set; }

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
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SelectFileAction), new UIPropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        protected override void Invoke(object param)
        {
            OpenFileDialog dlgGetFiles = new OpenFileDialog();

            dlgGetFiles.Title = this.Title;
            dlgGetFiles.InitialDirectory = this.InitialDirectory;
            dlgGetFiles.FileName = this.FileName;
            dlgGetFiles.DefaultExt = this.DefaultExt;
            dlgGetFiles.CheckPathExists = this.CheckPathExists;
            dlgGetFiles.Multiselect = this.Multiselect;

            var resDlg = dlgGetFiles.ShowDialog();

            if ((bool)resDlg == false)
            {
                return;
            }

            this.FileNames = dlgGetFiles.FileNames;
            dlgGetFiles = null;

            foreach (string fileName in this.FileNames)
            {
                Command.Execute(fileName);
            }
        }
    }
}
