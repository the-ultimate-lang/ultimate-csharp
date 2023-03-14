#if false
using System;
using System.IO;
namespace JavaCommons;
public class Prepare7zDll
{
    private static readonly string ProfDir;
    private static readonly string SevenZipDir;
    private static readonly string? DllName;
    public static readonly string SevenZipDll;
    static Prepare7zDll()
    {
        ProfDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //Console.WriteLine(ProfDir);
        SevenZipDir = ProfDir + @"\.7z";
        //Console.WriteLine(SevenZipDir);
        if (IntPtr.Size == 4)
        {
            //Console.WriteLine("32ビットで動作しています");
            DllName = "7z2201-x32.dll";
        }
        else if (IntPtr.Size == 8)
        {
            //Console.WriteLine("64ビットで動作しています");
            DllName = "7z2201-x64.dll";
        }
        byte[] binary;
        using (var s = typeof(Prepare7zDll).Assembly.GetManifestResourceStream("JavaCommons.Extra." + DllName))
        {
            binary = new byte[(int)s.Length];
            s.Read(binary, 0, (int)s.Length);
        }
        //Console.WriteLine(binary.Length);
        SevenZipDll = SevenZipDir + @"\" + DllName;
        Directory.CreateDirectory(Path.GetDirectoryName(SevenZipDll));
        if (!File.Exists(SevenZipDll))
        {
            File.WriteAllBytes(SevenZipDll, binary);
        }
    }
}
#endif