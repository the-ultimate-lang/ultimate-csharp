//using CSScripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using JavaCommons;

//using CSScriptLib;

namespace csscript
{
    internal static class Extension
    {
        internal static Process RunAsync(this string exe, string args, string dir = null)
        {
            var process = new Process();

            process.StartInfo.FileName = exe;
            process.StartInfo.Arguments = args;
            process.StartInfo.WorkingDirectory = dir;

            // hide terminal window
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            return process;
        }

        internal static int Run(this string exe, string args, string dir = null, Action<string> onOutput = null,
            Action<string> onError = null)
        {
            var process = RunAsync(exe, args, dir);

            var error = StartMonitor(process.StandardError, onError);
            var output = StartMonitor(process.StandardOutput, onOutput);

            process.WaitForExit();

            return process.ExitCode;
        }

        internal static Thread StartMonitor(StreamReader stream, Action<string> action = null)
        {
            var thread = new Thread(x =>
            {
                try
                {
                    string line = null;
                    while (null != (line = stream.ReadLine()))
                        action?.Invoke(line);
                }
                catch
                {
                }
            });
            thread.Start();
            return thread;
        }

        /// <summary>
        /// A list of XElement descendent elements with the supplied local name (ignoring any namespace), or null if the element is not found.
        /// </summary>
        internal static IEnumerable<XElement> FindDescendants(this XElement likeThis, string elementName)
        {
            // https://9to5answer.com/use-linq-to-xml-with-xml-namespaces
            var result = likeThis.Descendants().Where(ele => ele.Name.LocalName == elementName);
            return result;
        }

        /// <summary>
        /// Selects the first element that satisfies the specified path.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="path">The path.</param>
        /// <returns>Selected XML element</returns>
        internal static XElement SelectFirst(this XContainer element, string path)
        {
            string[] parts = path.Split('/');

            var e = element.Elements()
                .Where(el => el.Name.LocalName == parts[0])
                .GetEnumerator();

            if (!e.MoveNext())
                return null;

            if (parts.Length == 1) //the last link in the chain
                return e.Current;
            else
                return e.Current.SelectFirst(path.Substring(parts[0].Length + 1)); //be careful RECURSION
        }

        internal static string TrimMatchingQuotes(this string input, char quote)
        {
            if (input.Length >= 2)
            {
                //"-sconfig:My Script.cs.config"
                if (input.First() == quote && input.Last() == quote)
                {
                    return input.Substring(1, input.Length - 2);
                }
                //-sconfig:"My Script.cs.config"
                else if (input.Last() == quote)
                {
                    var firstQuote = input.IndexOf(quote);
                    if (firstQuote != input.Length - 1) //not the last one
                        return input.Substring(0, firstQuote) +
                               input.Substring(firstQuote + 1, input.Length - 2 - firstQuote);
                }
            }

            return input;
        }

        internal static string[] RemovePathDuplicates(this string[] list)
        {
            return list.Where(x => x != "" /*x.IsNotEmpty()*/)
                .Select(x =>
                {
                    var fullPath = Path.GetFullPath(x);
                    if (File.Exists(fullPath))
                        return fullPath;
                    else
                        return x;
                })
                .Distinct()
                .ToArray();
        }
        internal static string GetFileName2(this string path) => Path.GetFileName(path);
        internal static bool Contains2(this string text, string pattern, bool ignoreCase)
            => text.IndexOf(pattern, ignoreCase ? StringComparison.OrdinalIgnoreCase : default(StringComparison)) != -1;
        internal static string GetDirName2(this string path)
            => path == null ? null : Path.GetDirectoryName(path);
        internal static string PathJoin2(this string path, params object[] parts)
        {
            var allParts = new[] { path ?? "" }.Concat(parts.Select(x => x?.ToString() ?? ""));
            return Path.Combine(allParts.ToArray());
        }
        internal static bool IsEmpty2(this string text) => string.IsNullOrEmpty(text);
        internal static string EnsureDir2(this string path, bool rethrow = true)
        {
            try
            {
                Directory.CreateDirectory(path);

                return path;
            }
            catch { if (rethrow) throw; }
            return null;
        }
        internal static string DeleteDir2(this string path, bool handleExceptions = false, bool doNotDeletеRoot = false)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    void del_dir(string d)
                    {
                        try { Directory.Delete(d); }
                        catch (Exception)
                        {
                            Thread.Sleep(1);
                            Directory.Delete(d);
                        }
                    }

                    var dirs = new Queue<string>();
                    dirs.Enqueue(path);

                    while (dirs.Any())
                    {
                        var dir = dirs.Dequeue();

                        foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                            File.Delete(file);

                        Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                            .ForEach2(dirs.Enqueue);
                    }

                    var emptyDirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                        .Reverse();

                    emptyDirs.ForEach2(del_dir);

                    if (!doNotDeletеRoot)
                        del_dir(path);
                }
                catch
                {
                    if (!handleExceptions) throw;
                }
            }
            return path;
        }
        internal static IEnumerable<T> ForEach2<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
            return collection;
        }
        internal static string GetPath2(this Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }
        internal static bool IsNotEmpty2(this string text) => !string.IsNullOrEmpty(text);
        internal static T With2<T>(this T @object, Action<T> action)
        {
            action(@object);
            return @object;
        }
        public static bool DirExists2(this string path) => path.IsNotEmpty2() ? Directory.Exists(path) : false;
    }

    class PackageInfo
    {
        public string SpecFile;
        public string Version;
        public string Name;
    }

    // Tempting to use "NuGet.Core" NuGet package to avoid deploying and using nuget.exe.
    // However it is not compatible with .NET Core runtime (at least as at 19.05.2018)
    // Next candidate is the REST API (e.g. https://api-v2v3search-0.nuget.org/query?q=cs-script&prerelease=false)
    public class NuGet
    {
        static NuGetCore _nuget = new NuGetCore();

        static public string[] Resolve(params NuGetCore.PackageSpec[] packages)
            => _nuget.Resolve(packages);
    }

    public class NuGetCore
    {
        public struct PackageSpec
        {
            public string Name;
            public string Version;
        }

        //https://docs.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders
        // .NET Mono, .NET Core
        public string NuGetCache => Runtime2.IsWin
            ? Environment.ExpandEnvironmentVariables(@"%userprofile%\.nuget\packages")
            : "~/.nuget/packages";

        ////public bool NewPackageWasInstalled { get; set; }

        internal static bool IsRuntimeCompatibleAsm(string file)
        {
            try
            {
                System.Reflection.AssemblyName.GetAssemblyName(file);
                return true;
            }
            catch
            {
            }

            return false;
        }

        public void InstallPackage(string packageNameMask, string version = null)
        {
            var packages = new string[0];
            //index is 1-based, exactly as it is printed with ListPackages
            if (int.TryParse(packageNameMask, out int index))
            {
                var all_packages = ListPackages();
                if (0 < index && index <= all_packages.Count())
                    packages = new string[] { all_packages[index - 1] };
                else
                    Console.WriteLine("There is no package with the specified index");
            }
            else
            {
                // Regex is too much at this stage
                // string pattern = CSSUtils.ConvertSimpleExpToRegExp();
                // Regex wildcard = new Regex(pattern, RegexOptions.IgnoreCase);

                if (packageNameMask.EndsWith("*"))
                    packages = ListPackages()
                        .Where(x => x.StartsWith(packageNameMask.Substring(0, packageNameMask.Length - 1))).ToArray();
                else
                    packages = new[] { packageNameMask };
            }

            // C:\Users\user\AppData\Local\Temp\csscript.core\.nuget\333
            var nuget_dir = Runtime2.GetScriptTempDir()
                .PathJoin2(".nuget", Process.GetCurrentProcess().Id)
                .EnsureDir2();

            try
            {
                var proj_template = nuget_dir.PathJoin2("build.csproj");

                if (!File.Exists(proj_template))
                {
                    "dotnet".Run("new console", nuget_dir);
                    foreach (var name in packages)
                    {
                        var ver = "";
                        if (version != null)
                            ver = "-v " + version;
                        "dotnet".Run($"add package {name} {ver}", nuget_dir, x => Console.WriteLine(x));
                    }

                    // intercept and report incompatible packages (maybe)
                }
            }
            finally
            {
                Task.Run(() =>
                {
                    nuget_dir.DeleteDir2();
                    ClearAnabdonedNugetDirs(nuget_dir.GetDirName2());
                });
            }
        }

        void ClearAnabdonedNugetDirs(string nuget_root)
        {
            // not implemented yet
            foreach (var item in Directory.GetDirectories(nuget_root))
            {
                if (int.TryParse(item.GetFileName2(), out int proc_id))
                {
                    if (Process.GetProcessById(proc_id) == null)
                        try
                        {
                            item.DeleteDir2();
                        }
                        catch
                        {
                        }
                }
            }
        }

        IEnumerable<PackageInfo> ResolveDependenciesFor(IEnumerable<PackageInfo> packages)
        {
            var result = new List<PackageInfo>(packages);
            var queue = new Queue<PackageInfo>(packages);
            while (queue.Any())
            {
                PackageInfo item = queue.Dequeue();

                IEnumerable<XElement> dependencyPackages;

                var dependenciesSection = XElement.Parse(File.ReadAllText(item.SpecFile))
                    .FindDescendants("dependencies")
                    .FirstOrDefault();
                if (dependenciesSection == null)
                    continue;

                // <dependencies>
                //   <group targetFramework=".NETStandard2.0">
                //     <dependency id="Microsoft.Extensions.Logging.Abstractions" version="2.1.0" exclude="Build,Analyzers" />
                var frameworks = dependenciesSection.FindDescendants("group");
                if (frameworks.Any())
                {
                    IEnumerable<XElement> frameworkGroups = dependenciesSection.FindDescendants("group");

                    dependencyPackages = GetCompatibleTargetFramework(frameworkGroups, item)
                                             ?.FindDescendants("dependency")
                                         ?? new XElement[0];
                }
                else
                    dependencyPackages = dependenciesSection.FindDescendants("dependency");

                Util.Print(dependencyPackages, "dependencyPackages");
                foreach (var element in dependencyPackages)
                {
                    var newPackage = new PackageInfo
                    {
                        Name = element.Attribute("id").Value,
                        Version = element.Attribute("version").Value,
                        ////PreferredRuntime = item.PreferredRuntime
                    };

                    newPackage.SpecFile =
                        NuGetCache.PathJoin2(newPackage.Name, newPackage.Version, newPackage.Name + ".nuspec");

                    if (!result.Any(x => x.Name == newPackage.Name) && File.Exists(newPackage.SpecFile))
                    {
                        queue.Enqueue(newPackage);
                        result.Add(newPackage);
                    }
                }
            }

            Util.Print(result, "result");
            return result.ToArray();
        }

        /// <summary>
        /// Gets the compatible target framework. Similar to `GetPackageCompatibleLib` but relies on NuGet spec file
        /// </summary>
        /// <param name="freameworks">The frameworks.</param>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        XElement GetCompatibleTargetFramework(IEnumerable<XElement> freameworks, PackageInfo package)
        {
            // https://docs.microsoft.com/en-us/dotnet/standard/frameworks
            // netstandard?.?
            // netcoreapp?.?
            // net?? | net???
            // ""  (no framework element, meaning "any framework")
            // Though packages use Upper case with '.' preffix: '<group targetFramework=".NETStandard2.0">'

            XElement findMatch(Predicate<string> matchTest)
            {
                var items = freameworks.Select(x => new { Name = x.Attribute("targetFramework")?.Value, Element = x })
                    .OrderByDescending(x => x.Name)
                    .ToArray();

                var match = items.FirstOrDefault(x => matchTest(x.Name ?? ""))?.Element ?? // exact match
                            items.FirstOrDefault(x => x.Name == null)
                                ?.Element; // universal dependency specified by not supplying targetFramework element

                return match;
            }

            /*
            if (package.PreferredRuntime != null)
            {
                // by requested runtime
                return findMatch(x => x.Contains(package.PreferredRuntime));
            }
            else
            */
            {
#if false
                if (CSharpCompiler.DefaultCompilerRuntime == DefaultCompilerRuntime.Standard)
                {
                    // by configured runtime
                    return findMatch(x => x.Contains("netstandard", ignoreCase: true));
                }
                else
#endif
                {
                    if (Runtime2.IsCore)
                        // by runtime of the host
                        return findMatch(x => x.Contains2("netcore", ignoreCase: true))
                               ?? findMatch(x => x.Contains2("netstandard", ignoreCase: true));
                    else
                        // by .NET full as tehre is no other options
#if false
                        return findMatch(x => (x.StartsWith("net", ignoreCase: true)
                                               || x.StartsWith(".net", ignoreCase: true))
                                         && !x.Contains("netcore", ignoreCase: true)
                                         && !x.Contains("netstandard", ignoreCase: true))
                               ?? findMatch(x => x.Contains("netstandard", ignoreCase: true));
#else
                        return findMatch(x => (x.ToLower().StartsWith("net")
                                               || x.ToLower().StartsWith(".net"))
                                              && !x.Contains2("netcore", ignoreCase: true)
                                              && !x.Contains2("netstandard", ignoreCase: true))
                               ?? findMatch(x => x.Contains2("netstandard", ignoreCase: true));
#endif
                }
            }
        }

        /// <summary>
        /// Gets the package compatible library. Similar to `GetCompatibleTargetFramework` but relies on file structure
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns></returns>
        string GetPackageCompatibleLib(PackageInfo package)
        {
            var libDir = package.SpecFile.GetDirName2().PathJoin2("lib");

            if (!Directory.Exists(libDir))
                return null;

            var frameworks = Directory.GetDirectories(package.SpecFile.GetDirName2().PathJoin2("lib"))
                .OrderByDescending(x => x)
                .Select(x => new { Runtime = x.GetFileName2(), Path = x });
            
            Util.Print(frameworks.Select(x => x.Runtime), "frameworks");

#if false
            if (Runtime2.IsCore)
                return (frameworks.FirstOrDefault(x => x.Runtime.ToLower().StartsWith("netcore"))
                        ?? frameworks.FirstOrDefault(x => x.Runtime.ToLower().StartsWith("netstandard")))?.Path;
            else
                return frameworks.FirstOrDefault(x => x.Runtime.ToLower().StartsWith("net")
                                                      && !x.Runtime.ToLower().StartsWith("netcore")
                                                      && !x.Runtime.ToLower().StartsWith("netstandard"))?.Path;
#else
            string found = null;
            var frameworkNames = frameworks.Select(x => x.Runtime);
            foreach (var frameworkName in frameworkNames)
            {
                if (frameworkName.StartsWith("net4"))
                {
                    found = frameworkName;
                    break;
                }
                if (frameworkName.StartsWith("netstandard") && frameworkName != "netstandard2.1")
                //if (frameworkName == "netstandard2.0")
                {
                    found = frameworkName;
                    break;
                }
            }

            Util.Print(found, "found");
            if (found == null) return null;
            return frameworks.First(x => x.Runtime == found).Path;
#endif
        }

        string[] GetCompatibleAssemblies(PackageInfo package)
        {
            Util.Print(package, $"GetCompatibleAssemblies({package.Name}, {package.Version})");
            var lib = GetPackageCompatibleLib(package);
            if (lib != null)
                return Directory.GetFiles(GetPackageCompatibleLib(package), "*.dll")
                    .Where(item => !item.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase))
                    .Where(x => /*Utils.*/IsRuntimeCompatibleAsm(x))
                    .ToArray();
            else
                return new string[0];
        }

        public string[] ListPackages()
        {
            return Directory.GetDirectories(NuGetCache)
                .Select(x =>
                {
                    var spec = Directory.GetFiles(x, "*.nuspec", SearchOption.AllDirectories).FirstOrDefault();
                    if (spec != null)
                    {
                        return XDocument.Load(spec)
                            .SelectFirst("package/metadata/id")?.Value;
                    }

                    return null;
                })
                .Where(x => !x.IsEmpty2())
                .ToArray();
        }

        PackageInfo FindPackage(string name, string version)
        {
            var packages = Directory.GetDirectories(NuGetCache, name, SearchOption.TopDirectoryOnly)
                .SelectMany(x =>
                {
                    return Directory.GetFiles(x, "*.nuspec", SearchOption.AllDirectories)
                        .Select(spec =>
                        {
                            if (spec != null)
                            {
                                var doc = XDocument.Load(spec);
                                return new PackageInfo
                                {
                                    SpecFile = spec,
                                    Version = doc.SelectFirst("package/metadata/version")?.Value,
                                    Name = doc.SelectFirst("package/metadata/id")?.Value
                                };
                            }

                            return null;
                        });
                })
                .OrderByDescending(x => x.Version)
                .Where(x => x != null)
                .ToArray();

            return packages.FirstOrDefault(x => x.Name == name && (version.IsEmpty2() || version == x.Version));
        }

        public string[] Resolve(NuGetCore.PackageSpec[] packages)
        {
            var assemblies = new List<string>();
            var all_packages = new List<PackageInfo>();

            bool promptPrinted = false;
            foreach (NuGetCore.PackageSpec item in packages)
            {
                string package = item.Name;

                string packageVersion = (item.Version == null || item.Version == "*" || item.Version.IsEmpty2())
                    ? ""
                    : item.Version;

                var package_info = FindPackage(package, packageVersion);
                Util.Print(package_info);

                if (package_info == null)
                {
                    if (!promptPrinted)
                        Console.WriteLine("NuGet> Processing NuGet packages...");

                    promptPrinted = true;

                    try
                    {
                        Util.Print(new [] {package, packageVersion });
                        InstallPackage(package, packageVersion);
                        package_info = FindPackage(package, packageVersion);
                    }
                    catch
                    {
                    }

                    try
                    {
                        File.SetLastWriteTimeUtc(package_info.SpecFile, DateTime.Now.ToUniversalTime());
                    }
                    catch
                    {
                    }
                }

                if (package_info == null)
                    throw new ApplicationException("Cannot process NuGet package '" + package + "'");

                all_packages.Add(package_info);
            }
            
            Util.Print(all_packages);

            foreach (PackageInfo package in ResolveDependenciesFor(all_packages))
            {
                assemblies.AddRange(GetCompatibleAssemblies(package));
            }

            return assemblies.ToArray().RemovePathDuplicates();
        }
    }
}