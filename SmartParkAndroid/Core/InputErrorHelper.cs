using System;
using System.Net.Mail;
using Android.Support.Design.Widget;

namespace SmartParkAndroid.Core
{
    public static class InputErrorHelper
    {
        public static bool SetError(string error, TextInputLayout inputLayout)
        {
            inputLayout.Error = error;
            return string.IsNullOrEmpty(error);
        }

        public static bool EmailValidateFunc(TextInputLayout textView)
        {
            if (string.IsNullOrEmpty(textView.EditText.Text))
            {
                return SetError("Adres email jest wymagany", textView);
            }
            try
            {
                new MailAddress(textView.EditText.Text);
                return SetError(string.Empty, textView);
            }
            catch (FormatException)
            {
                return SetError("Podany adres email jest niepoprawny", textView);
            }
        }

        public static bool PasswordValidationFunc(TextInputLayout textView)
        {
            return SetError(string.IsNullOrEmpty(textView.EditText.Text)
                ? "Has³o jest wymagane"
                : string.Empty, textView);
        }
    }
}