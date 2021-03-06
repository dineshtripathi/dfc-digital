﻿using DFC.Digital.Data.Interfaces;

namespace DFC.Digital.Data.Model
{
    public class DfcPageSiteNode : IDigitalDataModel
    {
        /// <summary>
        /// Gets or sets the Url.
        /// </summary>
        /// <value>
        /// A string that represents the Url of the DFC Page Node.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <value>
        /// A string that represents the Title of the DFC Page Node.
        /// </value>
        public string Title { get; set; }
    }
}