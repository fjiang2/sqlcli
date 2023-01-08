using System.Collections.Generic;
using syscon.stdio.Cli;

namespace syscon.stdio
{
    public interface IApplicationCommand : ICommand
    {
        string InputPath();
        string OutputPath();
    }
}