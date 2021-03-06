﻿using DFC.Digital.AcceptanceTest.Infrastructure.Config;
using DFC.Digital.AcceptanceTest.Infrastructure.Pages;
using DFC.Digital.AcceptanceTest.Infrastructure.Utilities;
using DFC.Digital.Automation.Test.Utilities;
using DFC.Digital.Data.Interfaces;
using DFC.Digital.Data.Model;
using DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Models;
using FluentAssertions;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Abstractions;

namespace DFC.Digital.AcceptanceTest.AcceptanceCriteria.Steps
{
    [Binding]
    public class JobProfileSearchSteps : BaseStep
    {
        #region Fields

        private SearchResultItem<JobProfileIndex> givenJobProfile;
        private ISearchQueryService<JobProfileIndex> searchQueryService;
        private ISearchService<JobProfileIndex> searchService;
        private ISearchIndexConfig indexConfig;

        #endregion Fields

        #region Ctor

        public JobProfileSearchSteps(ISearchQueryService<JobProfileIndex> searchQueryService, ISearchService<JobProfileIndex> searchService, ISearchIndexConfig indexConfig, ITestOutputHelper outputHelper, BrowserStackSelenoHost browserStackSelenoHost, ScenarioContext scenarioContext) : base(browserStackSelenoHost, scenarioContext)
        {
            this.searchQueryService = searchQueryService;
            this.searchService = searchService;
            OutputHelper = outputHelper;
            this.indexConfig = indexConfig;
        }

        #endregion Ctor

        #region Properties

        private ITestOutputHelper OutputHelper { get; set; }

        #endregion Properties

        #region Given

        [Given(@"that I am viewing the search results page for '(.*)'")]
        public void GivenThatIAmViewingTheSearchResultsPageFor(string previousSearchTerm)
        {
            NavigateToHomePage<HomePage, JobProfileSearchBoxViewModel>()
                .Search<SearchPage>(new JobProfileSearchBoxViewModel
                {
                    SearchTerm = previousSearchTerm
                })
                .SaveTo(ScenarioContext);
        }

        [Given(@"a job profile exists which has an AlternativeTitle")]
        public async Task GivenAJobProfileExistsWhichHasAnAlternativeTitle()
        {
            var result = await searchQueryService.SearchAsync(
                "*",
                new SearchProperties { UseRawSearchTerm = true, Count = 1, FilterBy = "(FilterableAlternativeTitle ne '' and FilterableAlternativeTitle ne null)" });

            result.Results.Count().Should().BeGreaterThan(0);

            givenJobProfile = result.Results.First();
            NavigateToHomePage<HomePage, JobProfileSearchBoxViewModel>();
        }

        [Given(@"a job profile exists which has no AlternativeTitle")]
        public async Task GivenAJobProfileExistsWhichHasNoAlternativeTitle()
        {
            var result = await searchQueryService.SearchAsync(
                "*",
                new SearchProperties { UseRawSearchTerm = true, Count = 1, FilterBy = "(FilterableAlternativeTitle eq '' or FilterableAlternativeTitle eq null)" });

            result.Results.Count().Should().BeGreaterThan(0);

            givenJobProfile = result.Results.First();
            NavigateToHomePage<HomePage, JobProfileSearchBoxViewModel>();
        }

        [Given(@"multiple job profiles exist for '(.*)':")]
        public async Task GivenMultipleJobProfilesPossessSimilarTitles(string searchTerm)
        {
            var result = await searchQueryService.SearchAsync(searchTerm);

            result.Results.Count().Should().BeGreaterThan(1);
            NavigateToHomePage<HomePage, JobProfileSearchBoxViewModel>();
        }

        [Given(@"the following job profiles exist:")]
        public async Task GivenTheFollowingJobProfilesExist(Table table)
        {
            await searchService.PopulateIndexAsync(table.ToJobProfileSearchIndex());
        }

        [Given(@"'(.*)' job profiles exist with '(.*)':")]
        public async Task GivenJobProfilesExistWith(int countOfJobProfiles, string searchTerm)
        {
            await searchService.PopulateIndexAsync(countOfJobProfiles.CreateWithTitle(searchTerm));
        }

        [Given(@"that I am viewing the search results page")]
        public void GivenThatIAmViewingTheSearchResultsPage()
        {
            NavigateToSearchResultsPage<SearchPage, JobProfileSearchResultViewModel>(null);
        }

        #endregion Given

        #region When

        [When(@"I search using '(.*)'")]
        public void WhenISearchUsing(string searchTerm)
        {
            GetNavigatedPage<HomePage>()
                .Search<SearchPage>(new JobProfileSearchBoxViewModel
                {
                    SearchTerm = searchTerm
                })
                .SaveTo(ScenarioContext);
            ScenarioContext.Set(searchTerm, "searchTerm");
        }

        [When(@"I search using the profile's Title")]
        public void WhenISearchUsingTheProfileSTitle()
        {
            GetNavigatedPage<HomePage>()
                .Search<SearchPage>(new JobProfileSearchBoxViewModel
                {
                    SearchTerm = givenJobProfile.ResultItem.FilterableTitle
                })
                .SaveTo(ScenarioContext);
        }

        [When(@"I search using '(.*)' on the results page")]
        public void WhenISearchUsingOnTheResultsPage(string searchTerm)
        {
            GetNavigatedPage<SearchPage>()
                .Search<SearchPage>(new JobProfileSearchResultViewModel
                {
                    SearchTerm = searchTerm,
                    SearchResults = null
                })
                .SaveTo(ScenarioContext);
        }

        [When(@"I click on result title no '(\d+)'")]
        public void WhenIClickOnResultTitle(int result)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            ScenarioContext.Set(searchPage.SelectedProfileTitle(result), "profileSelected");
            ScenarioContext.Set(searchPage.SelectedProfileUrl(result), "profileURL");
            var jobProfilePage = searchPage.GoToResult<JobProfilePage>(result).SaveTo(ScenarioContext);
        }

        [When(@"I click the fill in short survey link")]
        public void WhenIClickTheLink()
        {
            GetNavigatedPage<HomePage>().GoToSurvey<VocSurveyPage>().SaveTo(ScenarioContext);
        }

        [When(@"I navigate to the next page of results")]
        public void WhenINavigateToTheNextPageOfResults()
        {
            GetNavigatedPage<SearchPage>().NextPage<SearchPage>().SaveTo(ScenarioContext);
        }

        [When(@"I click the close survey link")]
        public void WhenIClickTheCloseSurveyLink()
        {
            GetNavigatedPage<HomePage>().CloseSurvey();
        }

        [When(@"I delete the cookie and refresh the page")]
        public void WhenIDeleteTheCookieAndRefreshThePage()
        {
            DeleteCookie("survey");
            RefreshPage();
        }

        [When(@"I click the Find a Career breadcrumb on '(.*)'")]
        public void WhenIClickTheFindACareerBreadcrumb(string page)
        {
            switch (page)
            {
                case "Search results":
                    var searchPage = GetNavigatedPage<SearchPage>();
                    searchPage.ClickFindACareerBreadcrumb<HomePage>()
                        .SaveTo(ScenarioContext);
                    break;

                case "Job profile":
                    var jobProfile = GetNavigatedPage<JobProfilePage>();
                    jobProfile.ClickFindACareerBreadcrumb<HomePage>()
                        .SaveTo(ScenarioContext);
                    break;

                case "Job category":
                    var jobCategory = GetNavigatedPage<JobProfileCategoryPage>();
                    jobCategory.ClickFindACareerBreadcrumb<HomePage>()
                        .SaveTo(ScenarioContext);
                    break;
            }
        }

        [When(@"I select suggested result no '(.*)' and search")]
        public void WhenISelectSuggestedResultNo(int suggestionNo)
        {
            var homePage = GetNavigatedPage<HomePage>();
            ScenarioContext.Set(homePage.GetSuggestedSearchText(suggestionNo), "selectedSearch");
            homePage.SelectSuggestedSearch(suggestionNo);
            homePage.Search<SearchPage>()
            .SaveTo(ScenarioContext);
        }

        [When(@"I enter the term '(.*)'")]
        public void WhenIEnterTheTerm(string searchTerm)
        {
            var homePage = GetNavigatedPage<HomePage>();
            homePage.EnterSearchText(searchTerm);
        }

        [When(@"I click the first category link on result no '(.*)'")]
        public void WhenIClickTheFirstCategoryLinkOnResultNo(int result)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            ScenarioContext.Set(searchPage.GetCategoryInSearchTitle(result), "SelectedCategoryLink");
            searchPage.GoToFirstCategoryLink<JobProfileCategoryPage>(result)
                .SaveTo(ScenarioContext);
        }

        #endregion When

        #region Then

        [Then(@"I can search again using '(.*)' and see '(.*)'")]
        public void ThenICanSearchAgainUsingAndSee(string searchTerm, string totalResult)
        {
            var searchPage = GetNavigatedPage<SearchPage>().Search<SearchPage>(new JobProfileSearchResultViewModel
            {
                SearchTerm = searchTerm,
                SearchResults = null
            })
            .SaveTo(ScenarioContext);
            searchPage.DisplayedTotalNumberOfResults.Should().EndWith(totalResult);
        }

        [Then(@"the search box should be visible")]
        public void ThenTheSearchBoxShouldBeVisible()
        {
            GetNavigatedPage<SearchPage>().HasSearchBox.Should().BeTrue();
        }

        [Then(@"I am redirected to the survey page")]
        public void ThenIAmRedirectedToTheSurveyPage()
        {
            GetNavigatedPage<VocSurveyPage>().IsSurveyDisplayed();
        }

        [Then(@"the total number of results found that match the search term display '(.*)'")]
        public void ThenTheTotalNumberOfResultsFoundThatMatchTheSearchTermDisplay(string totalResult)
        {
            GetNavigatedPage<SearchPage>().DisplayedTotalNumberOfResults.Should().EndWith(totalResult);
        }

        [Then(@"the each profile has an associated RankId value which reflects the position in the list")]
        public void ThenTheEachProfileHasAnAssociatedRankIdValueWhichReflectsThePositionInTheList()
        {
            //TODO: Findout why its 2
            GetNavigatedPage<SearchPage>().DistinctRankIds.Should().BeGreaterThan(2);
        }

        [Then(@"Job Title is '(.*)' , Alt Title is '(.*)', Rank Id is '(.*)'")]
        public void ThenJobTitleIsAltTitleIsDescriptionIsAverageTitleIsRankIdIs(bool titleVisible, bool alternativeTitleVisible, bool rankIdVisible)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.HasJobProfileTitle.Should().Be(titleVisible);
            searchPage.HasAlternativeTitle.Should().Be(alternativeTitleVisible);
            searchPage.HasRankId.Should().Be(rankIdVisible);
        }

        [Then(@"the survey banner should be displayed")]
        public void ThenTheSurveyBannerShouldBeDisplayed()
        {
            var homePage = GetNavigatedPage<HomePage>();
            homePage.IsSurveyBannerDisplayed.Should().BeTrue();
        }

        [Then(@"the survey banner should not be displayed")]
        public void ThenTheSurveyBannerShouldNotBeDisplayed()
        {
            var homePage = GetNavigatedPage<HomePage>();
            homePage.IsSurveyBannerDisplayed.Should().BeFalse();
        }

        [Then(@"retain the search term '(.*)' in the search box")]
        public void ThenRetainTheSearchTermInTheSearchBox(string searchTerm)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.SearchBoxValue.Should().BeEquivalentTo(searchTerm);
        }

        [Then(@"I get directed to the job profile page with url '(.*)' and page heading '(.*)'")]
        public void ThenIGetDirectedToTheJobProfilePageWithUrl(string urlname, string pageHeading)
        {
            var jobProfilePage = GetNavigatedPage<JobProfilePage>();
            jobProfilePage.ContainsUrlName(urlname).Should().BeTrue($" the urlname should be {urlname}");
            jobProfilePage.PageHeading.Should().StartWith(pageHeading, $" the page heading should be {pageHeading}");
        }

        [Then(@"the last result is '(.*)' on the page")]
        public void ThenTheLastResultIsOnThePage(string showFlag)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            if (showFlag.IsShown())
            {
                searchPage.HighestRankOnPage.ShouldBeEquivalentTo(searchPage.TotalResultsCount);
            }
            else
            {
                searchPage.HighestRankOnPage.Should().BeLessThan(searchPage.TotalResultsCount);
            }
        }

        [Then(@"the Next pagination control is '(.*)'")]
        public void ThenTheNextPaginationControlIs(string showFlag)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.HasNextPage.ShouldBeEquivalentTo(showFlag.IsShown());
        }

        [Then(@"the Next Pagination control label is '(.*)'")]
        public void ThenTheNextPaginationControlLabelIs(string labelText)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.NextPageLabel.Should().Be(labelText);
        }

        [Then(@"the Next Pagination control page reference is '(.*)'")]
        public void ThenTheNextPaginationControlPageReferenceIs(string pageReference)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.NextPageNumbersLabel.Should().Be($"{pageReference} of {searchPage.TotalPageCount}");
        }

        [Then(@"the first result is '(.*)' on the page")]
        public void ThenTheFirstResultIsOnThePage(string showFlag)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            if (showFlag.IsShown())
            {
                searchPage.LowestRankOnPage.ShouldBeEquivalentTo(1);
            }
            else
            {
                searchPage.LowestRankOnPage.Should().BeGreaterThan(1);
            }
        }

        [Then(@"the Next Pagination control page reference is '(.*)' of Total Page Number")]
        public void ThenTheNextPaginationControlPageReferenceIsOfTotalPageNumber(string pageReference)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.NextPageNumbersLabel.Should().Be($"{pageReference} of {searchPage.TotalPageCount}");
        }

        [Then(@"the Previous Pagination control page reference is '(.*)' of total number of pages")]
        public void ThenThePreviousPaginationControlPageReferenceIsOfTotalNumberOfPages(string pageReference)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.PreviousPageNumbersLabel.Should().Be($"{pageReference} of {searchPage.TotalPageCount}");
        }

        [Then(@"the Previous pagination control is '(.*)'")]
        public void ThenThePreviousPaginationControlIs(string showFlag)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.HasPreviousPage.ShouldBeEquivalentTo(showFlag.IsShown());
        }

        [Then(@"the Previous Pagination control label is '(.*)'")]
        public void ThenThePreviousPaginationControlLabelIs(string labelText)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.PreviousPageLabel.Should().Be(labelText);
        }

        [Then(@"the Previous Pagination control page reference is '(.*)'")]
        public void ThenThePreviousPaginationControlPageReferenceIs(string pageReference)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.PreviousPageNumbersLabel.Should().Be(pageReference);
        }

        [Then(@"clicking on the Next pagination control directs the user to the next page of results")]
        public void ThenClickingOnTheNextPaginationControlDirectsTheUserToTheNextPageOfResults()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            var firstJobProfileUrl = searchPage.FirstJobProfileUrl;
            searchPage = searchPage.NextPage<SearchPage>();
            searchPage.FirstJobProfileUrl.Should().NotBe(firstJobProfileUrl);
        }

        [Then(@"clicking on the Previous pagination control directs the user to the previous page of results")]
        public void ThenClickingOnThePreviousPaginationControlDirectsTheUserToThePreviousPageOfResults()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            var firstJobProfileUrl = searchPage.FirstJobProfileUrl;
            searchPage = searchPage.PreviousPage<SearchPage>();
            searchPage.FirstJobProfileUrl.Should().NotBeSameAs(firstJobProfileUrl);
        }

        [Then(@"the result list will contain '(.*)' profile\(s\)")]
        public void ThenTheResultListWillContainProfileS(int numberDisplayedProfilesExpected)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.NumberOfJobProfilesDisplayed.Should().Be(numberDisplayedProfilesExpected);
        }

        [Then(@"theses profiles titles are listed :")]
        public void ThenTheProfilesAreListedInTheFollowingOrder(Table table)
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            var expected = table.CreateSet<string>(d => d.GetString("Title"));
            var actual = searchPage.DisplayedJobProfileTitles;

            //Log results
            OutputHelper.WriteLine($"Expected order expected {JsonConvert.SerializeObject(expected)}");
            OutputHelper.WriteLine($"  Actual order expected {JsonConvert.SerializeObject(actual)}");
            actual.ShouldBeEquivalentTo(expected);
        }

        [Then(@"the search title should be visible")]
        public void ThenTheSearchTitleShouldBeVisible()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.SearchTitle.Should().NotBeEmpty();
        }

        [Then(@"the result count should be hidden")]
        public void ThenTheResultCountShouldBeHidden()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.DisplayedTotalNumberOfResults.Should().BeEmpty();
        }

        [Then(@"I get directed to the correct job profile page")]
        public void ThenIGetDirectedToTheCorrectJobProfilePage()
        {
            var jobProfilePage = GetNavigatedPage<JobProfilePage>();
            string profileChosen;
            ScenarioContext.TryGetValue("profileSelected", out profileChosen);
            jobProfilePage.ProfilePageHeading.Should().Be(profileChosen);
        }

        [Then(@"a cookie should appear saying dismissed")]
        public void ThenTheCookieShouldAppearSayingDismissed()
        {
            var value = GetCookieValue("survey");
            value.Should().Be("dismissed");
        }

        [Then(@"the no results message is displayed")]
        public void ThenTheNoResultsMessageIsDisplayed()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            searchPage.DisplayedTotalNumberOfResults.Should().Contain("0 results found - try again using a different job title");
        }

        [Then(@"the correct '(.*)' breadcrumb is displayed")]
        public void ThenTheCorrecrSearchBreadcrumbIsDisplayed(string text)
        {
            switch (text)
            {
                case "Search results":
                    var searchPage = GetNavigatedPage<SearchPage>();
                    searchPage.BreadcrumbText.Contains(text);
                    break;

                case "Job profile":
                    var jobProfile = GetNavigatedPage<JobProfilePage>();
                    jobProfile.BreadcrumbText.Contains(jobProfile.ProfilePageHeading);
                    break;

                case "Job category":
                    var jobCategory = GetNavigatedPage<JobProfileCategoryPage>();
                    jobCategory.BreadcrumbText.Contains(jobCategory.CategoryTitle);
                    break;
            }
        }

        [Then(@"the suggested results should appear")]
        public void ThenTheSuggestedResultsShouldAppear()
        {
            var homePage = GetNavigatedPage<HomePage>();
            homePage.HasSuggestedSearch.Should().BeTrue();
        }

        [Then(@"the search box should populate with the selected result")]
        public void ThenTheSearchBoxShouldPopulateWithTheResult()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            string selectedSearch;
            ScenarioContext.TryGetValue("selectedSearch", out selectedSearch);
            searchPage.SearchBoxValue.Should().Contain(selectedSearch);
        }

        [Then(@"the suggested box should be disappear")]
        public void ThenTheSuggestedBoxShouldBeDisappear()
        {
            var homePage = GetNavigatedPage<HomePage>();
            homePage.HasSuggestedSearch.Should().BeFalse();
        }

        #endregion Then

        [Then(@"search should display the Did You Mean text")]
        public void ThenSearchShouldDisplayTheDidYouMeanText()
        {
            GetNavigatedPage<SearchPage>().HasDidYouMeanText.Should().NotBeNullOrEmpty();
        }

        [When(@"I click the suggested text")]
        public void WhenIClickTheSuggestedText()
        {
            ScenarioContext.Set(GetNavigatedPage<SearchPage>().DidYouMeanText, "didyoumeansearchTerm");
            GetNavigatedPage<SearchPage>().DidYouMeanPage<SearchPage>().SaveTo(ScenarioContext);
        }

        [Then(@"I am redirected to the correct search results page")]
        public void ThenIAmRedirectedToTheCorrectSearchResultsPage()
        {
            var searchPage = GetNavigatedPage<SearchPage>();
            ScenarioContext.TryGetValue("didyoumeansearchTerm", out string searchTerm);
            searchPage.SearchBoxValue.Should().BeEquivalentTo(searchTerm);
        }

        [Then(@"search should not display the Did You Mean text")]
        public void ThenSearchShouldNotDisplayTheDidYouMeanText()
        {
            GetNavigatedPage<SearchPage>().HasDidYouMeanText.Should().BeNullOrEmpty();
        }

        [Then(@"the results display the Found In category text")]
        public void ThenTheResultsDisplayTheFoundInCategoryText()
        {
            GetNavigatedPage<SearchPage>()
                .HasFoundInField.Should().BeTrue();
        }

        [Then(@"I am redirected to the correct job category page")]
        public void ThenIAmRedirectedToTheCorrectJobCategoryPage()
        {
            var categoryPage = GetNavigatedPage<JobProfileCategoryPage>();
            string category;
            ScenarioContext.TryGetValue("SelectedCategoryLink", out category);
            categoryPage.CategoryTitle.Should().Contain(category);
        }
    }
}