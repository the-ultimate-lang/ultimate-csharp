using System;
using System.IO;
using System.Runtime.InteropServices;

namespace JavaCommons.Windows;

public static class Windows
{
    public static void FreeConsole()
    {
        Console.SetOut(TextWriter.Null);
        NativeMethods.FreeConsole();
    }
    public static void Message(dynamic x, string title = null)
    {
        if (title == null) title = "Message";
        if ((x as string) != null)
        {
            var s = (string)x;
            NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
            return;
        }
        if (JavaCommons.Util.FullName(x) == "System.Xml.Linq.XDocument" || JavaCommons.Util.FullName(x) == "System.Xml.Linq.XElement")
        {
            string xml = JavaCommons.Util.ToXml(x);
            System.Diagnostics.Debug.WriteLine(xml);
            NativeMethods.MessageBoxW(IntPtr.Zero, xml, title, 0);
        }
        else
        {
            string json = JavaCommons.Util.ToJson(x, true);
            NativeMethods.MessageBoxW(IntPtr.Zero, json, title, 0);
        }
    }
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern int MessageBoxA(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);
        [DllImport("kernel32.dll")]
        internal static extern bool FreeConsole();
    }
}
