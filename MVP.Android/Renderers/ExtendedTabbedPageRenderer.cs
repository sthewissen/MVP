using Android.Content;
using Android.Graphics;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Internal;
using MVP.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(ExtendedTabbedPageRenderer))]
namespace MVP.Droid.Renderers
{
    public class ExtendedTabbedPageRenderer : TabbedPageRenderer
    {
        BottomNavigationView bottomNavigationView;

        public ExtendedTabbedPageRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                bottomNavigationView = (GetChildAt(0) as Android.Widget.RelativeLayout).GetChildAt(1) as BottomNavigationView;
                ChangeFont();
            }
        }

        void ChangeFont()
        {
            var fontFace = Typeface.CreateFromAsset(Context.Assets, "SourceSansPro-Regular.ttf");

            if (!(bottomNavigationView.GetChildAt(0) is BottomNavigationMenuView bottomNavMenuView))
                return;

            for (var i = 0; i < bottomNavMenuView.ChildCount; i++)
            {
                var item = bottomNavMenuView.GetChildAt(i) as BottomNavigationItemView;
                var itemTitle = item.GetChildAt(1);

                var smallTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(0));
                var largeTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(1));

                smallTextView.SetTypeface(fontFace, TypefaceStyle.Normal);
                largeTextView.SetTypeface(fontFace, TypefaceStyle.Normal);
            }
        }
    }
}
