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
    // -a �� -aaa �̓�w��\
    [Option('a', "aaa", Required = false, HelpText = "A�̐����ł��B")]
    public string A
    {
        get;
        set;
    }
    // string �ȊO�ł��󂯎���i���̏ꍇ�̓I�v�V���������邩�ǂ����j
    [Option('b', "bbb", Required = false, HelpText = "B�̐����ł��B")]
    public bool B
    {
        get;
        set;
    }
    // Sepalator�Ŏw�艺��������؂蕶���Ƃ��āA�����̒l��n����
    [Option('c', "ccc", Separator = ',', HelpText = "C�̐����ł��B")]
    public IEnumerable<string> C
    {
        get;
        set;
    }
    // enum��������iHoge, Fuga �ȂǂƎw�肷��j
    [Option('d', "ddd", HelpText = "C�̐����ł��B")]
    public HogeInfo D
    {
        get;
        set;
    }
    // ��L�w��ȊO�̃I�v�V�����╶���񂪓���
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
        // �W�F�l���N�X�ŃI�v�V�����N���X���w�肵�A�p�[�X����
        var parseResult = Parser.Default.ParseArguments<Options>(args);
        Options opt = null;
        // ���ʂ�Tag�ɓ����Ă���
        switch (parseResult.Tag)
        {
        // �p�[�X����
        case ParserResultType.Parsed:
            // �p�[�X�̐��ۂŃp�[�X���ʂ̃I�u�W�F�N�g�̕����ς��
            var parsed = parseResult as Parsed<Options>;
            // �������̓L���X�g�����I�u�W�F�N�g����p�[�X���ʂ��擾�\
            opt = parsed.Value;
            // �\���p�ɐ��`
            string strC = string.Concat("{ ", string.Join(", ", opt.C.Select(e => $"\"{e}\"")), " }");
            string strOthers = string.Concat("{ ", string.Join(", ", opt.Others.Select(e => $"\"{e}\"")), " }");
            Console.WriteLine($"opt.A = {opt.A}");
            Console.WriteLine($"opt.B = {opt.B}");
            Console.WriteLine($"opt.C = {strC}");
            Console.WriteLine($"opt.D = {opt.D}");
            Console.WriteLine($"opt.Others = {strOthers}");
            break;
        // �p�[�X���s
        case ParserResultType.NotParsed:
            // �p�[�X�̐��ۂŃp�[�X���ʂ̃I�u�W�F�N�g�̕����ς��
            var notParsed = parseResult as NotParsed<Options>;
            break;
        }
    }
}
