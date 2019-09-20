using AkExpenses.Client.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AkExpenses.WPF.Windows
{
    /// <summary>
    /// Interaction logic for ProvidersWindow.xaml
    /// </summary>
    public partial class ProvidersWindow : ReactiveUserControl<ProviderViewModel>
    {
        public ProvidersWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new ProviderViewModel();

            //Do the bindings
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Providers,
                    view => view.lstProviders.ItemsSource)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.SelectedProvider,
                    view => view.lstProviders.SelectedItem)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.Name,
                    view => view.txtName.Text)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.Query,
                    view => view.txtSearchQuery.Text)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.AddProviderCommand,
                    view => view.btnAdd)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.EditProviderCommand,
                    view => view.btnEdit)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.DeleteProviderCommand,
                    view => view.btnDelete)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.ToggleList,
                    view => view.btnCollapse)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.ToggleList,
                    view => view.btnShow)
                .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsListCollapsed,
                    view => view.leftGrid.Visibility,
                    value => value ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsListCollapsed,
                    view => view.hiddenGrid.Visibility,
                    value => value ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsBusy,
                    view => view.faSpinner.Spin)
                .DisposeWith(disposables);

            });
        }
    }
}
