using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MoonGate
{
    public class FileListStyleModel : INotifyPropertyChanged
    {
        public FileListStyleModel()
        {
            ObservableCollection<FileListStyle> files = new ObservableCollection<FileListStyle>();
            for (int i = 0; i < 10; i++)
            {
                files.Add(new FileListStyle() { Name = "Image.jpg", ImageSize = "256 × 256", Type = "JPEG イメージ", Size = "256 KB", CreateDate = "2011/11/11 11:11" });
            }
            Files = files;
            SelectedFile = files[0];
        }

        public ObservableCollection<FileListStyle> Files { get; set; }
        private FileListStyle selectedFile;
        public FileListStyle SelectedFile
        {
            get
            {
                return selectedFile;
            }
            set
            {
                selectedFile = value;
                NotifyPropertyChanged("SelectedFile");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}

