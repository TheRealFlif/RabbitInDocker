using System.Text.Json;

namespace Producer.Entities;

public class Envelope<T>
{
    public Envelope(T data)
    {
        MetaData = new Dictionary<string, string>([]);
        Data = data;
    }

    public Dictionary<string, string> MetaData { get; set; }
    public T Data { get; set; }

    public string? this[string key]
    {
        get => MetaData.TryGetValue(key, out var returnValue) ? returnValue : null;
        set
        {
            if (value != null)
            {
                MetaData[key] = value;
            }
        }
    }

    public string To()
    {
        return JsonSerializer.Serialize(this);
    }

    public static Envelope<T>? From(string json)
    {
        return JsonSerializer.Deserialize<Envelope<T>>(json);
    }
}
