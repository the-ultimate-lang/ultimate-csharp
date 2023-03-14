using System.Drawing;
using System.IO;
using System.Net;
namespace JavaCommons.Windows;
public class ImageUtil
{
    public static Image GetImageFromUrl(string url)
    {
        using (WebClient webClient = new WebClient())
        {
            byte[] data = webClient.DownloadData(url);
            using (MemoryStream mem = new MemoryStream(data))
            {
                return new Bitmap(Image.FromStream(mem));
            }
        }
    }
}
