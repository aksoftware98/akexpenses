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
    public class MoneyTypeViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "MoneyType";
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

        private string description;
        public string Description
        {
            get { return description; }
            set { this.RaiseAndSetIfChanged(ref description, value); }
        }

        private MoneyType selectedMoneyType;
        public MoneyType SelectedMoneyType
        {
            get { return selectedMoneyType; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedMoneyType, value);
                if (selectedMoneyType != null)
                {
                    this.Name = SelectedMoneyType.Name;
                    this.Description = SelectedMoneyType.Description;
                }
            }
        }

        private SourceCache<MoneyType, string> _moneyTypesSource = new SourceCache<MoneyType, string>(c => c.Id);
        private readonly ReadOnlyObservableCollection<MoneyType> _moneyTypes;
        public ReadOnlyObservableCollection<MoneyType> MoneyTypes => this._moneyTypes;

        #endregion

        #region Constructor

        public MoneyTypeViewModel(IScreen screen = null, IConfiguration configuration = null)
        {
            this.HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this._configuration = configuration ?? Locator.Current.GetService<IConfiguration>();

            this._moneyTypesSource
                .AsObservableCache()
                .Connect()
                .Transform(c => c)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this._moneyTypes)
                .Subscribe();

            isValid = this.WhenAnyValue(vm => vm.Name, vm => vm.Description, vm => vm.SelectedMoneyType)
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

                    if (!string.IsNullOrWhiteSpace(o.Item2) && o.Item2.Length > 256)
                    {
                        Message = "Description is too long";
                        return false;
                    }

                    if (string.IsNullOrEmpty(o.Item1) && string.IsNullOrEmpty(o.Item2))
                    {
                        Message = string.Empty;
                        return false;
                    }

                    Message = string.Empty;
                    return true;
                }).ToProperty(this, vm => vm.IsValid, out isValid);

            var canEdit = this.WhenAnyValue(vm => vm.IsValid, vm => vm.SelectedMoneyType)
                .Select(o =>
                {
                    return o.Item1 && (o.Item2 != null);
                });

            this.EditMoneyTypeCommand =
                ReactiveCommand.CreateFromTask(editMoneyType, canEdit, RxApp.MainThreadScheduler);

            var canAdd = this.WhenAnyValue(vm => vm.IsValid);

            this.AddMoneyTypeCommand =
                ReactiveCommand.CreateFromTask(addMoneyType, canAdd, RxApp.MainThreadScheduler);

            var canDelete = this.WhenAnyValue(vm => vm.SelectedMoneyType)
                .Select(o => o != null);

            this.DeleteMoneyTypeCommand =
                ReactiveCommand.CreateFromTask(deleteMoneyType, canDelete, RxApp.MainThreadScheduler);

            this.ToggleList = ReactiveCommand.Create(() => this.IsListCollapsed = !this.IsListCollapsed);

            getMoneyTypes();
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> EditMoneyTypeCommand { get; }
        public ReactiveCommand<Unit, Unit> AddMoneyTypeCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteMoneyTypeCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleList { get; }


        #endregion

        #region Methods

        private async Task addMoneyType()
        {
            IsBusy = true;

            var service = new MoneyTypeService(_configuration);
            var response = await service.Create(new Models.Shared.ViewModels.MoneyTypeViewModel
            {
                Name = this.Name,
                Description = this.Description
            });

            if (response != null)
            {
                this._moneyTypesSource.AddOrUpdate(response);
            }

            IsBusy = false;
        }

        private async Task editMoneyType()
        {
            IsBusy = true;

            if (SelectedMoneyType != null)
            {
                var service = new MoneyTypeService(_configuration);

                var response = await service.Edit(new Models.Shared.ViewModels.MoneyTypeViewModel
                {
                    Name = this.Name,
                    Description = this.Description,
                    MoneyTypeId = SelectedMoneyType.Id
                });

                if (response != null)
                {
                    this._moneyTypesSource.AddOrUpdate(response);
                }
            }
            IsBusy = false;
        }
        private async Task deleteMoneyType()
        {
            IsBusy = true;
            if (SelectedMoneyType != null)
            {
                var service = new MoneyTypeService(_configuration);
                var response = await service.Delete(SelectedMoneyType.Id);

                if (response != null)
                {
                    this._moneyTypesSource.Remove(response);
                }
            }
            IsBusy = false;
        }
        private async Task<bool> getMoneyTypes()
        {

            var service = new MoneyTypeService(_configuration);

            var response = await service.GetAll();

            if (response != null)
            {
                Parallel.ForEach(response, moneyType =>
                {
                    this._moneyTypesSource.AddOrUpdate(moneyType);
                });

                return true;
            }

            return false;
        }

        #endregion
    }
}
