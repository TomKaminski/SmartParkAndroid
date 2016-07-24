using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;

namespace SmartParkAndroid.Fragments
{
    public class SettingsFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.settings_fragment, container, false);

            var sb = new SpannableStringBuilder(Resources.GetString(Resource.String.settings_desc_first));
            var boldSpan = new StyleSpan(TypefaceStyle.Bold);
            var normalSpan = new StyleSpan(TypefaceStyle.Normal);

            sb.SetSpan(normalSpan, 0, 104, SpanTypes.InclusiveInclusive);
            sb.SetSpan(boldSpan, 105, Resources.GetString(Resource.String.settings_desc_first).Length - 1, SpanTypes.InclusiveInclusive);

            var textView = view.FindViewById<TextView>(Resource.Id.settings_desc_textview_first);

            textView.TextFormatted = sb;

            var settingsBtn = view.FindViewById<Button>(Resource.Id.btnSettins);
            settingsBtn.Click += SettingsBtn_Click;

            return view;
        }

        public override void OnInit()
        {
        }

        private void SettingsBtn_Click(object sender, System.EventArgs e)
        {
            var uri = Uri.Parse("http://smartparkath.azurewebsites.net/Portal/Konto");
            var intent = new Intent(Intent.ActionView);
            intent.SetData(uri);
            var chooser = Intent.CreateChooser(intent, "Open with");
            StartActivity(chooser);
        }
    }
}