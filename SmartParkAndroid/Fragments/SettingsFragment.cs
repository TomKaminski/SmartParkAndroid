using Android.OS;
using Android.Views;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SmartParkAndroid.Fragments
{
    public class SettingsFragment : SupportFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.settings_fragment, container, false);

            return view;
        }
    }
}