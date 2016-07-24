using System;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using SmartParkAndroid.Core;
using SmartParkAndroid.Core.Helpers;
using SmartParkAndroid.Models;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace SmartParkAndroid.Fragments
{
    public class LoginFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_fragment, container, false);

            var currentLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.fragment_current_linear_layout);
            currentLinearLayout.Click += CurrentLinearLayout_Click;

            var btnLogin = view.FindViewById<Button>(Resource.Id.btnLogin);
            var passwordInput = view.FindViewById<TextInputLayout>(Resource.Id.txtInputPassword_layout);
            var emailInput = view.FindViewById<TextInputLayout>(Resource.Id.txtInputEmail_layout);

            emailInput.EditText.AfterTextChanged += (sender, e) => EmailInput_AfterTextChanged(sender, e, emailInput);
            emailInput.FocusChange += EditTextFocusChange;

            passwordInput.EditText.AfterTextChanged += (sender, e) => PasswordInput_AfterTextChanged(sender, e, passwordInput);
            passwordInput.FocusChange += EditTextFocusChange;

            btnLogin.Click += async (o, e) =>
            {
                if (InputErrorHelper.EmailValidateFunc(emailInput) && InputErrorHelper.PasswordValidationFunc(passwordInput))
                {
                    btnLogin.Enabled = false;
                    Activity.RunOnUiThread(() =>
                    {
                        (Activity as MainActivity).HideSoftKeyboard();
                        (Activity as MainActivity).ShowProgressBar();
                    });
                    var txtPassword = passwordInput.EditText.Text;
                    var txtEmail = emailInput.EditText.Text;
                    var smartHttpClient = new SmartParkHttpClient();
                    await smartHttpClient.Post<SmartJsonResult<User>, object>(new Uri("https://smartparkath.azurewebsites.net/api/Account/Login", UriKind.Absolute),
                        new
                        {
                            Username = txtEmail,
                            Password = txtPassword
                        }, response =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                btnLogin.Enabled = true;
                                (Activity as MainActivity).HideProgressBar();
                                LogIn(response, view);
                            });
                            return true;
                        }, null, response =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                btnLogin.Enabled = true;
                                (Activity as MainActivity).HideProgressBar();
                                SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                            });
                            return true;
                        });
                }
            };
            return view;
        }

        public override void OnInit()
        {
            var passwordInput = View.FindViewById<TextInputLayout>(Resource.Id.txtInputPassword_layout);
            if (string.IsNullOrEmpty(passwordInput.EditText.Text))
            {
                InputErrorHelper.SetError(string.Empty, passwordInput);
            }
            else
            {
                InputErrorHelper.PasswordValidationFunc(passwordInput);
            }

            var emailInput = View.FindViewById<TextInputLayout>(Resource.Id.txtInputEmail_layout);
            if (string.IsNullOrEmpty(emailInput.EditText.Text))
            {
                InputErrorHelper.SetError(string.Empty, emailInput);
            }
            else
            {
                InputErrorHelper.EmailValidateFunc(emailInput);
            }
        }

        private void CurrentLinearLayout_Click(object sender, EventArgs e)
        {
            (Activity as MainActivity).HideSoftKeyboard();
        }

        private void EditTextFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                InputMethodManager inputManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
                var currentFocus = Activity.CurrentFocus;
                if (currentFocus != null)
                {
                    inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }
            }
        }

        private void PasswordInput_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e, TextInputLayout wrapper)
        {
            InputErrorHelper.PasswordValidationFunc(wrapper);
        }

        private void EmailInput_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e, TextInputLayout wrapper)
        {
            InputErrorHelper.EmailValidateFunc(wrapper);
        }

        private void LogIn(SmartJsonResult<User> model, View view)
        {
            if (model.IsValid)
            {
                var result = model.Result;

                var mainActiviy = (MainActivity)Activity;
                StaticManager.LoggedIn = true;

                var prefs = mainActiviy.GetPreferences(FileCreationMode.Private);
                var editor = prefs.Edit();
                editor.PutString("username", result.Email);
                editor.PutString("userhash", result.PasswordHash);
                editor.PutString("imagebase64", result.ImageBase64);
                editor.PutInt("charges", result.Charges);

                editor.Commit();

                StaticManager.InitBase(true, result.Email, result.PasswordHash, result.Charges, result.ImageBase64);

                mainActiviy.LogIn();
            }
            else
            {
                SnackbarHelper.Error(model.ValidationErrors.FirstOrDefault(), view);
            }

        }

    }
}