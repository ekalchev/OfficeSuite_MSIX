using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Xaml;

namespace AutoUpdater
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var packageManager = new PackageManager();
                var appInstallerUri = "\\\\fileserver\\Public\\ekalchev\\OfficeSuite_MSIX_Test\\OfficeSuiteInstaller.appinstaller";
                Console.WriteLine("This AutoUpdater process!");
                Console.WriteLine($"AppInstaller URI: {appInstallerUri}");
                Console.WriteLine("Hit Enter to check for updates");
                Console.ReadLine();

                while (true)
                {
                    if (await CheckForUpdatesAsync(packageManager) == true) break;
                    Console.WriteLine("Wait 5 sec, before next check...");
                    await Task.Delay(5000);
                }

                Console.WriteLine("Hit Enter to start installing update.");
                Console.ReadLine();

                Console.WriteLine("Start Package Download");

                var uri = new Uri("\\\\fileserver\\Public\\ekalchev\\OfficeSuite_MSIX_Test\\OfficeSuiteInstaller_1.0.33.0_Debug_Test\\OfficeSuiteInstaller_1.0.31.0_AnyCPU_Debug.msixbundle");
                var deploymentTask = packageManager.StagePackageAsync(uri, null);

                //var deploymentTask = packageManager.RequestAddPackageByAppInstallerFileAsync(new Uri(appInstallerUri),
                //                AddPackageByAppInstallerOptions.ForceTargetAppShutdown, packageManager.GetDefaultPackageVolume());

                deploymentTask.Progress = (task, progress) =>
                {
                    Console.WriteLine($"Progress: {progress.percentage} - Status: {task.Status}");
                };

                var result = await deploymentTask;

                Console.WriteLine("Do you want to register?");
                Console.ReadLine();
                deploymentTask = packageManager.RegisterPackageAsync(uri, null, DeploymentOptions.ForceTargetApplicationShutdown);

                if (result.ExtendedErrorCode != null)
                {
                    Console.WriteLine($"Download Package Failed. Error Code: {result.ExtendedErrorCode}");
                }

                Console.WriteLine("Package Download Completed Successfully. Hit Enter to start the app.");
                Console.ReadLine();

                //string location = System.Reflection.Assembly.GetEntryAssembly().Location;
                //int index = location.IndexOf("\\AutoUpdater");
                //var processPath = $"{location.Substring(0, index)}\\Mail\\Mail.exe";
                //Process.Start(processPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        public static async Task<bool> CheckForUpdatesAsync(PackageManager packageManager)
        {
            bool updateIsAvailable = false;
            try
            {
                Console.WriteLine("Checking for updates...");
                Package package = packageManager.FindPackageForUser(string.Empty, Package.Current.Id.FullName);

                PackageUpdateAvailabilityResult result = await package.CheckUpdateAvailabilityAsync();
                switch (result.Availability)
                {
                    case PackageUpdateAvailability.Available:
                    case PackageUpdateAvailability.Required:
                        Console.WriteLine("Update is available");
                        updateIsAvailable = true;
                        break;
                    case PackageUpdateAvailability.NoUpdates:
                        Console.WriteLine("Update not available");
                        break;
                    case PackageUpdateAvailability.Unknown:
                    default:
                        Console.WriteLine("PackageUpdateAvailability.Unknown");
                        break;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }

            return updateIsAvailable;
        }
    }
}