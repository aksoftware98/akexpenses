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
    /// Interaction logic for MoneyTypesWindow.xaml
    /// </summary>
    public partial class MoneyTypesWindow : ReactiveUserControl<MoneyTypeViewModel>
    {
        public MoneyTypesWindow()
        {
            InitializeComponent();

            DataContext = ViewModel = new MoneyTypeViewModel();

            //Do the bindings
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.MoneyTypes,
                    view => view.lstMoneyTypes.ItemsSource)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.SelectedMoneyType,
                    view => view.lstMoneyTypes.SelectedItem)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.Name,
                    view => view.txtName.Text)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.Description,
                    view => view.txtDescription.Text)
                .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.Query,
                    view => view.txtSearchQuery.Text)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.AddMoneyTypeCommand,
                    view => view.btnAdd)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.EditMoneyTypeCommand,
                    view => view.btnEdit)
                .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                    viewModel => viewModel.DeleteMoneyTypeCommand,
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
