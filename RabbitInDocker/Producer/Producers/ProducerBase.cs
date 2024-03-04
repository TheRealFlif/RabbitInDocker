﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers;

public class ProducerBase<T> : IProducer
{
    protected IModel Channel { get; init; }
    protected ProducerSettings Settings { get; init; }

    public ProducerBase(
        IModel channel, 
        ProducerSettings settings)
    {
        Channel = channel;
        Settings = settings;
        if (_waitTimeCreator == null)
        {
            _waitTimeCreator = new WaitTimeCreator(settings.MinWait, settings.MaxWait);
        }
    }
            
    public virtual void Send(T? body)
    {
        var envelope = Create(body);
        if (envelope != null)
        {
            Channel.BasicPublish(
                Settings.ExchangeName, 
                Settings.RoutingKey, 
                null, 
                System.Text.Encoding.UTF8.GetBytes(envelope.To()));
        }
        Sleep();
    }

    int _messageNumber = 0;
    protected virtual Envelope<T>? Create(T? body)
    {
        Envelope<T>? returnValue = default;
        if (body != null)
        {
            returnValue = new Envelope<T>(body);
            returnValue["sender"] = Settings.Name;
            returnValue["messageNumber"] = $"{++_messageNumber:00}";
        }
        return returnValue;
    }

    public virtual void ShutDown() { }

    static WaitTimeCreator _waitTimeCreator;
    protected void Sleep()
    {
        Thread.Sleep(_waitTimeCreator.GetMilliseconds());
    }
}