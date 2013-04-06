//-----------------------------------------------------------------------
// <summary>ファイル選択ダイアログ表示メッセージクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-03-23 10:25:37  +9:00 $</date>
// <copyright file="$Name: SelectFileMessage.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace MoonGate.Component.Message
{
    /// <summary>
    /// VMからファイル選択ダイアログを表示させるためのMessageクラス
    /// </summary>
    class SelectFileMessage : BaseMessage
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

        ///// <summary>
        ///// 処理結果
        ///// </summary>
        //public bool Result { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender"></param>
        public SelectFileMessage(object sender)
            : base(sender)
        {
        }
    }
}
