//-----------------------------------------------------------------------
// <summary>鍵情報入力ダイアログ表示メッセージクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-04-06 22:52:00  +9:00 $</date>
// <copyright file="$Name: InputPassMessage.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Security;

namespace MoonGate.Component.Message
{
    /// <summary>
    /// VMから鍵情報入力ダイアログを表示させるMessageクラス
    /// </summary>
    class InputPassMessage : BaseMessage
    {
        /// <summary>
        /// パスワード
        /// </summary>
        public SecureString PassWord { get; set; }

        /// <summary>
        /// パスファイル
        /// </summary>
        public FileInfo PassFile { get; set; }

        /// <summary>
        /// パスドライブ
        /// </summary>
        public string PassDrive { get; set; }

        /// <summary>
        /// 選択されているタブ
        /// </summary>
        public int SelectedIndex { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender"></param>
        public InputPassMessage(object sender) 
            : base(sender) { }
    }
}
