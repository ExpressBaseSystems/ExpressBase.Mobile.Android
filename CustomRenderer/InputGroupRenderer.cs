using Android.Content;
using Android.Graphics.Drawables;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(InputGroup), typeof(InputGroupRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class InputGroupRenderer : FrameRenderer
    {
        readonly GradientDrawable drawable;

        public InputGroupRenderer(Context context) : base(context)
        {
            drawable = new GradientDrawable();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);

            var ctrl = e.NewElement as IEbCustomControl;

            drawable.SetShape(ShapeType.Rectangle);
            drawable.SetCornerRadius(ctrl.BorderRadius);

            if (ctrl.BorderColor != null)
                drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());

            if (ctrl.XBackgroundColor != null)
                drawable.SetColor(ctrl.XBackgroundColor.ToAndroid());

            this.SetBackground(drawable);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            InputGroup ctrl = (InputGroup)sender; 

            if (e.PropertyName == InputGroup.XBackgroundColorProperty.PropertyName)
            {
                drawable.SetColor(ctrl.XBackgroundColor.ToAndroid());
            }
            else if (e.PropertyName == InputGroup.BorderColorProperty.PropertyName)
            {
                drawable.SetStroke(ctrl.BorderThickness, ctrl.BorderColor.ToAndroid());
            }
        }
    }
}