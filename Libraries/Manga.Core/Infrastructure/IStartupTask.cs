
namespace Manga.Core.Infrastructure
{
    /// <summary>
    /// Interface which should be implemented by tasks run on startup
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        void Execute();

        /// <summary>
        /// Gets the delay which task will run after
        /// </summary>
        int Delay { get; }

        /// <summary>
        /// Gets the order of execution
        /// </summary>
        int Order { get; }
    }
}
