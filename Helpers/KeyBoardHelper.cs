
using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(KeyBoardHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class KeyBoardHelper : IKeyboardHelper
    {
        public KeyBoardHelper() { }

        public void HideKeyboard()
        {
            try
            {
                var context = Application.Context;
                InputMethodManager inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                if (inputMethodManager != null && context is Activity)
                {
                    var activity = context as Activity;
                    var token = activity.CurrentFocus?.WindowToken;
                    inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                    activity.Window.DecorView.ClearFocus();
                }
            }
            catch
            {
                //handle exceptions
            }
        }
    }
}