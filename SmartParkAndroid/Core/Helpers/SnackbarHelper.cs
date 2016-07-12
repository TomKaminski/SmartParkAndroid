using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace SmartParkAndroid.Core.Helpers
{
    public static class SnackbarHelper
    {
        public static void ShowSnackbar(string text, View view, bool isError = false, bool boldText = true)
        {
            var snackbar = Snackbar.Make(view, text, Snackbar.LengthLong);
            snackbar.View.SetBackgroundColor(isError ? Color.Rgb(234, 69, 75) : Color.Rgb(76, 175, 80));
            var snackbarTextView = snackbar.View.FindViewById<TextView>(Resource.Id.snackbar_text);
            snackbarTextView.SetTypeface(null, boldText ? TypefaceStyle.Bold : TypefaceStyle.Normal);
            snackbar.Show();
        }
    }
}