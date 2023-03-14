using System;
using System.Collections.Generic;
using JavaCommons;

namespace JCS;

public static class jcs
{
    public static readonly string jcs_version;

    static jcs()
    {
        jcs.jcs_version = Util.ResourceAsText(typeof(Program).Assembly, "jcs.version.txt");
    }

    public static void print(dynamic x, string title = null)
    {
        Util.Print(x, title);
    }

    public static string getenv(string name)
    {
        return Environment.GetEnvironmentVariable(name);
    }

    public static List<string> lines_to_list(string input)
    {
        List<string> list = new List<string>(
            input.Split(new string[] { "\r\n", "\n" },
                StringSplitOptions.None));
        return list;
    }
}