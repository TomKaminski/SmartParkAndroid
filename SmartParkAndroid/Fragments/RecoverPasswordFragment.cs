using System;
using System.Linq;
using System.Net.Mail;
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
    public class RecoverPasswordFragment : SupportFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.recover_password_fragment, container, false);
            var btnRecover = view.FindViewById<Button>(Resource.Id.btn_recover_password);

            var emailInput = view.FindViewById<TextInputLayout>(Resource.Id.input_recovery_password);
            emailInput.EditText.AfterTextChanged += (sender, e) => EmailInput_AfterTextChanged(sender, e, emailInput);


            btnRecover.Click += async (o, e) =>
            {
                if (InputErrorHelper.EmailValidateFunc(emailInput))
                {
                    btnRecover.Enabled = false;

                    Activity.RunOnUiThread(() =>
                    {
                        (Activity as MainActivity).ShowProgressBar();
                    });
                    var txtEmail = emailInput.EditText.Text;
                    var smartHttpClient = new SmartParkHttpClient();
                    await smartHttpClient.Post<SmartJsonResult<bool>, object>(new Uri("https://smartparkath.azurewebsites.net/api/Account/Forgot", UriKind.Absolute),
                        new
                        {
                            Email = txtEmail,
                        }, response =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                btnRecover.Enabled = true;

                                (Activity as MainActivity).HideProgressBar();
                                if (response.IsValid)
                                {
                                    SnackbarHelper.ShowSnackbar("Na podany adres email zosta³y wys³ane kolejne instrukcje przywrócenia has³a.", view, false, true);

                                }
                                else
                                {
                                    SnackbarHelper.ShowSnackbar(response.ValidationErrors.FirstOrDefault(), view, true, true);
                                }
                            });
                            return true;
                        }, null, response =>
                        {
                            btnRecover.Enabled = true;

                            Activity.RunOnUiThread(() =>
                            {
                                (Activity as MainActivity).HideProgressBar();
                                SnackbarHelper.ShowSnackbar(response.ValidationErrors.FirstOrDefault(), view, true, true);
                            });
                            return true;
                        });
                }
            };

            return view;
        }

        private void EmailInput_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e, TextInputLayout wrapper)
        {
            InputErrorHelper.EmailValidateFunc(wrapper);
        }

       
    }
}