using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Ollon.VisualStudio.Extensibility.Dialogs.Input
{
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private readonly Action<object> executeMethod;
        private readonly Func<object, bool> canExecuteMethod;
        private bool _isActive;
        private List<WeakReference> _canExecuteChangedHandlers;

        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), DialogResources.DelegateCommandDelegatesCannotBeNull);
            }
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                {
                    return;
                }
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        public virtual event EventHandler IsActiveChanged;

        protected virtual void OnIsActiveChanged()
        {
            EventHandler isActiveChanged = IsActiveChanged;
            if (isActiveChanged == null)
            {
                return;
            }
            isActiveChanged(this, EventArgs.Empty);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        protected void Execute(object parameter)
        {
            executeMethod(parameter);
        }

        protected bool CanExecute(object parameter)
        {
            if (canExecuteMethod != null)
            {
                return canExecuteMethod(parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            remove => WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
        }
    }
}
