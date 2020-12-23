using Android.Content;
using ExpressBase.Mobile.CustomControls;
using ExpressBase.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(EbXFrame), typeof(EbXFrameRenderer))]

namespace ExpressBase.Mobile.Droid.CustomRenderer
{
    public class EbXFrameRenderer : FrameRenderer
    {
        public EbXFrameRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            EbXFrame element = e.NewElement as EbXFrame;

            //if (element == null) return;

            //if (element.HasShadow)
            //{
            //    Elevation = 30.0f;
            //    TranslationZ = 0.0f;
            //    SetZ(30f);
            //}
        }
    }
}