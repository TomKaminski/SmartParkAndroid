using Android.Content;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using UK.CO.Chrisjenx.Calligraphy;

namespace SmartParkAndroid
{
    public class BaseActivity : AppCompatActivity
    {
        protected void SetActivityScreenOrientation(ScreenOrientation orientation)
        {
            RequestedOrientation = orientation;
        }

        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }


        protected void RemoveNavHeaders(NavigationView view)
        {
            var headersInNav = view.HeaderCount;
            for (int i = 0; i < headersInNav; i++)
            {
                view.RemoveHeaderView(view.GetHeaderView(i));
            }
        }

        public void ShowProgressBar()
        {
            FindViewById<ProgressBar>(Resource.Id.progress_bar).Visibility = ViewStates.Visible;
        }

        public void HideProgressBar()
        {
            FindViewById<ProgressBar>(Resource.Id.progress_bar).Visibility = ViewStates.Invisible;
        }
    }
}