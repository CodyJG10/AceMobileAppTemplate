using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AceMobileAppTemplate.Controls;
using AceMobileAppTemplate.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace AceMobileAppTemplate.iOS.Renderers
{
    class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            this.Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}
