using System;
using Android.Content.Res;
using Android.Views;

namespace SmartParkAndroid.Core
{
    public class MenuItemsProvider
    {
        private const int GroupId = 0;
        public static void GetNotLoggedInMenuItems(IMenu menu, string[] menuTitles)
        {
            menu.Clear();

            var home = menu.Add(GroupId, Convert.ToInt32(MenuItems.Home), 0, menuTitles[0]);
            home.SetIcon(Resource.Drawable.ic_dashboard);
            
            var recoverPassword = menu.Add(GroupId, Convert.ToInt32(MenuItems.RecoverPassword), 0, menuTitles[1]);
            recoverPassword.SetIcon(Resource.Drawable.ic_headset);

            var about = menu.Add(GroupId, Convert.ToInt32(MenuItems.About), 0, menuTitles[2]);
            about.SetIcon(Resource.Drawable.ic_forum);
        }

        public static void GetLoggedInMenuItems(IMenu menu, string[] menuTitles)
        {
            menu.Clear();

            var home = menu.Add(GroupId, Convert.ToInt32(MenuItems.Home), 0, menuTitles[0]);
            home.SetIcon(Resource.Drawable.ic_dashboard);

            var settings = menu.Add(GroupId, Convert.ToInt32(MenuItems.Settings), 0, menuTitles[1]);
            settings.SetIcon(Resource.Drawable.ic_event);

            var about = menu.Add(GroupId, Convert.ToInt32(MenuItems.About), 0, menuTitles[2]);
            about.SetIcon(Resource.Drawable.ic_forum);

            var logout = menu.Add(GroupId, Convert.ToInt32(MenuItems.Logout), 0, menuTitles[3]);
            logout.SetIcon(Resource.Drawable.ic_discuss);
        }

        public static void UncheckItems(IMenu menu)
        {
            for (var i = 0; i < menu.Size(); i++)
            {
                menu.GetItem(i).SetChecked(false);
            }
        }
    }

    public enum MenuItems
    {
        Home = 11,
        Settings = 12,
        Logout = 13,
        About = 14,
        RecoverPassword = 15
    }
}