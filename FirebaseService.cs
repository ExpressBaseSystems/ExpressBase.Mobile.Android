using Android.App;
using Android.Content;
using Firebase.Messaging;
using Android.Support.V4.App;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Constants;
using ExpressBase.Mobile.Models;
using System;
using Android.OS;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExpressBase.Mobile.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            Store.SetValue(AppConst.PNS_TOKEN, token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            RemoteMessage.Notification nfc = message.GetNotification();

            if (nfc != null)
            {
                EbNFData nf = new EbNFData { Message = nfc.Body };
                this.SendLocalNotification(nf);
            }
            else
            {
                try
                {
                    if (message.Data?.Count > 0)
                    {
                        EbNFData nfData = this.CreateNFTemplate(message.Data);
                        this.SendLocalNotification(nfData);
                    }
                }
                catch (Exception ex)
                {
                    EbLog.Error("error on deserializing EbNFData");
                    EbLog.Error(ex.Message);
                }
            }
        }

        EbNFData CreateNFTemplate(IDictionary<string, string> data)
        {
            EbNFData nf = new EbNFData();

            if (data.ContainsKey("Title"))
                nf.Title = data["Title"];

            if (data.ContainsKey("Message"))
                nf.Message = data["Message"];

            if (data.ContainsKey("Link"))
            {
                try
                {
                    nf.Link = JsonConvert.DeserializeObject<EbNFLink>(data["Link"]);
                }
                catch (Exception ex)
                {
                    EbLog.Error("error on deserializing EbNFLink");
                    EbLog.Error(ex.Message);
                }
            }
            return nf;
        }

        void SendLocalNotification(EbNFData data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("nf_data", JsonConvert.SerializeObject(data));

            var requestCode = new Random().Next();
            var pendingIntent = PendingIntent.GetActivity(this, requestCode, intent, PendingIntentFlags.OneShot);

            var noti = new NotificationCompat.Builder(this, NFConstants.ChannelId);

            noti.SetContentTitle(data.Title);
            noti.SetContentText(data.Message);
            noti.SetSmallIcon(Resource.Drawable.ic_launcher);
            noti.SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);
            noti.SetAutoCancel(true);
            noti.SetShowWhen(true);
            noti.SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                noti.SetChannelId(NFConstants.ChannelId);
            }

            var manager = NotificationManager.FromContext(this);
            manager.Notify(requestCode, noti.Build());
        }
    }
}