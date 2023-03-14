using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Runtime.Loader;
//using CSScripting;

namespace csscript
{
    /// <summary>
    /// A class that hosts the most common properties of the runtime environment.
    /// </summary>
    public static class Runtime2
    {
        /// <summary>
        /// Occurs when an exception is not caught.
        /// </summary>
        static public event UnhandledExceptionEventHandler UnhandledException;

        internal static bool RaiseUnhandledExceptionIfSubscribed(object sender, UnhandledExceptionEventArgs e)
        {
            var handlers = UnhandledException?.GetInvocationList();

            if (handlers?.Any() == true)
            {
                handlers.ForEach2(x => x.DynamicInvoke(sender, e));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns the name of the temporary folder in the CSSCRIPT subfolder of Path.GetTempPath().
        /// <para>
        /// Under certain circumstances it may be desirable to the use the alternative location for
        /// the CS-Script temporary files. In such cases use SetScriptTempDir() to set the
        /// alternative location.
        /// </para>
        /// </summary>
        /// <returns>Temporary directory name.</returns>
        static public string GetScriptTempDir()
        {
            if (tempDir == null)
            {
                tempDir = Environment.GetEnvironmentVariable("CSS_CUSTOM_TEMPDIR") ??
                          Path.GetTempPath().PathJoin2("csscript.core");

                tempDir.EnsureDir2();
            }
            return tempDir;
        }

        static string tempDir = null;

#if false
        /// <summary>
        /// Cleans the abandoned script execution cache.
        /// </summary>
        public static void CleanAbandonedCache()
        {
            var rootDir = GetScriptTempDir().PathJoin("cache");

            if (Directory.Exists(rootDir))
            {
                foreach (var cacheDir in Directory.GetDirectories(rootDir))
                    try
                    {
                        // line 0: <clr version> line 1: <dir>
                        var infoFile = cacheDir.PathJoin("css_info.txt");
                        var sourceDir = File.Exists(infoFile) ? File.ReadAllLines(infoFile)[1] : null;

                        if (sourceDir.IsEmpty() || !Directory.Exists(sourceDir))
                        {
                            cacheDir.DeleteDir();
                        }
                        else
                        {
                            string sourceName(string path) =>
                                path.GetFileNameWithoutExtension(); // remove `.dll` in `script.cs.dll`

                            var sorceFiles = Directory.GetFiles(cacheDir, "*.dll")
                                                      .Select(x => new
                                                      {
                                                          Source = sourceDir.PathJoin(sourceName(x)),
                                                          PureName = x.GetFileName().Split('.').First(),
                                                      })
                                                      .ToArray();

                            sorceFiles.Where(x => !File.Exists(x.Source))
                                      .ForEach(file => Directory.GetFiles(cacheDir, $"{file.PureName}.*")
                                                                .ForEach(x =>
                                                                {
                                                                    x.FileDelete(rethrow: false);
                                                                }));
                        }
                    }
                    catch { }
            }
        }

        /// <summary>
        /// The delegate for creating unloadable <see cref="AssemblyLoadContext"/> assembly load
        /// context. The delegate is required to be set from the host process at runtime. It is
        /// because it is not available at compile time since CSScriptLib assembly is compiled as
        /// '.NET Standard 2.0' which does not implement <see cref="AssemblyLoadContext"/> but its
        /// abstract type only.
        /// <para>
        /// CS-Script uses intensive reflection technique to retrieve the host environment <see
        /// cref="AssemblyLoadContext"/> implementation. So you no not need to set it.
        /// </para>
        /// </summary>
        internal static Func<AssemblyLoadContext> CreateUnloadableAssemblyLoadContext;

        /// <summary>
        /// Cleans the snippets.
        /// </summary>
        public static void CleanSnippets()
        {
            // CSScript.GetScriptTempDir()
            var dir = Runtime.GetScriptTempDir().PathJoin("snippets");

            if (!Directory.Exists(dir))
                return;

            var runningProcesses = Process.GetProcesses().Select(x => x.Id);

            foreach (var script in Directory.GetFiles(dir, "*.*.cs"))
            {
                int hostProcessId = int.Parse(script.GetFileName().Split('.')[1]);
                if (Process.GetCurrentProcess().Id == hostProcessId || !runningProcesses.Contains(hostProcessId))
                    script.FileDelete(rethrow: false);
            }
        }

        /// <summary>
        /// Cleans the unused temporary files.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="verifyPid">if set to <c>true</c> [verify pid].</param>
        public static void CleanUnusedTmpFiles(string dir, string pattern, bool verifyPid)
        {
            if (!Directory.Exists(dir))
                return;

            string[] oldTempFiles = Directory.GetFiles(dir, pattern);

            foreach (string file in oldTempFiles)
            {
                try
                {
                    if (verifyPid)
                    {
                        string name = Path.GetFileName(file);

                        int pos = name.IndexOf('.');

                        if (pos > 0)
                        {
                            string pidValue = name.Substring(0, pos);

                            int pid = 0;

                            if (int.TryParse(pidValue, out pid))
                            {
                                //Didn't use GetProcessById as it throws if pid is not running
                                if (Process.GetProcesses().Any(p => p.Id == pid))
                                    continue; //still running
                            }
                        }
                    }

                    file.FileDelete(false);
                }
                catch { }
            }
        }
#endif

        /// <summary>
        /// Gets the nuget cache path in the form displayable in Console.
        /// </summary>
        /// <value>The nu get cache view.</value>
        static public string NuGetCacheView => "<not defined>";

        /// <summary>
        /// Gets a value indicating whether the host OS Windows.
        /// </summary>
        /// <value><c>true</c> if the host OS is Windows; otherwise, <c>false</c>.</value>
        public static bool IsWin => !IsLinux;

        /// <summary>
        /// Note it is not about OS being exactly Linux but rather about OS having Linux type of
        /// file system. For example path being case sensitive
        /// </summary>
        public static bool IsLinux { get; } = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX);

        /// <summary>
        /// Gets a value indicating whether the runtime is core.
        /// </summary>
        /// <value><c>true</c> if the runtime is core; otherwise, <c>false</c>.</value>
        public static bool IsCore { get; } = "".GetType().Assembly.Location.Split(Path.DirectorySeparatorChar).Contains("Microsoft.NETCore.App");

        static internal string CustomCommandsDir
            => Environment.SpecialFolder.CommonApplicationData.GetPath2()
                                        .PathJoin2("cs-script", "commands")
                                        .EnsureDir2(false);

        static internal string GlobalIncludsDir
        {
            get
            {
                try
                {
                    var globalIncluds = Environment.GetEnvironmentVariable("CSSCRIPT_INC");
                    if (globalIncluds.IsNotEmpty2())
                        return globalIncluds.EnsureDir2();

                    return Environment.SpecialFolder.CommonApplicationData.GetPath2()
                                                    .PathJoin2("cs-script", "inc")
                                                    .With2(x =>
                                                    {
                                                        Environment.SetEnvironmentVariable("CSSCRIPT_INC", x, EnvironmentVariableTarget.User);
                                                    })
                                                    .EnsureDir2(rethrow: false);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Returns path to the `Microsoft.WindowsDesktop.App` shared assemblies of the compatible
        /// runtime version.
        /// <para>
        /// Note, there is no warranty that the dotnet dedktop assemblies belong to the same distro
        /// version as dotnet Core:
        /// <para>- C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.0-rc.1.20451.14</para>
        /// <para>- C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\5.0.0-rc.1.20452.2</para>
        /// </para>
        /// </summary>
        public static string DesktopAssembliesDir
        {
            get => FindLatestSharedDir("Microsoft.WindowsDesktop.App");
        }

        /// <summary>
        /// Returns path to the `Microsoft.AspNetCore.App` shared assemblies of the compatible
        /// runtime version.
        /// </summary>
        public static string WebAssembliesDir
        {
            get => FindLatestSharedDir("Microsoft.AspNetCore.App");
        }

        static string FindLatestSharedDir(string appType)
        {
            try
            {
                // There is no warranty that the dotnet dedktop assemblies belong to the same distro
                // version as dotnet Core: C:\Program
                // Files\dotnet\shared\Microsoft.NETCore.App\5.0.0-rc.1.20451.14 C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\5.0.0-rc.1.20452.2
                var netCoreDir = typeof(string).Assembly.Location.GetDirName2();
                var dir = netCoreDir.Replace("Microsoft.NETCore.App", appType);

                if (dir.DirExists2())
                    return dir; // Microsoft.WindowsDesktop.App and Microsoft.NETCore.App are of teh same version

                var desiredVersion = netCoreDir.GetFileName2();

                int howSimilar(string stringA, string stringB)
                {
                    var maxSimilariry = Math.Min(stringA.Length, stringB.Length);

                    for (int i = 0; i < maxSimilariry; i++)
                        if (stringA[i] != stringB[i])
                            return i;

                    return maxSimilariry;
                }

                var allDesktopVersionsRootDir = dir.GetDirName2();

                var allInstalledVersions = Directory.GetDirectories(allDesktopVersionsRootDir)
                                                    .Select(d => new
                                                    {
                                                        Path = d,
                                                        Version = d.GetFileName2(),
                                                        SimialrityIndex = howSimilar(d.GetFileName2(), desiredVersion)
                                                    })
                                                    .OrderByDescending(x => x.SimialrityIndex);

                return allInstalledVersions.FirstOrDefault()?.Path;
            }
            catch
            {
                return null;
            }
        }

#if false
        /// <summary>
        /// Determines whether .NET SDK is installed.
        /// </summary>
        /// <returns><c>true</c> if [is SDK installed]; otherwise, <c>false</c>.</returns>
        public static bool IsSdkInstalled()
        {
            return Globals.csc.FileExists();
            // "dotnet --list-sdks" is more accurate but it is fragile on WLS2
            //var output = "";
            //"dotnet".Run("--list-sdks", null, onOutput: x => output += x);
            //return output.IsNotEmpty();
        }
#endif
    }
}