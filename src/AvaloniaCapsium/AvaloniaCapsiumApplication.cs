using Avalonia;
using Avalonia.Threading;
using Capsium;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaCapsium
{
    public class AvaloniaCapsiumApplication<T> : Application, IApp
        where T : class, ICapsiumDevice
    {
        public CancellationToken CancellationToken => throw new NotImplementedException();

        public static T Device => Resolver.Services.Get<ICapsiumDevice>() as T;

        public Dictionary<string, string> Settings => new Dictionary<string, string>();

        protected AvaloniaCapsiumApplication()
        {
        }

        public void InvokeOnMainThread(Action<object?> action, object? state = null)
        {
            Dispatcher.UIThread.Post(() => action(state));
        }

        virtual public Task OnError(Exception e)
        {
            return Task.CompletedTask;
        }

        virtual public Task OnShutdown()
        {
            return Task.CompletedTask;
        }

        virtual public void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            approveUpdate = true;
        }

        virtual public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            rollbackUpdate = false;
        }

        virtual public Task Run()
        {
            return Task.CompletedTask;
        }

        public virtual Task InitializeCapsium()
        {
            return Task.CompletedTask;
        }

        Task IApp.Initialize()
        {
            return InitializeCapsium();
        }

        protected void LoadCapsiumOS()
        {
            new Thread((o) =>
            {
                _ = CapsiumOS.Start(this, null);
            })
            {
                IsBackground = true
            }
            .Start();
        }
    }
}