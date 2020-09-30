using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
[assembly: ExportRenderer(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]
[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomSelectRenderer))]
[assembly: ExportRenderer(typeof(ListViewSearchBar), typeof(CustomSearchRenderer))]
[assembly: ExportRenderer(typeof(ComboBoxLabel), typeof(ComboLabelRenderer))]
[assembly: ExportRenderer(typeof(InputGroup), typeof(InputGroupRenderer))]
[assembly: ExportRenderer(typeof(HiddenEntry), typeof(HiddenEntryRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        public CustomDatePickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as IEbCustomControl;

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    gd.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.BgColor != null)
                    gd.SetColor(ctrl.BgColor.ToAndroid());

                Control.SetBackground(gd);
            }
        }
    }

    public class CustomTimePickerRenderer : TimePickerRenderer
    {
        public CustomTimePickerRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as IEbCustomControl;

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    gd.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.BgColor != null)
                    gd.SetColor(ctrl.BgColor.ToAndroid());

                Control.SetBackground(gd);
            }
        }
    }

    public class CustomSelectRenderer : PickerRenderer
    {
        public CustomSelectRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as IEbCustomControl;

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    gd.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.BgColor != null)
                    gd.SetColor(ctrl.BgColor.ToAndroid());

                Control.SetBackground(gd);
            }
        }
    }

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

    public class InputGroupRenderer : FrameRenderer
    {
        public InputGroupRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);

            var ctrl = e.NewElement as IEbCustomControl;

            GradientDrawable gd = new GradientDrawable();
            gd.SetShape(ShapeType.Rectangle);
            gd.SetCornerRadius(ctrl.BorderRadius);

            if (ctrl.BorderColor != null)
                gd.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

            if (ctrl.BgColor != null)
                gd.SetColor(ctrl.BgColor.ToAndroid());

            this.SetBackground(gd);
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