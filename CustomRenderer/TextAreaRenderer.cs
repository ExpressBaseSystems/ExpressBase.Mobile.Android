using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TextArea), typeof(TextAreaRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    class TextAreaRenderer : EditorRenderer
    {
        readonly GradientDrawable drawable;

        public TextAreaRenderer(Context context) : base(context)
        {
            drawable = new GradientDrawable();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as IEbCustomControl;

                drawable.SetShape(ShapeType.Rectangle);
                drawable.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.XBackgroundColor != null)
                    drawable.SetColor(ctrl.XBackgroundColor.ToAndroid());

                Control.SetBackground(drawable);

                TextArea textbox = e.NewElement as TextArea;

                if (textbox.EnableFocus)
                {
                    textbox.Focused += Textbox_Focused;
                    textbox.Unfocused += Textbox_Unfocused;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TextArea.XBackgroundColorProperty.PropertyName)
            {
                drawable.SetColor((sender as TextArea).XBackgroundColor.ToAndroid());
            }
        }

        private void Textbox_Unfocused(object sender, FocusEventArgs e)
        {
            var ctrl = (TextArea)sender;
            drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());
        }

        private void Textbox_Focused(object sender, FocusEventArgs e)
        {
            var ctrl = (TextArea)sender;
            drawable.SetStroke(2, ctrl.BorderOnFocus.ToAndroid());
        }
    }
}