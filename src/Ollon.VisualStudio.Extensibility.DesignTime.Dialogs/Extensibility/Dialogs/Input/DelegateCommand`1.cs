using System;

namespace Ollon.VisualStudio.Extensibility.Dialogs.Input
{
    public class DelegateCommand<T> : DelegateCommandBase
    {
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, o => true)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => executeMethod((T) o), o => canExecuteMethod((T) o))
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), DialogResources.DelegateCommandDelegatesCannotBeNull);
            }
            Type type = typeof(T);
            if (type.IsValueType && (!type.IsGenericType || !typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                throw new InvalidCastException(DialogResources.DelegateCommandInvalidGenericPayloadType);
            }
        }

        public bool CanExecute(T parameter)
        {
            return CanExecute((object) parameter);
        }

        public void Execute(T parameter)
        {
            Execute((object) parameter);
        }
    }
}
