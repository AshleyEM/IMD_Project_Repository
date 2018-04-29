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

namespace Exploretum // FIRST SCREEN USER SEES
{
    [Activity(Label = "Tasks", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            
            string json;
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            JObject obj = JObject.Parse(json);
            string first_s_type = (string)obj.SelectToken("Game.Quests[0].stops[0].stoptype");      
            Button buttonStart = FindViewById<Button>(Resource.Id.buttonStart);
    
            buttonStart.Click += delegate
            {
                Console.WriteLine(">>>>>>>>>>> TYPE IS {0}", first_s_type);    
                if (first_s_type == "story"){
                    var intentStart = new Intent(this, typeof(StoryActivity));
                    intentStart.PutExtra("questNumber", (int)0);
                    intentStart.PutExtra("stopNumber", (int)0);
                    StartActivity(intentStart);
                }else if (first_s_type == "checkpoint"){
                    Console.WriteLine(">>>>>>>>>>> LAUNCHING {0}", first_s_type);
                    var intentStart = new Intent(this, typeof(CheckpointActivity));
                    intentStart.PutExtra("questNumber", (int)0);
                    intentStart.PutExtra("stopNumber", (int)0);
                    StartActivity(intentStart);
                }
            };
        }

  
    }
}

