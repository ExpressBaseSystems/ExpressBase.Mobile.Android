using Android.Content;
using Android.Graphics.Drawables;
using Android.Util;
using ExpressBase.Mobile.CustomControls.XControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EbXLabel), typeof(EbXLabelRenderer))]
namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class EbXLabelRenderer : LabelRenderer
    {
        readonly GradientDrawable drawable;

        public EbXLabelRenderer(Context context) : base(context)
        {
            drawable = new GradientDrawable();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var ctrl = e.NewElement as EbXLabel;

                drawable.SetShape(ShapeType.Rectangle);
                drawable.SetCornerRadius(ctrl.BorderRadius);

                if (ctrl.BorderColor != null)
                    drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

                if (ctrl.XBackgroundColor != null)
                    drawable.SetColor(ctrl.XBackgroundColor.ToAndroid());

                Control.SetBackground(drawable);

                //EbXLabel Label = e.NewElement as EbXLabel;
            }
        }

        public static float DpToPixels(Context context, float valueInDp)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}