using Android.App;
using Android.Content;

namespace ExpressBase.Mobile.Droid.Helpers
{
    [BroadcastReceiver]
    [IntentFilter(new[] { "com.android.vending.INSTALL_REFERRER" })]
    public class InstallReferrerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            string sid = intent.GetStringExtra("referrer");
        }
    }
}