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
using ExpressBase.Mobile.Services.Navigation;
using Xamarin.Forms.Platform.Android;
using Android.Views;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = "@style/MainTheme",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : FormsAppCompatActivity
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

        public static FormsAppCompatActivity Instance { get; private set; }

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
            //Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            //Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            EbNFData ebnfData = this.GetNFDataOnCreate();

            application = new App(ebnfData);
            LoadApplication(application);

            if (Configuration.EbBuildConfig.NFEnabled)
            {
                IsPlayServicesAvailable();
                CreateNotificationChannel();
            }

            Instance = this;
            this.SetStatusBarColor(Android.Graphics.Color.ParseColor(Configuration.EbBuildConfig.StatusBarColor));
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            bool opt = Convert.ToInt32(Build.VERSION.SdkInt) >= 35;
            if (opt)
            {
                Window.DecorView.SetOnApplyWindowInsetsListener(new InsetsListener());
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }
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
                    await NavigationService.Current.InitRecievedIntentAction(Parse(nf_data));
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
            try
            {
                return JsonConvert.DeserializeObject<EbNFData>(nfdata);
            }
            catch (Exception) { }
            return null;
        }
    }

    public class InsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        public WindowInsets OnApplyWindowInsets(View v, WindowInsets insets)
        {
            var insetsCompat = insets?.GetInsets(WindowInsets.Type.SystemBars());
            if (insetsCompat != null)
            {
                int bottomInset = insetsCompat.Bottom;
                int topInset = insetsCompat.Top;

                // Prevent double-padding
                if (v.PaddingBottom != bottomInset || v.PaddingTop != topInset)
                {
                    v.SetPadding(0, topInset, 0, bottomInset);
                }
            }

            return insets;
        }
    }
}