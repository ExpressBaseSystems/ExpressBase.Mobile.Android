using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EbXPicker), typeof(EbXSelectRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class EbXSelectRenderer : PickerRenderer
    {
        public EbXSelectRenderer(Context context) : base(context)
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

                if (ctrl.XBackgroundColor != null)
                    gd.SetColor(ctrl.XBackgroundColor.ToAndroid());

                Control.SetBackground(gd);
            }
        }
    }
}