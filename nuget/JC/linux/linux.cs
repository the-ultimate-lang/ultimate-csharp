using CommandLine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
enum HogeInfo
{
    Hoge, Fuga, Piyo
}
class Options
{
    // -a と -aaa の二つ指定可能
    [Option('a', "aaa", Required = false, HelpText = "Aの説明です。")]
    public string A
    {
        get;
        set;
    }
    // string 以外でも受け取れる（この場合はオプションがあるかどうか）
    [Option('b', "bbb", Required = false, HelpText = "Bの説明です。")]
    public bool B
    {
        get;
        set;
    }
    // Sepalatorで指定下文字を区切り文字として、複数の値を渡せる
    [Option('c', "ccc", Separator = ',', HelpText = "Cの説明です。")]
    public IEnumerable<string> C
    {
        get;
        set;
    }
    // enumもいける（Hoge, Fuga などと指定する）
    [Option('d', "ddd", HelpText = "Cの説明です。")]
    public HogeInfo D
    {
        get;
        set;
    }
    // 上記指定以外のオプションや文字列が入る
    [Value(1, MetaName = "Others")]
    public IEnumerable<string> Others
    {
        get;
        set;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Assembly myAssembly = Assembly.GetEntryAssembly();
        string path = myAssembly.Location;
        Console.WriteLine(path.ToUpper());
        // ジェネリクスでオプションクラスを指定し、パースする
        var parseResult = Parser.Default.ParseArguments<Options>(args);
        Options opt = null;
        // 結果はTagに入っている
        switch (parseResult.Tag)
        {
        // パース成功
        case ParserResultType.Parsed:
            // パースの成否でパース結果のオブジェクトの方が変わる
            var parsed = parseResult as Parsed<Options>;
            // 成功時はキャストしたオブジェクトからパース結果が取得可能
            opt = parsed.Value;
            // 表示用に整形
            string strC = string.Concat("{ ", string.Join(", ", opt.C.Select(e => $"\"{e}\"")), " }");
            string strOthers = string.Concat("{ ", string.Join(", ", opt.Others.Select(e => $"\"{e}\"")), " }");
            Console.WriteLine($"opt.A = {opt.A}");
            Console.WriteLine($"opt.B = {opt.B}");
            Console.WriteLine($"opt.C = {strC}");
            Console.WriteLine($"opt.D = {opt.D}");
            Console.WriteLine($"opt.Others = {strOthers}");
            break;
        // パース失敗
        case ParserResultType.NotParsed:
            // パースの成否でパース結果のオブジェクトの方が変わる
            var notParsed = parseResult as NotParsed<Options>;
            break;
        }
    }
}
