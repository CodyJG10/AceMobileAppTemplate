using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AceMobileAppTemplate.Controls;
using AceMobileAppTemplate.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdControlView), typeof(AdViewRenderer))]
namespace AceMobileAppTemplate.Droid.Renderer
{
    public class AdViewRenderer : ViewRenderer<AdControlView, AdView>
    {
        public AdViewRenderer(Context context) : base(context) { }

        string adUnitId = "ca-app-pub-9220085631369979/2462881464";
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        AdView CreateAdView()
        {
            if (adView != null)
            {
                return adView;
            }
            adView = new AdView(Context);

            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;
            var adParams = new LinearLayout.LayoutParams(
                LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest.Builder().Build());

            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdControlView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                CreateAdView();
                SetNativeControl(adView);
            }
        }
    }
}