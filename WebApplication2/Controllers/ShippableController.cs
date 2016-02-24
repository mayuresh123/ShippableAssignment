using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using ShippableAssignment.Models;

namespace ShippableAssignment.Controllers
{
    /// <summary>
    /// ShippableController
    /// </summary>
    public class ShippableController : ApiController
    {
        /// <summary>
        /// Get github repository details.
        /// </summary>
        /// <param name="url">Github repository url</param>
        /// <returns>Returns list of githubinfo</returns>
        [HttpGet]
        public IEnumerable<IEnumerable<GitHubInfo>> GetDetails(string url)
        {
            WebClient webClient = new WebClient();
            //download web page
            string html = webClient.DownloadString(url);

            //remove escape sequences
            html = Regex.Unescape(html);

            //regex pattern to search for issue details
            string pattern = "<div[^<>]*class=\"table-list-cell issue-title\"[^<>]*>(?<content>.*?)</div>";

            html = html.Replace("\n", "");

            //remove unwanted code from the issue details
            html = Regex.Replace(html, "<svg.*?>(.*?)</svg>", string.Empty);

            //match collection containing issue details
            MatchCollection matches = Regex.Matches(html, pattern);

            //Github info model to hold issue details
            List<GitHubInfo> gitHubInfoList = new List<GitHubInfo>();
            foreach (Match item in matches)
            {
                string htmlText = item.Groups[0].Value;

                //Regex to get issue name
                Match anchorMatch = Regex.Match(htmlText, "<a.*?>(.*?)</a>");

                //Regex to get issue date time
                Match timeMatch = Regex.Match(htmlText, "<time.*?>(.*?)</time>");

                if (anchorMatch.Value != null && timeMatch.Value != null)
                {
                    if (anchorMatch.Groups.Count > 0 && timeMatch.Groups.Count > 0)
                    {
                        GitHubInfo gitHubInfo = new GitHubInfo();
                        gitHubInfo.IssueName = anchorMatch.Groups[1].Value.Trim();
                        string dateTime = timeMatch.Groups[0].Value;
                        int indexfrom = dateTime.IndexOf("datetime=\"");
                        int indexTo = dateTime.IndexOf("\"", indexfrom + 1);
                        string sub = dateTime.Substring(indexfrom + 10, indexTo).Replace("T", " ");
                        DateTime dt = Convert.ToDateTime(sub);
                        gitHubInfo.IssueDate = dt;
                        gitHubInfoList.Add(gitHubInfo);
                    }
                }
            }

            List<GitHubInfo> _24Hrs = new List<GitHubInfo>();
            List<GitHubInfo> _24HrsTo7Days = new List<GitHubInfo>();
            List<GitHubInfo> moreThan7Days = new List<GitHubInfo>();
            DateTime now = DateTime.Now;

            //Number of open issues that were opened in the last 24 hours
            _24Hrs = gitHubInfoList.Where(x => x.IssueDate > now.AddHours(-24) && x.IssueDate <= now).ToList<GitHubInfo>();

            //Number of open issues that were opened more than 24 hours ago but less than 7 days ago
            _24HrsTo7Days = gitHubInfoList.Where(x => x.IssueDate > now.AddDays(-7) && x.IssueDate <= now).ToList<GitHubInfo>();

            //Number of open issues that were opened more than 7 days ago
            moreThan7Days = gitHubInfoList.Where(x => x.IssueDate <= now.AddDays(-7)).ToList<GitHubInfo>();
            List<List<GitHubInfo>> myList = new List<List<GitHubInfo>>();
            myList.Add(_24Hrs);
            myList.Add(_24HrsTo7Days);
            myList.Add(moreThan7Days);
            return myList;
        }
    }
}
