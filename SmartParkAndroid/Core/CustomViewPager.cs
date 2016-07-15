using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

namespace SmartParkAndroid.Core
{
    public class CustomViewPager: ViewPager
    {
        private bool _swipeEnabled;

        public CustomViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            _swipeEnabled = true;
        }

        public CustomViewPager(Context context) : base(context)
        {
            _swipeEnabled = true;

        }

        public CustomViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _swipeEnabled = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (_swipeEnabled)
            {
                return base.OnTouchEvent(e);
            }
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (_swipeEnabled)
            {
                return base.OnInterceptTouchEvent(ev);
            }
            return false;
        }

        public void SetSwipePagingEnabled(bool enabled)
        {
            _swipeEnabled = enabled;
        }
    }
}