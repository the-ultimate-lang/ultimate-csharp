using Microsoft.ClearScript.Windows;
namespace JavaCommons.Windows;
public class VBScript
{
    public static VBScriptEngine CreateEngine()
    {
        var engine = new VBScriptEngine();
        engine.AddHostType("Util", typeof(JavaCommons.Util));
        return engine;
    }
}
