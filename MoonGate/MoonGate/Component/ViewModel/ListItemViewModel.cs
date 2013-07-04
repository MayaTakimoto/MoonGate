using System.Windows.Media.Imaging;

namespace MoonGate.Component.ViewModel
{
    /// <summary>
    /// リスト項目クラス
    /// </summary>
    public class ListItemViewModel : FileInfoViewModel
    {
        /// <summary>
        /// アイコンパス
        /// </summary>
        public BitmapSource IconPath { get; set; }

        /// <summary>
        /// クラウド上ファイルがどうかフラグ
        /// </summary>
        public bool IsCloud { get; set; }

        /// <summary>
        /// ディレクトリかどうかフラグ
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTransceived { get; set; }
    }
}
