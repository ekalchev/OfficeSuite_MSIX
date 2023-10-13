using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDotNet
{
    public class ViewModel
    {
        public string InfoText
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                var applicationVersion = string.Format("{0}.{1}.{2}.{3}",
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision);

                return "Application Version " + applicationVersion;
            }
        }

        public void StartAutoUpdaterProcess()
        {
            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            int index = location.IndexOf("\\Mail");
            var processPath = $"{location.Substring(0, index)}\\AutoUpdater\\AutoUpdater.exe";
            Process.Start(processPath);
        }
    }
}
