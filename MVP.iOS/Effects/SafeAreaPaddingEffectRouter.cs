using System;
using MVP.Effects;
using MVP.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("MVP")]
[assembly: ExportEffect(typeof(SafeAreaPaddingEffectRouter), nameof(SafeAreaPaddingEffectRouter))]
namespace MVP.iOS.Effects
{
    class SafeAreaPaddingEffectRouter : PlatformEffect
    {
        Thickness _padding;

        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;

                    if (insets.Top > 0)
                    {

                        element.Padding = new Thickness(
                            _padding.Left + insets.Left,
                            _padding.Top + (SafeAreaPaddingEffect.GetHasTopSafeAreaPadding(element) ? insets.Top : 0),
                            _padding.Right + insets.Right,
                            _padding.Bottom + (SafeAreaPaddingEffect.GetHasBottomSafeAreaPadding(element) ? insets.Bottom : 0)
                        );

                        return;
                    }
                }

                element.Padding = new Thickness(_padding.Left, _padding.Top + 20, _padding.Right, _padding.Bottom + 20);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _padding;
            }
        }
    }
}