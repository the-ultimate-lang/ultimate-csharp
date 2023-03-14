#if false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenZipExtractor;

namespace JavaCommons.Extra;
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
#endif