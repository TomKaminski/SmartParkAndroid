﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using SmartParkAndroid.Core;
using SmartParkAndroid.Fragments;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Uri = Android.Net.Uri;

namespace SmartParkAndroid
{
    [Activity(Label = "SmartPark", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SmartParkTheme")]
    public class MainActivity : BaseActivity
    {
        private DrawerLayout _drawerLayout;
        private CustomViewPager _viewPager;
        private TabAdapter _tabAdapter;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetActivityScreenOrientation(ScreenOrientation.Portrait);
            InitUserContext();

            SetContentView(Resource.Layout.Main);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            SetSupportActionBar(FindViewById<SupportToolbar>(Resource.Id.toolBar));
            InitSupportActionBar();

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
                if (StaticManager.LoggedIn)
                {
                    SetUpNavHeaderLoggedInContent(navigationView, StaticManager.ImageBase64);
                }
                else
                {
                    SetUpNavHeaderNotLoggedInContent(navigationView);
                }
            }

            _viewPager = FindViewById<CustomViewPager>(Resource.Id.viewpager);

            SetUpViewPager(_viewPager, navigationView);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            fab.Click += OnClickNavHeaderBtn;

            _drawerLayout.DrawerOpened += _drawerLayout_DrawerOpened;
        }


        private void _drawerLayout_DrawerOpened(object sender, DrawerLayout.DrawerOpenedEventArgs e)
        {
            HideSoftKeyboard();
        }

        private void InitSupportActionBar()
        {
            var ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);
            SetSupportActionBarTitle("Home");
        }

        private void SetUpNavHeaderNotLoggedInContent(NavigationView view)
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

            navHeaderView.FindViewById<Button>(Resource.Id.header_register_btn).Click += OnClickNavHeaderBtn;
        }

        private void SetUpNavHeaderLoggedInContent(NavigationView view, string imageBase64)
        {
            RemoveNavHeaders(view);
            view.InflateHeaderView(Resource.Layout.nav_header_logged);
            var navHeaderView = view.GetHeaderView(0);

            var emailTextView = navHeaderView.FindViewById<TextView>(Resource.Id.nav_header_email_logged_in);
            emailTextView.Text = StaticManager.UserName;

            var circleImgView = navHeaderView.FindViewById<CircularImageView>(Resource.Id.circle_photo_image);

            circleImgView.SetImageFromBase64(imageBase64);
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
            _tabAdapter = new TabAdapter(SupportFragmentManager);
            if (StaticManager.LoggedIn)
            {
                _tabAdapter.AddFragment(new LoggedInFragment(), "Logged In Fragment");
                _tabAdapter.AddFragment(new SettingsFragment(), "Settings Fragment");
                _tabAdapter.AddFragment(new AboutFragment(), "About Fragment");
            }
            else
            {
                _tabAdapter.AddFragment(new LoginFragment(), "Login Fragment");
                _tabAdapter.AddFragment(new RecoverPasswordFragment(), "Recover Password Fragment");
                _tabAdapter.AddFragment(new AboutFragment(), "About Fragment");
            }
            viewPager.Adapter = _tabAdapter;
            viewPager.PageSelected += (sender, ev) =>
            {
                HideSoftKeyboard();
                OnViewPagerPageSelected(navView, ev);
                var baseFragment = _tabAdapter.GetItem(ev.Position) as BaseFragment;
                baseFragment?.OnInit();
            };
        }

        private void OnViewPagerPageSelected(NavigationView navView, ViewPager.PageSelectedEventArgs ev)
        {
            var menuItemList = StaticManager.LoggedIn
                ? Resources.GetStringArray(Resource.Array.logged_in_menu_string)
                : Resources.GetStringArray(Resource.Array.not_logged_in_menu_string);

            MenuItemsProvider.UncheckItems(navView.Menu);
            SetSideMenuItemChecked(navView, ev.Position);
            SetSupportActionBarTitle(menuItemList[ev.Position]);
        }

        public void LogIn()
        {
            var navView = FindViewById<NavigationView>(Resource.Id.nav_view);

            SetUpViewPager(_viewPager, navView);
            MenuItemsProvider.GetLoggedInMenuItems(navView.Menu, Resources.GetStringArray(Resource.Array.logged_in_menu_string));
            SetSideMenuItemChecked(navView, 0);
            SetUpNavHeaderLoggedInContent(navView, StaticManager.ImageBase64);

        }

        public void Logout()
        {
            LogoutUser();

            var navView = FindViewById<NavigationView>(Resource.Id.nav_view);

            SetUpViewPager(_viewPager, navView);
            MenuItemsProvider.GetNotLoggedInMenuItems(navView.Menu, Resources.GetStringArray(Resource.Array.not_logged_in_menu_string));
            SetSideMenuItemChecked(navView, 0);
            SetUpNavHeaderNotLoggedInContent(navView);
        }

        private void LogoutUser()
        {
            StaticManager.InitBase(false, null, null, 0, null);

            var preferences = GetPreferences(FileCreationMode.Private);
            var prefEditor = preferences.Edit();
            prefEditor.Remove("username");
            prefEditor.Remove("userhash");
            prefEditor.Remove("charges");
            prefEditor.Remove("imagebase64");

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
                    HideSoftKeyboard();
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
            var imageId = preferences.GetString("imagebase64", "avatar-placeholder.jpg");

            StaticManager.InitBase(username != null && userHash != null, username, userHash, userCharges, imageId);
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
            SetSideMenuItemChecked(navigationView, 0);

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

                HideSoftKeyboard();
                SetSupportActionBarTitle(menuItemList[drawerPositon]);
                _viewPager.SetCurrentItem(drawerPositon, false);

                var baseFragment = _tabAdapter.GetItem(drawerPositon) as BaseFragment;
                baseFragment?.OnInit();

                _drawerLayout.CloseDrawers();
            };
        }

        private void SetSideMenuItemChecked(NavigationView view, int itemNumber)
        {
            view.Menu.GetItem(itemNumber).SetChecked(true);
        }

        private void SetSupportActionBarTitle(string title)
        {
            SupportActionBar.Title = title;

        }
    }


}

