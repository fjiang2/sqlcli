using System.Collections.Generic;
using Sys.Stdio.Cli;

namespace Sys.Stdio
{
    public interface IApplicationCommand : ICommand
    {
        string InputPath();
        string OutputPath();
    }
}