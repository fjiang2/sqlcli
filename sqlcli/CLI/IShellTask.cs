using Sys.Stdio;

namespace Sys.Stdio.Cli
{
    public interface IShellTask
    {
        ISide TheSide { get; }
        string CurrentPath { get; }
        IShellTask CreateTask();
        void Help();
        void ChangeSide(ISide side);
        NextStep DoMultipleLineCommand(string text);
        NextStep DoSingleLineCommand(string line);
    }
}