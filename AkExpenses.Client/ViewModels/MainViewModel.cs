using AkExpenses.Models.Interfaces;
using AkExpenses.Models.Utitlity;
using AkExpenses.Services;
using AKSoftware.WebApi.Client;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reflection;
using System.Text;

namespace AkExpenses.Client.ViewModels
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; }

        public ReactiveCommand<Unit, Unit> GoBack { get; }


        public MainViewModel()
        {
            Router = new RoutingState();

            // Register all the views and viewmodels 
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly()); 

            // Register the IScreen in the container 
            Locator.CurrentMutable.RegisterLazySingleton(() => this, typeof(IScreen));

            var serviceClient = Locator.Current.GetService<ServiceClient>();
            var _configuration = Locator.Current.GetService<IConfiguration>();

            if (!string.IsNullOrEmpty(_configuration.AccessToken))
                navigateTo(new DashboardViewModel(this));
            else
                navigateTo(new RegisterPageViewModel(new Auth(serviceClient, _configuration))); 

                GoBack = Router.NavigateBack;
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="model"></param>
        void navigateTo(IRoutableViewModel model)
        {
            Router.Navigate.Execute(model); 
        }

    }
}
