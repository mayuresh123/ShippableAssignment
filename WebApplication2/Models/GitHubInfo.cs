using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShippableAssignment.Models
{
    /// <summary>
    /// GtiHubInfo model containing git repository details
    /// </summary>
    public class GitHubInfo
    {
        /// <summary>
        /// issue description
        /// </summary>
        public string IssueName { get; set; }

        /// <summary>
        /// issue date
        /// </summary>
        public DateTime IssueDate { get; set; }
    }
}