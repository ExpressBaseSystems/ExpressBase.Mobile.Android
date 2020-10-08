using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class CustomDatePickerRenderer : DatePickerRenderer
    {
        readonly GradientDrawable drawable;

        public CustomDatePickerRenderer(Context context) : base(context)
        {
            drawable = new GradientDrawable();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
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
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomDatePicker.XBackgroundColorProperty.PropertyName)
            {
                drawable.SetColor((sender as CustomDatePicker).XBackgroundColor.ToAndroid());
            }
        }
    }
}