﻿using RabbitMQ.Client;
using System.Text.Json;

namespace Producer.Entities;

public class BasicProperties : IBasicProperties
{
    public string AppId {get; set;}
    public string ClusterId {get; set;}
    public string ContentEncoding {get; set;}
    public string ContentType {get; set;}
    public string CorrelationId {get; set;}
    public byte DeliveryMode {get; set;}
    public string Expiration {get; set;}
    public IDictionary<string, object> Headers {get; set;}
    public string MessageId {get; set;}
    public bool Persistent {get; set;}
    public byte Priority {get; set;}
    public string ReplyTo {get; set;}
    public PublicationAddress ReplyToAddress {get; set;}
    public AmqpTimestamp Timestamp {get; set;}
    public string Type {get; set;}
    public string UserId {get; set;}

    public ushort ProtocolClassId => throw new NotImplementedException();

    public string ProtocolClassName => throw new NotImplementedException();

    public void ClearAppId()
    {
        throw new NotImplementedException();
    }

    public void ClearClusterId()
    {
        throw new NotImplementedException();
    }

    public void ClearContentEncoding()
    {
        throw new NotImplementedException();
    }

    public void ClearContentType()
    {
        throw new NotImplementedException();
    }

    public void ClearCorrelationId()
    {
        throw new NotImplementedException();
    }

    public void ClearDeliveryMode()
    {
        throw new NotImplementedException();
    }

    public void ClearExpiration()
    {
        throw new NotImplementedException();
    }

    public void ClearHeaders()
    {
        throw new NotImplementedException();
    }

    public void ClearMessageId()
    {
        throw new NotImplementedException();
    }

    public void ClearPriority()
    {
        throw new NotImplementedException();
    }

    public void ClearReplyTo()
    {
        throw new NotImplementedException();
    }

    public void ClearTimestamp()
    {
        throw new NotImplementedException();
    }

    public void ClearType()
    {
        throw new NotImplementedException();
    }

    public void ClearUserId()
    {
        throw new NotImplementedException();
    }

    public bool IsAppIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsClusterIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsContentEncodingPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsContentTypePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsCorrelationIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsDeliveryModePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsExpirationPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsHeadersPresent() => Headers.Count > 0;

    public bool IsMessageIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsPriorityPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsReplyToPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsTimestampPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsTypePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsUserIdPresent()
    {
        throw new NotImplementedException();
    }
}

public class Envelope<T>
{
    public Envelope(T data)
    {
        MetaData = new Dictionary<string, object>();
        Data = data;
    }

    public Dictionary<string, object> MetaData { get; set; }
    public T Data { get; set; }

    public object? this[string key]
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
