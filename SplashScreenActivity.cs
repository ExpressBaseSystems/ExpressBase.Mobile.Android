using Android.App;
using Android.OS;
using ExpressBase.Mobile.Configuration;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = Config.SplashTheme, MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            System.Threading.Thread.Sleep(100);
            this.StartActivity(typeof(MainActivity));
        }
    }
}