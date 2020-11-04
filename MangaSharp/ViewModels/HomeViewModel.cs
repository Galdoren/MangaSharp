using Caliburn.Micro;
using MangaSharp.Infrastructure;

namespace MangaSharp.ViewModels
{
    [ViewModelLocation("Main Window")]
    public class HomeViewModel : Screen, IViewModel
    {
        #region Fields

        #endregion

        #region Ctor

        public HomeViewModel()
            : base()
        {
            this.DisplayName = "HOME";
        }

        #endregion

        #region Properties

        public int Order
        {
            get { return 0; }
        }

        #endregion

        #region Methods

        #endregion
    }
}
