using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DTOs;
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
        private string? _selectedImageFilePath;

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
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
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
                }
                tb.Foreground = Brushes.Gray;
            }
        }

        // Odabir slike
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Slike (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif"
            };
            if (dlg.ShowDialog() != true) return;

            _selectedImageFilePath = dlg.FileName;
            tbImagePath.Text = Path.GetFileName(_selectedImageFilePath);

            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(_selectedImageFilePath);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                imgPreview.Source = bmp;
            }
            catch
            {
                imgPreview.Source = null;
            }
        }

        // Dodaj novi proizvod
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadInputs(out var dto))
                return;

            // Popuni kategoriju
            var cat = cbCategory.SelectedItem as CategoryDto;
            dto.CategoryId = cat!.Id;
            dto.CategoryName = cat.Name;

            // Placeholder URL za sliku
            dto.ImageUrl = "https://via.placeholder.com/150";

            try
            {
                var created = await _service.CreateProductAsync(dto);
                _products.Add(created);
                ResetInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju proizvoda:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Inline update
        private async void DgProducts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is ProductDto dto)
            {
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
        }

        // Brisanje
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not ProductDto dto) return;

            var result = MessageBox.Show(
                $"Obrisati proizvod '{dto.Name}'?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (result != MessageBoxResult.Yes) return;

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

        // Zatvori prozor
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Validacija inputa
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

            dto.Description = tbDesc.Foreground == Brushes.Gray
                              ? ""
                              : tbDesc.Text;

            if (!decimal.TryParse(tbPrice.Text, out var price))
            {
                MessageBox.Show("Neispravna cijena.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            dto.Price = price;

            if (cbCategory.SelectedIndex <= 0)
            {
                MessageBox.Show("Odaberite kategoriju.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Reset forme
        private void ResetInputs()
        {
            tbName.Text = "Naziv proizvoda";
            tbDesc.Text = "Opis (opcionalno)";
            tbPrice.Text = "Cijena";
            cbCategory.SelectedIndex = 0;
            tbName.Foreground = tbDesc.Foreground = tbPrice.Foreground = Brushes.Gray;
            tbImagePath.Text = "Nijedna slika odabrana";
            imgPreview.Source = null;
            _selectedImageFilePath = null;
        }
    }
}
