﻿using AutoMapper;
using DFC.Digital.Data.Interfaces;
using DFC.Digital.Data.Model;
using DFC.Digital.Web.Core.Base;
using DFC.Digital.Web.Sitefinity.Core.Utility;
using DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "PreSearchFilters", Title = "Configure Pre Search Filters", SectionName = SitefinityConstants.CustomWidgetSection)]
    public class PreSearchFiltersController : BaseDfcController
    {
        #region Private Fields

        private readonly IPreSearchFiltersFactory preSearchFiltersFactory;
        private readonly IPreSearchFilterStateManager preSearchFilterStateManager;
        private readonly IWebAppContext webAppContext;
        private readonly IMapper autoMapper;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreSearchFiltersController"/> class.
        /// </summary>
        /// <param name="applicationLogger">application logger</param>
        /// <param name="preSearchFiltersFactory">Sitefinity Repository to use</param>
        /// <param name="webAppContext">Sitefinity context</param>
        /// <param name="preSearchFilterStateManager">Pre search filter state manager</param>
        /// <param name="autoMapper">Instance of auto mapper</param>
        public PreSearchFiltersController(
            IWebAppContext webAppContext,
            IApplicationLogger applicationLogger,
            IMapper autoMapper,
            IPreSearchFiltersFactory preSearchFiltersFactory,
            IPreSearchFilterStateManager preSearchFilterStateManager) : base(applicationLogger)
        {
            this.preSearchFiltersFactory = preSearchFiltersFactory;
            this.webAppContext = webAppContext;
            this.autoMapper = autoMapper;
            this.preSearchFilterStateManager = preSearchFilterStateManager;
        }

        #endregion Constructors

        #region Public Properties

        [DisplayName("Section Title - must be unique across pages")]
        public string SectionTitle { get; set; } = "Demo Section";

        [DisplayName("Section Description")]
        public string SectionDescription { get; set; } = "Demo Description";

        [DisplayName("Section Content Type - One of  Interest, Enabler, EntryQualification, TrainingRoute, JobArea, CareerFocus, PreferredTaskType")]
        public PreSearchFilterType FilterType { get; set; }

        [DisplayName("Is this section a single option select only")]
        public bool SingleSelectOnly { get; set; } = false;

        [DisplayName("Previous page URL - where to post the form when back button is clicked")]
        public string PreviousPageURL { get; set; } = "homepageurl";

        [DisplayName("Next page URL - where to post the form when the continue button is clicked")]
        public string NextPageURL { get; set; } = "nextpageurl";

        [DisplayName("The number of this page in the selection sequence")]
        public int ThisPageNumber { get; set; } = 1;

        [DisplayName("The total number of selection pages")]
        public int TotalNumberOfPages { get; set; } = 99;

        #endregion Public Properties

        #region Actions

        // GET: PreSearchFilters
        public ActionResult Index()
        {
            return View(GetCurrentPageFilter());
        }

        [HttpPost]
        public ActionResult Index(PSFModel model, PsfSearchResultsViewModel resultsViewModel)
        {
            // If the previous page is search page then, there will not be any sections in the passed PSFModel
            var previousPsfPage = model?.Section == null ? resultsViewModel?.PreSearchFiltersModel : model;
            if (previousPsfPage != null)
            {
                preSearchFilterStateManager.RestoreState(previousPsfPage.OptionsSelected);
                if (preSearchFilterStateManager.ShouldSaveState(ThisPageNumber, previousPsfPage.Section.PageNumber))
                {
                    var previousFilterSection = autoMapper.Map<PreSearchFilterSection>(previousPsfPage.Section);
                    preSearchFilterStateManager.SaveState(previousFilterSection);
                }
            }

            var currentPageFilter = GetCurrentPageFilter(model);
            return View(currentPageFilter);
        }

        private PSFModel GetCurrentPageFilter(PSFModel previousPageFilter = null)
        {
            var savedSection = preSearchFilterStateManager.GetSavedSection(SectionTitle, FilterType);
            var restoredSection = preSearchFilterStateManager.RestoreOptions(savedSection, GetFilterOptions());

            //create the section for this page
            var currentSection = autoMapper.Map<PSFSection>(restoredSection);
            var filterSection = currentSection ?? new PSFSection();

            filterSection.Name = SectionTitle;
            filterSection.Description = SectionDescription;
            filterSection.SingleSelectOnly = SingleSelectOnly;
            filterSection.NextPageURL = NextPageURL;
            filterSection.PreviousPageURL = PreviousPageURL;
            filterSection.PageNumber = ThisPageNumber;
            filterSection.TotalNumberOfPages = TotalNumberOfPages;
            filterSection.SectionDataType = FilterType.ToString();

            var thisPageModel = new PSFModel
            {
                //Throw the state out again
                OptionsSelected = preSearchFilterStateManager.GetStateJson(),
                Section = filterSection,
            };

            //Need to do this to force the model we have changed to refresh
            ModelState.Clear();

            return thisPageModel;
        }

        #endregion Actions

        #region Private methods

        private IEnumerable<PreSearchFilter> GetFilterOptions()
        {
            switch (FilterType)
            {
                case PreSearchFilterType.Enabler:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFEnabler>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.EntryQualification:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFEntryQualification>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.Interest:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFInterest>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.TrainingRoute:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFTrainingRoute>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.JobArea:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFJobArea>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.CareerFocus:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFCareerFocus>().GetAllFilters().OrderBy(o => o.Order);
                    }

                case PreSearchFilterType.PreferredTaskType:
                    {
                        return preSearchFiltersFactory.GetRepository<PSFPreferredTaskType>().GetAllFilters().OrderBy(o => o.Order);
                    }

                default:
                    {
                        return Enumerable.Empty<PreSearchFilter>();
                    }
            }
        }

        #endregion
    }
}