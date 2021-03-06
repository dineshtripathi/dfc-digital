﻿using DFC.Digital.Core.Utilities;
using DFC.Digital.Data.Interfaces;
using DFC.Digital.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;

namespace DFC.Digital.Web.Sitefinity.Core
{
    public class WebAppContext : IWebAppContext
    {
        public bool IsContentAuthoringSite => SitefinityContext.IsBackend;

        public bool IsPreviewMode => SitefinityContext.IsPreviewMode;

        public bool IsContentPreviewMode
        {
            get
            {
                //The SitefinityContext.IsPreviewMode only seems to get set to true, when the preview button for a page template is used.
                //When you come via the content type preview button this is aways false.
                if (HttpContext.Current.Request.QueryString != null && HttpContext.Current.Request.QueryString["sf-content-action"] == "preview")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsContentAuthoringAndNotPreviewMode => IsContentAuthoringSite && !IsPreviewMode;

        public bool IsSearchResultsPage => HttpContext.Current.Request.Url.ToString().ToLower().Contains("/search-results");

        public bool IsCategoryPage => HttpContext.Current.Request.Url.ToString().ToLower().Contains("/job-categories");

        public bool IsJobProfilePage => HttpContext.Current.Request.Url.ToString().ToLower().Contains("/job-profiles");

        public NameValueCollection RequestQueryString => HttpContext.Current.Request.QueryString;

        public bool IsValidAndFormattedUrl(string urlTobeValidated) => Uri.IsWellFormedUriString($"{urlTobeValidated}", UriKind.RelativeOrAbsolute);

        public VocSurveyPersonalisation GetVocCookie(string cookieName)
        {
            var profile = new VocSurveyPersonalisation();
            var cookies = HttpContext.Current?.Request.Cookies;
            if (cookies != null)
            {
                var cookie = cookies.Get(cookieName)?.Value;
                if (!string.IsNullOrEmpty(cookie))
                {
                    profile = JsonConvert.DeserializeObject<VocSurveyPersonalisation>(cookie);
                }
            }

            if (!profile.Personalisation.ContainsKey(Constants.LastVisitedJobProfileKey))
            {
                profile.Personalisation.Add(Constants.LastVisitedJobProfileKey, Constants.Unknown);
            }

            profile.Personalisation.Add(Constants.GoogleClientIdKey, GetGAClientId());
            return profile;
        }

        public bool SetVocCookie(string cookieName, VocSurveyPersonalisation userPersonalisation)
        {
            var cookies = HttpContext.Current?.Response.Cookies;
            if (cookies != null)
            {
                cookies.Set(new HttpCookie(cookieName, JsonConvert.SerializeObject(userPersonalisation)));
                return true;
            }

            return false;
        }

        public string GetGAClientId()
        {
            var cookie = HttpContext.Current?.Request.Cookies.Get(Constants.GoogleAnalyticsCookie)?.Value;
            if (!string.IsNullOrEmpty(cookie))
            {
                // The GA id has got 3 components seperated by "." we are Interested in the value following second "."
                var clientId = $"{cookie.Substring(cookie.IndexOf('.', cookie.IndexOf('.') + 1) + 1)}";
                return clientId;
            }

            return "Unknown";
        }
    }
}