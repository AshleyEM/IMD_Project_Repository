﻿
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content.PM;
using Android.Locations;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Exploretum  // ALL QUEST SCREENS
{
    [Activity(Label = "Checkpoint")]

    public class CheckpointActivity : Activity, ILocationListener
    {
        int questNumber;
        int stopNumber;
        int questsLength;
        int stopsLength;
        const long ONE_SECOND = 500;
        const int RequestLocationId = 0;
        LocationManager locationManager;
        TextView longitude;
        TextView latitude;
        TextView provider;
        TextView distanceText;
        ImageView scope;
        ImageView spritePlaceholder;
        int[] scope_frames = { // hot-cold meter images
            Resource.Drawable.hotcold1,
            Resource.Drawable.hotcold2,
            Resource.Drawable.hotcold3,
            Resource.Drawable.hotcold4,
            Resource.Drawable.hotcold5,
            Resource.Drawable.hotcold6,
            Resource.Drawable.hotcold7,
            Resource.Drawable.hotcold8,
            Resource.Drawable.hotcold9,
            Resource.Drawable.hotcold10,
            Resource.Drawable.hotcold11
        };

        // permission to access course/fine location
        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Checkpoint);
            questNumber = Intent.GetIntExtra("questNumber", 0);
            stopNumber = Intent.GetIntExtra("stopNumber", 0);
            Console.WriteLine(">>>>>>>>>>> CHECKPOINT ACTIVITY");

            RequestPermissions(PermissionsLocation, RequestLocationId);

            // read from JSON data file
            string json;
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            JObject obj = JObject.Parse(json);
            questsLength = (int)obj.SelectToken("Game.Quests").Count() - 1;
            stopsLength = (int)obj.SelectToken("Game.Quests["+questNumber+"].stops").Count()-1; 

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
                string q_name = (string)obj.SelectToken("Game.Quests["+questNumber+"].name");
                string s_name = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].stopname");
                string s_type = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].stoptype");      

                TextView questName = FindViewById<TextView>(Resource.Id.questName);
                TextView stopName = FindViewById<TextView>(Resource.Id.stopName);
                questName.Text = q_name;
                stopName.Text = s_name + " " + s_type + "STOP NUMBER: "+stopNumber;
                // text views for coordinates
                latitude = FindViewById<TextView>(Resource.Id.latitude);
                longitude = FindViewById<TextView>(Resource.Id.longitude);
                provider = FindViewById<TextView>(Resource.Id.provider);
    
                // get location manager
                locationManager = (LocationManager)GetSystemService(LocationService);
                
                latitude.Text = "Getting GPS coordinates ...";
                longitude.Text = " ";
                provider.Text = " ";
                //display spectrescope as 'cold' until GPS coordinates are loaded
                scope = FindViewById<ImageView>(Resource.Id.scope);
                scope.SetImageResource(scope_frames[scope_frames.Length-1]);

                // (debugging) see distance between quest location/current location
                distanceText = FindViewById<TextView>(Resource.Id.distance);

                // MOVING TO NEXT STOP
                Button button = FindViewById<Button>(Resource.Id.button1);
                button.Click += delegate
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
            }

        }



        // ------GPS SAMPLE CODE (including comments)----
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                // If this is for my app and not someone else's (if this is my request ID):
                case RequestLocationId:
                    {
                        // If permission was granted to my app:
                        if (grantResults[0] == Permission.Granted)
                        {
                            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, ONE_SECOND, 1, this);
                        }
                        else
                            // If permission wasn't granted, then pop up a toast to say so
                            // *don't* try to request location updates (that would cause a security
                            // exception):
                            Toast.MakeText(this, "Location permission was denied!", ToastLength.Short).Show();
                    }
                    break;
            }

            // Call the superclass's method as well:
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
// -------------- GPS CONTROL ------------
        public void OnLocationChanged(Location location)
        {
            latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);

            double currentLat = location.Latitude;
            double currentLng = location.Longitude;
            float[] distancetmp = new float[1];
            double questLat = Convert.ToDouble(12222);
            double questLng = Convert.ToDouble(12222);
            Location.DistanceBetween(currentLat, currentLng, questLat, questLng, distancetmp);
            double distance = distancetmp[0];

            // display distance (double) from objective
            distanceText.Text = "Distance: " + distance.ToString() + " meters";

            // Show sprite based on quest number
            spritePlaceholder = FindViewById<ImageView>(Resource.Id.sprite);
            //int spriteImg = Quest.GetSpriteImg(questNumber);
            //spritePlaceholder.SetImageResource(spriteImg);

            int distanceRounded = Convert.ToInt32(distance); // round current distance 
            const int RANGE = 5; // How spread out distances are (smaller number = more frames). Multiplied by index of spectrescope frames.  
            int indexOfFrameToShow = distanceRounded / RANGE;
            //                          {img1,  img2,   img3 ... }    
            //                   HOT     0m      5m      10m     15m     20m   COLD
            //                          0*5     1*5     2*5     3*5     4*5 
            //                      index*RANGE                           

            // RANGE * index of image to show = current distance from objective  
            // index of image to show = current distance from objective / RANGE 
            // EXAMPLE: 
            //    current distance = 12
            //    current distance / RANGE = index of image to show 
            //    12 / 4 = index of image to show 
            //    = scope_frames[3] 
            scope = FindViewById<ImageView>(Resource.Id.scope);
            
            if(indexOfFrameToShow < scope_frames.Length){
                scope.SetImageResource(scope_frames[indexOfFrameToShow]);
            }else{
                // if distance is beyond the scope, just show the 'cold' (last) frame by default
                scope.SetImageResource(scope_frames[scope_frames.Length -1]);
            }
        

            // if distance is less than 4, start quest activity

            
            
            
        }
// -------------- /GPS CONTROL ------------




        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }
}