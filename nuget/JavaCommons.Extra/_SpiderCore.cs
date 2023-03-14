#if false
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JavaCommons
{
    public class SpiderCore
    {
        public static readonly dynamic? Config;
        public static readonly List<string> PathList;
        static SpiderCore()
        {
            SpiderCore.Config = Util.ResourceAsJson(typeof(SpiderCore).Assembly, "JavaCommons.spider.config.json");
            SpiderCore.PathList = SpiderCore._CalculatePathList();
        }
        public static string RepoPath()
        {
            return Dirs.DocumentsPath(".repo");
        }
        public static string RepoPath(string name)
        {
            return RepoPath() + @"\" + name;
        }
        public static string SoftwarePath()
        {
            return Dirs.ProfilePath(".software-2");
        }
        public static string SoftwarePath(string name, string version)
        {
            return SoftwarePath() + @"\" + name + @"-" + version;
        }
        public static string SoftwarePath(string name, string version, string path)
        {
            var pathList = path !.Split(';').ToList();
            string root = SoftwarePath(name, version);
            for (int i = 0; i < pathList.Count; i++)
            {
                string e = pathList[i];
                if (e == ".")
                {
                    pathList[i] = root;
                }
                else
                {
                    pathList[i] = (root + pathList[i]).Replace("/", @"\");
                }
            }
            return String.Join(";", pathList);
        }
        public static dynamic? GetSoftwareArray()
        {
            //var array = Util.FromJson(Util.HttpGetString("https://github.com/spider-explorer/spider-programs-2/releases/download/64bit/00-software.json")).software;
            var array = Util.FromJson(Util.HttpGetString("https://github.com/spider-explorer/spider-programs/releases/download/64bit/00-software.json"))?.software;
            return array;
        }
        private static List<string> _CalculatePathList()
        {
            var array = JavaCommons.SpiderCore.GetSoftwareArray();
            //Util.Print(array);
            string path = System.Environment.GetEnvironmentVariable("Path")!;
            var pathList = path !.Split(';').ToList();
            if (array != null)
            {
                foreach (var e in array)
                {
                    pathList.Insert(0, JavaCommons.SpiderCore.SoftwarePath((string)e.name, (string)e.path));
                }
            }
            return pathList;
        }
        public static string? Which(string name, List<string> pathList)
        {
            foreach (var e in pathList)
            {
                var fullPath = e + @"\" + name;
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }
        public static void OpenNyagos(string repoName)
        {
            string homeDir = SpiderCore.RepoPath("home");
            string repoDir = SpiderCore.RepoPath(repoName);
            //Util.Print(repoDir);
            var pathList = SpiderCore.PathList;
            //Util.Print(SpiderCore.Which("nyagos.exe", pathList));
            //Util.Print(SpiderCore.Which("wt.exe", pathList));
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.EnvironmentVariables["HOME"] = homeDir;
            startInfo.EnvironmentVariables["PATH"] = String.Join(";", pathList);
            startInfo.WorkingDirectory = repoDir;
            startInfo.FileName = SpiderCore.Which("wt.exe", pathList);
            var arguments = new List<string> {
                "nt", "-d", repoDir, "nyagos.exe"
            };
            startInfo.Arguments = String.Join(" ", arguments);
            Process.Start(startInfo);
        }
    }
}
#endif