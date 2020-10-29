using Caliburn.Metro.Core;
using MahApps.Metro.Controls;

namespace MangaSharp
{
    public class AppWindowManager : MetroWindowManager
    {
        protected override System.Windows.Window CreateWindow(object rootModel, bool isDialog, object context, System.Collections.Generic.IDictionary<string, object> settings)
        {
            return base.CreateWindow(rootModel, isDialog, context, settings);
        }

        public override MetroWindow CreateCustomWindow(object view, bool windowIsView)
        {
            if (windowIsView)
            {
                return view as MainWindowContainer;
            }
            
            return new MainWindowContainer
            {
                Content = view
            };
        }
    }
}
