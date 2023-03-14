using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JavaCommons;

public class Program
{
    static void AddToAssemblyList(List<Assembly> list, Assembly a)
    {
        Util.Print($"Loading: {a.GetName().Name}");
        list.Add(a);
    }

    public static void Main(string[] args)
    {
        var version = Util.ResourceAsText(typeof(Program).Assembly, "jcs.version.txt");
        Util.Print($"jcs v{version}");
        //Util.Print($"jcs v{typeof(Program).Assembly.GetName().Version}");
        if (args.Length == 0) Environment.Exit(1);
        string script = File.ReadAllText(args[0]);
        List<Assembly> extraAssemblies = new List<Assembly>();
        AddToAssemblyList(extraAssemblies, typeof(Program).Assembly); // ProcessX
        var jcAssembly = typeof(JavaCommons.Util).Assembly;
        AddToAssemblyList(extraAssemblies, jcAssembly);
        var jvExAssembly = typeof(JavaCommons.TwitterUtil).Assembly;
        AddToAssemblyList(extraAssemblies, jvExAssembly);
        AddToAssemblyList(extraAssemblies, typeof(Cysharp.Diagnostics.ProcessX).Assembly); // ProcessX
        AddToAssemblyList(extraAssemblies, typeof(Tweetinvi.TwitterClient).Assembly); // Tweetinvi
        var scripting = new JavaCommons.Scripting(false,
            new string[] { "JCS", "Zx" },
            extraAssemblies.ToArray()
            );
        bool success = scripting.Exec(
            script,
            new MyArgs()
            {
                args = args.Skip(1).ToArray()
            });
        if (!success) Environment.Exit(1);
        Environment.Exit(0);
    }

    public class MyArgs
    {
        public string[] args { get; set; }
    }
}