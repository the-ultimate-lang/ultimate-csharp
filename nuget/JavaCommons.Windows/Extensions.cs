using System.Net;
using System.Reflection;
namespace JavaCommons.Windows
{
    public static class Extensions
    {
        public static HttpWebResponse __GetResponse(this WebClient client)
        {
            FieldInfo responseField = client.GetType().GetField("m_WebResponse", BindingFlags.Instance | BindingFlags.NonPublic);
            if (responseField != null)
            {
                HttpWebResponse response = responseField.GetValue(client) as HttpWebResponse;
                return response;
            }
            return null;
        }
    }
}
