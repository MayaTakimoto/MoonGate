using System.Windows.Media.Imaging;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// ファイル情報保持クラス
    /// </summary>
    class FileInfoEntity
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// ファイル完全パス
        /// </summary>
        public string FilePath { get; set; }
    }
}
