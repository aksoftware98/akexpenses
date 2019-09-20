using AkExpenses.Models.Interfaces;
using AkExpenses.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AkExpenses.Client.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly Auth _authService;

        #region Properties

        public string UrlPathSegment => "Login";

        public IScreen HostScreen { get; }

        private readonly IConfiguration _configuration;

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                this.RaiseAndSetIfChanged(ref _email, value);
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                this.RaiseAndSetIfChanged(ref _password, value);
            }
        }

        private ObservableAsPropertyHelper<bool> _isValid;
        public bool IsValid => _isValid.Value;

        private string _message;
        public string Message
        {
            get => _message;
            private set
            {
                this.RaiseAndSetIfChanged(ref _message, value);
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                this.RaiseAndSetIfChanged(ref _isBusy, value);
            }
        }

        #endregion

        public LoginViewModel(Auth auth, IScreen screen = null, IConfiguration configuration = null)
        {
            this.HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _configuration = configuration ?? Locator.Current.GetService<IConfiguration>(); 

            this._authService = auth;

            _isValid = this.WhenAnyValue(x => x.Email, x => x.Password)
                .Select(o =>
                {
                    if (!Regex.IsMatch(o.Item1, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"))
                    {
                        Message = "Please enter a valid mail address";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(o.Item2) || o.Item2.Length < 6)
                    {
                        Message = "Password vield is invalid";
                        return false;
                    }

                    if (string.IsNullOrEmpty(o.Item1) && string.IsNullOrEmpty(o.Item2))
                    {
                        Message = "";
                        return false;
                    }

                    Message = "";
                    return true;
                })
                .ToProperty(this, x => x.IsValid, out _isValid);

            var canLogin = this.WhenAnyValue(x => x.IsValid);

            LoginCommand = ReactiveCommand.CreateFromTask(loginUserAsync, canLogin, RxApp.MainThreadScheduler);
            GoToRegisterCommand = ReactiveCommand.Create(goToRegisterPage);

        }

        #region Commands

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        #endregion

        #region Methods

        private async Task loginUserAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            var result = await _authService.LoginUserAsync(this.Email, this.Password);

            if (result)
            {
                Message = "Welcome...";
                await Task.Delay(2000);
                await HostScreen.Router.Navigate.Execute(new CategoryViewModel(HostScreen, _configuration));
            }
            else
                Message = "Username or password is incorrect";

            IsBusy = false;
        }

        private void goToRegisterPage()
        {
            Message = "";
            HostScreen.Router.Navigate.Execute(new RegisterPageViewModel(_authService));
        }
        #endregion
    }
}
