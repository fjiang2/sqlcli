using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sys.Stdio;
using Sys;
using Sys.IO;

namespace Sys.Cli
{
    public class Batch
    {
        private const string EXT = ".sqc";
        private readonly string path;
        private readonly IWorkSpace workspace;


        public bool IsBatch { get; } = false;

        public Batch(IWorkSpace workspace, string path)
        {
            this.workspace = workspace;
            this.path = GetFullPath(path);
            this.IsBatch = EXT == Path.GetExtension(this.path);
        }

        private string GetFullPath(string path)
        {
            string fullPath = workspace.WorkingDirectory.GetFullPath(path, EXT);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }

            if (string.IsNullOrEmpty(workspace.Path))
            {
                return string.Empty;
            }

            foreach (string _path in workspace.Path.Split(';'))
            {
                WorkingDirectory working = new WorkingDirectory(_path);
                try
                {
                    fullPath = working.GetFullPath(path, EXT);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                catch(Exception ex)
                {
                    cerr.WriteLine($"invalid path:\"{_path}\", using ; as delimiter", ex);
                }
            }

            return string.Empty;
        }


        public bool Call(IShellTask task, string[] args)
        {
            if (!IsBatch)
            {
                cerr.WriteLine($"must be {EXT} file: {path}");
                return false;
            }

            if (Exists)
            {
                var lines = ReadLines(args);

                IShellTask _task = task.CreateTask();
                var _shell = new Shell(_task);

                //go to current theSide
                if (task != null)
                    _task.SwitchTask(task);

                _shell.DoBatch(lines);

                return true;
            }
            else
            {
                cerr.WriteLine($"cannot find the file: {path}");
                return false;
            }
        }

        /// <summary>
        /// parameters: %1 %2 %3 ...
        /// </summary>
        /// <param name="args"></param>
        private string[] ReadLines(string[] args)
        {
            string[] lines = File.ReadAllLines(path);

            List<string> L = new List<string>();
            foreach (string line in lines)
            {
                string cmd = line.Trim();
                if (cmd == string.Empty)
                    continue;

                for (int i = 0; i < args.Length; i++)
                {
                    cmd = cmd.Replace($"%{i}", args[i]);
                }

                //replace unassigned parameter by string.Empty
                int k = args.Length;
                while (cmd.IndexOf("%") > 0)
                {
                    cmd = cmd.Replace($"%{k}", string.Empty);
                    k++;
                    if (k > 100)
                        break;
                }

                L.Add(cmd);
            }

            return L.ToArray();
        }

        public bool Exists => File.Exists(path);

        public override string ToString()
        {
            return Path.GetFullPath(this.path);
        }
    }
}
