using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace SmartParkAndroid.Fragments
{
    public class LoginFragment : SupportFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_fragment, container, false);

            var btnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);
            var passwordWrapper = view.FindViewById<EditText>(Resource.Id.txtPassword);
            var txtPassword = passwordWrapper.Text;

            //btnLogin.Click += (o, e) =>
            //{
            //    if (txtPassword != "1234")
            //    {
            //        passwordWrapper.Error = "Wrong password, try again";
            //    }
            //};

            return view;
        }
    }
}