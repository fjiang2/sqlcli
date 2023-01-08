namespace syscon.stdio.Cli
{
    public interface IShell
    {
        /// <summary>
        /// Implementation of commands in the Shell 
        /// </summary>
        IShellTask Task { get; }

        /// <summary>
        /// Read command line from system consolee and execute 
        /// </summary>
        void Run();

        /// <summary>
        /// Run commmand line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        NextStep Run(string line);
    }
}