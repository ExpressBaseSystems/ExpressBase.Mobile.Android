using Android.App;
using Android.OS;
using Android.Views;
using ExpressBase.Mobile.Configuration;

namespace ExpressBase.Mobile.Droid
{
    [Activity(Theme = Config.SplashTheme, MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)((int)Window.DecorView.SystemUiVisibility ^ (int)SystemUiFlags.LayoutStable ^ (int)SystemUiFlags.LayoutFullscreen);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            base.OnCreate(bundle);
            this.StartActivity(typeof(MainActivity));
        }
    }
}