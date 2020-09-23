using Android.App;
using Android.Content;
using Android.Gms.Auth.Api.Phone;
using Android.Gms.Common.Apis;
using Xamarin.Forms;

namespace ExpressBase.Mobile.Droid.Helpers
{
    [BroadcastReceiver]
    [IntentFilter(new[] { SmsRetriever.SmsRetrievedAction })]
    public class SMSBroadCastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != SmsRetriever.SmsRetrievedAction)
                return;

            var extra = intent.Extras;
            if (extra == null) return;
            var status = (Statuses)extra.Get(SmsRetriever.ExtraStatus);

            switch (status.StatusCode)
            {
                case CommonStatusCodes.Success:
                    var content = (string)extra.Get(SmsRetriever.ExtraSmsMessage);
                    MessagingCenter.Send<string>(content, "ReceivedOTP");
                    break;
                case CommonStatusCodes.Timeout:
                    break;
            }
        }
    }
}