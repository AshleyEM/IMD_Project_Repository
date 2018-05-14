
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
        string json;
        JObject obj;
        const long ONE_SECOND = 500;
        const int RequestLocationId = 0;
        LocationManager locationManager;
		ImageButton hintButton;
		ImageButton factButton;
		Button foundButton;
		Button debugNextButton;
        TextView longitude;
        TextView latitude;
        TextView distanceText;
        ImageView scope;
        ImageView spriteImgView;
        ImageView backgroundImgView;
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
            Resource.Drawable.hotcold11,
            Resource.Drawable.hotcold12,
            Resource.Drawable.hotcold13,
            Resource.Drawable.hotcold14,
            Resource.Drawable.hotcold15
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

            RequestPermissions(PermissionsLocation, RequestLocationId);

            // read from JSON data file
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            obj = JObject.Parse(json);
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
                string plant_name = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].plant"); 
                string common_name = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].common");     
                string bgImgName = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].backgroundImg"); 
                string spriteImgName = (string)obj.SelectToken("Game.Quests["+questNumber+"].spriteImg");   
                string factText = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].fact");  
                string hintText = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].hint");
                string next_stop_type = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+(stopNumber+1)+"].stoptype");

				// hide 'PLANT FOUND!' button
				foundButton = FindViewById<Button>(Resource.Id.found);
				foundButton.Visibility = ViewStates.Invisible;

				debugNextButton = FindViewById<Button>(Resource.Id.next);
                debugNextButton.Click += delegate {
				
                    Intent Debugintent;
                    // if reached the end of all stops, increment quest number and reset stop number
                    if (next_stop_type != "story" && next_stop_type != "checkpoint"){
                        questNumber = questNumber + 1;
                        stopNumber = 0; 
                        next_stop_type = (string)obj.SelectToken("Game.Quests["+questNumber+"].stops["+stopNumber+"].stoptype");
                        stopNumber = -1; // becase the code down below will increment 0 + 1, and will skip the first [0] stop of next quest
                    }
                    if (next_stop_type == "story")
                    {
                        Debugintent = new Intent(this, typeof(StoryActivity));
                        Debugintent.PutExtra("questNumber", questNumber);
                        Debugintent.PutExtra("stopNumber", stopNumber + 1);
                        StartActivity(Debugintent);
                    }
                    else if (next_stop_type == "checkpoint")
                    {
                        Debugintent = new Intent(this, typeof(CheckpointActivity));
                        Debugintent.PutExtra("questNumber", questNumber);
                        Debugintent.PutExtra("stopNumber", stopNumber + 1);
                        StartActivity(Debugintent);
                    }
				};
                // Show sprite based on quest number
                spriteImgView = FindViewById<ImageView>(Resource.Id.sprite);
                int spriteImgId = Resources.GetIdentifier(spriteImgName, "drawable", PackageName);
                spriteImgView.SetImageResource(spriteImgId);
                backgroundImgView = FindViewById<ImageView>(Resource.Id.background);
                int bgImgId = Resources.GetIdentifier(bgImgName, "drawable", PackageName);
                backgroundImgView.SetImageResource(bgImgId);

                ImageButton homeButton = FindViewById<ImageButton>(Resource.Id.home);
                homeButton.Click += delegate {
                    var intent = new Intent(this, typeof(MainActivity));
					StartActivity(intent);
				};

				// display info/hint on button click
				hintButton = FindViewById<ImageButton>(Resource.Id.hint);
                factButton = FindViewById<ImageButton>(Resource.Id.fact);
                hintButton.Click += delegate {
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					AlertDialog popup = alert.Create();
					popup.SetTitle("Hint");
					popup.SetMessage(hintText);
					popup.SetButton("Ok", (c, ev) => { });
					popup.Show();
				};   
                factButton.Click += delegate {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    AlertDialog popup = alert.Create();
                    popup.SetTitle("Fact");
                    popup.SetMessage(factText);
                    popup.SetButton("Ok", (c, ev) => { });
                    popup.Show();
                };  
                    
                // display lat/long
                latitude = FindViewById<TextView>(Resource.Id.latitude);
                longitude = FindViewById<TextView>(Resource.Id.longitude);              

                // get location manager
                locationManager = (LocationManager)GetSystemService(LocationService);
                
                latitude.Text = "Loading GPS...";
                longitude.Text = " ";
                //display spectrescope as 'cold' until GPS coordinates are loaded
                scope = FindViewById<ImageView>(Resource.Id.scope);
                scope.SetImageResource(scope_frames[scope_frames.Length-1]);

                // (debugging) see distance between quest location/current location
                distanceText = FindViewById<TextView>(Resource.Id.distance);

                // Go to next stop
                foundButton.Click += delegate
                {   
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
            using (StreamReader r = new StreamReader(Assets.Open("GameData.json")))
            {
                json = r.ReadToEnd();
            }
            obj = JObject.Parse(json);

            latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);

            double currentLat = location.Latitude;
            double currentLng = location.Longitude;
            float[] distancetmp = new float[1];
            double lat = (double)obj.SelectToken("Game.Quests[" + questNumber + "].stops[ "+ stopNumber+" ].lat");
            double lng = (double)obj.SelectToken("Game.Quests[" + questNumber + "].stops[ " + stopNumber + " ].long");
            Location.DistanceBetween(currentLat, currentLng, lat, lng, distancetmp);
            double distance = Math.Round(distancetmp[0],2);

            // display distance (double) from objective
            distanceText.Text = "Distance: " + distance.ToString() + " meters";

            int distanceRounded = Convert.ToInt32(distance); // round current distance 
            const int RANGE = 8; // How spread out distances are (smaller number = more frames). Multiplied by index of spectrescope frames.  
            int indexOfFrameToShow = distanceRounded / RANGE;
            //                          {img1,  img2,   img3 ... }    
            //                   HOT     0m      8m      16m     24m     32m   COLD
            //                          0*8     1*8     2*8     3*8     4*8 
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
			// if close to coordinates, reveal PLANT FOUND! button
			if (distance <= 20)
			{
				foundButton.Visibility = ViewStates.Visible;
			}
			else
				foundButton.Visibility = ViewStates.Invisible;    
            
            
            
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
