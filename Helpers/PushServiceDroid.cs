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
using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;
using Firebase.Iid;
using WindowsAzure.Messaging;

[assembly: Xamarin.Forms.Dependency(typeof(PushServiceDroid))]
namespace ExpressBase.Mobile.Droid.Helpers
{
    public class PushServiceDroid : IPushService
    {
        public void Register(string authid)
        {
            
        }
    }
}