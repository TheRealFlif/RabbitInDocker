#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Text;

namespace Producer.Producers;

public class WaitTimeCreator
{
    private int _minWait;
    private int _maxWait;
    private static Random _random = new Random();

    public WaitTimeCreator(int minWait, int maxWait)
    {
        _minWait = minWait < maxWait ? minWait : maxWait;
        _maxWait = maxWait > minWait ? maxWait : minWait;
        if (_minWait < 0) { _minWait = 0; }
        if (_maxWait < 0) { _maxWait = 0; }
    }
    public int GetMilliseconds()
    {
        return _random.Next(_minWait, _maxWait);
    }
}

internal class LatinWordCreator
{
    private static Random _randomizer = new ();
    private static string[] _words = {
        "somnum", "variant", "stella", "aliqua", "substantia", "corpus", "os", "range", "commercia", "fuge",
        "ictus", "expendas", "major", "soror", "distant", "uxorem", "calceum", "iustus", "concrescunt", "coeperunt",
        "sententia", "amplitudo", "moleculo", "relinquo", "melius", "docebit", "horologium", "postero", "pupillam", "noun",
        "percute", "pullus", "cogitavit", "calidum", "solis", "exemplar", "compare", "saepe", "aquam", "vel",
        "natant", "evolvere", "tall", "puer", "tempore", "misit", "pugna", "expandit", "fune", "cor",
        "parum", "peroratum", "quibus", "agitur", "humilis", "alia", "vi", "cursus", "partum", "festinate",
        "at", "frater", "winter", "pupa", "habent", "celeritate", "coloniam", "imo", "locant", "avem",
        "pauci", "cat", "dirige", "differunt", "vitro", "resuscitabo", "ipsum", "centum", "coeperunt", "mitis",
        "expendas", "finem", "quod", "hora", "comprehendo", "surgere", "mutatio", "current", "studiorum", "post",
        "somniatis", "paragraph", "tabulam", "pupillam", "multum", "disponere", "aliquam", "pulchrae", "audivit", "puer",
        "clamor", "iungere", "multiplicabo", "art", "ieiunium", "proxime", "reliquit", "disputatio", "pecuniam", "timere",
        "puer", "relinquo", "pullus", "imo", "caudae", "observa", "occides", "quisque", "suadeant", "adipem",
        "decem", "ædificabis", "past", "cervicibus", "requirunt", "ipse", "supra", "call", "portus", "describere",
        "cathedra", "avem", "ferrum", "column", "calor", "cubili", "calidum", "neque", "compare", "testimonium",
        "moleculo", "mittite", "vigilate", "cat", "molli", "initium", "pleni", "amicitia", "oppositi", "eventus",
        "volumen", "capillum", "pecuniam", "filii", "praxi", "arenam", "intelligantur", "usus", "cuius", "studiorum",
        "mediam", "os", "tunica", "locus", "silentium", "taberna", "praeparabit", "describere", "ex", "disputatio",
        "bigas", "reprehendo", "frustrum", "adulescens", "cantabo", "mitis", "fregit", "fractionem", "somniatis", "lac",
        "pluviae", "memores", "colligunt", "plures", "sensi", "sive", "dux", "metiretur", "aperi", "share", "congregate",
        "syllabae", "octo", "plural", "ubi", "magna", "aer", "class", "scitis", "legem" };

    internal string CreateCombination(int numberOfWords, char separator)
    {
        var returnValue = new StringBuilder();
        for (var i = 0; i < numberOfWords; i++)
        {
            var index = _randomizer.Next(0, _words.Length);
            returnValue.Append(_words[index]);
            if(i != numberOfWords - 1)
            {
                returnValue.Append(separator);
            }
        }

        return returnValue.ToString();
    }
}