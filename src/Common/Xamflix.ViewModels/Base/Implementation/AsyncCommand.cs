using System;
using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;
using QQPad.Mobile.ViewModels.Resources.Strings.Common;
using Xamflix.Core.AsyncVoid;

namespace Xamflix.ViewModels.Base.Implementation
{
    /// <summary>
    ///     Base implementation of <see cref="BaseBindableCommand" /> interface.
    /// </summary>
    /// <seealso cref="IAsyncCommand" />
    /// <seealso cref="IAsyncCommand" />
    [AddINotifyPropertyChangedInterface]
    public abstract class AsyncCommand : BaseBindableCommand, IAsyncCommand
    {
        protected AsyncCommand(bool canExecute = true) : base(canExecute)
        {
        }

        [AsyncVoidCheckExemption("Execute void method of AsyncCommand servers as a bridge between UI non-async code and async business logic.")]
        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public bool IsBusy { get; private set; }
        public string FailureMessage { get; protected set; }
        public bool IsSuccessful { get; private set; }

        /// <summary>
        ///     Base implementation of <see cref="IAsyncCommand.ExecuteAsync" /> method. Contains logic to handle the state of the
        ///     command.
        ///     Generally this method should not be overridden by child classes, instead, you should override
        ///     <see cref="ExecuteCoreAsync" />.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Asynchronous await-able task.</returns>
        public virtual async Task ExecuteAsync(object parameter, CancellationToken token = default)
        {
            if(IsBusy)
            {
                return;
            }

            //Reset the state of the command
            IsBusy = true;
            IsSuccessful = false;
            FailureMessage = null;
            try
            {
                IsSuccessful = await ExecuteCoreAsync(parameter, token);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(ex);
            }
            finally
            {
                //At the end set that execution has finished
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Executes the core asynchronous operation. Child classes should override this method with their operation
        ///     implementation.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        ///     Child implementation should return <code>true</code> if the operation is considered successful.
        ///     The returned value will be set to <see cref="IsSuccessful" /> property.
        /// </returns>
        protected abstract Task<bool> ExecuteCoreAsync(object parameter, CancellationToken token = default);

        /// <summary>
        ///     Default exception handler. Sets the <see cref="FailureMessage" /> to
        ///     <see cref="CommonStrings.GeneralFailureMessage" />.
        ///     Override this method for custom exception handling and be sure to set the value of <see cref="FailureMessage" />
        ///     to user-friendly message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual Task HandleExceptionAsync(Exception exception)
        {
            FailureMessage = CommonStrings.GeneralFailureMessage;
            return Task.CompletedTask;
        }
    }
}