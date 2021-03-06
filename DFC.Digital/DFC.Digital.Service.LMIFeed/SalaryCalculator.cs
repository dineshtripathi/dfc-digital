﻿using DFC.Digital.Data.Interfaces;
using DFC.Digital.Data.Model;
using System.Linq;
using DFC.Digital.Core.Utilities;

namespace DFC.Digital.Service.LMIFeed
{
    public class SalaryCalculator : ISalaryCalculator
    {
        #region Implementation of ISalaryCalculator

        public decimal? GetStarterSalary(JobProfileSalary jobProfileSalary)
        {
            return jobProfileSalary?.Deciles.Min(s => s.Value) * Constants.MULTIPLIER;
        }

        public decimal? GetExperiencedSalary(JobProfileSalary jobProfileSalary)
        {
            return jobProfileSalary?.Deciles.Max(s => s.Value) * Constants.MULTIPLIER;
        }

        #endregion Implementation of ISalaryCalculator
    }
}