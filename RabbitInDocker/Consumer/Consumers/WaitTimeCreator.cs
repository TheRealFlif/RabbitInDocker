﻿namespace Consumer.Consumers;


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