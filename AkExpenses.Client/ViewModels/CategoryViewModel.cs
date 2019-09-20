using AkExpenses.Models;
using AkExpenses.Models.Interfaces;
using AkExpenses.Services;
using DynamicData;
using DynamicData.Binding;
using Microsoft.AspNetCore.Http;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace AkExpenses.Client.ViewModels
{
    public class CategoryViewModel : ReactiveObject, IRoutableViewModel
    {
        #region Properties

        private readonly IConfiguration _configuration;
        public string UrlPathSegment => "Category";
        public IScreen HostScreen { get; }

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

        private string iconPath = "https://www.easylivingfootwear.com.au/media/catalog/product/cache/1/small_image/578x/9df78eab33525d08d6e5fb8d27136e95/placeholder/default/Image_placeholder_1.png";
        public string IconPath
        {
            get { return iconPath; }
            set
            {
                this.RaiseAndSetIfChanged(ref iconPath, !string.IsNullOrEmpty(value) ? value: "https://www.easylivingfootwear.com.au/media/catalog/product/cache/1/small_image/578x/9df78eab33525d08d6e5fb8d27136e95/placeholder/default/Image_placeholder_1.png");
            }
        }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedCategory, value);
                if (selectedCategory != null)
                {
                    this.Name = SelectedCategory.Name;
                    this.Description = SelectedCategory.Description;
                    this.IconPath = SelectedCategory.IconPath;
                }
            }
        }

        private SourceCache<Category, string> _categoriesSource = new SourceCache<Category, string>(c => c.Id);
        private readonly ReadOnlyObservableCollection<Category> _categories;
        public ReadOnlyObservableCollection<Category> Categories => this._categories;
        #endregion

        #region Constructor

        public CategoryViewModel(IScreen screen = null, IConfiguration configuration = null)
        {
            this.HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this._configuration = configuration ?? Locator.Current.GetService<IConfiguration>();

            this._categoriesSource
                .AsObservableCache()
                .Connect()
                .Transform(c => c)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out this._categories)
                .Subscribe();

            isValid = this.WhenAnyValue(vm => vm.Name, vm => vm.Description, vm => vm.SelectedCategory)
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

            var canEdit = this.WhenAnyValue(vm => vm.IsValid, vm => vm.SelectedCategory)
                .Select(o =>
                {
                    return o.Item1 && (o.Item2 != null);
                });

            this.EditCategoryCommand =
                ReactiveCommand.CreateFromTask(editCategory, canEdit, RxApp.MainThreadScheduler);

            var canAdd = this.WhenAnyValue(vm => vm.IsValid);

            this.AddCategoryCommand =
                ReactiveCommand.CreateFromTask(addCategory, canAdd, RxApp.MainThreadScheduler);

            var canDelete = this.WhenAnyValue(vm => vm.SelectedCategory)
                .Select(o => o != null);

            this.DeleteCategoryCommand =
                ReactiveCommand.CreateFromTask(deleteCategory, canDelete, RxApp.MainThreadScheduler);

            this.ToggleList = ReactiveCommand.Create(() => this.IsListCollapsed = !this.IsListCollapsed);

            this.UploadIconCommand = ReactiveCommand.Create(chooseImage, outputScheduler: RxApp.MainThreadScheduler);

            getCategories();

        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit,Unit> EditCategoryCommand { get; }
        public ReactiveCommand<Unit, Unit> AddCategoryCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCategoryCommand { get; }
        public ReactiveCommand<Unit, Unit> UploadIconCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleList { get; }

        #endregion

        #region Methods

        private async Task<bool> getCategories()
        {
            var service = new CategoryService(_configuration);

            var response = await service.GetAll();

            if (response != null)
            {
                Parallel.ForEach(response, category =>
                {
                    this._categoriesSource.AddOrUpdate(category);
                });

                return true;
            }

            return false;
        }

        private async Task editCategory()
        {
            IsBusy = true;

            if (SelectedCategory != null)
            {
                var service = new CategoryService(_configuration);

                var response = await service.Edit(new Models.Shared.ViewModels.CategoryViewModel
                {
                    Name = this.Name,
                    Description = this.Description,
                    CategoryId = SelectedCategory.Id
                });

                if (response != null)
                {
                    this._categoriesSource.AddOrUpdate(response);

                    //Upload image
                    if (!string.IsNullOrEmpty(this.IconPath))
                    {
                        var result = await service.UploadIcon(SelectedCategory.Id, this.IconPath);
                        if (result == null)
                        {
                            Message = "Image has not been uploaded";
                        }
                        this.IconPath = string.Empty;
                    }
                }
            }
            IsBusy = false;
        }

        private async Task addCategory()
        {
            IsBusy = true;

            var service = new CategoryService(_configuration);
            var response = await service.Create(new Models.Shared.ViewModels.CategoryViewModel
            {
                Name = this.Name,
                Description = this.Description
            });

            if (response != null)
            {
                this._categoriesSource.AddOrUpdate(response);

                //Upload image
                if (!string.IsNullOrEmpty(this.IconPath))
                {
                    var result = await service.UploadIcon(response.Id, this.IconPath);
                    if (result == null)
                    {
                        Message = "Image has not been uploaded";
                    }
                    this.IconPath = string.Empty;
                }
            }

            IsBusy = false;
        }

        private async Task deleteCategory()
        {
            IsBusy = true;
            if (SelectedCategory != null)
            {
                var service = new CategoryService(_configuration);
                var response = await service.Delete(SelectedCategory.Id);

                if (response != null)
                {
                    this._categoriesSource.Remove(response);
                }
            }
            IsBusy = false;
        }

        private void chooseImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Images|*.jpg;*png;*.bmp";
                ofd.FileName = string.Empty;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.IconPath = ofd.FileName;
                }
            }
        }

        #endregion

    }
}
