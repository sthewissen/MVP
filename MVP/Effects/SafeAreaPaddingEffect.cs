using System.Linq;
using Xamarin.Forms;

namespace MVP.Effects
{
    // TODO: This will be in XamarinCommunityToolkit, so replace once that has a release.
    public static class SafeAreaPaddingEffect
    {
        public static readonly BindableProperty HasSafeAreaPaddingProperty =
            BindableProperty.CreateAttached("HasSafeAreaPadding", typeof(bool), typeof(SafeAreaPaddingEffect), false,
                propertyChanged: OnHasSafeAreaPaddingChanged);

        public static readonly BindableProperty HasTopSafeAreaPaddingProperty =
          BindableProperty.CreateAttached("HasTopSafeAreaPadding", typeof(bool), typeof(SafeAreaPaddingEffect), false);

        public static readonly BindableProperty HasBottomSafeAreaPaddingProperty =
          BindableProperty.CreateAttached("HasBottomSafeAreaPadding", typeof(bool), typeof(SafeAreaPaddingEffect), false);

        public static bool GetHasSafeAreaPadding(BindableObject view)
        {
            return (bool)view.GetValue(HasSafeAreaPaddingProperty);
        }

        public static void SetHasSafeAreaPadding(BindableObject view, bool value)
        {
            view.SetValue(HasSafeAreaPaddingProperty, value);
        }

        public static bool GetHasTopSafeAreaPadding(BindableObject view)
        {
            return (bool)view.GetValue(HasTopSafeAreaPaddingProperty);
        }

        public static void SetHasTopSafeAreaPadding(BindableObject view, bool value)
        {
            view.SetValue(HasTopSafeAreaPaddingProperty, value);
        }

        public static bool GetHasBottomSafeAreaPadding(BindableObject view)
        {
            return (bool)view.GetValue(HasBottomSafeAreaPaddingProperty);
        }

        public static void SetHasBottomSafeAreaPadding(BindableObject view, bool value)
        {
            view.SetValue(HasBottomSafeAreaPaddingProperty, value);
        }

        static void OnHasSafeAreaPaddingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            bool hasSafeArea = (bool)newValue;

            if (hasSafeArea)
            {
                view.Effects.Add(new SafeAreaPaddingEffectRouter());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is SafeAreaPaddingEffectRouter);

                if (toRemove != null)
                {
                    view.Effects.Remove(toRemove);
                }
            }
        }

        public class SafeAreaPaddingEffectRouter : RoutingEffect
        {
            public SafeAreaPaddingEffectRouter() : base("MVP.SafeAreaPaddingEffectRouter")
            {
            }
        }
    }
}
