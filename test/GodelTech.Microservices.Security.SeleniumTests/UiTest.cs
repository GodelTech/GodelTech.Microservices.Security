using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Xunit;

namespace GodelTech.Microservices.Security.SeleniumTests
{
    public class UiTest : IDisposable
    {
        private readonly IWebDriver _webDriver;

        public UiTest()
        {
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");

            new DriverManager().SetUpDriver(new ChromeConfig());

            _webDriver = new ChromeDriver(chromeOptions);
        }

        public void Dispose()
        {
            _webDriver.Quit();
            _webDriver.Dispose();
        }

        [Fact]
        public void Create_WhenExecuted_ReturnsCreateView()
        {
            _webDriver.Navigate().GoToUrl("https://code-maze.com/selenium-aspnet-core-ui-tests/");

            Assert.Equal("Create - EmployeesApp", _webDriver.Title);
            Assert.Contains("Please provide a new employee data", _webDriver.PageSource);
        }
    }
}
