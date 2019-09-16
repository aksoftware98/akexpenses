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

            var serviceClient = new ServiceClient();
            var _configuration = new Configuration("settings.json");
            _configuration.LoadSettings(); 
            Locator.CurrentMutable.RegisterConstant(_configuration, typeof(IConfiguration));

            if (_configuration.AccessToken != null)
                navigateTo(new DashboardViewModel());
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
