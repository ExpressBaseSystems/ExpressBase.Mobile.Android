using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TextBox), typeof(TextBoxRenderer))]
[assembly: ExportRenderer(typeof(NumericTextBox), typeof(NumericBoxRenderer))]
[assembly: ExportRenderer(typeof(XButton), typeof(ButtonRenderer))]
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
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.White);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
                Control.SetBackground(gd);
            }
        }
    }

    class NumericBoxRenderer : EntryRenderer
    {
        public NumericBoxRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.White);
                gd.SetCornerRadius(10.0f);
                gd.SetStroke(1, Android.Graphics.Color.ParseColor("#cccccc"));
                Control.SetBackground(gd);
            }
        }
    }

    public class XButtonRenderer : ButtonRenderer
    {
        public XButtonRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
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
                gd.SetColor(Android.Graphics.Color.White);
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
                gd.SetColor(Android.Graphics.Color.White);
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
                GradientDrawable gd = new GradientDrawable();
                gd.SetShape(ShapeType.Rectangle);
                gd.SetColor(Android.Graphics.Color.White);
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