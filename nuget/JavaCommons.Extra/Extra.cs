using System.IO;
using System.Net;
using System.Text;

namespace JavaCommons.Extra;
public static class Extra
{
    public static string GetStringFromUrl(string url)
    {
        //HttpWebRequest request = (WebRequest.Create(url) as HttpWebRequest)!;
        WebRequest request = /*System.Net.*/HttpWebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        WebHeaderCollection header = response.Headers;
        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
    public static void DownloadBinaryFromUrl(string url, string destinationPath)
    {
        WebRequest request = /*System.Net.*/HttpWebRequest.Create(url);
        var objResponse = request.GetResponse();
        byte[] buffer = new byte[32768];
        using (Stream input = objResponse.GetResponseStream())
        {
            using (FileStream output = new FileStream(destinationPath, FileMode.CreateNew))
            {
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
