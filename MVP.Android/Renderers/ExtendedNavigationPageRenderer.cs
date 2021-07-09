//using System.Collections.Generic;
//using System.Linq;
//using Android.Content;
//using Android.Support.V7.Widget;
//using Android.Views;
//using MVP.Android.Renderers;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;
//using Xamarin.Forms.Platform.Android.AppCompat;
//using Android = Droid;

//[assembly: ExportRenderer(typeof(NavigationPage), typeof(ExtendedNavigationPageRenderer))]

//namespace MVP.Android.Renderers
//{
//    public class ExtendedNavigationPageRenderer : NavigationPageRenderer
//    {
//        Toolbar modalToolbar;

//        public ExtendedNavigationPageRenderer(Context context)
//            : base(context)
//        {
//        }

//        protected override void OnAttachedToWindow()
//        {
//            base.OnAttachedToWindow();

//            if (Element.CurrentPage.ToolbarItems.Any(x => x.Priority == -1))
//            {
//                var activity = Context as FormsAppCompatActivity;
//                var content = activity.FindViewById(Droid.Resource.Id.content) as ViewGroup;

//                var toolbars = content.GetChildrenOfType<Toolbar>();

//                modalToolbar = toolbars.Last();
//                modalToolbar.NavigationClick += ModalToolbarOnNavigationClick;
//            }
//        }

//        protected override void OnDetachedFromWindow()
//        {
//            base.OnDetachedFromWindow();

//            if (modalToolbar != null)
//            {
//                modalToolbar.NavigationClick -= ModalToolbarOnNavigationClick;
//            }
//        }

//        protected override void OnLayout(bool changed, int l, int t, int r, int b)
//        {
//            base.OnLayout(changed, l, t, r, b);

//            if (Element.CurrentPage.ToolbarItems.Any(x=>x.Priority == -1))
//            {
//                modalToolbar?.SetNavigationIcon(Resource.Drawable.baseline_close_white_24);
//            }
//        }

//        private void ModalToolbarOnNavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
//        {
//            if (Element.CurrentPage.ToolbarItems.Any(x => x.Priority == -1))
//            {
//                modalPage.Dismiss();
//            }
//            else
//            {
//                Element.SendBackButtonPressed();
//            }
//        }
//    }
//}