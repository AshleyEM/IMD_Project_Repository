﻿
using System;
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

namespace Exploretum  // ALL QUEST SCREENS
{
    [Activity(Label = "Quest")]

    public class QuestActivity : Activity, ILocationListener
    {
        int questNumber;
        const long ONE_SECOND = 500;
        const int RequestLocationId = 0;
        LocationManager locationManager;
        TextView longitude;
        TextView latitude;
        TextView provider;
        TextView distanceText;
        TextView hotCold;

        // permission to access course/fine location
        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Quest);

            // text views for coordinates
            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            provider = FindViewById<TextView>(Resource.Id.provider);

            // get location manager
            locationManager = (LocationManager)GetSystemService(LocationService);

            latitude.Text = "Getting initial GPS coordinates ...";
            longitude.Text = " ";
            provider.Text = " ";

            RequestPermissions(PermissionsLocation, RequestLocationId);


            // ---MY CODE----
            questNumber = Intent.GetIntExtra("questNumber", 0);
            // if end of quest list
            if (questNumber == Quest.QuestLimit() + 1)
            {
                // Go to end screen
                var intentEnd = new Intent(this, typeof(EndActivity));
                StartActivity(intentEnd);
            }
            else
            {
                LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
                layout.SetBackgroundResource(Resource.Drawable.pink_flowers);

                // display quest info 
                TextView questName = FindViewById<TextView>(Resource.Id.questName);
                questName.Text = Quest.GetQuestName(questNumber);
                TextView story = FindViewById<TextView>(Resource.Id.queststory);
                hotCold = FindViewById<TextView>(Resource.Id.hotcold);

                // display sprite relative to quest number
                ImageView spritePlaceholder = FindViewById<ImageView>(Resource.Id.sprite);
                int spriteImg = Quest.GetSpriteImg(questNumber);
                spritePlaceholder.SetImageResource(spriteImg);

                // (debugging) see distance between quest location/current location
                story.Text = Quest.GetQuestStory(questNumber);
                distanceText = FindViewById<TextView>(Resource.Id.distance);
                // For debugging only
                Button button = FindViewById<Button>(Resource.Id.button1);
                button.Click += delegate
                {
                    // Start Quest Activity
                    var intentStartQuest = new Intent(this, typeof(CompleteQuestActivity));
                    intentStartQuest.PutExtra("questNumber", questNumber); // passing variable to QuestActivity
                    StartActivity(intentStartQuest); // passing variable to QuestActivity
                };
            }



            // -/---MY CODE----
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
            double questLat = Convert.ToDouble(Quest.QuestCoords(questNumber)[0]);
            double questLng = Convert.ToDouble(Quest.QuestCoords(questNumber)[1]);
            Location.DistanceBetween(currentLat, currentLng, questLat, questLng, distancetmp);
            double distance = distancetmp[0];

            // display distance (double) from objective
            distanceText.Text = "Distance: " + distance.ToString() + " meters";

            int distanceRounded = Convert.ToInt32(distance); // round current distance 
            const int RANGE = 5; // How spread out distances are (smaller number = more frames). Multiplied by index of spectrescope frames.  
            int indexOfFrameToShow = distanceRounded / RANGE;
            string[] scope_frames = { "img1", "img2", "img3", "img4", "img5" };
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
            
            if(indexOfFrameToShow < scope_frames.Length){
                hotCold.Text = scope_frames[indexOfFrameToShow];
            }else{
                // if distance is beyond the scope, just show the 'cold' (last) frame by default
                hotCold.Text = scope_frames[scope_frames.Length - 1];
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