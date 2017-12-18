using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Mono.Options;
using Framework.Log;
using Framework.IO;
using H = Framework.Security.Hash;

namespace Hash.Net45
{
    class Program
    {
        static int Main(string[] args)
        {
            string filePath = null;
            string hashType = "SHA1";
            string logPath = null;
            bool showHelp = false;

            OptionSet optionSet = new OptionSet()
            {
                {"p|path=", "file path. ex c:/dir/file ", v => filePath = v},
                {"type=", "compute hash type [MD5|SHA1|SHA256|SHA384|SHA512]. default SHA1", v => hashType = v},
                {"l|log=", "log file.", v => logPath = v},
                {"?|help", "show help.", v => showHelp = v != null}
            };

            try
            {
                List<string> extra = optionSet.Parse(args);
                //extra.ForEach(t => Console.WriteLine(t));
            }
            catch (OptionException ex)
            {
                ShowExceptionMessage(ex);
                return 1;
            }

            if (showHelp || filePath == null)
            {
                ShowUsage(optionSet);
                return 1;
            }


            try
            {
                Logger.AddConsoleTraceListener();
                Logger.AddListener(logPath);

                Logger.WriteLine("--------------------------------------------------");
                Logger.WriteLine("file path : {0}", filePath);

                DateTime startDateTime = DateTime.Now;
                Logger.WriteLine("start datetime : {0}", startDateTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));

                Stopwatch sw = new Stopwatch();
                sw.Start();


                byte[] result;
                using (FileStream input = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (FileReader file = new FileReader(input))
                {
                    Logger.WriteLine("input : {0} length : {1} byte", filePath, file.Length.ToString("#,0"));

                    if (string.Compare(hashType, "SHA1", true) == 0)
                    {
                        Logger.WriteLine("hash : SHA1");
                        result = H.Hash.ComputeSHA1(file);
                    }
                    else if (string.Compare(hashType, "SHA256", true) == 0)
                    {
                        Logger.WriteLine("hash : SHA256");
                        result = H.Hash.ComputeSHA256(file);
                    }
                    else if (string.Compare(hashType, "SHA384", true) == 0)
                    {
                        Logger.WriteLine("hash : SHA384");
                        result = H.Hash.ComputeSHA384(file);
                    }
                    else if (string.Compare(hashType, "SHA512", true) == 0)
                    {
                        Logger.WriteLine("hash : SHA512");
                        result = H.Hash.ComputeSHA512(file);
                    }
                    else
                    {
                        Logger.WriteLine("hash : MD5");
                        result = H.Hash.ComputeMD5(file);
                    }
                }

                sw.Stop();
                Logger.WriteLine("Stopwatch : {0}({1} msec)", sw.Elapsed, sw.Elapsed.TotalMilliseconds);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                    sb.Append(result[i].ToString("x2"));

                Logger.WriteLine("compute (binary) : {0}", sb.ToString());
                Logger.WriteLine("compute (base64) : {0}", Convert.ToBase64String(result));
                return 0;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message);
                return 1;
            }
            finally
            {
                Logger.ClearListener();
            }
        }

        static void ShowExceptionMessage(OptionException optionException)
        {
            Console.Error.Write("{0}: ", System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
            Console.Error.WriteLine(optionException.Message);
            Console.Error.WriteLine("Try `{0} --help' for more information.", System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
        }

        static void ShowUsage(OptionSet optionSet)
        {
            Console.Error.WriteLine("Usage: {0} [options]", System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
            Console.Error.WriteLine();
            Console.Error.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Error);
        }

    }
}
