using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;

namespace AkExpenses.Client.ViewModels
{
    public class DashboardViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Dashboard";

        public IScreen HostScreen { get; }

        public DashboardViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>(); 
        }
    }
}
