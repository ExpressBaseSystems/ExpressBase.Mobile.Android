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

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]
[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace ExpressBase.Mobile.Droid.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public NativeHelper() { }

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