using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using SmartParkAndroid.Core;
using SmartParkAndroid.Fragments;
using UK.CO.Chrisjenx.Calligraphy;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;

namespace SmartParkAndroid
{
    [Activity(Label = "SmartParkAndroid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SmartParkTheme")]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout _drawerLayout;
        private CustomViewPager _viewPager;

        protected override void OnCreate(Bundle bundle)
        {
            RequestedOrientation = ScreenOrientation.Portrait;

            base.OnCreate(bundle);

            InitUserContext();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolbar);

            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.Title = "Home";

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
                if (StaticManager.LoggedIn)
                {
                    SetUpNavHeaderLoggedInContent(navigationView);
                }
                else
                {
                    SetUpNavHeaderContent(navigationView);
                }

            }

            _viewPager = FindViewById<CustomViewPager>(Resource.Id.viewpager);

            SetUpViewPager(_viewPager, navigationView);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            fab.Click += OnClickNavHeaderBtn;
        }

        protected override void AttachBaseContext(Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        private void RemoveNavHeaders(NavigationView view)
        {
            var headersInNav = view.HeaderCount;
            for (int i = 0; i < headersInNav; i++)
            {
                view.RemoveHeaderView(view.GetHeaderView(i));
            }
        }

        private void SetUpNavHeaderContent(NavigationView view)
        {
            RemoveNavHeaders(view);
            view.InflateHeaderView(Resource.Layout.nav_header);

            var sb = new SpannableStringBuilder(Resources.GetString(Resource.String.nav_header_not_logged_in_text));
            var boldSpan = new StyleSpan(TypefaceStyle.Bold);
            sb.SetSpan(boldSpan, 0, 11, SpanTypes.InclusiveInclusive);
            var normalSpan = new StyleSpan(TypefaceStyle.Normal);
            sb.SetSpan(normalSpan, 12, Resources.GetString(Resource.String.nav_header_not_logged_in_text).Length - 1, SpanTypes.InclusiveInclusive);
            
            var navHeaderView = view.GetHeaderView(0);
            var navHeaderTextView = navHeaderView.FindViewById<TextView>(Resource.Id.nav_header_text_view);

            navHeaderTextView.TextFormatted = sb;
        }

        private void SetUpNavHeaderLoggedInContent(NavigationView view)
        {
            RemoveNavHeaders(view);
            view.InflateHeaderView(Resource.Layout.nav_header_logged);
            var navHeaderView = view.GetHeaderView(0);

            var emailTextView = navHeaderView.FindViewById<TextView>(Resource.Id.nav_header_email_logged_in);
            emailTextView.Text = StaticManager.UserName;

            //var circleImgView = navHeaderView.FindViewById<CircularImageView>(Resource.Id.circle_photo_image);
            //circleImgView.SetImageURI(Uri.Parse("http://smartparkath.azurewebsites.net/images/user-avatars/6d560766-0073-452f-b492-df13d17a0f2a.jpg"));
        }
        
        private void OnClickNavHeaderBtn(object sender, System.EventArgs e)
        {
            var uri = Uri.Parse("http://smartparkath.azurewebsites.net/Portal");
            var intent = new Intent(Intent.ActionView);
            intent.SetData(uri);
            var chooser = Intent.CreateChooser(intent, "Open with");
            StartActivity(chooser);
        }

        private void SetUpViewPager(CustomViewPager viewPager, NavigationView navView)
        {
            var tabAdapter = new TabAdapter(SupportFragmentManager);
            if (StaticManager.LoggedIn)
            {
                tabAdapter.AddFragment(new LoggedInFragment(), "Logged In Fragment");
                tabAdapter.AddFragment(new SettingsFragment(), "Settings Fragment");
                tabAdapter.AddFragment(new AboutFragment(), "About Fragment");

                viewPager.Adapter = tabAdapter;
                viewPager.PageSelected += (sender, ev) =>
                {
                    OnViewPagerPageSelected(navView, ev);
                };
            }
            else
            {
                tabAdapter.AddFragment(new LoginFragment(), "Login Fragment");
                tabAdapter.AddFragment(new RecoverPasswordFragment(), "Recover Password Fragment");
                tabAdapter.AddFragment(new AboutFragment(), "About Fragment");

                viewPager.Adapter = tabAdapter;
                viewPager.PageSelected += (sender, ev) =>
                {
                    OnViewPagerPageSelected(navView, ev);
                };
            }

        }

        private void OnViewPagerPageSelected(NavigationView navView, ViewPager.PageSelectedEventArgs ev)
        {
            var menuItemList = StaticManager.LoggedIn
                ? Resources.GetStringArray(Resource.Array.logged_in_menu_string)
                : Resources.GetStringArray(Resource.Array.not_logged_in_menu_string);
            MenuItemsProvider.UncheckItems(navView.Menu);
            navView.Menu.GetItem(ev.Position).SetChecked(true);
            SupportActionBar.Title = menuItemList[ev.Position];
        }

        public void LogIn()
        {
            var navView = FindViewById<NavigationView>(Resource.Id.nav_view);
            SetUpViewPager(_viewPager, navView);
            MenuItemsProvider.GetLoggedInMenuItems(navView.Menu, Resources.GetStringArray(Resource.Array.logged_in_menu_string));
            navView.Menu.GetItem(0).SetChecked(true);
            SetUpNavHeaderLoggedInContent(navView);

        }

        public void Logout()
        {
            LogoutUser();
            var navView = FindViewById<NavigationView>(Resource.Id.nav_view);
            SetUpViewPager(_viewPager, navView);
            MenuItemsProvider.GetNotLoggedInMenuItems(navView.Menu, Resources.GetStringArray(Resource.Array.not_logged_in_menu_string));
            navView.Menu.GetItem(0).SetChecked(true);
            SetUpNavHeaderContent(navView);

        }

        private void LogoutUser()
        {
            StaticManager.LoggedIn = false;
            StaticManager.UserHash = null;
            StaticManager.UserName = null;

            var preferences = GetPreferences(FileCreationMode.Private);
            var prefEditor = preferences.Edit();
            prefEditor.Remove("username");
            prefEditor.Remove("userhash");
            prefEditor.Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.right_action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void InitUserContext()
        {
            var preferences = GetPreferences(FileCreationMode.Private);
            var username = preferences.GetString("username", null);
            var userHash = preferences.GetString("userhash", null);
            var userCharges = preferences.GetInt("charges", 0);

            if (username != null && userHash != null)
            {
                StaticManager.InitBase(true, username, userHash, userCharges);
            }
            else
            {
                StaticManager.InitBase(false, username, userHash, userCharges);
            }
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            if (StaticManager.LoggedIn)
            {
                MenuItemsProvider.GetLoggedInMenuItems(navigationView.Menu, Resources.GetStringArray(Resource.Array.logged_in_menu_string));
            }
            else
            {
                MenuItemsProvider.GetNotLoggedInMenuItems(navigationView.Menu, Resources.GetStringArray(Resource.Array.not_logged_in_menu_string));
            }

            MenuItemsProvider.UncheckItems(navigationView.Menu);
            navigationView.Menu.GetItem(0).SetChecked(true);

            navigationView.NavigationItemSelected += (sender, e) =>
            {
                MenuItemsProvider.UncheckItems(navigationView.Menu);
                e.MenuItem.SetChecked(true);
                int drawerPositon;
                switch (e.MenuItem.ItemId)
                {

                    case (int)MenuItems.About:
                        {
                            drawerPositon = 2;
                            break;
                        }
                    case (int)MenuItems.Logout:
                        {
                            StaticManager.LoggedIn = false;
                            Logout();
                            drawerPositon = 0;
                            break;
                        }
                    case (int)MenuItems.RecoverPassword:
                    case (int)MenuItems.Settings:
                        {
                            drawerPositon = 1;
                            break;
                        }
                    case (int)MenuItems.Home:
                    default:
                        {
                            drawerPositon = 0;
                            break;
                        }
                }
                var menuItemList = StaticManager.LoggedIn
                    ? Resources.GetStringArray(Resource.Array.logged_in_menu_string)
                    : Resources.GetStringArray(Resource.Array.not_logged_in_menu_string);

                SupportActionBar.Title = menuItemList[drawerPositon];
                _viewPager.SetCurrentItem(drawerPositon, false);
                _drawerLayout.CloseDrawers();
            };
        }
    }
}

