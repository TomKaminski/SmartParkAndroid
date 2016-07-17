using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SmartParkAndroid.Core;
using SmartParkAndroid.Core.Helpers;
using SmartParkAndroid.Models;
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
            var passwordInput = view.FindViewById<TextInputLayout>(Resource.Id.txtInputPassword_layout);
            var emailInput = view.FindViewById<TextInputLayout>(Resource.Id.txtInputEmail_layout);

            emailInput.EditText.AfterTextChanged += (sender, e) => EmailInput_AfterTextChanged(sender, e, emailInput);

            passwordInput.EditText.AfterTextChanged += (sender, e) => PasswordInput_AfterTextChanged(sender, e, passwordInput);

            btnLogin.Click += async (o, e) =>
            {
                if (InputErrorHelper.EmailValidateFunc(emailInput) && InputErrorHelper.PasswordValidationFunc(passwordInput))
                {
                    btnLogin.Enabled = false;
                    Activity.RunOnUiThread(() =>
                    {
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
                                SnackbarHelper.ShowSnackbar(response.ValidationErrors.FirstOrDefault(), view, true, true);
                            });
                            return true;
                        });
                }
            };
            return view;
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
                var mainActiviy = (MainActivity)Activity;
                StaticManager.LoggedIn = true;

                var prefs = mainActiviy.GetPreferences(FileCreationMode.Private);
                var editor = prefs.Edit();
                editor.PutString("username", model.Result.Email);
                editor.PutString("userhash", model.Result.PasswordHash);
                editor.PutString("imageid", model.Result.ImageId);
                editor.PutInt("charges", model.Result.Charges);
                editor.Commit();

                StaticManager.UserName = model.Result.Email;
                StaticManager.UserHash = model.Result.PasswordHash;
                StaticManager.Charges = model.Result.Charges;
                StaticManager.ImageId = model.Result.ImageId;

                mainActiviy.LogIn();
            }
            else
            {
                SnackbarHelper.ShowSnackbar(model.ValidationErrors.FirstOrDefault(), view, true, true);
            }

        }

    }
}