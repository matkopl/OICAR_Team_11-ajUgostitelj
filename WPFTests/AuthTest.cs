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
    public class AuthTest
    {
        [Test]
        public void User_Can_Register_Login_ChangePassword_And_Anonymize()
        {
            // Generiraj random username/email za test
            var rand = new Random();
            var username = $"testuser{rand.Next(10000, 99999)}";
            var email = $"{username}@test.com";
            var password = "Test123!";
            var newPassword = "12345678";

            using (var app = Application.Launch("WPF.exe"))
            using (var automation = new UIA3Automation())
            {
                // 1. OTVORI REGISTER SCREEN
                var loginWindow = app.GetMainWindow(automation);
                var registerButton = loginWindow.FindFirstDescendant(cf => cf.ByText("Register here")).AsButton();
                registerButton.Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Register - ajUgostitelj")),
                    TimeSpan.FromSeconds(10));

                var registerWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Register - ajUgostitelj"));
                Assert.That(registerWindow, Is.Not.Null);

                // 2. ISPUNI REGISTRACIJU
                registerWindow.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox")).AsTextBox().Enter(username);
                registerWindow.FindFirstDescendant(cf => cf.ByAutomationId("EmailTextBox")).AsTextBox().Enter(email);
                registerWindow.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox().Enter(password);
                registerWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConfirmPasswordBox")).AsTextBox().Enter(password);

                var regButton = registerWindow.FindFirstDescendant(cf => cf.ByText("Register")).AsButton();
                regButton.Invoke();

                // 3. KLIKNI OK NA SUCCESS MESSAGEBOX
                var regMsgBox = Retry.WhileNull(() =>
                        automation.GetDesktop().FindAllChildren()
                            .FirstOrDefault(w =>
                                w.FindFirstDescendant(cf =>
                                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text)
                                )?.AsLabel().Text.Contains("Registration successful") == true
                            ),
                        TimeSpan.FromSeconds(10)).Result;

                Assert.That(regMsgBox, Is.Not.Null, "Registration MessageBox not found.");

                var regOkButton = regMsgBox.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button)).AsButton();
                Assert.That(regOkButton, Is.Not.Null, "OK button not found in registration MessageBox.");
                regOkButton.Invoke();
                // 4. LOGIN S NOVIM KORISNIKOM
                var loginWindow2 = Retry.WhileNull(() =>
                            app.GetAllTopLevelWindows(automation)
                                .FirstOrDefault(w => w.Title.Contains("Login - ajUgostitelj")),
                            TimeSpan.FromSeconds(15)).Result;
                Assert.That(loginWindow2, Is.Not.Null, "Login window after registration not found.");

                loginWindow2.FindFirstDescendant(cf => cf.ByAutomationId("UsernameTextBox")).AsTextBox().Enter(username);
                loginWindow2.FindFirstDescendant(cf => cf.ByAutomationId("PasswordBox")).AsTextBox().Enter(password);
                loginWindow2.FindFirstDescendant(cf => cf.ByAutomationId("LoginButton")).AsButton().Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj")),
                    TimeSpan.FromSeconds(10));

                var mainUiWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Home - AjUgostitelj"));
                Assert.That(mainUiWindow, Is.Not.Null);

                // 5. OTVORI PROFILE DETAILS
                var profileButton = mainUiWindow.FindFirstDescendant(cf => cf.ByText("Profile details")).AsButton();
                profileButton.Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Profile - ajUgostitelj")),
                    TimeSpan.FromSeconds(10));

                var profileWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Profile - ajUgostitelj"));
                Assert.That(profileWindow, Is.Not.Null);

                // 6. PROMIJENI LOZINKU
                var changePassButton = profileWindow.FindFirstDescendant(cf => cf.ByText("Change Password")).AsButton();
                changePassButton.Invoke();

                Retry.WhileNull(() =>
                    app.GetAllTopLevelWindows(automation)
                        .FirstOrDefault(w => w.Title.Contains("Profile - Password management")),
                    TimeSpan.FromSeconds(10));

                var changePassWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Profile - Password management"));
                Assert.That(changePassWindow, Is.Not.Null);

                var currentPassBox = changePassWindow.FindFirstDescendant(cf => cf.ByAutomationId("CurrentPasswordBox"));
                Assert.That(currentPassBox, Is.Not.Null, "CurrentPasswordBox not found.");
                currentPassBox.Focus();
                FlaUI.Core.Input.Keyboard.Type(password);

                Thread.Sleep(200);

                var newPassBox = changePassWindow.FindFirstDescendant(cf => cf.ByAutomationId("NewPasswordBox"));
                Assert.That(newPassBox, Is.Not.Null, "NewPasswordBox not found.");
                newPassBox.Focus();
                FlaUI.Core.Input.Keyboard.Type(newPassword);
                Thread.Sleep(200);

                var submitButton = changePassWindow.FindFirstDescendant(cf => cf.ByText("Submit")).AsButton();
                Assert.That(submitButton, Is.Not.Null, "Submit button not found.");
                submitButton.Invoke();

                Thread.Sleep(500);
                foreach (var win in automation.GetDesktop().FindAllChildren())
                {
                    var label = win.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text));
                }

                var passMsgBox = Retry.WhileNull(() =>
                    automation.GetDesktop().FindAllChildren()
                        .FirstOrDefault(w =>
                            w.FindFirstDescendant(cf =>
                                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text)
                            )?.AsLabel().Text.ToLower().Contains("password changed") == true
                        ),
                    TimeSpan.FromSeconds(10)).Result;
                Assert.That(passMsgBox, Is.Not.Null, "Password changed MessageBox not found.");

                var passOkButton = passMsgBox.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button)).AsButton();
                Assert.That(passOkButton, Is.Not.Null, "OK button not found in change password MessageBox.");
                passOkButton.Invoke();


                // 7. ANONIMIZIRAJ KORISNIKA
                profileWindow = app.GetAllTopLevelWindows(automation)
                    .FirstOrDefault(w => w.Title.Contains("Profile - ajUgostitelj"));


                var anonCheck = profileWindow.FindFirstDescendant(cf => cf.ByAutomationId("AnonCheckBox")).AsCheckBox();
                Assert.That(anonCheck, Is.Not.Null, "AnonCheckBox not found.");

                anonCheck.Focus();
                anonCheck.Click(); 

                var anonMsgBox = Retry.WhileNull(() =>
                    automation.GetDesktop().FindAllChildren()
                        .FirstOrDefault(w => w.FindFirstDescendant(cf =>
                            cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text)
                        )?.AsLabel().Text.Contains("anonimizirati") == true),
                    TimeSpan.FromSeconds(10)).Result;
                Assert.That(anonMsgBox, Is.Not.Null, "Anonimizacija MessageBox not found.");

                var yesButton = anonMsgBox.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))
                    .FirstOrDefault(b => b.Name.Equals("Yes", StringComparison.OrdinalIgnoreCase));
                Assert.That(yesButton, Is.Not.Null, "Yes button not found.");
                yesButton.AsButton().Invoke();


                // KLIKNI OK NA ZAVRŠNU PORUKU
                var anonSuccessBox = Retry.WhileNull(() =>
    automation.GetDesktop().FindAllChildren()
        .FirstOrDefault(w =>
            w.FindFirstDescendant(cf =>
                cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text)
            )?.AsLabel().Text.ToLower().Contains("uspješno anonimiziran") == true
        ),
    TimeSpan.FromSeconds(10)).Result;
                Assert.That(anonSuccessBox, Is.Not.Null, "Anonimizacija success MessageBox not found.");

                var okButton = anonSuccessBox.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))
                    .FirstOrDefault(b =>
                        b.Name.Equals("OK", StringComparison.OrdinalIgnoreCase) ||
                        b.Name.Equals("U redu", StringComparison.OrdinalIgnoreCase)
                    );
                Assert.That(okButton, Is.Not.Null, "OK button not found in anonimizacija MessageBox.");
                okButton.AsButton().Invoke();


                // Ovdje će se aplikacija zatvoriti (Application.Current.Shutdown())
            }
        }
    }
}
