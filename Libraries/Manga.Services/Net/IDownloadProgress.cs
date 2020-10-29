using System;
using System.ComponentModel;

namespace Manga.Services.Net
{
    public interface IDownloadProgress : INotifyPropertyChanged
    {
        String Title { get; set; }
        String StatusText { get; set; }
        double Progress { get; set; }
        DownloadStatus Status { get; set; }
    }

    public enum DownloadStatus
    {
        Created = 0,
        Downloading = 1,
        Completed = 2,
        Paused = 4,
        Cancelled = 8,
        Error = 16,
    }
}
