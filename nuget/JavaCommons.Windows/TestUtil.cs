#nullable enable
#pragma warning disable CS0618
using System;
using System.IO;
using JavaCommons.Json;
using JavaCommons.Json.Bson;
using JavaCommons.Json.Linq;

namespace JavaCommons.Windows;

public static class TestUtil
{
    public static string __ToJson(dynamic x, bool indent = false)
    {
        return JsonConvert.SerializeObject(x, indent ? Formatting.Indented : Formatting.None);
    }
    public static dynamic? __FromJson(string json)
    {
        if (String.IsNullOrEmpty(json)) return null;
        return JsonConvert.DeserializeObject(json, new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.None
        });
    }
    public static T? __FromJson<T>(string json, T? fallback = default(T))
    {
        //if (String.IsNullOrEmpty(json)) return default(T);
        if (String.IsNullOrEmpty(json)) return fallback;
        return JsonConvert.DeserializeObject<T>(json);
    }
    public static dynamic? __FromObject(dynamic x)
    {
        if (x == null) return null;
        var o = (dynamic)JObject.FromObject(new { x = x },
                           new JsonSerializer
                           {
                               DateParseHandling = DateParseHandling.None
                           });
        return o.x;
    }
    public static byte[] __ToBson(dynamic x)
    {
        MemoryStream ms = new MemoryStream();
        using (BsonWriter writer = new BsonWriter(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(writer, x);
        }
        return ms.ToArray();
    }
    public static dynamic? __FromBson(byte[] bson)
    {
        if (bson == null) return null;
        MemoryStream ms = new MemoryStream(bson);
        using (BsonReader reader = new BsonReader(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Deserialize(reader);
        }
    }
    public static T? __FromBson<T>(byte[] bson)
    {
        if (bson == null) return default(T);
        MemoryStream ms = new MemoryStream(bson);
        using (BsonReader reader = new BsonReader(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }
    }
    public static T? __FromObject<T>(dynamic x)
    {
        byte[] bson = __ToBson(x);
        return __FromBson<T>(bson);
    }
}
