using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Calligraphy;

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

        public void HideSoftKeyboard()
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
            var currentFocus = CurrentFocus;
            if (currentFocus != null)
            {
                inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                currentFocus.ClearFocus();
            }
        }

     

        //public override bool DispatchTouchEvent(MotionEvent ev)
        //{
        //    if (isOnClick)
        //    {
                
        //    }
        //    return base.DispatchTouchEvent(ev);
        //}
    }


    //public class HideSoftKeyboardOnTouchListener : View.IOnTouchListener
    //{
    //    private BaseActivity _activity;

    //    public HideSoftKeyboardOnTouchListener(BaseActivity activity)
    //    {
    //        _activity = activity;
    //    }

    //    public void Dispose()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IntPtr Handle { get; }
    //    public bool OnTouch(View v, MotionEvent e)
    //    {
    //        _activity.HideSoftKeyboard();
    //        return false;
    //    }
    //}
}