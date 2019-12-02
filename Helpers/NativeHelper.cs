using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Models;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]
[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace ExpressBase.Mobile.Droid.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public NativeHelper() { }

        private string _devoiceid;

        public string DeviceId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_devoiceid))
                    return _devoiceid;

                _devoiceid = Android.OS.Build.Serial;
                if (string.IsNullOrWhiteSpace(_devoiceid) || _devoiceid == Build.Unknown || _devoiceid == "0")
                {
                    try
                    {
                        var context = Android.App.Application.Context;
                        _devoiceid = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                    }
                }

                return _devoiceid;
            }
        }

        public string AppVersion
        {
            get
            {
                var context = Android.App.Application.Context;
                var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                return $"{info.VersionName}.{info.VersionCode.ToString()}";
            }
        }

        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }

    public class ToastMessage : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}