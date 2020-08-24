using Android.Widget;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class ToastMessage : IToast
    {
        public void Show(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}