using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Views
{
    public partial class TablesWindow : Window
    {
        private readonly ITableRepository _tableRepo;
        private readonly string _token;
        private ObservableCollection<TableDto> _tables = new();

        public TablesWindow(string token)
        {
            InitializeComponent();
            _token = token;

            _tableRepo = ((App)Application.Current)
                .ServiceProvider
                .GetRequiredService<ITableRepository>();

            Loaded += async (_, __) => await LoadTablesAsync();
        }

        private async Task LoadTablesAsync()
        {
            try
            {
                var all = await _tableRepo.GetAllAsync(_token);
                _tables = new ObservableCollection<TableDto>(all);
                dgTables.ItemsSource = _tables;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Greška pri učitavanju stolova:\n{ex.Message}",
                    "Greška",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = tbNewTableName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name) || name == "Unesite naziv stola...")
            {
                MessageBox.Show(
                    "Unesite naziv stola.",
                    "Upozorenje",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dto = new TableDto { Name = name };
                var created = await _tableRepo.CreateAsync(_token, dto);
                _tables.Add(created);
                // reset placeholder
                tbNewTableName.Text = "Unesite naziv stola...";
                tbNewTableName.Foreground = Brushes.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Greška pri dodavanju stola:\n{ex.Message}",
                    "Greška",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTables.SelectedItem is not TableDto toDelete) return;

            var result = MessageBox.Show(
                $"Obrisati stol '{toDelete.Name}'?",
                "Potvrdi brisanje",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _tableRepo.DeleteAsync(_token, toDelete.Id);
                _tables.Remove(toDelete);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Greška pri brisanju stola:\n{ex.Message}",
                    "Greška",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        
        private void TbNewTableName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbNewTableName.Text == "Unesite naziv stola...")
            {
                tbNewTableName.Text = "";
                tbNewTableName.Foreground = Brushes.Black;
            }
        }

        private void TbNewTableName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNewTableName.Text))
            {
                tbNewTableName.Text = "Unesite naziv stola...";
                tbNewTableName.Foreground = Brushes.Gray;
            }
        }

        private void BtnGenerateQr_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("QR kod generiran. Bravo!",
                            "Generiranje QR koda",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

    }
}
