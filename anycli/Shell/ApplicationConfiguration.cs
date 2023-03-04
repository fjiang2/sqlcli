using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys;
using Sys.Data;
using syscon.stdio;
using Sys.IO;

namespace anycli
{
    class ApplicationConfiguration : Configuration, IApplicationConfiguration
    {
        private const string _PATH = "path";

        public string OutputFile { get; set; }
        public WorkingDirectory WorkingDirectory { get; }

        public ApplicationConfiguration()
        {
            WorkingDirectory = new WorkingDirectory();
        }

        public override bool Initialize(ConfigurationPath cfg)
        {
            const string _FILE_OUTPUT = "output";
            const string _WORKING_DIRECTORY = "working.directory.commands";

            base.Initialize(cfg);

            this.OutputFile = GetValue<string>(_FILE_OUTPUT, "script.any");
            this.WorkingDirectory.SetCurrentDirectory(GetValue<string>(_WORKING_DIRECTORY, "."));

            Context.SetValue(_PATH, GetValue(_PATH, "."));
            return true;
        }

        public string Path => Context.GetValue(_PATH, string.Empty);

    }
}
