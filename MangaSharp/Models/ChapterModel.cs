using Manga.Core.Domain;
using Manga.Core.Infrastructure;
using System;

namespace MangaSharp.Models
{
    public class ChapterModel : ObservableObject
    {
        #region Fields

        private readonly Chapter _chapter;
        private readonly MangaModel _manga;

        private String _name;
        private String _url;
        private int _index;
        private int _size;
        private DateTime _date;
        private bool _isDownloaded;

        private Boolean _isSelected;

        #endregion

        #region Ctor

        public ChapterModel(MangaModel manga, Chapter chapter)
        {
            this._chapter = chapter;
            this._manga = manga;

            Initialize();
        }

        private void Initialize()
        {
            _name = _chapter.Name;
            _url = _chapter.Url;
            _index = _chapter.Index;
            _size = _chapter.Size;
            _date = _chapter.Date;
            _isDownloaded = _chapter.IsDownloaded;

            _isSelected = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the chapter
        /// </summary>
        public String Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets the address of the chapter
        /// </summary>
        public String Url
        {
            get { return _url; }
            private set
            {
                _url = value;
                RaisePropertyChanged("Url");
            }
        }

        /// <summary>
        /// Gets the chapter index
        /// </summary>
        public int Index
        {
            get { return _index; }
            private set
            {
                _index = value;
                RaisePropertyChanged("Index");
            }
        }

        /// <summary>
        /// Gets the chapter size
        /// </summary>
        public int Size
        {
            get { return _size; }
            private set
            {
                _size = value;
                RaisePropertyChanged("Size");
            }
        }

        /// <summary>
        /// Gets the chapter date
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            private set
            {
                _date = value;
                RaisePropertyChanged("Date");
            }
        }

        /// <summary>
        /// Gets the manga this chapter belongs to
        /// </summary>
        public MangaModel Manga
        {
            get { return _manga; }
        }

        public Boolean IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets the chapter
        /// </summary>
        public Chapter Chapter
        {
            get { return _chapter; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the model with underlying data object
        /// </summary>
        public void Update()
        {
            Name = _chapter.Name;
            Url = _chapter.Url;
            Index = _chapter.Index;
            Size = _chapter.Size;
            Date = _chapter.Date;
            _isDownloaded = _chapter.IsDownloaded;
        }

        #endregion
    }
}
