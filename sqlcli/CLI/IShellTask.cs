using Sys.Stdio;

namespace sqlcli
{
    interface IShellTask
    {
        Side TheSide { get; }
        PathManager ThePath { get; }
        void ChangeSide(Side side);
        NextStep DoMultipleLineCommand(string text);
        NextStep DoSingleLineCommand(ApplicationCommand cmd);
    }
}