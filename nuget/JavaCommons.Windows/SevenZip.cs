using System.IO;
using SevenZipExtractor;

namespace JavaCommons.Windows;

public class SevenZip
{
    public static void ExtractToDirectory(string archive, string destDir)
    {
        Directory.CreateDirectory(destDir);
        using (ArchiveFile archiveFile = new ArchiveFile(archive))
        {
            archiveFile.Extract(destDir); // extract all
        }
    }

    public static void ExtractToDirectory(Stream archive, string destDir)
    {
        Directory.CreateDirectory(destDir);
        using (ArchiveFile archiveFile = new ArchiveFile(archive))
        {
            archiveFile.Extract(destDir); // extract all
        }
    }

}
