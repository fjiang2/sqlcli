namespace Sys.IO
{
    public interface IWorkspace
    {
        WorkingDirectory WorkingDirectory { get; }
        string Path { get; }
    }
}