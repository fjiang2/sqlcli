using Sys.Stdio;

namespace sqlcli
{
    interface IShellTask
    {
        ISide TheSide { get; }
        string CurrentPath { get; }
        void ChangeSide(ISide side);
        NextStep DoMultipleLineCommand(string text);
        NextStep DoSingleLineCommand(ApplicationCommand cmd);
    }
}