using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SmartParkAndroid.Core;
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

            btnLogin.Click += (o, e) =>
            {
                LogIn();
            };

            return view;
        }

        private void LogIn()
        {
            var username = "tkaminski93@gmail.com";
            var hash = "hash";
            var charges = 12;

            var mainActiviy = (MainActivity)Activity;
            StaticManager.LoggedIn = true;

            var prefs = mainActiviy.GetPreferences(FileCreationMode.Private);
            var editor = prefs.Edit();
            editor.PutString("username", username);
            editor.PutString("userhash", hash);
            editor.PutInt("charges", charges);
            editor.Commit();

            StaticManager.UserName = username;
            StaticManager.UserHash = hash;
            StaticManager.Charges = charges;

            mainActiviy.LogIn();
        }
    }
}