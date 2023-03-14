#if false
//using Jint;
//using SQLite;
//using MsieJavaScriptEngine;
//using MsieJavaScriptEngine.Helpers;
namespace JavaCommons;
public class TwitterUtil
{
    //private static readonly Engine JsEngine;
    //private static readonly MsieJsEngine _msieEngine;
    private static readonly Jurassic.ScriptEngine _jurassic;
    static TwitterUtil()
    {
        //JsEngine = JintUtil.CreateEngine();
        string script = Util.ResourceAsText(typeof(TwitterUtil).Assembly,
                                            "JavaCommons.Extra.twitter-text-3.1.0.js");
        //Util.Print(script);
        //JsEngine.Execute(script);
        //Util.Print("static TwitterUtil()");

        //_msieEngine = new MsieJsEngine();
        //_msieEngine.Execute(script);
        _jurassic = new Jurassic.ScriptEngine();
        _jurassic.Execute(script);

    }
    public static bool TweetTextIsValid(string tweetText)
    {
#if false
        JsEngine.SetValue("tweetText", tweetText);
        dynamic result = JsEngine.Evaluate(@"twttr.txt.parseTweet(tweetText)").ToObject();
        //dynamic result = JsEngine.Execute(@"twttr.txt.parseTweet(tweetText)").GetCompletionValue().ToObject();
        return (bool)result.valid;
#else
        _jurassic.SetGlobalValue("tweetText", tweetText);
        var result = _jurassic.Evaluate(@"twttr.txt.parseTweet(tweetText).valid");
        return (bool)result;
#endif
    }
    public static int TweetTextLength(string tweetText)
    {
#if false
        JsEngine.SetValue("tweetText", tweetText);
        dynamic result = JsEngine.Evaluate(@"twttr.txt.parseTweet(tweetText)").ToObject();
        //dynamic result = JsEngine.Execute(@"twttr.txt.parseTweet(tweetText)").GetCompletionValue().ToObject();
        return (int)result.weightedLength;
#else
        _jurassic.SetGlobalValue("tweetText", tweetText);
        var result = _jurassic.Evaluate(@"twttr.txt.parseTweet(tweetText).weightedLength");
        return (int)result;
#endif
    }
}
#endif