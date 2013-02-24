using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate
{
    /// <summary>
    /// 自作ファイル選択ダイアログのエクスプローラバー風パーツ(子部品)
    /// </summary>
    public class FolderExplorerNode
    {
        private readonly DirectoryInfo folderInfo;

        public string FolderName
        {
            get
            {
                return this.folderInfo.Name;
            }
        }

        public IEnumerable<FolderExplorerNode> FolderBranch
        {
            get
            {
                try
                {
                    return Directory.EnumerateDirectories(this.folderInfo.FullName).Select(s => new FolderExplorerNode(s));
                }
                catch (Exception)
                {
                    // folder permission is handled by catching exception
                    return null;
                }
            }
        }

        public FolderExplorerNode(string folderPath)
        {
            this.folderInfo = new DirectoryInfo(folderPath);
        }
    }
}
