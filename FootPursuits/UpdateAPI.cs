using System;
using System.Net;
using Rage;

namespace FootPursuits
{
    public class UpdateAPI
    {
        protected const string UpdateEndpoint = "http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={0}&beta={1}&textOnly=true";
        public static string GetLatestVersion(int fileId, bool betaBranch = false)
        {
            string latestVersion = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    latestVersion = client.DownloadString(string.Format(UpdateEndpoint, fileId, betaBranch.ToString().ToLower()));
                }
            }
            catch (Exception e)
            {
                Game.LogExtremelyVerbose(e.ToString());
            }

            return latestVersion;
        }
    }
}
