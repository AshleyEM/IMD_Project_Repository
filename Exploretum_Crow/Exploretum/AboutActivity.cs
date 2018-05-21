
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Exploretum
{
    [Activity(Label = "AboutActivity")]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.About);

            Button back = FindViewById<Button>(Resource.Id.back);
            Button clear = FindViewById<Button>(Resource.Id.clear);
            back.Click += delegate {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("questNumber", (int)0);
                intent.PutExtra("stopNumber", (int)SavedState.getStateOf(0));
                StartActivity(intent);                 
            };
            clear.Click += delegate {
                for (int i = 0; i < 3; i++){
                    SavedState.setStateOf(i, 0);  
                }

            };
        }
    }
}
