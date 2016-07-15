using System.Runtime.InteropServices;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using SmartParkAndroid.Core;
using SmartParkAndroid.Core.Helpers;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SmartParkAndroid.Fragments
{
    public class LoggedInFragment : SupportFragment
    {
        const int secondsToElapse = 5;
        static bool isDelayedForTwoSeconds = false;

        private static Button gateButton;
        private Handler _handler = new Handler();
        private CustomViewPager _viewPager;

        private Runnable _longPressedRunnable = new Runnable(LongRunnableFunction);

        public static void LongRunnableFunction()
        {
            StartTransition(500, gateButton);
            gateButton.SetTextColor(Color.White);
            gateButton.Text = "Zwolnij przycisk";
            isDelayedForTwoSeconds = true;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.logged_in_fragment, container, false);

            view.FindViewById<TextView>(Resource.Id.logged_as_email).Text = StaticManager.UserName;
            var chargesTextView = view.FindViewById<TextView>(Resource.Id.charges_num);
            chargesTextView.Text = StaticManager.Charges.ToString();
            _viewPager = Activity.FindViewById<CustomViewPager>(Resource.Id.viewpager);

            var chargesArrowAnimation = AnimationUtils.LoadAnimation(this.Context, Resource.Animation.slide_down_fade_out);
            view.FindViewById<ImageView>(Resource.Id.img_refresh_charges_arrows).StartAnimation(chargesArrowAnimation);

            SetChargesColor(view);

            gateButton = view.FindViewById<Button>(Resource.Id.open_gate_btn);
            gateButton.Touch += (sender, e) =>
            {
                BtnOpenGate_Touch(sender, e, view);
            };

            chargesTextView.Click += (sender, e) =>
            {
                OnClickRefreshCharges(sender, e, view);
            };

            return view;
        }

        private static void StartTransition(int millis, View v)
        {
            if (v.Background is TransitionDrawable)
            {
                var d = (TransitionDrawable) v.Background;
                d.StartTransition(millis);
            }
        }

        private static void ReverseTransition(int millis, View v)
        {
            if (v.Background is TransitionDrawable)
            {
                var d = (TransitionDrawable)v.Background;
                d.ReverseTransition(millis);
            }
        }

        private void BtnOpenGate_Touch(object sender, View.TouchEventArgs e, View view)
        {
            var button = (Button)sender;

            switch (e.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    _viewPager.SetSwipePagingEnabled(false);
                    _handler.PostDelayed(_longPressedRunnable, 2000);
                    break;
                
                case MotionEventActions.Up:
                    _viewPager.SetSwipePagingEnabled(true);
                    _handler.RemoveCallbacks(_longPressedRunnable);
                    button.SetTextColor(Resources.GetColor(Resource.Color.main_blue));
                    button.Text = Resources.GetString(Resource.String.open_gate_text);

                    if (isDelayedForTwoSeconds)
                    {
                        ReverseTransition(500, gateButton);
                        OnClickOpenGate(button, view);
                        isDelayedForTwoSeconds = false;
                    }

                    break;
            }


        }

        private void SetChargesColor(View view)
        {
            var textView = view.FindViewById<TextView>(Resource.Id.charges_num);
            if (StaticManager.Charges <= 5)
            {
                textView.SetTextColor(Resources.GetColor(Resource.Color.main_red));
            }
            else if (StaticManager.Charges <= 10)
            {
                textView.SetTextColor(Resources.GetColor(Resource.Color.main_orange));
            }
        }



        private void OnClickRefreshCharges(object sender, System.EventArgs e, View view)
        {
            StaticManager.Charges = 50;
            view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();
            SnackbarHelper.ShowSnackbar("Wyjazdy zosta³y odœwie¿one", view, false, true);
        }



        private void OnClickOpenGate(Button button, View view)
        {

            if (StaticManager.Charges > 0)
            {
                StaticManager.Charges--;
                view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();

                button.Enabled = false;

                const int toElapse = 5;
                int counter = 5;
                button.Text = $"{toElapse}...";

                var timerHelper = new TimerActionHelper();
                timerHelper.Do(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        button.Enabled = true;
                        button.Text = Resources.GetString(Resource.String.open_gate_text);
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
                SetChargesColor(view);
            }
            else
            {
                SnackbarHelper.ShowSnackbar("Brak wyjazdów, spróbuj odœwie¿yæ liczbê wyjazdów.", view, true, true);
            }
        }
    }
}