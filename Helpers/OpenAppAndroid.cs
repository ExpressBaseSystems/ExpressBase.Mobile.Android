using Android.Content;
using Android.Content.PM;
using Android.Webkit;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenAppAndroid))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class OpenAppAndroid : IAppHandler
    {
        public Task<string> PrintPdfFile(string filePath)
        {
            string result = null, packageName = null;
            string package1 = "com.hp.android.print";//eprint
            string package2 = "com.hp.android.printservice";//print service plugin            
            string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.Parse(filePath).ToString());
            string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);

            try
            {
                if (extension?.ToLower() == "pdf")
                {
                    if (IsAppInstalled(package1))
                        packageName = package1;
                    else if (IsAppInstalled(package2))
                        packageName = package2;
                }

                Java.IO.File file = new Java.IO.File(filePath);
                file.SetReadable(true);
                Android.Net.Uri uri = Android.Support.V4.Content.FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                Intent intent = new Intent(Intent.ActionSend);
                if (packageName != null)
                    intent.SetPackage(packageName);
                intent.SetDataAndType(uri, mimeType);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.AddFlags(ActivityFlags.NoHistory);
                intent.AddFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Task.FromResult(result);
        }

        private bool IsAppInstalled(string packageName)
        {
            PackageManager pm = Android.App.Application.Context.PackageManager;
            bool installed;
            try
            {
                pm.GetPackageInfo(packageName, PackageInfoFlags.Activities);
                installed = true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                installed = false;
            }
            return installed;
        }
    }
}