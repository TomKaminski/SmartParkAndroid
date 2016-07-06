using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Test.Suitebuilder.Annotation;
using Android.Views;
using Java.Lang;
using SmartParkAndroid.Fragments;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SmartParkAndroid
{
    [Activity(Label = "SmartParkAndroid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SmartParkTheme")]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout _drawerLayout;
        private ViewPager _viewPager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
                //HOME!
                navigationView.Menu.GetItem(0).SetChecked(true);
                SetUpDrawerContent(navigationView);
            }

            _viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(_viewPager, navigationView);
        }

        private void SetUpViewPager(ViewPager viewPager, NavigationView navView)
        {
            var tabAdapter = new TabAdapter(SupportFragmentManager);
            tabAdapter.AddFragment(new LoginFragment(), "Login Fragment", Resource.Id.nav_home);
            tabAdapter.AddFragment(new TestFragment(), "Test Fragment", Resource.Id.nav_messages);

            viewPager.Adapter = tabAdapter;
            viewPager.PageSelected += (sender, ev) =>
            {
                var menuItemList = Resources.GetStringArray(Resource.Array.not_logged_in_menu_string);
                navView.Menu.GetItem(ev.Position).SetChecked(true);
                SupportActionBar.Title = menuItemList[ev.Position];
            };
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
                case Resource.Id.action_web:
                    var uri = Uri.Parse("http://smartparkath.azurewebsites.net");
                    var intent = new Intent(Intent.ActionView);
                    intent.SetData(uri);
                    var chooser = Intent.CreateChooser(intent, "Open with");
                    StartActivity(chooser);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                int drawerPositon;
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_messages:
                        {
                            drawerPositon = 1;
                            break;
                        }
                    case Resource.Id.nav_home:
                    default:
                        {
                            drawerPositon = 0;
                            break;
                        }
                }
                var menuItemList = Resources.GetStringArray(Resource.Array.not_logged_in_menu_string);
                SupportActionBar.Title = menuItemList[drawerPositon];
                _viewPager.SetCurrentItem(drawerPositon, false);
                _drawerLayout.CloseDrawers();
            };
        }

        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name, int id)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }

            public override int Count => Fragments.Count;

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentNames[position]);
            }

        }
    }
}

