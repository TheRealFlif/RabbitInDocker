using System.Text.Json;

namespace Producer;

public  class Envelope<T>
{
    public Envelope(T data)
    {
        MetaData = new Dictionary<string, string>([]);
        Data = data;
    }

    public Dictionary<string, string> MetaData { get; set; }
    public T Data { get; set; }

    public string this[string key]
    {
        get { return MetaData.TryGetValue(key, out string returnValue) ? returnValue : null; }
        set { MetaData[key] = value; }
    }

    public string To()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Envelope<T> From<T>(string json)
    {
        return JsonSerializer.Deserialize<Envelope<T>>(json);
    }
}
