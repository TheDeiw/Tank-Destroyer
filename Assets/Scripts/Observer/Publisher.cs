using System.Collections.Generic;
using UnityEngine;

public class GameEventPublisher<TContext>
{
    private readonly List<ISubscriber<TContext>> _subscribers = new List<ISubscriber<TContext>>();

    public void Subscribe(ISubscriber<TContext> subscriber)
    {
        if (!_subscribers.Contains(subscriber))
        {
            _subscribers.Add(subscriber);
        }
    }

    public void Unsubscribe(ISubscriber<TContext> subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    public void NotifySubscribers(TContext context)
    {
        for (int i = _subscribers.Count - 1; i >= 0; i--)
        {
            if (_subscribers[i] is MonoBehaviour monoBehaviour && monoBehaviour == null)
            {
                _subscribers.RemoveAt(i);
                continue;
            }
            _subscribers[i].OnNotify(context);
        }
    }
}