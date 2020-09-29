﻿using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NumericTextBox), typeof(NumericTextBoxRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    class NumericTextBoxRenderer : EntryRenderer
    {
        readonly GradientDrawable drawable;

        public NumericTextBoxRenderer(Context context) : base(context)
        {
            drawable = new GradientDrawable();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as IEbCustomControl;

                drawable.SetShape(ShapeType.Rectangle);
                drawable.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.BgColor != null)
                    drawable.SetColor(ctrl.BgColor.ToAndroid());

                Control.SetBackground(drawable);

                NumericTextBox textbox = e.NewElement as NumericTextBox;

                if (textbox.EnableFocus)
                {
                    textbox.Focused += Textbox_Focused;
                    textbox.Unfocused += Textbox_Unfocused;
                }
            }
        }

        private void Textbox_Unfocused(object sender, FocusEventArgs e)
        {
            var ctrl = (NumericTextBox)sender;
            drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());
        }

        private void Textbox_Focused(object sender, FocusEventArgs e)
        {
            var ctrl = (NumericTextBox)sender;
            drawable.SetStroke(2, ctrl.BorderOnFocus.ToAndroid());
        }
    }
}