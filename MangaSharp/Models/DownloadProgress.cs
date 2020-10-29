using Manga.Core.Infrastructure;
using Manga.Services.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaSharp.Models
{
    public class DownloadProgress : ObservableObject, IDownloadProgress
    {
        #region Fields

        private bool _isNotifying;
        private string _title;
        private string _statusText;
        private double _progress;
        private DownloadStatus _status;

        #endregion

        #region Ctor

        public DownloadProgress()
        {
            _isNotifying = true;
            _title = String.Empty;
            _statusText = String.Empty;
            _progress = 0.0d;
            _status = DownloadStatus.Created;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the progress title
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets or sets the progress status text
        /// </summary>
        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                _statusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        /// <summary>
        /// Gets or sets the progress value out of 100
        /// </summary>
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public DownloadStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        #endregion
    }
}
