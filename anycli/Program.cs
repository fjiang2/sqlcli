﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Sys;
using Tie;
using syscon.stdio;

namespace anycli
{
    class Program
    {
        public static ApplicationConfiguration Configuration;

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "anycli";
            Console.WriteLine($"Any command tool interface [Version {ShellHelp.ApplicationVerison}]");
            Console.WriteLine($"Copyright (c) 2021-{DateTime.Today.Year} Datconn. All rights reserved.");
            Console.WriteLine();

            Constant.MAX_CPU_REG_NUM = 2 * 1024;
            Constant.MAX_STRING_SIZE = 24 * 1024 * 1024;
            Constant.MAX_SRC_COL = 24 * 1024 * 1024;
            Constant.MAX_INSTRUCTION_NUM = 1 * 1024 * 1024;
            var cfg = ConfigurationEnvironment.PrepareConfiguration(false);

            int i = 0;
            while (i < args.Length)
            {
                switch (args[i++])
                {
                    case "/cfg":
                        if (i < args.Length && !args[i].StartsWith("/"))
                        {
                            cfg.Personal = args[i++];
                            goto L1;
                        }
                        else
                        {
                            Cout.WriteLine("/cfg configuration file missing");
                            return;
                        }

                    case "/h":
                    case "/?":
                        anycli.Main.ShowHelp();
                        return;
                }

            }

            L1:


            Configuration = new ApplicationConfiguration();
            try
            {

                if (!Configuration.Initialize(cfg))
                    return;
            }
            catch (Exception ex)
            {
                Cout.WriteLine("error on configuration file {0}, {1}:", cfg.Personal, ex.Message);
                return;
            }

            //Config.Init(Configuration);

#if DEBUG
            var site = new Main(Configuration);
            site.Run(args);
#else
            try
            {
                var site = new Main(Configuration);
                site.Run(args);
            }
            catch (Exception ex)
            {
                cerr.WriteLine(ex.Message);
                //stdio.ShowError(ex.StackTrace);
                cerr.WriteLine("fatal error, hit any key to exit");
                cin.ReadKey();
            }
#endif
        }


    }
}
