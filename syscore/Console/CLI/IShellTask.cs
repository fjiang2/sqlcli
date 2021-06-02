using Sys.Stdio;

namespace Sys.Cli
{
    /// <summary>
    /// The task of shell
    /// </summary>
    public interface IShellTask
    {
        /// <summary>
        /// Current path in the command line
        /// </summary>
        string CurrentPath { get; }

        /// <summary>
        /// Create new task for batch 
        /// </summary>
        /// <returns></returns>
        IShellTask CreateTask();

        /// <summary>
        /// Switch current task context into batch context
        /// </summary>
        /// <param name="task"></param>
        void SwitchTask(IShellTask task);

        /// <summary>
        /// Run command in multiple lines 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        NextStep DoMultipleLineCommand(string text);

        /// <summary>
        /// Run commnad in single line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        NextStep DoSingleLineCommand(string line);

        /// <summary>
        /// Display help of commands
        /// </summary>
        void Help();
    }
}