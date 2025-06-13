using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace WPFTests
{
    [TestFixture, Apartment(System.Threading.ApartmentState.STA)]
    public class AddAndDeleteInventoryItemTest
    {
        [Test]
        public void Admin_Can_Add_And_Delete_Inventory_Item()
        {
            using (var app = Application.Launch("WPF.exe"))
            using (var automation = new UIA3Automation())
            {
                var loginWindow = app.GetMainWindow(automation);
                loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox")).AsTextBox().Enter("mihoperic");
                loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox().Enter("12345678");
                loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")).AsButton().Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj")),
                    TimeSpan.FromSeconds(5));

                var mainWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj"));
                Assert.That(mainWindow, Is.Not.Null);

                mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AdminPanelButton")).AsButton().Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Admin panel - AjUgostitelj")),
                    TimeSpan.FromSeconds(5));

                var adminPanelWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Admin panel - AjUgostitelj"));
                Assert.That(adminPanelWindow, Is.Not.Null);

                adminPanelWindow.FindFirstDescendant(cf => cf.ByText("Manage inventory")).AsButton().Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Inventory management")),
                    TimeSpan.FromSeconds(5));

                var inventoryWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Inventory management"));
                Assert.That(inventoryWindow, Is.Not.Null);

                var inventoryList = inventoryWindow.FindFirstDescendant(cf => cf.ByAutomationId("InventoryListView")).AsListBox();
                Assert.That(inventoryList, Is.Not.Null);

                int countAtStart = inventoryList.Items.Length;

                inventoryWindow.FindFirstDescendant(cf => cf.ByText("Add to Inventory")).AsButton().Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Inventory management - Create")),
                    TimeSpan.FromSeconds(5));

                var createInventoryWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Inventory management - Create"));
                Assert.That(createInventoryWindow, Is.Not.Null);

                var productComboBox = createInventoryWindow.FindFirstDescendant(cf => cf.ByAutomationId("ProductComboBox")).AsComboBox();
                Assert.That(productComboBox, Is.Not.Null);
                productComboBox.Select(0);

                var quantityTextBox = createInventoryWindow.FindFirstDescendant(cf => cf.ByAutomationId("QuantityTextBox")).AsTextBox();
                Assert.That(quantityTextBox, Is.Not.Null);
                int randomQuantity = new Random().Next(1, 100);
                quantityTextBox.Enter(randomQuantity.ToString());

                createInventoryWindow.FindFirstDescendant(cf => cf.ByText("Add")).AsButton().Invoke();

                var messageBox = Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Create") || w.Title.Contains("Information") || w.Title.Contains("Success")),
                    TimeSpan.FromSeconds(5)).Result;
                Assert.That(messageBox, Is.Not.Null, "Success MessageBox not found after adding inventory.");

                var okButton = messageBox.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))?.AsButton();
                Assert.That(okButton, Is.Not.Null, "OK button not found in MessageBox.");
                okButton.Invoke();

                Retry.WhileTrue(() =>
                {
                    inventoryList = inventoryWindow.FindFirstDescendant(cf => cf.ByAutomationId("InventoryListView")).AsListBox();
                    return inventoryList.Items.Length != countAtStart + 1;
                }, timeout: TimeSpan.FromSeconds(5));

                var lastItem = inventoryList.Items.LastOrDefault();
                Assert.That(lastItem, Is.Not.Null, "No inventory items found to delete.");
                lastItem.Select();

                var deleteButton = inventoryWindow.FindFirstDescendant(cf => cf.ByText("Delete Inventory")).AsButton();
                Assert.That(deleteButton, Is.Not.Null, "Delete Inventory button not found.");
                deleteButton.Invoke();

                var deleteMsgBox = Retry.WhileNull(() =>
                        automation.GetDesktop().FindAllChildren()
                                .FirstOrDefault(w =>
                                    w.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button)) != null &&
                                    w.FindFirstDescendant(cf => cf.ByText("Inventory item deleted successfully!")) != null
                                ),
                                TimeSpan.FromSeconds(5)).Result;

                if (deleteMsgBox == null)
                {
                    var allWindows = automation.GetDesktop().FindAllChildren();
                    foreach (var win in allWindows)
                    {
                        var label = win.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
                        if (label != null)
                            Console.WriteLine($"  Label: {label.Name} | {label.AsLabel()?.Text}");
                    }
                }
                Assert.That(deleteMsgBox, Is.Not.Null, "Delete MessageBox not found.");

                var deleteOkButton = deleteMsgBox.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))?.AsButton();
                Assert.That(deleteOkButton, Is.Not.Null, "OK button not found in delete MessageBox.");
                deleteOkButton.Invoke();


                Retry.WhileTrue(() =>
                {
                    inventoryList = inventoryWindow.FindFirstDescendant(cf => cf.ByAutomationId("InventoryListView")).AsListBox();
                    return inventoryList.Items.Length != countAtStart;
                }, timeout: TimeSpan.FromSeconds(5));

                Assert.That(inventoryList.Items.Length, Is.EqualTo(countAtStart), $"Inventory item was not deleted. Start: {countAtStart}, Now: {inventoryList.Items.Length}");
            }
        }
    }
}
