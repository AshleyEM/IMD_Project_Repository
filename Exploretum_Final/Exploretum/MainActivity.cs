// Ashley McLemore
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System;
using Android.Views;

namespace Exploretum // FIRST SCREEN USER SEES
{
    [Activity(Label = "Exploretum", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
			Console.WriteLine(">>>>>>>>> MAIN ACTIVITY");
            RequestWindowFeature(WindowFeatures.NoTitle);  
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            
            string json;
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            JObject obj = JObject.Parse(json);
            string first_s_type = (string)obj.SelectToken("Game.Quests[0].stops[0].stoptype");

	        ImageButton startMoth = FindViewById<ImageButton>(Resource.Id.Moth);
	        ImageButton startPine = FindViewById<ImageButton>(Resource.Id.Pine);
            ImageButton startHummingbird = FindViewById<ImageButton>(Resource.Id.Hummingbird);
			ImageButton about = FindViewById<ImageButton>(Resource.Id.logo); 
            about.Click += delegate {
                var intent = new Intent(this, typeof(AboutActivity));
			    StartActivity(intent);
			};
            startMoth.Click += delegate
            { 
                if (first_s_type == "story"){
                    var intentStart = new Intent(this, typeof(StoryActivity));
                    intentStart.PutExtra("questNumber", (int)0);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(0));
                    StartActivity(intentStart);
                }else if (first_s_type == "checkpoint"){
                    var intentStart = new Intent(this, typeof(CheckpointActivity));
                    intentStart.PutExtra("questNumber", (int)0);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(0));
                    StartActivity(intentStart);
                }
            };
            startPine.Click += delegate
            { 
                if (first_s_type == "story"){
                    var intentStart = new Intent(this, typeof(StoryActivity));
                    intentStart.PutExtra("questNumber", (int)1);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(1));
                    StartActivity(intentStart);
                }else if (first_s_type == "checkpoint"){
                    var intentStart = new Intent(this, typeof(CheckpointActivity));
                    intentStart.PutExtra("questNumber", (int)1);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(1));
                    StartActivity(intentStart);
                }
            };
            startHummingbird.Click += delegate
            { 
                if (first_s_type == "story"){
                    var intentStart = new Intent(this, typeof(StoryActivity));
                    intentStart.PutExtra("questNumber", (int)2);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(2));
                    StartActivity(intentStart);
                }else if (first_s_type == "checkpoint"){
                    var intentStart = new Intent(this, typeof(CheckpointActivity));
                    intentStart.PutExtra("questNumber", (int)2);
                    intentStart.PutExtra("stopNumber", (int)SavedState.getStateOf(2));
                    StartActivity(intentStart);
                }
            };
            
           
        }

  
    }
}

