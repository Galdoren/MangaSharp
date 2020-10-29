using Caliburn.Micro;

namespace MangaSharp.ViewModels
{
    public interface IViewModel : IScreen
    {
        int Order { get; }
    }
}
