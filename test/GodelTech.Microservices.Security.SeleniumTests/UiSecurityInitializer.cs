using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Xunit;

namespace GodelTech.Microservices.Security.SeleniumTests
{
    [Collection("TestCollection")]
    public sealed class UiTest : IDisposable
    {
        private readonly TestFixture _fixture;

        private readonly IWebDriver _webDriver;

        public UiTest(TestFixture fixture)
        {
            _fixture = fixture;

            var chromeOptions = new ChromeOptions
            {
                AcceptInsecureCertificates = true
            };
            
            chromeOptions.AddArguments("headless");

            new DriverManager().SetUpDriver(new ChromeConfig());

            _webDriver = new ChromeDriver(AppContext.BaseDirectory, chromeOptions);
        }

        public void Dispose()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [Theory]
        [InlineData("test@example.com", "Secret1234!", "John Doe")]
        [InlineData("alice", "alice", "Alice Smith")]
        [InlineData("bob", "bob", "Bob Smith")]
        public void SecuredPageRequested_RedirectsToIdentityServerLoginPage(string username, string password, string fullName)
        {
            // Arrange
            _webDriver.Navigate().GoToUrl(new Uri(_fixture.MvcApplication.Url, "User"));

            var loginInput = _webDriver.FindElement(By.Id("Username"));
            var passwordInput = _webDriver.FindElement(By.Id("Password"));
            var submitButton = _webDriver.FindElement(By.XPath("//button[@value='login']"));

            loginInput.SendKeys(username);
            passwordInput.SendKeys(password);

            // Act
            submitButton.Click();

            // Assert
            Assert.True(_webDriver.FindElement(By.XPath($"//h1[contains(text(), '{fullName}')]")).Displayed);
        }
    }
}
