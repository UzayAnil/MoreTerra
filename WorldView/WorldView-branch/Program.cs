﻿using System.Runtime.InteropServices;

public class Win32
{
    /// <summary>
    /// Allocates a new console for current process.
    /// </summary>
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();

    /// <summary>
    /// Frees the console.
    /// </summary>
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();
}


namespace WorldView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using NDesk.Options;
    using System.IO;

    using WorldView;

    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Initialize Manager
            ResourceManager.Instance.Initialize();
            SettingsManager.Instance.Initialize();
         
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WorldViewForm());
                SettingsManager.Instance.Shutdown();                
            }
            else
            {
                Win32.AllocConsole();

                //for (int i = 1; i < args.Length; i += 2)
                //{
                //    MessageBox.Show(args[i - 1] + args[i]);
                //}
                
                bool show_help = false;
                string worldPath = string.Empty;
                string mapPath = string.Empty;

                var p = new OptionSet () {
                    { "w|world=", "The path to the {WORLD} to map.",
                       v => worldPath = v },
                    { "o|output=", "The path to the {OUTPUT} file where the map PNG will be written.",
                       v => mapPath = v},
                    { "h|help",  "Show this message and exit.", 
                       v => show_help = v != null },
                };

                List<string> extra = new List<string>();

                try
                {
                    extra = p.Parse(args);
                }
                catch (OptionException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try '" + System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) + " --help' for more information.");
                }


                if (show_help)
                {
                    Console.WriteLine("Generates a PNG from a Terraria World file (*.wld).");
                    Console.WriteLine();
                    Console.WriteLine("Usage:");
                    Console.WriteLine(" " + System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToUpper() + " [option1] <path1> [option2] <path2>");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    p.WriteOptionDescriptions(Console.Out);
                }
                else if (worldPath == string.Empty || !File.Exists(worldPath))
                {
                    Console.WriteLine("The World file could not be found.");
                }
                else
                {
                    Console.WriteLine("Generating map from: " + worldPath);                    

                    WorldMapper mapper = new WorldMapper();

                    mapper.Initialize();
                    mapper.OpenWorld(worldPath);
                    mapper.CreatePreviewPNG(mapPath);
                    mapper.CloseWorld();
                    Console.WriteLine("Done! Saved to: " + mapPath);
                }

                //Console.ReadLine();
                Win32.FreeConsole();
            }          
        }
    }
}