using Autofac.Extras.Attributed;
using Caliburn.Micro;
using Manga.Core.Domain.Logging;
using Manga.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaSharp.ViewModels
{
    public class AppViewModel : Conductor<IViewModel>.Collection.OneActive, IHandle<StatusLog>
    {
        #region Fields

        //protected string _displayName = "MangaSharp";
        private bool _isSettingsOpen;
        private string _statusText;

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Ctor

        public AppViewModel(IEventAggregator eventAggregator, [WithMetadata("Location", "Main Window")]IEnumerable<IViewModel> tabs)
        {
            this._eventAggregator = eventAggregator;            
            this.Items.AddRange(tabs.OrderBy(t => t.Order));
            
            Initialize();
        }

        #endregion

        #region Properties
        /*
        public override string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }
        */

        public bool IsSettingsOpen
        {
            get { return _isSettingsOpen; }
            set
            {
                _isSettingsOpen = value;
                NotifyOfPropertyChange(() => IsSettingsOpen);
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            DisplayName = "MangaSharp";
            _isSettingsOpen = false;
            StatusText = "Ready";
        }

        public void ToggleSettings()
        {
            IsSettingsOpen = !IsSettingsOpen;
        }

        public void WindowSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            // TODO add normal layout and big layout
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void Handle(StatusLog message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            StatusText = message.Text;
        }

        #endregion
    }
}
