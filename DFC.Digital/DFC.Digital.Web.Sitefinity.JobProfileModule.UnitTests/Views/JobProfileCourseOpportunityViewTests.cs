﻿using ASP;
using DFC.Digital.Data.Model;
using DFC.Digital.Web.Sitefinity.JobProfileModule.Mvc.Models;
using FluentAssertions;
using HtmlAgilityPack;
using RazorGenerator.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.Digital.Web.Sitefinity.JobProfileModule.Tests.Views
{
    public class JobProfileCourseOpportunityViewTests
    {
        //As a Citizen, I want to the see the current available training courses
        //A1 - Display of Current trainings section for available courses
        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(0)]
        public void DFC_1508_TrainingCourseFieldsCorrectTest(int coursesCount)
        {
            //Arrange
            var index = new _MVC_Views_JobProfileCourseOpportunity_Index_cshtml();
            var jobProfileApprenticeViewModel = GenerateJobProfileApprenticeshipTrainingCourseViewModel(coursesCount);

            //Act
            var htmlDom = index.RenderAsHtml(jobProfileApprenticeViewModel);

            //Assert
            GetCoursesSectionTitleDetailsText(htmlDom).Should().Contain(jobProfileApprenticeViewModel.CoursesSectionTitle);
            GetCoursesSectionTitleDetailsText(htmlDom).Should().Contain(jobProfileApprenticeViewModel.CoursesLocationDetails);
            GetFindTrainingCoursesLink(htmlDom).Should().Be(jobProfileApprenticeViewModel.FindTrainingCoursesLink);
            GetFindTrainingCoursesText(htmlDom).Should().Be(jobProfileApprenticeViewModel.FindTrainingCoursesText);
            GetFindTrainingCourses(htmlDom).ShouldBeEquivalentTo(jobProfileApprenticeViewModel.Courses);
            if (coursesCount == 0)
            {
                GetNoTrainingCoursesText(htmlDom).Should().Be(jobProfileApprenticeViewModel.NoTrainingCoursesText);
            }
        }

        public JobProfileCourseSearchViewModel GenerateJobProfileApprenticeshipTrainingCourseViewModel(int courseCount)
        {
            return new JobProfileCourseSearchViewModel
            {
                MainSectionTitle = "7. Current Opportunities",
                CoursesSectionTitle = nameof(JobProfileCourseSearchViewModel.CoursesSectionTitle),
                FindTrainingCoursesText = nameof(JobProfileCourseSearchViewModel.FindTrainingCoursesText),
                NoTrainingCoursesText = nameof(JobProfileCourseSearchViewModel.NoTrainingCoursesText),
                FindTrainingCoursesLink = nameof(JobProfileCourseSearchViewModel.FindTrainingCoursesLink),
                CoursesLocationDetails = nameof(JobProfileCourseSearchViewModel.CoursesLocationDetails),
                Courses = GetDummyCourses(courseCount)
            };
        }

        private string GetNoTrainingCoursesText(HtmlDocument htmlDom)
        {
            return htmlDom.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.Attributes["class"].Value.Contains("dfc-code-jp-NoTrainingCoursesText"))?
                .InnerText.Trim();
        }

        private IEnumerable<Course> GetFindTrainingCourses(HtmlDocument htmlDom)
        {
            List<Course> displayedCourses = new List<Course>();
            foreach (HtmlNode opportunity in htmlDom.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.Attributes["class"].Value.Contains("dfc-code-jp-trainingCourse"))?.Descendants("div").Where(div => div.Attributes["class"].Value.Contains("opportunity-item"))?.ToList())
            {
                var p = new Course
                {
                    Title = opportunity.Descendants("h3").FirstOrDefault()?.Descendants("a").FirstOrDefault()?.InnerText,
                    Location = opportunity.Descendants("li").ElementAt(2)?.InnerText.Substring(opportunity.Descendants("li").ElementAt(2).InnerText.IndexOf(":", StringComparison.Ordinal) + 1).Trim(),
                    CourseId = opportunity.Descendants("h3").FirstOrDefault()?.Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty),
                    StartDate = Convert.ToDateTime(opportunity.Descendants("li").ElementAt(1)?.InnerText.Substring(opportunity.Descendants("li").ElementAt(1).InnerText.IndexOf(":", StringComparison.Ordinal) + 1).Trim()),
                    ProviderName = opportunity.Descendants("li").ElementAt(0)?.InnerText.Substring(opportunity.Descendants("li").ElementAt(0).InnerText.IndexOf(":", StringComparison.Ordinal) + 1).Trim(),
                };
                displayedCourses.Add(p);
            }

            return displayedCourses;
        }

        private string GetFindTrainingCoursesText(HtmlDocument htmlDom)
        {
            return htmlDom.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.Attributes["class"].Value.Contains("dfc-code-jp-FindTrainingCoursesLink"))?
                .Descendants("a").FirstOrDefault()?.InnerText;
        }

        private string GetFindTrainingCoursesLink(HtmlDocument htmlDom)
        {
            return htmlDom.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.Attributes["class"].Value.Contains("dfc-code-jp-FindTrainingCoursesLink"))?
                .Descendants("a").FirstOrDefault()?.GetAttributeValue("href", string.Empty);
        }

        private string GetCoursesSectionTitleDetailsText(HtmlDocument htmlDom)
        {
            return htmlDom.DocumentNode.Descendants("div")
                .FirstOrDefault(div => div.Attributes["class"].Value.Contains("dfc-code-jp-trainingCourse"))?
                .Descendants("h3").FirstOrDefault()?.InnerText;
        }

        private IEnumerable<Course> GetDummyCourses(int courseCount)
        {
            var courses = new List<Course>();
            for (int i = 0; i < courseCount; i++)
            {
                courses.Add(new Course
                {
                    Title = $"dummy {nameof(Course.Title)}",
                    Location = $"dummy {nameof(Course.Location)}",
                    CourseId = $"dummy {nameof(Course.CourseId)}",
                    StartDate = default(DateTime),
                    ProviderName = $"dummy {nameof(Course.ProviderName)}",
                });
            }

            return courses;
        }
    }
}