﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sys;
using Sys.Stdio;
using Sys.Stdio.Cli;

namespace sqlcli
{
    class ApplicationCommand : Command, IApplicationCommand
    {

        public bool Refresh { get; private set; }
        public bool IsVertical { get; private set; }
        public bool IsSchema { get; private set; }
        public bool HasHelp { get; private set; }
        public bool HasIfExists { get; private set; }
        public bool HasPage { get; private set; }
        public bool HasDefinition { get; private set; }
        public bool HasPrimaryKey { get; private set; }
        public bool HasForeignKey { get; private set; }
        public bool HasIdentityKey { get; private set; }
        public bool HasDependency { get; private set; }
        public bool HasIndex { get; private set; }
        public bool HasStorage { get; private set; }
        public bool Append { get; private set; }
        public int Top { get; private set; }


        private bool hasRowId;
        private readonly IApplicationConfiguration cfg;

        public ApplicationCommand(IApplicationConfiguration cfg, string line)
        {
            this.cfg = cfg;
            this.Top = cfg.TopLimit;

            ParseLine(line);
        }

        protected override bool CustomerizeOption(string a)
        {
            switch (a)
            {
                case "/def":
                    HasDefinition = true;
                    return true;

                case "/s":
                    IsSchema = true;
                    return true;

                case "/t":
                    IsVertical = true;
                    return true;

                case "/if":
                    HasIfExists = true;
                    return true;

                case "/p":
                    HasPage = true;
                    return true;

                case "/all":
                    Top = 0;
                    return true;

                case "/r":
                    hasRowId = true;
                    return true;

                case "/refresh":
                    Refresh = true;
                    return true;

                case "/?":
                    HasHelp = true;
                    return true;

                case "/pk":
                    HasPrimaryKey = true;
                    return true;

                case "/fk":
                    HasForeignKey = true;
                    return true;

                case "/ik":
                    HasIdentityKey = true;
                    return true;

                case "/dep":
                    HasDependency = true;
                    return true;

                case "/ind":
                    HasIndex = true;
                    return true;

                case "/sto":
                    HasStorage = true;
                    return true;

                default:
                    if (a.StartsWith("/top:"))
                    {
                        if (int.TryParse(a.Substring(5), out var _top))
                            Top = _top;

                        return true;
                    }

                    return false;
            }
        }

        public IApplicationConfiguration Configuration => this.cfg;

        public bool HasRowId => hasRowId || Has("edit");


        public string Wildcard
        {
            get
            {
                foreach (var path in paths)
                {
                    var pathName = new PathName(path);

                    if (pathName.Wildcard != null)
                        return pathName.Wildcard;
                }

                if (Path1 == null)
                    return null;
                else
                    return Path1.Wildcard;
            }
        }

        public string Where
        {
            get
            {
                foreach (var path in paths)
                {
                    var pathName = new PathName(path);

                    if (pathName.Expression != null)
                        return pathName.Expression;
                }

                if (Path1 == null)
                    return null;
                else
                    return Path1.Expression;
            }
        }


        public string[] Columns => GetStringArray("col");

        public string[] Includes => GetStringArray("include");

        public string[] Excludes => GetStringArray("exclude");

       

        /// <summary>
        /// Get dictionary
        /// e.g. /column:file=File,type=Type
        ///   GetNapArray("column") => {file:"File", type: "Type"};
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public IDictionary<string, string> GetDictionary(string name, Dictionary<string, string> defaultValue = null)
        {
            Dictionary<string, string> dict;
            if (defaultValue != null)
            {
                dict = new Dictionary<string, string>(defaultValue);
            }
            else
            {
                dict = new Dictionary<string, string>();
            }

            string value = GetValue(name);
            if (value == null)
                return dict;

            string[] values = value.Split(',');
            foreach (string kvp in values)
            {
                string[] items = kvp.Split('=');
                string key = null;
                string val = null;
                switch (items.Length)
                {
                    case 0:
                        break;

                    case 1:
                        key = items[0];
                        val = items[0];
                        break;

                    case 2:
                        key = items[0];
                        val = items[1];
                        break;

                    default:
                        key = items[0];
                        val = items[1];
                        break;
                }

                if (key == null)
                    continue;

                if (dict.ContainsKey(key))
                    dict.Remove(key);

                dict.Add(key, val);
            }

            return dict;
        }

        public string OutputDirectory()
        {
            string path = OutputPath();
            if (path == null)
                return null;

            if (Directory.Exists(path))
                return path;

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }



        public string OutputFileName()
        {
            string path = OutputPath();
            if (path == null)
                return null;

            if (File.Exists(path))
                return path;

            if (Directory.Exists(path))
                return null;

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return path;
        }

        public string OutputPath()
        {
            Append = Has("append");

            string path = GetValue("out");
            if (path == null)
                return null;

            return path;
        }

        public string OutputPath(string configKey, string defaultPath)
        {
            return OutputPath() ?? cfg.GetValue<string>(configKey, defaultPath);
        }

        public string InputDirectory()
        {
            string path = InputPath();
            if (path == null)
                return null;

            if (Directory.Exists(path))
                return path;

            string directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory))
                return directory;

            return null;
        }

        public string[] InputFiles(bool allDirectories)
        {
            string path = InputPath();
            if (path == null)
                return null;

            if (Directory.Exists(path))
                return Directory.GetFiles(path);

            string directory = Path.GetDirectoryName(path);
            string file = Path.GetFileName(path);
            if (Directory.Exists(directory))
                return Directory.GetFiles(directory, file, allDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (File.Exists(path))
                return new string[] { path };

            return null;
        }

        public string InputPath()
        {
            string path = GetValue("in");
            if (path == null)
                return null;

            return path;
        }

        public IDictionary<string, string[]> PK
        {
            // option: /pk:table1=pk1+pk2,table2=pk1
            // option: /pk:Product=Id+Name,Supply=Id
            get
            {
                Dictionary<string, string[]> d = new Dictionary<string, string[]>();

                string option = GetValue("pk");
                if (option == null)
                    return d;

                string[] items = option.Split(',');
                foreach (string item in items)
                {
                    string[] L1 = item.Split('=');
                    if (L1.Length != 2)
                        throw new Exception($"invalid argument /pk, format is /pk:table1=pk1+pk2,table2=pk1");

                    string table = L1[0];
                    string[] L2 = L1[1].Split('+');

                    if (d.ContainsKey(table))
                        throw new Exception($"duplicated table in option /pk, format is /pk:table1=pk1+pk2,table2=pk1");

                    d.Add(table, L2);
                }

                return d;
            }
        }

        public string GetValue(string name, string configKey, string defaultValue)
        {
            return GetValue(name) ?? cfg.GetValue<string>(configKey, defaultValue);
        }
    }
}
