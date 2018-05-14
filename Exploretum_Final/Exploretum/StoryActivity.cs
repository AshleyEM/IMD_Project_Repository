using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace Exploretum
{
    [Activity(Label = "Story")]
    public class StoryActivity : Activity 
    {
        ImageView backgroundImgView;
        ImageView spriteImgView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);		
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Story);
            int questNumber = Intent.GetIntExtra("questNumber", 0);
            int stopNumber = Intent.GetIntExtra("stopNumber", 0);

            // turn json file into string
            string json;
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            JObject obj = JObject.Parse(json);
            int questsLength = (int)obj.SelectToken("Game.Quests").Count() - 1;
            int stopsLength = (int)obj.SelectToken("Game.Quests["+questNumber+"].stops").Count() - 1; 

            // if end of quest list
            if (questNumber == 5) // if reached the end of all quests
            {
                // Go to end screen
                var intentEnd = new Intent(this, typeof(EndActivity));
                StartActivity(intentEnd);
            }
            else
            {
                // quest/stop info from JSON file
                string q_name = (string)obj.SelectToken("Game.Quests["+ questNumber +"].name");   
                string s_name = (string)obj.SelectToken("Game.Quests["+ questNumber +"].stops["+stopNumber+"].stopname");
                string s_type = (string)obj.SelectToken("Game.Quests["+ questNumber +"].stops["+stopNumber+"].stoptype");
                string narrative = (string)obj.SelectToken("Game.Quests["+ questNumber +"].stops["+stopNumber+"].narrative");
                string bgImgName = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].backgroundImg"); 
                string spriteImgName = (string)obj.SelectToken("Game.Quests["+questNumber+"].spriteImg");     

                // Show sprite based on quest number
                spriteImgView = FindViewById<ImageView>(Resource.Id.sprite);
                int spriteImgId = Resources.GetIdentifier(spriteImgName, "drawable", PackageName);
                spriteImgView.SetImageResource(spriteImgId);

                TextView narr = FindViewById<TextView>(Resource.Id.narrative);
                // background image
                backgroundImgView = FindViewById<ImageView>(Resource.Id.background);
                int bgImgId = Resources.GetIdentifier(bgImgName, "drawable", PackageName);
                backgroundImgView.SetImageResource(bgImgId);
				// narrative
                narr.Text = narrative;                    
            
                ImageButton buttonNext = FindViewById<ImageButton>(Resource.Id.next);
                ImageButton buttonHome = FindViewById<ImageButton>(Resource.Id.home);
                // MOVING TO NEXT STOP
                buttonNext.Click += delegate
                {   
                    string next_stop_type = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+(stopNumber+1)+"].stoptype"); 
                    Intent intent;
                    // if reached the end of all stops, increment quest number and reset stop number
                    if (next_stop_type != "story" && next_stop_type != "checkpoint"){
                        questNumber = questNumber + 1;
                        stopNumber = 0; 
                        next_stop_type = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].stoptype");
                        stopNumber = -1; // becase the code down below will increment 0 + 1, and will skip the first [0] stop of next quest
                    }
                    if (next_stop_type == "story")
                    {
                        intent = new Intent(this, typeof(StoryActivity));
                        intent.PutExtra("questNumber", questNumber);
                        intent.PutExtra("stopNumber", stopNumber + 1);
                        StartActivity(intent);
                    }
                    else if (next_stop_type == "checkpoint")
                    {
                        intent = new Intent(this, typeof(CheckpointActivity));
                        intent.PutExtra("questNumber", questNumber);
                        intent.PutExtra("stopNumber", stopNumber + 1);
                        StartActivity(intent);
                    }
                };
                buttonHome.Click += delegate {
                    Intent intentHome = new Intent(this, typeof(MainActivity));
                    StartActivity(intentHome);
                };
            }
        }
    }
}
