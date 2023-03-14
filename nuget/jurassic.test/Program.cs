using System;
using JavaCommons;
using Jurassic;
using Jurassic.Library;

namespace CUI;

public class Math2 : ObjectInstance
{
    public Math2(ScriptEngine engine)
        : base(engine)
    {
        this.PopulateFunctions();
    }

    [JSFunction(Name = "log10")]
    public static double Log10(double num)
    {
        return Math.Log10(num);
    }
}

public static class Program
{
    public static void Main()
    {
        Util.Print(new[] { "Hello", "World" });
        Jurassic.ScriptEngine engine = new Jurassic.ScriptEngine();
        engine.SetGlobalValue("console", new Jurassic.Library.FirebugConsole(engine));
        engine.Execute("var x=123");
        engine.Execute("console.log(x)");
        engine.Execute("console.log(-9223372036854775808)");
        engine.Execute("console.log(9223372036854775807)");
        /*
        _jurassic.SetGlobalValue("tweetText", tweetText);
        var result = _jurassic.Evaluate(@"twttr.txt.parseTweet(tweetText).valid");
        return (bool)result;
        */
        engine.SetGlobalValue("math2", new Math2(engine));
        Console.WriteLine(engine.Evaluate<double>("math2.log10(1000)"));
    }
}