using Microsoft.ClearScript.Windows;
namespace JavaCommons.Windows;
public class JScript
{
    public static JScriptEngine CreateEngine()
    {
        var engine = new JScriptEngine();
        engine.AddHostType("Util", typeof(JavaCommons.Util));
        return engine;
    }
}
