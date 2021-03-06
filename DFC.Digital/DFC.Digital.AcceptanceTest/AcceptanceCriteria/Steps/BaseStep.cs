﻿using DFC.Digital.AcceptanceTest.Infrastructure.Config;
using DFC.Digital.AcceptanceTest.Infrastructure.Pages;
using TechTalk.SpecFlow;
using TestStack.Seleno.Configuration;

namespace DFC.Digital.AcceptanceTest.AcceptanceCriteria.Steps
{
    public class BaseStep
    {
        private readonly BrowserStackSelenoHost browserStackSelenoHost;

        public BaseStep(BrowserStackSelenoHost browserStackSelenoHost, ScenarioContext scenarioContext)
        {
            this.browserStackSelenoHost = browserStackSelenoHost;
            ScenarioContext = scenarioContext;
            RootUrl = browserStackSelenoHost.Seleno.Application.WebServer.BaseUrl;
        }

        public string CurrentBrowserUrl => Instance.Application.Browser.Url;

        protected ScenarioContext ScenarioContext { get; set; }

        protected SelenoHost Instance => browserStackSelenoHost.Seleno;

        protected string RootUrl { get; set; }

        public string GetCookieValue(string cookie)
        {
            var selecedCookie = Instance.Application.Browser.Manage().Cookies.GetCookieNamed(cookie);

            return selecedCookie?.Value;
        }

        public void DeleteCookie(string cookie)
        {
            Instance.Application.Browser.Manage().Cookies.DeleteCookieNamed(cookie);
        }

        public void RefreshPage()
        {
            Instance.Application.Browser.Navigate().Refresh();
        }

        public void PressBack()
        {
            Instance.Application.Browser.Navigate().Back();
        }

        internal TPage NavigateToHomePage<TPage, TModel>()
            where TPage : SitefinityPage<TModel>, new()
            where TModel : class, new()
        {
            //this.Instance = LocalBrowserHost.GetInstanceFor("HomePage");
            return NavigateToPage<TPage, TModel>(RootUrl);
        }

        internal TPage NavigateToSearchResultsPage<TPage, TModel>(string searchTerm)
            where TPage : SitefinityPage<TModel>, new()
            where TModel : class, new()
        {
            //this.Instance = LocalBrowserHost.GetInstanceFor("SearchResultsPage");
            return NavigateToPage<TPage, TModel>($"{RootUrl}/search-results?searchTerm={searchTerm}");
        }

        internal TPage NavigateToJobProfilePage<TPage, TModel>(string jobProfile)
            where TPage : SitefinityPage<TModel>, new()
            where TModel : class, new()
        {
            //this.Instance = LocalBrowserHost.GetInstanceFor("JobProfilePage");
            return NavigateToPage<TPage, TModel>($"{RootUrl}/job-profiles/{jobProfile}");
        }

        internal TPage NavigateToCategoryPage<TPage, TModel>(string category)
            where TPage : SitefinityPage<TModel>, new()
            where TModel : class, new()
        {
            return NavigateToPage<TPage, TModel>($"{RootUrl}/job-categories/{category}");
        }

        internal TPage GetNavigatedPage<TPage>()
            where TPage : class, new()
        {
            TPage page;
            if (ScenarioContext.TryGetValue(out page))
            {
                return page;
            }

            return null;
        }

        private TPage NavigateToPage<TPage, TModel>(string url)
            where TPage : SitefinityPage<TModel>, new()
            where TModel : class, new()
        {
            var page = Instance.NavigateToInitialPage<TPage>(url);
            page.AwaitInitialisation();
            ScenarioContext.Set(page);

            return page;
        }
    }
}