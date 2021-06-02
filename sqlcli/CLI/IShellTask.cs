using Sys.Stdio;

namespace Sys.Cli
{
    public interface IShellTask
    {
        string CurrentPath { get; }
        IShellTask CreateTask();
        void SwitchTask(IShellTask task);
        NextStep DoMultipleLineCommand(string text);
        NextStep DoSingleLineCommand(string line);
        void Help();
    }
}