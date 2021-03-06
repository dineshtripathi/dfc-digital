﻿using AutoMapper;
using DFC.Digital.Data.Model;
using DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Models;

namespace DFC.Digital.Web.Sitefinity.JobProfileModule.Config
{
    public class JobProfilesAutoMapperProfile : Profile
    {
        public JobProfilesAutoMapperProfile()
        {
            CreateMap<JobProfileIndex, JobProfile>()
                .ForMember(d => d.AlternativeTitle, o => o.MapFrom(s => string.Join(", ", s.AlternativeTitle).Trim().TrimEnd(',')));

            CreateMap<SearchResultItem<JobProfileIndex>, JobProfileSearchResultItemViewModel>()
                .ForMember(d => d.ResultItemAlternativeTitle, o => o.MapFrom(s => string.Join(", ", s.ResultItem.AlternativeTitle).Trim().TrimEnd(',')))
                .ForMember(c => c.JobProfileCategoriesWithUrl, m => m.MapFrom(j => j.ResultItem.JobProfileCategoriesWithUrl));

            CreateMap<JobProfile, JobProfileDetailsViewModel>()
                .ForMember(d => d.MinimumHours, o => o.MapFrom(s => (s.MinimumHours != null) ? s.MinimumHours.Value.ToString("#.#") : string.Empty))
                .ForMember(d => d.MaximumHours, o => o.MapFrom(s => (s.MaximumHours != null) ? s.MaximumHours.Value.ToString("#.#") : string.Empty));

            CreateMap<JobProfileSection, AnchorLink>()
                .ForMember(d => d.LinkText, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.LinkTarget, o => o.MapFrom(s => s.ContentField))
                ;

            CreateMap<PSFModel, PreSearchFiltersResultsModel>();
            CreateMap<PSFSection, FilterResultsSection>();
            CreateMap<PSFOption, FilterResultsOption>();

            CreateMap<PSFSection, PreSearchFilterSection>();
            CreateMap<PSFOption, PreSearchFilterOption>();
            CreateMap<PreSearchFilterSection, PSFSection>();
            CreateMap<PreSearchFilterOption, PSFOption>();
        }

        public override string ProfileName => "DFC.Digital.Web.Sitefinity.JobProfileModule";
    }
}