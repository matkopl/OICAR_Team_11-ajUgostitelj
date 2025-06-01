using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DTOs;
using WebAPI.Models;
using WPF.Repositories;
using WPF.Services;

namespace WPF.Views
{
    public partial class ProductWindow : Window
    {
        private readonly IProductRepository _repo;
        private readonly ProductService _service;
        private readonly ICategoryRepository _categoryRepo;
        private readonly string _token;

        private ObservableCollection<ProductDto> _products = new();
        private ObservableCollection<CategoryDto> _categories = new();

        public ProductWindow(string token)
        {
            InitializeComponent();
            _token = token;
            var sp = ((App)Application.Current).ServiceProvider;
            _repo = sp.GetRequiredService<IProductRepository>();
            _service = new ProductService(_repo, _token);
            _categoryRepo = sp.GetRequiredService<ICategoryRepository>();

            Loaded += async (_, __) =>
            {
                await LoadCategoriesAsync();
                await LoadProductsAsync();
            };
        }

        
        private async Task LoadCategoriesAsync()
        {
            try
            {
                var allCats = await _categoryRepo.GetAllAsync(_token);
                _categories = new ObservableCollection<CategoryDto>(allCats);

                
                _categories.Insert(0, new CategoryDto { Id = 0, Name = "— odaberi kategoriju —" });
                cbCategory.ItemsSource = _categories;
                cbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dohvaćanju kategorija:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async Task LoadProductsAsync()
        {
            try
            {
                var all = (await _service.GetAllProductsAsync()).ToList();

                
                foreach (var prod in all)
                {
                    var cat = _categories.FirstOrDefault(c => c.Id == prod.CategoryId);
                    prod.CategoryName = cat?.Name ?? "(nedefinirano)";
                }

                _products = new ObservableCollection<ProductDto>(all);
                dgProducts.ItemsSource = _products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju proizvoda:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Foreground == Brushes.Gray)
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        
        private void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb) return;
            if (!string.IsNullOrWhiteSpace(tb.Text)) return;

            switch (tb.Name)
            {
                case "tbName":
                    tb.Text = "Naziv proizvoda";
                    break;
                case "tbDesc":
                    tb.Text = "Opis (opcionalno)";
                    break;
                case "tbPrice":
                    tb.Text = "Cijena";
                    break;
                case "tbQuantity":
                    tb.Text = "Količina";
                    break;
                case "tbImageUrl":
                    tb.Text = "Image URL";
                    break;
            }

            tb.Foreground = Brushes.Gray;
        }

        
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadInputs(out var dto))
                return;

            
            var cat = cbCategory.SelectedItem as CategoryDto;
            dto.CategoryId = cat!.Id;
            dto.CategoryName = cat.Name;

            try
            {
                var created = await _service.CreateProductAsync(dto);
                
                created.CategoryName = dto.CategoryName;
                _products.Add(created);
                ResetInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju proizvoda:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private async void DgProducts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not ProductDto dto) return;

            
            try
            {
                await _service.UpdateProductAsync(dto.Id, dto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri spremanju izmjena:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not ProductDto dto)
                return;

            var result = MessageBox.Show(
                $"Obrisati proizvod '{dto.Name}'?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _service.DeleteProductAsync(dto.Id);
                _products.Remove(dto);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju proizvoda:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private bool TryReadInputs(out ProductDto dto)
        {
            dto = new ProductDto();

            
            if (string.IsNullOrWhiteSpace(tbName.Text) || tbName.Foreground == Brushes.Gray)
            {
                MessageBox.Show("Unesite naziv proizvoda.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            dto.Name = tbName.Text;

            
            dto.Description = (tbDesc.Foreground == Brushes.Gray)
                              ? ""
                              : tbDesc.Text;

            
            if (!decimal.TryParse(tbPrice.Text, out var price))
            {
                MessageBox.Show("Neispravna cijena.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            dto.Price = price;

            
            if (!int.TryParse(tbQuantity.Text, out var qty))
            {
                MessageBox.Show("Neispravna količina.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            dto.Quantity = qty;

           
            if (string.IsNullOrWhiteSpace(tbImageUrl.Text) || tbImageUrl.Foreground == Brushes.Gray)
            {
                MessageBox.Show("Unesite Image URL.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            dto.ImageUrl = tbImageUrl.Text;

            
            if (cbCategory.SelectedIndex <= 0)
            {
                MessageBox.Show("Odaberite kategoriju.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        
        private void ResetInputs()
        {
            tbName.Text = "Naziv proizvoda";
            tbDesc.Text = "Opis (opcionalno)";
            tbPrice.Text = "Cijena";
            tbQuantity.Text = "Količina";
            tbImageUrl.Text = "Image URL";
            cbCategory.SelectedIndex = 0;

            tbName.Foreground = Brushes.Gray;
            tbDesc.Foreground = Brushes.Gray;
            tbPrice.Foreground = Brushes.Gray;
            tbQuantity.Foreground = Brushes.Gray;
            tbImageUrl.Foreground = Brushes.Gray;
        }
    }
}
