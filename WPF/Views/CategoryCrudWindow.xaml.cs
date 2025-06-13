using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF.Services;
using WebAPI.DTOs;
using Microsoft.Extensions.DependencyInjection;
using WPF.Repositories;

namespace WPF.Views
{
    public partial class CategoryCrudWindow : Window
    {
        private readonly ICategoryService _service;
        private readonly IProductRepository _productRepo;
        private readonly string _token;
        private readonly ObservableCollection<CategoryDto> _categories = new();

        public CategoryCrudWindow(string token)
        {
            InitializeComponent();
            _token = token;

            var sp = (App.Current as App)!.ServiceProvider;
            var catRepo = sp.GetRequiredService<ICategoryRepository>();
            _service = new CategoryService(catRepo, token);

            
            _productRepo = sp.GetRequiredService<IProductRepository>();

            Loaded += async (_, __) => await LoadCategoriesAsync();
        }


        private async Task LoadCategoriesAsync()
        {
            _categories.Clear();
            foreach (var c in await _service.GetAllAsync())
                _categories.Add(c);
            dgCategories.ItemsSource = _categories;
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = tbNewCategory.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Unesite naziv kategorije.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var created = await _service.CreateAsync(name);
                _categories.Add(created);
                tbNewCategory.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DgCategories_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is CategoryDto dto)
            {
                try
                {
                    await _service.UpdateAsync(dto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri spremanju: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not CategoryDto dto)
                return;

            
            var prodsInCat = await _productRepo.GetByCategoryAsync(_token, dto.Id);
            if (prodsInCat.Any())
            {
                MessageBox.Show(
                    $"Ne možete obrisati kategoriju “{dto.Name}”\r\n" +
                    $"jer sadrži {prodsInCat.Count()} proizvoda.",
                    "Upozorenje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            
            var result = MessageBox.Show(
                $"Obrisati kategoriju '{dto.Name}'?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _service.DeleteAsync(dto.Id);
                _categories.Remove(dto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
