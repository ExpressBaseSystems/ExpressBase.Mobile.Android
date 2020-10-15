using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Plugin.Media;
using Android.Util;
using Android.Gms.Common;
using Android.Content;
using ExpressBase.Mobile.Models;
using Newtonsoft.Json;
using System;
using Android;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = "@style/MainTheme", 
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string EbNFDataKey = "nf_data";

        private App application;

        const int RequestLocationId = 0;

        readonly string[] LocationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.Camera
        };

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
            }
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            EbNFData ebnfData = this.GetNFDataOnCreate();

            application = new App(ebnfData);
            LoadApplication(application);

            if (Configuration.EbBuildConfig.NFEnabled)
            {
                IsPlayServicesAvailable();
                CreateNotificationChannel();
            }

            this.SetStatusBarColor(Android.Graphics.Color.ParseColor(Configuration.EbBuildConfig.StatusBarColor));
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent?.Extras != null)
            {
                string nf_data = intent.GetStringExtra(EbNFDataKey);

                if (nf_data != null)
                {
                    await application?.NewIntentAction(Parse(nf_data));
                }
            }
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
                string channelId = NFConstants.ChannelId;
                Java.Lang.String channelNameJava = new Java.Lang.String(NFConstants.ChannelName);

                NotificationChannel channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = NFConstants.ChannelDescription
                };
                NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private EbNFData GetNFDataOnCreate()
        {
            EbNFData data = null;

            if (Intent.Extras != null)
            {
                string nf_string = Intent.Extras.GetString(EbNFDataKey);

                if (nf_string != null)
                    data = Parse(nf_string);
            }
            return data;
        }

        private EbNFData Parse(string nfdata)
        {
            EbNFData data = null;
            try
            {
                data = JsonConvert.DeserializeObject<EbNFData>(nfdata);
            }
            catch (Exception)
            {
                //NFData parsing error
            }
            return data;
        }
    }
}