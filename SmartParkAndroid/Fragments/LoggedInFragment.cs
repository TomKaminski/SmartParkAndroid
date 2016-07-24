using System;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using SmartParkAndroid.Core;
using SmartParkAndroid.Core.Helpers;

namespace SmartParkAndroid.Fragments
{
    public class LoggedInFragment : BaseFragment
    {
        static bool _isDelayedForOneSecond;

        private static Button _gateButton;
        private readonly Handler _handler = new Handler();
        private CustomViewPager _viewPager;

        private bool _chargesRefreshing;

        private readonly Runnable _longPressedRunnable = new Runnable(LongRunnableFunction);

        public static void LongRunnableFunction()
        {
            StartTransition(350, _gateButton);
            _gateButton.SetTextColor(Color.White);
            _gateButton.Text = "Zwolnij przycisk";
            _isDelayedForOneSecond = true;
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

            _gateButton = view.FindViewById<Button>(Resource.Id.open_gate_btn);
            _gateButton.Touch += (sender, e) =>
            {
                BtnOpenGate_Touch(sender, e, view);
            };

            chargesTextView.Click += (sender, e) =>
            {
                OnClickRefreshCharges(sender, e, view);
            };

            return view;
        }

        public override void OnInit()
        {
        }

        private static void StartTransition(int millis, View v)
        {
            if (v.Background is TransitionDrawable)
            {
                var d = (TransitionDrawable)v.Background;
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
                    _handler.PostDelayed(_longPressedRunnable, 1000);
                    break;

                case MotionEventActions.Up:
                    _viewPager.SetSwipePagingEnabled(true);
                    _handler.RemoveCallbacks(_longPressedRunnable);
                    button.SetTextColor(Resources.GetColor(Resource.Color.main_blue));

                    if (_isDelayedForOneSecond)
                    {
                        ReverseTransition(350, _gateButton);
                        OnClickOpenGate(button, view);
                        _isDelayedForOneSecond = false;
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

        private async void OnClickRefreshCharges(object sender, System.EventArgs e, View view)
        {
            if (!_chargesRefreshing)
            {
                _chargesRefreshing = true;
                Activity.RunOnUiThread(() =>
                {
                    (Activity as MainActivity).ShowProgressBar();
                });
                var smartHttpClient = new SmartParkHttpClient();
                await smartHttpClient.Post<SmartJsonResult<int?>, object>(new Uri("https://smartparkath.azurewebsites.net/api/Parking/RefreshCharges", UriKind.Absolute),
                    new
                    {
                        Email = StaticManager.UserName,
                    }, response =>
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            _chargesRefreshing = false;
                            (Activity as MainActivity).HideProgressBar();
                            if (response.IsValid)
                            {
                                StaticManager.Charges = response.Result.Value;
                                var mainActiviy = (MainActivity)Activity;
                                var prefs = mainActiviy.GetPreferences(FileCreationMode.Private);
                                var editor = prefs.Edit();
                                editor.PutInt("charges", response.Result.Value);
                                editor.Commit();

                                view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();
                                SnackbarHelper.Success("Wyjazdy zosta³y odœwie¿one", view);
                            }
                            else
                            {
                                SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                            }
                        });
                        return true;
                    }, null, response =>
                    {
                        _chargesRefreshing = false;

                        Activity.RunOnUiThread(() =>
                        {
                            (Activity as MainActivity).HideProgressBar();
                            SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                        });
                        return true;
                    });
            }
        }

        private async void OnClickOpenGate(Button button, View view)
        {
            if (StaticManager.Charges > 0)
            {
                button.Text = "Chwileczkê...";
                Activity.RunOnUiThread(() =>
                {
                    button.Enabled = false;
                    (Activity as MainActivity).ShowProgressBar();
                });
                var smartHttpClient = new SmartParkHttpClient();
                await smartHttpClient.Post<SmartJsonResult<int>, object>(new Uri("https://smartparkath.azurewebsites.net/api/Parking/OpenGate", UriKind.Absolute),
                    new
                    {
                        Email = StaticManager.UserName,
                    }, response =>
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            (Activity as MainActivity).HideProgressBar();
                            OpenGateAfter(response, view, button);
                        });
                        return true;
                    }, null, response =>
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            (Activity as MainActivity).HideProgressBar();
                            SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                        });
                        return true;
                    });
            }
            else
            {
                SnackbarHelper.Error("Brak wyjazdów, spróbuj odœwie¿yæ liczbê wyjazdów.", view);
            }
        }

        private void OpenGateAfter(SmartJsonResult<int> response, View view, Button button)
        {
            if (response.IsValid)
            {
                StaticManager.Charges = response.Result;
                var mainActiviy = (MainActivity)Activity;

                var prefs = mainActiviy.GetPreferences(FileCreationMode.Private);
                var editor = prefs.Edit();
                editor.PutInt("charges", response.Result);
                editor.Commit();

                view.FindViewById<TextView>(Resource.Id.charges_num).Text = StaticManager.Charges.ToString();
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
                SnackbarHelper.Success("Brama otwierana, mi³ego dnia!", view);
                SetChargesColor(view);
            }
            else
            {
                Activity.RunOnUiThread(() =>
                {
                    button.Enabled = true;
                    button.Text = Resources.GetString(Resource.String.open_gate_text);
                });
                SnackbarHelper.Error(response.ValidationErrors.FirstOrDefault(), view);
                SetChargesColor(view);
            }
        }
    }
}