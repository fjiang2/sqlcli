namespace Sys.Cli
{
    public interface IShell
    {
        NextStep Run(string line);
    }
}