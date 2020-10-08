using Android.Content;
using Android.Widget;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ListViewSearchBar), typeof(CustomSearchRenderer))]
[assembly: ExportRenderer(typeof(ComboBoxLabel), typeof(ComboLabelRenderer))]
[assembly: ExportRenderer(typeof(EbXHiddenEntry), typeof(HiddenEntryRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class CustomSearchRenderer : SearchBarRenderer
    {
        public CustomSearchRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                RemoveUnderLine();

                var ctrl = e.NewElement as ListViewSearchBar;

                if (ctrl.HideIcon)
                {
                    this.HideSearchIcon();
                }
            }
        }

        private void RemoveUnderLine()
        {
            int plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
            var plate = Control.FindViewById(plateId);
            plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        private void HideSearchIcon()
        {
            int searchIconId = Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            var icon = (ImageView)Control.FindViewById(searchIconId);
            icon.LayoutParameters = new LinearLayout.LayoutParams(0, 0);
        }
    }

    public class ComboLabelRenderer : LabelRenderer
    {
        public ComboLabelRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
        }
    }

    public class HiddenEntryRenderer : EntryRenderer
    {
        public HiddenEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);

            Control.SetCursorVisible(false);
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}