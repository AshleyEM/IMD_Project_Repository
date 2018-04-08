// Ashley McLemore
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using System.Collections.Generic;

namespace Exploretum // FIRST SCREEN USER SEES
{
    [Activity(Label = "Tasks", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get button from the layout resource
            Button buttonStart = FindViewById<Button>(Resource.Id.buttonStart);

            buttonStart.Click += delegate {
                // Start Quest Activity
                var intentStartQuest = new Intent(this, typeof(QuestActivity));
                intentStartQuest.PutExtra("questNumber", (int)0); // passing variable to QuestActivity
                StartActivity(intentStartQuest); // passing variable to QuestActivity
            };
        }

  
    }
}

