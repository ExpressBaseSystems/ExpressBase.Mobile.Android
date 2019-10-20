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
[assembly: ExportRenderer(typeof(XButton), typeof(ButtonRenderer))]
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
}