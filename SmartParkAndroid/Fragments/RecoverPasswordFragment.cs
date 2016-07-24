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
    public class RecoverPasswordFragment : BaseFragment
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
            emailInput.FocusChange += EditTextFocusChange;

            var currentLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.fragment_current_linear_layout);
            currentLinearLayout.Click += CurrentLinearLayout_Click;


            btnRecover.Click += async (o, e) =>
            {
                if (InputErrorHelper.EmailValidateFunc(emailInput))
                {
                    btnRecover.Enabled = false;

                    Activity.RunOnUiThread(() =>
                    {
                        (Activity as MainActivity).HideSoftKeyboard();

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
                                    SnackbarHelper.Success("Na podany adres email zosta³y wys³ane kolejne instrukcje przywrócenia has³a.", view);

                                }
                                else
                                {
                                    SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                                }
                            });
                            return true;
                        }, null, response =>
                        {
                            btnRecover.Enabled = true;

                            Activity.RunOnUiThread(() =>
                            {
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
            var emailInput = View.FindViewById<TextInputLayout>(Resource.Id.input_recovery_password);
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
                (Activity as BaseActivity).HideSoftKeyboard();
            }
        }

        private void EmailInput_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e, TextInputLayout wrapper)
        {
            InputErrorHelper.EmailValidateFunc(wrapper);
        }
    }
}