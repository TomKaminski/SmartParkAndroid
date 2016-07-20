using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SmartParkAndroid.Core
{
    public class TabAdapter : FragmentStatePagerAdapter
    {
        public List<SupportFragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public TabAdapter(SupportFragmentManager sfm) : base(sfm)
        {
            Fragments = new List<SupportFragment>();
            FragmentNames = new List<string>();
        }

        public void AddFragment(SupportFragment fragment, string name)
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
            return new String(FragmentNames[position]);
        }
    }
}