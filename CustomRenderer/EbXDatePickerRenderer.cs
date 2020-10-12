using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EbXDatePicker), typeof(EbXDatePickerRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class EbXDatePickerRenderer : DatePickerRenderer
    {
        readonly GradientDrawable drawable;

        public EbXDatePickerRenderer(Context context) : base(context)
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

            var ctrl = (EbXDatePicker)sender;

            if (e.PropertyName == EbXDatePicker.XBackgroundColorProperty.PropertyName)
            {
                drawable.SetColor(ctrl.XBackgroundColor.ToAndroid());
            }
            else if (e.PropertyName == EbXDatePicker.BorderColorProperty.PropertyName)
            {
                drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());
            }
        }
    }
}