using System;
using System.Collections;
using System.IO;
using System.Reflection;
//using System.Runtime.Loader;
using System.Xml.Linq;
using csscript;
//using CSScriptLib;

//using CSScriptLib;

//namespace CUI;

public interface ICalc
{
    int Sum(int a, int b);
}

public interface ICalc2
{
    int Sum(int a, int b);

    int Div(int a, int b);
}

static class Program
{
    ////public static bool IsCore { get; } = ((IList)"".GetType().Assembly.Location.Split(Path.DirectorySeparatorChar)).Contains("Microsoft.NETCore.App");

    [STAThread]
    static void Main()
    {
        Console.WriteLine("Hello World!");
        //LoadCode();
        Console.WriteLine(Environment.OSVersion.Platform);
        Console.WriteLine(Runtime2.IsWin);
        Console.WriteLine(Runtime2.IsCore);
        ////Console.WriteLine(IsCore);
        Console.WriteLine(Environment.Version.ToString());
        var result = NuGet.Resolve(
            new NuGetCore.PackageSpec[] { new NuGetCore.PackageSpec{ Name="JavaCommons", Version="2022.1012.147.9-beta" }});
        foreach (var s in result)
        {
            Console.WriteLine(s);
        }
    }

    public static void LoadCode()
    {
    }
}