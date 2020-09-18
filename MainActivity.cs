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

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        App application;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();

            EbNFData nfData = null;
            if (Intent.Extras != null)
            {
                string nf_string = Intent.Extras.GetString("nf_data");

                if (nf_string != null)
                {
                    nfData = Parse(nf_string);
                }
            }

            application = new App(nfData);
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
                string nf_data = intent.GetStringExtra("nf_data");

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

        private EbNFData Parse(string nfdata)
        {
            return JsonConvert.DeserializeObject<EbNFData>(nfdata);
        }
    }
}