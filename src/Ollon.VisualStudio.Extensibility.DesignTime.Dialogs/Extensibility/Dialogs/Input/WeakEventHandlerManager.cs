using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Ollon.VisualStudio.Extensibility.Dialogs.Input
{
    internal static class WeakEventHandlerManager
    {
        public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers)
        {
            if (handlers == null)
            {
                return;
            }
            EventHandler[] callees = new EventHandler[handlers.Count];
            int count = 0;
            int num = CleanupOldHandlers(handlers, callees, count);
            for (int index = 0; index < num; ++index)
            {
                CallHandler(sender, callees[index]);
            }
        }

        private static void CallHandler(object sender, EventHandler eventHandler)
        {
            DispatcherProxy dispatcher = DispatcherProxy.CreateDispatcher();
            if (eventHandler == null)
            {
                return;
            }
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action<object, EventHandler>(CallHandler), sender, (object) eventHandler);
            }
            else
            {
                eventHandler(sender, EventArgs.Empty);
            }
        }

        private static int CleanupOldHandlers(IList<WeakReference> handlers, EventHandler[] callees, int count)
        {
            for (int index = handlers.Count - 1; index >= 0; --index)
            {
                if (!(handlers[index].Target is EventHandler target))
                {
                    handlers.RemoveAt(index);
                }
                else
                {
                    callees[count] = target;
                    ++count;
                }
            }
            return count;
        }

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (handlers == null)
            {
                handlers = defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>();
            }
            handlers.Add(new WeakReference(handler));
        }

        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers == null)
            {
                return;
            }
            for (int index = handlers.Count - 1; index >= 0; --index)
            {
                if (!(handlers[index].Target is EventHandler target) || target == handler)
                {
                    handlers.RemoveAt(index);
                }
            }
        }

        private class DispatcherProxy
        {
            private readonly Dispatcher innerDispatcher;

            private DispatcherProxy(Dispatcher dispatcher)
            {
                innerDispatcher = dispatcher;
            }

            public static DispatcherProxy CreateDispatcher()
            {
                if (Application.Current == null)
                {
                    return null;
                }
                return new DispatcherProxy(Application.Current.Dispatcher);
            }

            public bool CheckAccess()
            {
                return innerDispatcher.CheckAccess();
            }

            public DispatcherOperation BeginInvoke(Delegate method, params object[] args)
            {
                return innerDispatcher.BeginInvoke(method, DispatcherPriority.Normal, args);
            }
        }
    }
}
