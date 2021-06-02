namespace Sys.Cli
{
    public interface IShell
    {
        IShellTask Task { get; }
        void Run();
    }
}