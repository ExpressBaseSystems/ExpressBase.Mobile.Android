using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using ExpressBase.Mobile.Constants;
using System.IO;
using ExpressBase.Mobile.Helpers;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Plugin.Media;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Label = "ExpressBase.Mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string[] Permissions =
        {
            Android.Manifest.Permission.Internet,
            Android.Manifest.Permission.ReadExternalStorage,
            Android.Manifest.Permission.WriteExternalStorage,
            Android .Manifest.Permission.Camera
        };

        const int RequestId = 0;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();

            RequestPermissions(Permissions, RequestId);//permissions

            string sid = await Store.GetValueAsync(AppConst.SID);

            App _app;

            if (string.IsNullOrEmpty(sid))
            {
                _app = new App();
            }
            else
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), string.Format("{0}.db3", sid));

                if (!File.Exists(dbPath))
                {
                    Mono.Data.Sqlite.SqliteConnection.CreateFile(dbPath);
                }

                _app = new App(dbPath);
            }

            LoadApplication(_app);

            // Change the status bar color
            this.SetStatusBarColor(Android.Graphics.Color.ParseColor("#0046bb"));

            // Enable scrolling to the page when the keyboard is enabled
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}