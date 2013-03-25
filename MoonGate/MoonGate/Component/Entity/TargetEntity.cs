using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// リスト項目クラス
    /// </summary>
    class TargetEntity : FileInfoEntity
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
    }
}
