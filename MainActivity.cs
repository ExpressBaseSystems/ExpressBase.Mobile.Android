﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Plugin.Media;
using Android.Util;
using Android.Gms.Common;
using Android.Content;
using Android.Views;
using System.Linq;
using Xamarin.Forms;
using ExpressBase.Mobile.Views.Base;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();

            LoadApplication(new App());

            if (Configuration.EbBuildConfig.NFEnabled)
            {
                IsPlayServicesAvailable();
                CreateNotificationChannel();
            }

            this.SetStatusBarColor(Android.Graphics.Color.ParseColor(Configuration.EbBuildConfig.StatusBarColor));

            // Enable scrolling to the page when the keyboard is enabled
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(NFConstants.DeBugTag, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(NFConstants.DeBugTag, "This device is not supported");
                    Finish();
                }
                return false;
            }
            return true;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelName = NFConstants.Channel;
                var channelDescription = string.Empty;
                var channel = new NotificationChannel(channelName, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}