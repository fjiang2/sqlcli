namespace Sys.IO
{
    public interface IWorkSpace
    {
        WorkingDirectory WorkingDirectory { get; }
        string Path { get; }
    }
}