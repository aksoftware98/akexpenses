using AkExpenses.Models.Utitlity;
using AkExpenses.Services;
using AKSoftware.WebApi.Client;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace AkExpenses.Client.ViewModels
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; }

        public ReactiveCommand<Unit, IRoutableViewModel> GoRegister { get; }

        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public MainViewModel(object windowObject)
        {
            Router = new RoutingState();

            Locator.CurrentMutable.Register(() => windowObject, typeof(IViewFor<RegisterPageViewModel>));

            var serviceClient = new ServiceClient();
            var _configuration = new Configuration("settings.json");
            _configuration.SaveValue("ApiUri", "https://localhost:44391/api");

            GoRegister = ReactiveCommand.CreateFromObservable(() => Router.Navigate.Execute(new RegisterPageViewModel(new Auth(serviceClient, _configuration),this)));

            GoBack = Router.NavigateBack; 
        }

    }
}
