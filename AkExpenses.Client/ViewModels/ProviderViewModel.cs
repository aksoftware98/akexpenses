using AkExpenses.Models;
using AkExpenses.Models.Interfaces;
using AkExpenses.Services;
using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkExpenses.Client.ViewModels
{
    public class ProviderViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Provider";
        public IScreen HostScreen { get; }
        public IConfiguration _configuration { get; set; }

        #region Public Properties

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { this.RaiseAndSetIfChanged(ref isBusy, value); }
        }

        private bool isListCollapsed;
        public bool IsListCollapsed
        {
            get { return isListCollapsed; }
            set { this.RaiseAndSetIfChanged(ref isListCollapsed, value); }
        }

        private ObservableAsPropertyHelper<bool> isValid;
        public bool IsValid => isValid.Value;

        private string message;
        public string Message
        {
            get { return message; }
            set { this.RaiseAndSetIfChanged(ref message, value); }
        }

        private string query;
        public string Query
        {
            get { return query; }
            set { this.RaiseAndSetIfChanged(ref query, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        private Provider selectedProvider;
        public Provider SelectedProvider
        {
            get { return selectedProvider; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedProvider, value);
                if (selectedProvider != null)
                {
                    this.Name = SelectedProvider.Name;
                }
            }
        }

        private SourceCache<Provider, string> _providersSource = new SourceCache<Provider, string>(c => c.Id);
        private readonly ReadOnlyObservableCollection<Provider> _providers;
        public ReadOnlyObservableCollection<Provider> Providers => this._providers;

        #endregion

        #region Constructor

        public ProviderViewModel(IScreen screen = null, IConfiguration configuration = null)
        {
            this.HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this._configuration = configuration ?? Locator.Current.GetService<IConfiguration>();

            this._providersSource
                .AsObservableCache()
                .Connect()
                .Transform(c => c)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this._providers)
                .Subscribe();

            isValid = this.WhenAnyValue(vm => vm.Name, vm => vm.SelectedProvider)
                .Select(o =>
                {
                    if (string.IsNullOrWhiteSpace(o.Item1))
                    {
                        Message = "Name is invalid";
                        return false;
                    }
                    else if (o.Item1.Length > 50)
                    {
                        Message = "Name is too long";
                        return false;
                    }

                    if (string.IsNullOrEmpty(o.Item1))
                    {
                        Message = string.Empty;
                        return false;
                    }

                    Message = string.Empty;
                    return true;
                }).ToProperty(this, vm => vm.IsValid, out isValid);

            var canEdit = this.WhenAnyValue(vm => vm.IsValid, vm => vm.SelectedProvider)
                .Select(o =>
                {
                    return o.Item1 && (o.Item2 != null);
                });

            this.EditProviderCommand =
                ReactiveCommand.CreateFromTask(editProvider, canEdit, RxApp.MainThreadScheduler);

            var canAdd = this.WhenAnyValue(vm => vm.IsValid);

            this.AddProviderCommand =
                ReactiveCommand.CreateFromTask(addProvider, canAdd, RxApp.MainThreadScheduler);

            var canDelete = this.WhenAnyValue(vm => vm.SelectedProvider)
                .Select(o => o != null);

            this.DeleteProviderCommand =
                ReactiveCommand.CreateFromTask(deleteProvider, canDelete, RxApp.MainThreadScheduler);

            this.ToggleList = ReactiveCommand.Create(() => this.IsListCollapsed = !this.IsListCollapsed);

            getProviders();
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> EditProviderCommand { get; }
        public ReactiveCommand<Unit, Unit> AddProviderCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteProviderCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleList { get; }

        #endregion

        #region Methods

        private async Task addProvider()
        {
            IsBusy = true;

            var service = new ProviderService(_configuration);
            var response = await service.Create(new Models.Shared.ViewModels.ProviderViewModel
            {
                Name = this.Name
            });

            if (response != null)
            {
                this._providersSource.AddOrUpdate(response);
            }

            IsBusy = false;
        }

        private async Task editProvider()
        {
            IsBusy = true;

            if (SelectedProvider != null)
            {
                var service = new ProviderService(_configuration);

                var response = await service.Edit(new Models.Shared.ViewModels.ProviderViewModel
                {
                    Name = this.Name,
                    ProviderId = SelectedProvider.Id
                });

                if (response != null)
                {
                    this._providersSource.AddOrUpdate(response);
                }
            }
            IsBusy = false;
        }
        private async Task deleteProvider()
        {
            IsBusy = true;
            if (SelectedProvider != null)
            {
                var service = new ProviderService(_configuration);
                var response = await service.Delete(SelectedProvider.Id);

                if (response != null)
                {
                    this._providersSource.Remove(response);
                }
            }
            IsBusy = false;
        }
        private async Task<bool> getProviders()
        {

            var service = new ProviderService(_configuration);

            var response = await service.GetAll();

            if (response != null)
            {
                Parallel.ForEach(response, category =>
                {
                    this._providersSource.AddOrUpdate(category);
                });

                return true;
            }

            return false;
        }

        #endregion
    }
}
