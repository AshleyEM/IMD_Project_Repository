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
    [Activity(Label = "Map")]
    public class MapActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);

            Button buttonBack = FindViewById<Button>(Resource.Id.buttonBack);

            buttonBack.Click += delegate {
                // START NEW ACTIVITY
                var intentMain = new Intent(this, typeof(MainActivity));
                StartActivity(intentMain);
            };
        }
    }
}