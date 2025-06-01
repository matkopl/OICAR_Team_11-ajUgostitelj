using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using QRCoder;                        
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Views
{
    public partial class TablesWindow : Window
    {
        private readonly ITableRepository _repo;
        private readonly string _token;
        private byte[]? _lastQrBytes;

        public TablesWindow(string token)
        {
            InitializeComponent();
            _token = token;

            
            _repo = ((App)Application.Current)
                     .ServiceProvider
                     .GetRequiredService<ITableRepository>();

            Loaded += async (_, __) =>
            {
                await RefreshTablesAsync();
            };
        }

        
        private async Task RefreshTablesAsync()
        {
            try
            {
                var tables = await _repo.GetAllAsync(_token);
                dgTables.ItemsSource = tables;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju stolova:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async void BtnAddTable_Click(object sender, RoutedEventArgs e)
        {
            var name = tbNewTableName.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Upišite naziv novog stola.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newTable = new TableDto { Name = name };
                await _repo.CreateAsync(_token, newTable);
                tbNewTableName.Clear();
                await RefreshTablesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri dodavanju stola:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async void BtnDeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (dgTables.SelectedItem is not TableDto sel)
            {
                MessageBox.Show("Odaberite stol u tablici.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var res = MessageBox.Show(
                $"Obrisati stol '{sel.Name}'?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            try
            {
                await _repo.DeleteAsync(_token, sel.Id);
                await RefreshTablesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju stola:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnGenerateQr_Click(object sender, RoutedEventArgs e)
        {
            if (dgTables.SelectedItem is not TableDto table)
            {
                MessageBox.Show("Prvo odaberite stol.", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            string url = $"https://oicar-team-11-ajugostitelj-solowebapp.onrender.com/?table={table.Id}";

            
            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            _lastQrBytes = new PngByteQRCode(qrData).GetGraphic(20);

            
            var bmp = new BitmapImage();
            using var ms = new MemoryStream(_lastQrBytes);
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();

            imgQr.Source = bmp;
            btnSaveQr.IsEnabled = true;
        }



        private void BtnSaveQr_Click(object sender, RoutedEventArgs e)
        {
            if (_lastQrBytes == null) return;

            var dlg = new SaveFileDialog
            {
                Filter = "PNG slika (*.png)|*.png",
                FileName = $"QR_stol_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            };
            if (dlg.ShowDialog() != true) return;

            try
            {
                File.WriteAllBytes(dlg.FileName, _lastQrBytes);
                MessageBox.Show("QR kod spremljen.", "Uspjeh",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri spremanju:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
