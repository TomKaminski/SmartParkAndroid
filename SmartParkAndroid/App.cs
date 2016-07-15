using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using UK.CO.Chrisjenx.Calligraphy;

namespace SmartParkAndroid
{
    [Application]
    public class App : Application
    {
        public App(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/titil_reg.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());
        }
    }
}