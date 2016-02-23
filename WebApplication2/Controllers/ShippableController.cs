using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ShippableController : ApiController
    {
        [HttpGet]
        public IEnumerable<IEnumerable<GitHubInfo>> GetDetails(string url)
        {
            WebClient wc = new WebClient();
            string html = wc.DownloadString(url);
            html = Regex.Unescape(html);

            string pattern = "<div[^<>]*class=\"table-list-cell issue-title\"[^<>]*>(?<content>.*?)</div>";

            html = html.Replace("\n", "");

            html = Regex.Replace(html, "<svg.*?>(.*?)</svg>", string.Empty);

            MatchCollection matches = Regex.Matches(html, pattern);
            List<GitHubInfo> gitHubInfoList = new List<GitHubInfo>();
            foreach (Match item in matches)
            {
                string htmlText = item.Groups[0].Value;
                Match anchorMatch = Regex.Match(htmlText, "<a.*?>(.*?)</a>");
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
            _24Hrs = gitHubInfoList.Where(x => x.IssueDate > now.AddHours(-24) && x.IssueDate <= now).ToList<GitHubInfo>();
            _24HrsTo7Days = gitHubInfoList.Where(x => x.IssueDate > now.AddDays(-7) && x.IssueDate <= now).ToList<GitHubInfo>();
            moreThan7Days = gitHubInfoList.Where(x => x.IssueDate <= now.AddDays(-7)).ToList<GitHubInfo>();
            List<List<GitHubInfo>> myList = new List<List<GitHubInfo>>();
            myList.Add(_24Hrs);
            myList.Add(_24HrsTo7Days);
            myList.Add(moreThan7Days);
            return myList;
        }
    }
}
