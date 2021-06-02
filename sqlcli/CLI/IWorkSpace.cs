namespace Sys.Cli
{
    public interface IWorkSpace
    {
        WorkingDirectory WorkingDirectory { get; }
        string Path { get; }
    }
}