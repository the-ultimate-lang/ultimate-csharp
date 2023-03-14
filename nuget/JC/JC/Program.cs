using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Xml.Linq;
using JavaCommons;
using JavaCommons.Json.Linq;
using NUnit.Framework;
using System.Linq;
Console.WriteLine("Version: {0}", Environment.Version.ToString());
Util.Print(DateTime.Now);
Console.WriteLine(JavaCommons.Prepare7zDll.SevenZipDll);
{
    var json = Util.ToJson(new[] { 777, 888 });
    Util.Print(json);
    Assert.AreEqual("[777,888]", json);
}
{
    var jsonData = Util.FromObject(new[] { 777, 888 });
    Util.Print(jsonData);
    Assert.AreEqual("[777,888]", Util.ToJson(jsonData));
    Util.Print(jsonData[0]);
    Assert.AreEqual(777, (int)jsonData[0]);
    jsonData = Util.FromObject(jsonData);
    Util.Print(jsonData);
    Assert.AreEqual("[777,888]", Util.ToJson(jsonData));
    jsonData = Util.FromObject(new { a = 1, b = 2 });
    Util.Print(jsonData);
    Assert.AreEqual("{\"a\":1,\"b\":2}", Util.ToJson(jsonData));
    Util.Print(jsonData.a);
    Assert.AreEqual("1", Util.ToJson(jsonData.a));
    jsonData.a = 777;
    Util.Print(jsonData);
    Assert.AreEqual("{\"a\":777,\"b\":2}", Util.ToJson(jsonData));
    Util.Print(((object)jsonData).GetType().FullName);
    Assert.AreEqual("JavaCommons.Json.Linq.JObject", ((object)jsonData).GetType().FullName);
    Util.Print(jsonData.c);
    Assert.AreEqual(null, jsonData.c);
    jsonData["c"] = 888;
    Util.Print(jsonData);
    Assert.AreEqual("{\"a\":777,\"b\":2,\"c\":888}", Util.ToJson(jsonData));
}
//Util.Print(Wsl2.DistroNames(), "Wsl2.DistroNames()");
{
    string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': 'http://james.newtonking.com',
    'description': 'James Newton-King\'s blog.',
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";
    JObject rss = (JObject)Util.FromJson(json);
    Console.WriteLine(rss);
    JArray categories = (JArray)rss["channel"]["item"][0]["categories"];
    Console.WriteLine(categories);
    Assert.AreEqual("[\"Json.NET\",\"CodePlex\"]", Util.ToJson(categories));
}
{
    var n = Util.FromJson("18446744073709551615");
    Util.Print(n);
    Assert.AreEqual(18446744073709551615, (UInt64)n);
}
{
    dynamic flexible = new ExpandoObject();
    flexible.Int = 3;
    flexible.String = "hi";
    //flexible.Deep.Deeper = 777;
    flexible.Deep = new ExpandoObject();
    flexible.Deep.Deeper = 777;
    var dictionary = (IDictionary<string, object>)flexible;
    dictionary.Add("Bool", false);
    Util.Print(flexible);
    Assert.AreEqual("{\"Int\":3,\"String\":\"hi\",\"Deep\":{\"Deeper\":777},\"Bool\":false}", Util.ToJson(flexible));
}
Util.Print(Dirs.ProfilePath());
Util.Print(Dirs.ProfilePath("abc"));
//Util.Print(SpiderCore.SoftwarePath());
//Util.Print(SpiderCore.SoftwarePath("abc", "1.2.3"));
Util.Print(Dirs.DocumentsPath());
Util.Print(Dirs.DocumentsPath("abc"));
//Util.Print(SpiderCore.RepoPath());
//Util.Print(SpiderCore.CalculatePathList());
//Util.Print(SpiderCore.Which("nyagos.exe", SpiderCore.PathList));
//Util.Print(Util.ResourceNames(typeof(PrepareNugetExe).Assembly));
//Util.Print(PrepareNugetExe.NugetExe);
//var jc = Util.PrepareNugetPackage("JavaCommons");
//var se = Util.PrepareNugetPackage("SpiderExplorer");
//Util.Print(jc);
//Util.Print(se);
var settings = Util.FromJson("{}");
settings.a = (settings.a != null ? settings.a : Util.FromJson("{}"));
settings.a.b = 123;
settings.a.c = 456;
Util.Print(settings);
Assert.AreEqual("{\"a\":{\"b\":123,\"c\":456}}", Util.ToJson(settings));
/*
   //var test01Dll = Assembly.LoadFrom(se + "/tools/net472/SpiderExplorer.exe");
   //var test01Dll = Assembly.LoadFrom("D:/Users/javac/Documents/.repo/dev01/cs/Spider/SpiderExplorer/bin/Release/net472/SpiderExplorer.exe");
   var test01Dll = Assembly.LoadFrom("D:/Users/javac/Documents/.repo/dev01/cs/Spider/SpiderExplorer/bin/Debug/net6.0-windows/SpiderExplorer.dll");
   var appType = test01Dll.GetType("SpiderExplorer.Program");
   if (appType == null) Console.WriteLine("(barType == null)");
   //appType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, new [] { new string[]{} });
   var main = appType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
   if (main == null) Console.WriteLine("(main == null)");
   //main.Invoke(null, new object[] { });
 */
//Util.Dump(settings);
Util.Print("quote-\ntest");
//Util.Message("quote-\ntest");
//Util.Debug("quote-\ntest");
var results = JObject.Parse(@"
{ ""a"": 123 /* my commnet */}
", new JsonLoadSettings
{
    CommentHandling = CommentHandling.Load
});
Util.Print(results);
//xmlファイルを指定する
//XElement xml = XElement.Parse(Util.ResourceAsText(Assembly.GetExecutingAssembly(), "JC.test.xml"));
//XDocument xml = XDocument.Parse(Util.ResourceAsText(Assembly.GetExecutingAssembly(), "JC.test.xml"));
XDocument xml = Util.ParseXml(Util.ResourceAsText(Assembly.GetExecutingAssembly(), "JC.test.xml"));
//メンバー情報のタグ内の情報を取得する
IEnumerable<XElement> infos = from item in xml.Root!.Elements("メンバー情報")
                              select item;
//メンバー情報分ループして、コンソールに表示
foreach (XElement info in infos)
{
    Console.Write(info.Element("名前").Value + @",");
    Console.Write(info.Element("住所").Value + @",");
    Console.Write(info.Element("年齢").Value.ToDynamic() + @",");
    Console.Write((bool)info.Element("既婚") + @",");
    Console.WriteLine(info.Attribute("attr").Value);
    Util.Print(info, "info");
}
Util.Print(xml.ToStringWithDeclaration());
Util.Print(infos);
Util.Print(xml);
{
    string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': { '#text': 'http://james.newtonking.com', '@target': '_blank' },
    'description': { '#cdata-section': 'James Newton-King\'s blog.' },
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";
    JObject rss = (JObject)Util.FromJson(json);
    //Util.Print(rss);
    var xml2 = Util.ToXml(rss);
    Util.Print(xml2);
    Util.Print(Util.FromXml(xml2));
}
XElement data = new XElement("メンバー情報",
                             new XAttribute("属性1", true),
                             new XElement("名前", "田中"),
                             new XElement("住所", "大阪府大阪市"),
                             new XElement("年齢", "35"));
XElement list = new XElement("チーム", new XAttribute("属性2", DateTime.Now));
list.Add(data);
XDocument yourResult = new XDocument(list);
Util.Print(yourResult);
Util.Print(Util.FromObject(yourResult));
Util.Print("0".ToDynamic());
Util.Print("false".ToDynamic());
Util.Print("true".ToDynamic());
Console.WriteLine(Util.AsDateTime(yourResult.Root.Attribute("属性2").Value));
//var len = TwitterUtil.TweetTextLength("abc漢字");
//Util.Print(len, "len");
//Util.Print(TwitterUtil.TweetTextIsValid("abc漢字"), "valid");

Util.Print((int)"123".ToDynamic());
Util.Print((bool)"true".ToDynamic());
