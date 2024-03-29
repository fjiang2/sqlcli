﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Sys.Stdio;

namespace Sys
{

    public static class ConfigurationEnvironment
    {
        private const string USER_CFG_TEMPLATE = "user.ini";
        private const string USER_CFG = "user.cfg";

        public static string CompanyName { get; set; } = GetAttribute<AssemblyCompanyAttribute>().Company;
        public static string ProductName { get; private set; } = GetAttribute<AssemblyProductAttribute>().Product;
        public static string MyDocuments => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + ProductName;

        public static ConfigurationPath Path { get; } = new ConfigurationPath
        {
            System = $"{ProductName}.cfg",
            Personal = USER_CFG,
        };

        public static Configuration Load(string productName = null)
        {
            if (productName != null)
                ProductName = productName;

            var cfg = PrepareConfiguration(false);

            var Configuration = new Configuration();

            try
            {
                if (!Configuration.Initialize(cfg))
                    return null;
            }
            catch (Exception ex)
            {
                cout.WriteLine("error on configuration file {0}, {1}:", cfg.Personal, ex.Message);
                return null;
            }

            return Configuration;
        }

        private static T GetAttribute<T>() where T : Attribute
        {
            T[] attributes = (T[])Assembly.GetEntryAssembly().GetCustomAttributes(typeof(T), false);
            return attributes[0];
        }

        public static ConfigurationPath PrepareConfiguration(bool overwrite)
        {
            string usercfgFile = PrepareUserConfiguration(false);

            Path.System = $"{ProductName}.cfg";
            Path.Personal = usercfgFile;
            return Path;
        }

        private static string PrepareUserConfiguration(bool overwrite)
        {
            string cfgFile = USER_CFG;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            folder = System.IO.Path.Combine(folder, CompanyName, ProductName);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            bool exists = File.Exists(cfgFile);
            string file = System.IO.Path.Combine(folder, cfgFile);
            try
            {
                if (!File.Exists(file))
                {
                    if (!exists)
                    {
                        if (File.Exists(USER_CFG_TEMPLATE))
                        {
                            //copy user.cfg template
                            File.Copy(USER_CFG_TEMPLATE, file);
                        }
                        else
                        {
                            //create empty text file if file is missing
                            File.Create(file).Dispose();
                        }
                    }
                    else
                        File.Copy(cfgFile, file);

                    return file;
                }

                if (exists)
                {
                    FileInfo f = new FileInfo(file);
                    if (f.Length == 0)
                        overwrite = true;

                    if (!overwrite)
                    {
                        FileInfo c = new FileInfo(cfgFile);
                        overwrite = c.LastWriteTime > f.LastWriteTime;
                    }

                    if (overwrite)
                        File.Copy(cfgFile, file, true);
                }
            }
            catch (Exception ex)
            {
                cerr.WriteLine($"failed to initialize {file}, {ex.Message}");
            }

            return file;
        }
    }
}
