using System;
using System.Threading;

namespace Manga.Core.ComponentModel
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public static class ReaderWriteLockExtensions
    {
        private struct Disposable : IDisposable
        {
            private readonly Action m_action;
            private Sentinel m_sentinel;

            public Disposable(Action action)
            {
                m_action = action;
                m_sentinel = new Sentinel();
            }

            public void Dispose()
            {
                m_action();
                GC.SuppressFinalize(m_sentinel);
            }
        }

        private class Sentinel
        {
            ~Sentinel()
            {
                throw new InvalidOperationException("Lock not properly disposed.");
            }
        }

        /// <summary>
        /// Tries to enter the <see cref="ReaderWriterLockSlim"/> in read mode and releases the lock on dispose.
        /// </summary>
        /// <param name="_lock">The rw lock.</param>
        /// <returns>IDisposable</returns>
        public static IDisposable AcquireReadLock(this ReaderWriterLockSlim _lock)
        {
            _lock.EnterReadLock();
            return new Disposable(_lock.ExitReadLock);
        }

        /// <summary>
        /// Tries to enter the <see cref="ReaderWriterLockSlim"/> in upgradeable mode and releases the lock on dispose.
        /// </summary>
        /// <param name="_lock">The rw lock.</param>
        /// <returns>IDisposable</returns>
        public static IDisposable AcquireUpgradableReadLock(this ReaderWriterLockSlim _lock)
        {
            _lock.EnterUpgradeableReadLock();
            return new Disposable(_lock.ExitUpgradeableReadLock);
        }

        /// <summary>
        /// Tries to enter the <see cref="ReaderWriterLockSlim"/> in write mode and releases the lock on dispose.
        /// </summary>
        /// <param name="_lock">The rw lock.</param>
        /// <returns>IDisposable</returns>
        public static IDisposable AcquireWriteLock(this ReaderWriterLockSlim _lock)
        {
            _lock.EnterWriteLock();
            return new Disposable(_lock.ExitWriteLock);
        }
    }
}
