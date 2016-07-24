using SupportFragment = Android.Support.V4.App.Fragment;


namespace SmartParkAndroid.Fragments
{
    public abstract class BaseFragment : SupportFragment
    {
        public abstract void OnInit();
    }
}