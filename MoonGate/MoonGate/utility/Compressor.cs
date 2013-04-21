using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace MoonGate.Component
{
    /// <summary>
    /// 圧縮・解凍処理クラス
    /// </summary>
    public class Compressor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bCompressed"></param>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public bool Compress(out byte[] bCompressed, params string[] filePaths)
        {
            bCompressed = null;

            using (MemoryStream msOut = new MemoryStream())
            {
                using (ZipOutputStream zosOut = new ZipOutputStream(msOut))
                {
                    zosOut.SetLevel(9);

                    foreach (string filePath in filePaths)
                    {
                        using (FileStream fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {

                        }
                    }
                }

                bCompressed = msOut.ToArray();
            }
            
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bDecompress"></param>
        /// <returns></returns>
        public bool Decompress(string filePath, byte[] bDecompress)
        {
            bDecompress = null;

            return true;
        }
    }
}
