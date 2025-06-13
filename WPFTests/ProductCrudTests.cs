using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace WPFTests
{
    [TestFixture, Apartment(System.Threading.ApartmentState.STA)]
    public class ProductCrudTests
    {
       
        private static readonly string AppPath =
            Path.Combine(TestContext.CurrentContext.TestDirectory, "WPF.exe");

        private const string AdminUsername = "mihoperic";
        private const string AdminPassword = "12345678";

        [Test]
        public void Admin_Can_Create_And_Delete_Product()
        {
            
            var rand = new Random();
            var testName = $"testtttt";
            var testDesc = "testttttt";
            var testPrice = "9.99";
            var testQty = "5";
            var testImg = "https://via.placeholder.com";

            
            using var app = Application.Launch(AppPath);
            using var automation = new UIA3Automation();

           
            var loginWin = app.GetMainWindow(automation);
            Assert.That(loginWin.Title, Does.Contain("Login"), "Nismo na Login prozoru");

            loginWin.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))!.AsTextBox()!.Enter(AdminUsername);
            loginWin.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"))!.AsTextBox()!.Enter(AdminPassword);
            loginWin.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))!.AsButton()!.Invoke();

            
            var mainWin = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj")),
                TimeSpan.FromSeconds(10)).Result;
            Assert.That(mainWin, Is.Not.Null, "Glavni prozor nije pronađen");

            
            var adminBtn = mainWin.FindFirstDescendant(cf => cf.ByText("Admin Panel"))!.AsButton()!;
            adminBtn.Invoke();

            var panelWin = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Admin panel")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(panelWin, Is.Not.Null, "Admin panel nije otvoren");

            
            var manageProdBtn = panelWin
                .FindFirstDescendant(cf => cf.ByText("Manage products")
                                           .Or(cf.ByText("Manage Products")))!
                .AsButton()!;
            manageProdBtn.Invoke();

            
            var prodWin = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Upravljanje proizvodima")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(prodWin, Is.Not.Null, "ProductWindow nije otvoren");

            
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("tbName"))!.AsTextBox()!.Enter(testName);
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("tbDesc"))!.AsTextBox()!.Enter(testDesc);
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("tbPrice"))!.AsTextBox()!.Enter(testPrice);
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("tbQuantity"))!.AsTextBox()!.Enter(testQty);
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("tbImageUrl"))!.AsTextBox()!.Enter(testImg);

            
            var combo = prodWin.FindFirstDescendant(cf => cf.ByAutomationId("cbCategory"))!.AsComboBox()!;
            Assert.That(combo.Items.Length, Is.GreaterThan(1), "Nema dostupnih kategorija");
            combo.Select(1);

            
            prodWin.FindFirstDescendant(cf => cf.ByAutomationId("btnAdd"))!.AsButton()!.Invoke();

            
            var grid = prodWin.FindFirstDescendant(cf => cf.ByAutomationId("dgProducts"))!.AsDataGridView()!;
            Retry.WhileFalse(() =>
                grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testName),
                TimeSpan.FromSeconds(5));

            var newRow = grid.Rows.First(r => r.Cells[0].Value?.ToString() == testName);
            Assert.That(newRow, Is.Not.Null, $"Proizvod {testName} nije pronađen");

            
            var delBtn = newRow.Cells.Last()
                                 .FindFirstDescendant(cf => cf.ByControlType(ControlType.Button))!
                                 .AsButton()!;
            delBtn.Invoke();

            
            var confirm = Retry.WhileNull(
                () => automation.GetDesktop().FindAllChildren()
                         .FirstOrDefault(w =>
                             w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))
                              ?.AsLabel().Text.Contains("Obrisati proizvod") == true),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(confirm, Is.Not.Null, "Potvrda brisanja nije prikazana");

            var yes = confirm.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
                             .FirstOrDefault(b =>
                                 b.Name.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
                                 b.Name.Equals("Da", StringComparison.OrdinalIgnoreCase))
                             ?.AsButton();
            Assert.That(yes, Is.Not.Null, "Dugme za potvrdu brisanja nije pronađeno");
            yes!.Invoke();

            
            Retry.WhileTrue(() =>
                grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testName),
                TimeSpan.FromSeconds(5));
            Assert.False(
                grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testName),
                "Obrisani proizvod se i dalje prikazuje");
        }
    }
}
