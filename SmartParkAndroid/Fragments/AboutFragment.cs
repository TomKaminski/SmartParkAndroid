using Android.OS;
using Android.Views;


namespace SmartParkAndroid.Fragments
{
    public class AboutFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.about_fragment, container, false);
            return view;
        }

        public override void OnInit()
        {
        }
    }
}