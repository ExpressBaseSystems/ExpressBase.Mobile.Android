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
[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomSelectRenderer))]
[assembly: ExportRenderer(typeof(CustomSearchBar), typeof(CustomSearchRenderer))]
[assembly: ExportRenderer(typeof(ComboBoxLabel), typeof(ComboLabelRenderer))]
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
                var textbox = e.NewElement as TextBox;

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);

                if (textbox.HasBackground)
                    gd.SetColor(Android.Graphics.Color.White);
                else
                    gd.SetColor(Android.Graphics.Color.Transparent);

                gd.SetCornerRadius(textbox.BorderRadius);

                if (textbox.HasBorder)
                    gd.SetStroke(textbox.BorderThickness, Android.Graphics.Color.ParseColor(textbox.BorderColor));

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
                var textbox = e.NewElement as NumericTextBox;

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);

                if (textbox.HasBackground)
                    gd.SetColor(Android.Graphics.Color.White);
                else
                    gd.SetColor(Android.Graphics.Color.Transparent);

                gd.SetCornerRadius(textbox.BorderRadius);

                if (textbox.HasBorder)
                    gd.SetStroke(textbox.BorderThickness, Android.Graphics.Color.ParseColor(textbox.BorderColor));

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
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.Transparent);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
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
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.Transparent);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
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
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.Transparent);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
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

                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.Transparent);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
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
}