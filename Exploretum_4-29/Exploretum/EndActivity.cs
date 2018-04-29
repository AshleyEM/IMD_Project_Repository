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
    [Activity(Label = "EndActivity")]
    public class EndActivity : Activity // END SCREEN
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.End);

            Button buttonMain = FindViewById<Button>(Resource.Id.buttonMain);

            buttonMain.Click += delegate {
                var intentMain = new Intent(this, typeof(MainActivity));
                StartActivity(intentMain);
            };
        }
    }
}
