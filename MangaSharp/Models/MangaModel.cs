using Manga.Core.Infrastructure;
using MangaSharp.Extensions;
using MangaSharp.Infrastructure;
using System;

namespace MangaSharp.Models
{
    public class MangaModel : ObservableObject
    {
        #region Fields

        private readonly Manga.Core.Domain.Manga _manga;

        private String _name;
        private String _author;
        private String _artist;
        private String _description;
        private int _year;
        private Uri _imageSource;
        private ThreadSafeObservableCollection<ChapterModel> _chapters;

        #endregion

        #region Ctor

        public MangaModel(Manga.Core.Domain.Manga manga)
        {
            this._manga = manga;
            Initialize();
        }

        private void Initialize()
        {
            _name = _manga.Name;
            _author = _manga.Author;
            _artist = _manga.Artist;
            _description = _manga.Description;
            _year = _manga.Year;
            _imageSource = String.IsNullOrEmpty(_manga.ImageUrl) ? null : new Uri(_manga.ImageUrl);

            if (_manga.Chapters.Count > 0)
            {
                this.Chapters.SuspendCollectionChangeNotification();
                this._chapters.Clear();
                this._chapters.AddRange(_manga.Chapters.ToModel(this));
                this._chapters.ResumeCollectionChangeNotification();
            }
        }

        #endregion

        #region Properties

        public Manga.Core.Domain.Manga Manga
        {
            get { return _manga; }
        }

        public String Name 
        { 
            get { return _name; }
            private set 
            { 
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        
        public String Author 
        { 
            get { return _author; } 
            private set
            {
                _author = value;
                RaisePropertyChanged("Author");
            }
        }

        public String Artist
        {
            get { return _artist; }
            private set
            {
                _artist = value;
                RaisePropertyChanged("Artist");
            }
        }

        public String Description
        {
            get { return _description; }
            private set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        public int Year
        {
            get { return _year; }
            private set
            {
                _year = value;
                RaisePropertyChanged("Year");
            }
        }

        public Uri ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                RaisePropertyChanged("ImageSource");
            }
        }

        public ThreadSafeObservableCollection<ChapterModel> Chapters
        {
            get 
            {
                if (_chapters == null)
                    _chapters = new ThreadSafeObservableCollection<ChapterModel>();
                return _chapters; 
            }
        }

        /// <summary>
        /// Returns the chapter model of in given index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Chapter model</returns>
        public ChapterModel this[int index]
        {
            get
            {
                return Chapters[index];
            }
        }

        #endregion

        #region Methods

        public void Update()
        {
            Name = _manga.Name;
            Author = _manga.Author;
            Artist = _manga.Artist;
            Description = _manga.Description;
            Year = _manga.Year;
            ImageSource = String.IsNullOrEmpty(_manga.ImageUrl) ? null : new Uri(_manga.ImageUrl);

            if (Manga.Chapters.Count > 0)
            {
                this.Chapters.SuspendCollectionChangeNotification();
                this._chapters.Clear();
                this._chapters.AddRange(_manga.Chapters.ToModel(this));
                this._chapters.ResumeCollectionChangeNotification();
            }
        }

        #endregion
    }
}
