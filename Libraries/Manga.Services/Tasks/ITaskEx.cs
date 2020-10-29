
namespace Manga.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface ITaskEx
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute();
    }
}
