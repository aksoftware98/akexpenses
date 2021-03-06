﻿using AkExpenses.Client.ViewModels;
using AkExpenses.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for LoginPageWindow.xaml
    /// </summary>
    public partial class LoginPageWindow : ReactiveUserControl<LoginViewModel>
    {
        public LoginPageWindow()
        {
            InitializeComponent();

            var auth = Locator.Current.GetService<Auth>();

            DataContext = ViewModel = new LoginViewModel(auth);

            this.WhenActivated((disposable) =>
            {
                this.Bind(ViewModel,
                    viewmodel => viewmodel.Email,
                    view => view.txtEmail.Text);

                this.Bind(ViewModel,
                    viewmodel => viewmodel.Password,
                    view => view.txtPassword.Text);

                this.OneWayBind(ViewModel,
                 viewmodel => viewmodel.IsBusy,
                 view => view.progressCircle.Spin);

                this.OneWayBind(ViewModel,
                viewmodel => viewmodel.Message,
                view => view.lblMessage.Text);

                this.BindCommand(ViewModel,
                    viewmodel => viewmodel.LoginCommand,
                    view => view.btnLogin);

                this.BindCommand(ViewModel,
                    viewmodel => viewmodel.GoToRegisterCommand,
                    view => view.btnRegister);

            });
        }
    }
}
