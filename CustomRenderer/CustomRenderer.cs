using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TextBox), typeof(TextBoxRenderer))]
[assembly: ExportRenderer(typeof(NumericTextBox), typeof(NumericTextBoxRenderer))]
[assembly: ExportRenderer(typeof(TextArea), typeof(TextAreaRenderer))]
[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
[assembly: ExportRenderer(typeof(CustomTimePicker), typeof(CustomTimePickerRenderer))]
[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomSelectRenderer))]
[assembly: ExportRenderer(typeof(CustomSearchBar), typeof(CustomSearchRenderer))]
[assembly: ExportRenderer(typeof(ComboBoxLabel), typeof(ComboLabelRenderer))]
[assembly: ExportRenderer(typeof(InputGroup), typeof(InputGroupRenderer))]
[assembly: ExportRenderer(typeof(HiddenEntry), typeof(HiddenEntryRenderer))]


namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    class TextBoxRenderer : EntryRenderer
    {
        public TextBoxRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
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

    class NumericTextBoxRenderer : EntryRenderer
    {
        public NumericTextBoxRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
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

    class TextAreaRenderer : EditorRenderer
    {
        public TextAreaRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
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
        public CustomSearchRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                LinearLayout linearLayout = this.Control.GetChildAt(0) as LinearLayout;
                linearLayout = linearLayout.GetChildAt(2) as LinearLayout;
                linearLayout = linearLayout.GetChildAt(1) as LinearLayout;

                linearLayout.Background = null; //removes underline

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