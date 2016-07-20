using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace SmartParkAndroid.Core.Helpers
{
    public static class SnackbarHelper
    {
        public static void Success(string text, View view, bool boldText = true)
        {
            var snackbar = Snackbar.Make(view, text, Snackbar.LengthLong);
            snackbar.View.SetBackgroundColor(Color.Rgb(76, 175, 80));
            var snackbarTextView = snackbar.View.FindViewById<TextView>(Resource.Id.snackbar_text);
            snackbarTextView.SetTypeface(null, boldText ? TypefaceStyle.Bold : TypefaceStyle.Normal);
            snackbar.Show();
        }

        public static void Error(string text, View view, bool boldText = true)
        {
            var snackbar = Snackbar.Make(view, text, Snackbar.LengthLong);
            snackbar.View.SetBackgroundColor(Color.Rgb(234, 69, 75));
            var snackbarTextView = snackbar.View.FindViewById<TextView>(Resource.Id.snackbar_text);
            snackbarTextView.SetTypeface(null, boldText ? TypefaceStyle.Bold : TypefaceStyle.Normal);
            snackbar.Show();
        }
    }
}