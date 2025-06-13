using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Linq;

namespace WPFTests
{
    [TestFixture, Apartment(System.Threading.ApartmentState.STA)]
    public class CategoryCrudTests
    {
        private const string AppPath = "WPF.exe";
        private const string AdminUsername = "mihoperic";
        private const string AdminPassword = "12345678";

        [Test]
        public void Admin_Can_Create_And_Delete_Category()
        {
            var rnd = new Random();
            var testCatName = "testttttt";

            using var app = Application.Launch(AppPath);
            using var automation = new UIA3Automation();

            
            var login = app.GetMainWindow(automation);
            login.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))
                 .AsTextBox().Enter(AdminUsername);
            login.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"))
                 .AsTextBox().Enter(AdminPassword);
            login.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))
                 .AsButton().Invoke();

            
            var home = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj")),
                TimeSpan.FromSeconds(10)).Result;
            Assert.NotNull(home, "Home window did not appear");

            
            home.FindFirstDescendant(cf => cf.ByText("Admin Panel"))
                .AsButton().Invoke();
            var panel = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Admin panel")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.NotNull(panel, "Admin panel did not open");

            
            panel.FindFirstDescendant(cf => cf.ByText("Manage categories")
                                           .Or(cf.ByText("Manage Categories")))
                 .AsButton().Invoke();
            var catWin = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                         .FirstOrDefault(w => w.Title.Contains("Upravljanje kategorijama")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.NotNull(catWin, "CategoryCrudWindow did not open");

            
            var nameBox = catWin.FindFirstDescendant(cf => cf.ByAutomationId("tbNewCategory"))
                                .AsTextBox();
            Assert.NotNull(nameBox, "Name textbox not found");
            nameBox.Enter(testCatName);

            
            var addBtn = catWin.FindFirstDescendant(cf => cf.ByAutomationId("btnAdd"))
                               .AsButton();
            Assert.NotNull(addBtn, "Add button not found");
            addBtn.Invoke();

            
            var grid = catWin.FindFirstDescendant(cf => cf.ByAutomationId("dgCategories"))
                             .AsDataGridView();
            Assert.NotNull(grid, "DataGrid not found");

            Retry.WhileFalse(
                () => grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testCatName),
                TimeSpan.FromSeconds(5)
            );

            var newRow = grid.Rows.First(r => r.Cells[0].Value?.ToString() == testCatName);
            Assert.NotNull(newRow, $"New row '{testCatName}' not found");

            
            var delBtn = newRow.FindFirstDescendant(cf => cf.ByAutomationId("btnDelete"))
                               .AsButton();
            Assert.NotNull(delBtn, "Row delete button not found");
            delBtn.Invoke();

            
            var confirm = Retry.WhileNull(
                () => automation.GetDesktop().FindAllChildren()
                           .FirstOrDefault(w =>
                               w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))
                                ?.AsLabel().Text.Contains("Obrisati kategoriju") == true),
                TimeSpan.FromSeconds(5)
            ).Result;
            Assert.NotNull(confirm, "Confirmation dialog not found");

            confirm.FindFirstDescendant(cf => cf.ByText("Yes").Or(cf.ByText("Da")))
                   .AsButton().Invoke();

            
            Retry.WhileTrue(
                () => grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testCatName),
                TimeSpan.FromSeconds(5)
            );
            Assert.False(
                grid.Rows.Any(r => r.Cells[0].Value?.ToString() == testCatName),
                "Deleted category still in grid"
            );
        }
    }
}
