using System;

namespace WowSuite.Launcher.Logic.Updating
{
    /// <summary>
    /// Аргументы события изменения прогресса выполнения какого-либо действия.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int progress)
        {
            Progress = progress;
        }

        /// <summary>
        /// Прогресс выполнения какой-либо операции
        /// </summary>
        public int Progress { get; protected set; }
    }
}