using System;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;

namespace WPFTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class TableCrudTests
    {
        private const string AppPath = "WPF.exe";
        private const string AdminUsername = "mihoperic";
        private const string AdminPassword = "12345678";

        [Test]
        public void Admin_Can_Create_And_Delete_Table()
        {
            var rnd = new Random();
            var testTableName = "testttttttttttt";

            using var app = Application.Launch(AppPath);
            using var automation = new UIA3Automation();

            
            var login = app.GetMainWindow(automation);
            login.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox"))
                 .AsTextBox().Enter(AdminUsername);
            login.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox"))
                 .AsTextBox().Enter(AdminPassword);
            login.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton"))
                 .AsButton().Invoke();

            
            var main = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                          .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj")),
                TimeSpan.FromSeconds(10)).Result;
            Assert.That(main, Is.Not.Null);

            
            var adminBtn = main.FindFirstDescendant(cf => cf.ByText("Admin Panel")).AsButton();
            adminBtn.Invoke();

            
            var panel = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                          .FirstOrDefault(w => w.Title.Contains("Admin panel")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(panel, Is.Not.Null);

            
            var manageTables = panel.FindFirstDescendant(cf =>
                                   cf.ByText("Manage tables")
                                  .Or(cf.ByText("Manage Tables")))
                                  .AsButton();
            manageTables.Invoke();

            
            var tw = Retry.WhileNull(
                () => app.GetAllTopLevelWindows(automation)
                          .FirstOrDefault(w => w.Title.Contains("Upravljanje stolovima")),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(tw, Is.Not.Null);

            
            var txtNew = tw.FindFirstDescendant(cf => cf.ByAutomationId("tbNewTableName"))
                           .AsTextBox();
            txtNew.Enter(testTableName);
            tw.FindFirstDescendant(cf => cf.ByAutomationId("btnAddTable"))
              .AsButton().Invoke();

            
            var grid = tw.FindFirstDescendant(cf => cf.ByAutomationId("dgTables"))
                         .AsDataGridView();
            Retry.WhileFalse(
                () => grid.Rows.Any(r => r.Cells[1].Value?.ToString() == testTableName),
                TimeSpan.FromSeconds(5));
            var newRow = grid.Rows.First(r => r.Cells[1].Value?.ToString() == testTableName);

            
            newRow.Cells[1].Click();
            tw.FindFirstDescendant(cf => cf.ByAutomationId("btnDeleteTable"))
              .AsButton().Invoke();

            
            var confirm = Retry.WhileNull(
                () => automation.GetDesktop().FindAllChildren()
                    .FirstOrDefault(w =>
                        w.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))
                         ?.AsLabel().Text.Contains("Obrisati stol") == true),
                TimeSpan.FromSeconds(5)).Result;
            Assert.That(confirm, Is.Not.Null);

            
            confirm.FindFirstDescendant(cf =>
                    cf.ByControlType(ControlType.Button)
                      .And(cf.ByText("Yes").Or(cf.ByText("Da"))))
                   .AsButton().Invoke();

            
            Retry.WhileTrue(
                () => grid.Rows.Any(r => r.Cells[1].Value?.ToString() == testTableName),
                TimeSpan.FromSeconds(5));
            Assert.False(
                grid.Rows.Any(r => r.Cells[1].Value?.ToString() == testTableName),
                "Deleted table still appears");
        }
    }
}
