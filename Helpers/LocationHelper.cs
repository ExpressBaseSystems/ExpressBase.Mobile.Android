using System;
using Android.Content;
using Android.Locations;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(LocationHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class LocationHelper : ILocationHelper
    {
        public void OpenSettings()
        {
            try
            {
                LocationManager manager = (LocationManager)MainActivity.Instance.GetSystemService(Context.LocationService);

                if (!manager.IsProviderEnabled(LocationManager.GpsProvider))
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
                    MainActivity.Instance.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                EbLog.Error("failed to open location settings, " + ex.Message);
            }
        }
    }
}