using System;
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SmartParkAndroid.Core;
using SmartParkAndroid.Core.Helpers;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SmartParkAndroid.Fragments
{
    public class LoggedInFragment : SupportFragment
    {
        const int secondsToElapse = 5;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.logged_in_fragment, container, false);

            view.FindViewById<TextView>(Resource.Id.logged_as).Text = StaticManager.UserName;
            view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();

            var btnOpenGate = view.FindViewById<Button>(Resource.Id.open_gate_btn);
            btnOpenGate.Click += (sender, e) =>
            {
                OnClickOpenGate(sender, e, view);
            };

            var btnRefreshCharges = view.FindViewById<Button>(Resource.Id.btnRefresh);
            btnRefreshCharges.Click += (sender, e) =>
            {
                OnClickRefreshCharges(sender, e, view);
            };

            return view;
        }

        private void OnClickRefreshCharges(object sender, System.EventArgs e, View view)
        {
            StaticManager.Charges = 50;
            view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();
            SnackbarHelper.ShowSnackbar("Wyjazdy zosta³y odœwie¿one", view, false, true);
        }

        private void OnClickOpenGate(object sender, System.EventArgs e, View view)
        {
            if (StaticManager.Charges > 0)
            {
                StaticManager.Charges--;
                view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();

                var button = sender as Button;
                button.Enabled = false;
                button.SetBackgroundColor(Color.Rgb(132, 218, 251));

                const int toElapse = 5;
                int counter = 5;
                button.Text = $"{toElapse}...";

                var timerHelper = new TimerActionHelper();
                timerHelper.Do(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        button.Enabled = true;
                        button.SetBackgroundColor(Color.Rgb(0, 173, 239));
                        button.Text = "Otwórz bramê";
                    });
                }, count =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        counter--;
                        button.Text = $"{counter}...";
                    });
                    return true;
                }, toElapse);
                SnackbarHelper.ShowSnackbar("Brama otwierana, mi³ego dnia!", view, false, true);
            }
            else
            {
                SnackbarHelper.ShowSnackbar("Brak wyjazdów, spróbuj odœwie¿yæ liczbê wyjazdów.", view, true, true);
            }
        }
    }
}