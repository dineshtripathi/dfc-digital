﻿using DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Models;
using System;
using System.Linq;
using TestStack.Seleno.PageObjects;

namespace DFC.Digital.AcceptanceTest.Infrastructure.Pages
{
    public class HomePage : SitefinityPage<JobProfileSearchBoxViewModel>
    {
        public bool ServiceName => Find.OptionalElement(OpenQA.Selenium.By.Id("site-header")) != null;

        public bool HasErrorMessage => Find.OptionalElement(OpenQA.Selenium.By.ClassName("error")) != null;

        public bool HasJobProfileCategoriesSection => Find.OptionalElement(OpenQA.Selenium.By.ClassName("homepage-jobcategories")) != null;

        internal bool HasSearchWidget => Find.OptionalElement(OpenQA.Selenium.By.Id("header-search")) != null;

        internal bool HasSuggestedSearch => Find.OptionalElement(OpenQA.Selenium.By.ClassName("ui-menu-item-wrapper")) != null;

        internal bool UrlShowsSearchAction => UrlContains("/home/search/");

        public string GetJobProfileCategoryUrl(int category)
        {
            return Find.Elements(OpenQA.Selenium.By.CssSelector(".homepage-jobcategories a"))?.ElementAt(category - 1).GetAttribute("href");
        }

        public T Search<T>(JobProfileSearchBoxViewModel model)
            where T : UiComponent, new()
        {
            Input.Model(model);
            return Navigate.To<T>(OpenQA.Selenium.By.ClassName("submit"));
        }

        public T Search<T>()
            where T : UiComponent, new()
        {
            return Navigate.To<T>(OpenQA.Selenium.By.ClassName("submit"));
        }

        public T GoToResult<T>(int index)
            where T : UiComponent, new()
        {
            var results = Find.Elements(OpenQA.Selenium.By.CssSelector(".homepage-jobcategories a")).ToList();
            return Navigate.To<T>(results[index - 1].GetAttribute("href"));
        }

        public T ClickCookiesLink<T>()
            where T : UiComponent, new()
        {
            return Navigate.To<T>(OpenQA.Selenium.By.CssSelector(".footer-meta-inner a"));
        }

        public T GoToSurvey<T>()
            where T : UiComponent, new()
        {
            return Navigate.To<T>(OpenQA.Selenium.By.ClassName("survey_action"));
        }

        public void SelectSuggestedSearch(int index)
        {
            var list = Find.Elements(OpenQA.Selenium.By.ClassName("ui-menu-item-wrapper")).ToList();
            list.ElementAt(index - 1).Click();
        }

        internal string GetSuggestedSearchText(int index) => Find.Elements(OpenQA.Selenium.By.ClassName("ui-menu-item-wrapper")).ElementAt(index - 1).Text;

        internal void EnterSearchText(string text) => Find.OptionalElement(OpenQA.Selenium.By.Id("SearchTerm")).SendKeys(text); //had to use send keys to simulate typing.

        internal bool HasExploreCategoryText(string expectedText) => Find.Element(OpenQA.Selenium.By.ClassName("heading-medium"))
            .Text.Equals(expectedText, StringComparison.InvariantCultureIgnoreCase);
    }
}