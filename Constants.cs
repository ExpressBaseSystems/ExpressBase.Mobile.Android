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

namespace ExpressBase.Mobile.Droid
{
    public static class Constants
    {
        public const string NotificationTitle = "EXPRESSbase";

        public const string ListenConnectionString = "Endpoint=sb://eb-notf-ns-1.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=lUNytUkQZzHdcjn/1iBt2875oYxDG3GxEY9WXCC/cSc=";//"<Listen connection string>";

        public const string NotificationHubName = "eb-notf-hb-1";//"<hub name>";
    }
}