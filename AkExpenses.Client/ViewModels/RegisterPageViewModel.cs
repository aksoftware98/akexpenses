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
    public class RegisterPageViewModel : ReactiveObject, IRoutableViewModel
    {

        private readonly Auth _authService;
        public string UrlPathSegment => "register";

        public IScreen HostScreen { get; }

        #region Properties
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


        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                this.RaiseAndSetIfChanged(ref _confirmPassword, value);
            }
        }

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                this.RaiseAndSetIfChanged(ref _firstName, value);
            }
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set
            {
                this.RaiseAndSetIfChanged(ref _lastName, value);
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

        #region Constructions

        public RegisterPageViewModel(Auth authSerivce, IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            _authService = authSerivce;

            // Set the observables and prorties 
            _isValid = this.WhenAnyValue(x => x.FirstName, x => x.LastName, x => x.Email, x => x.Password, x => x.ConfirmPassword)
                .Select(o =>
                {

                    if (string.IsNullOrWhiteSpace(o.Item1) || o.Item1.Length < 3)
                    {
                        Message = "Please enter a valid name";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(o.Item2) || o.Item2.Length < 3)
                    {
                        Message = "Please enter a valid name";
                        return false;
                    }

                    if (!Regex.IsMatch(o.Item3, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"))
                    {
                        Message = "Please enter a valid mail address";
                        return false;
                    }

                    if (string.IsNullOrEmpty(o.Item1) && string.IsNullOrEmpty(o.Item2) && string.IsNullOrEmpty(o.Item3) &&
                       string.IsNullOrEmpty(o.Item4) && string.IsNullOrEmpty(o.Item5))
                    {
                        Message = "";
                        return false;
                    }

                    Message = "";
                    return true;
                })
                .ToProperty(this, x => x.IsValid, out _isValid);

            var canRegister = this.WhenAnyValue(x => x.IsValid);

            // Prepare the commands 
            RegisterCommand = ReactiveCommand.CreateFromTask(registerUserAsync, canRegister, RxApp.MainThreadScheduler);
            GoToLoginCommand = ReactiveCommand.Create(goToLoginPage);
        }

        #endregion 

        #region Commands
        public ICommand RegisterCommand { get; }

        public ICommand GoToLoginCommand { get; }

        #endregion

        #region Methods
        // Register a new user into the system 
        private async Task registerUserAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            var result = await _authService.RegisterUserAsync(new Models.Shared.RegisterViewModel
            {
                Email = Email,
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                FirstName = FirstName,
                LastName = LastName
            });

            if (result.IsSuccess)
            {
                Message = "Your account has been created successfully!";
                // TODO: Go to the main page 
                await Task.Delay(1500);
                await HostScreen.Router.Navigate.Execute(new LoginViewModel(_authService));
            }
            else
                Message = result.Message;

            IsBusy = false;
        }

        private void goToLoginPage()
        {
            var auth = Locator.Current.GetService<Auth>();
            HostScreen.Router.Navigate.Execute(new LoginViewModel(auth, HostScreen));
        }

        #endregion 
    }
}
