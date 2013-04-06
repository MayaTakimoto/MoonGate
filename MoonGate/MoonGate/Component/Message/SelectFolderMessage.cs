
namespace MoonGate.Component.Message
{
    /// <summary>
    /// 
    /// </summary>
    class SelectFolderMessage : BaseMessage
    {
        /// <summary>
        /// 選択されたフォルダの一覧
        /// </summary>
        public string[] FolderNames { get; set; }

        ///// <summary>
        ///// 処理結果
        ///// </summary>
        //public bool Result { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sender"></param>
        public SelectFolderMessage(object sender)
            : base(sender) { }
    }
}
