using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using Google.MobileAds;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AceMobileAppTemplate.Controls.AdControlView), typeof(AceMobileAppTemplate.iOS.Renderers.AdViewRenderer))]
namespace AceMobileAppTemplate.iOS.Renderers
{
    public class AdViewRenderer : ViewRenderer<Controls.AdControlView, BannerView>
    {
        private string bannerId = "ca-app-pub-9220085631369979/2462881464";
        private BannerView adView;
        
        private BannerView CreateNativeAdControl()
        {
            if (adView != null)
                return adView;

            adView = new BannerView(size: AdSizeCons.SmartBannerPortrait,
                                    origin: new CGPoint(0, UIScreen.MainScreen.Bounds.Size.Height - AdSizeCons.Banner.Size.Height))
            {
                AdUnitId = bannerId,
                RootViewController = GetVisibleViewController()
            };


            adView.LoadRequest(GetRequest());
            return adView;
        }

        private Request GetRequest()
        {
            var request = Request.GetDefaultRequest();
            MobileAds.SharedInstance.RequestConfiguration.TestDeviceIdentifiers = new[]
            {
                Request.SimulatorId.ToString(),
                "00008030-001235D41E02802E"
            };
            return request;
        }

        private UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;
            }

            if (rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Controls.AdControlView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeAdControl();
                SetNativeControl(adView);
            }
        }
    }
}