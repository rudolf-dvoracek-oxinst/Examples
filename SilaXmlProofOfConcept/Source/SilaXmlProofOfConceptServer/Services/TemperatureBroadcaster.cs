namespace SilaXmlProofOfConceptServer.Services;

using System;
using System.Collections.Generic;

public sealed class TemperatureBroadcaster : ITemperatureBroadcaster
{
    private readonly List<Action<TemperatureSample>> subscribers = new();
    private readonly object sync = new();

    public void Broadcast(TemperatureSample sample)
    {
        Action<TemperatureSample>[] snapshot;
        lock (this.sync)
        {
            snapshot = this.subscribers.ToArray();
        }

        foreach (var s in snapshot)
        {
            try { s(sample); } catch { /* ignore individual subscriber errors */ }
        }
    }

    public IDisposable Subscribe(Action<TemperatureSample> handler)
    {
        lock (this.sync)
        {
            this.subscribers.Add(handler);
        }
        return new Unsubscriber(this, handler);
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly TemperatureBroadcaster parent;
        private readonly Action<TemperatureSample> handler;
        private bool disposed;

        public Unsubscriber(TemperatureBroadcaster parent, Action<TemperatureSample> handler)
        {
            this.parent = parent;
            this.handler = handler;
        }

        public void Dispose()
        {
            if (this.disposed) return;
            this.disposed = true;
            lock (this.parent.sync)
            {
                this.parent.subscribers.Remove(this.handler);
            }
        }
    }
}