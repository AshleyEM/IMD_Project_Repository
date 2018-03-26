
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

namespace app_test  
{
    [Activity(Label = "CompleteQuestActivity")]
    public class CompleteQuestActivity : Activity  
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CompleteQuest);
            // get quest number for this quest
            int questNumber = Intent.GetIntExtra("questNumber", 0);
            // fill the spirit thanks text from quest list
            TextView spirit_thanks = FindViewById<TextView>(Resource.Id.thanks);
            spirit_thanks.Text = Quest.GetQuestCompleted(questNumber);
            Button buttonNextQ = FindViewById<Button>(Resource.Id.buttonNextQuest);
            
            buttonNextQ.Click += delegate {
                // START NEW quest
                var intentQuest = new Intent(this, typeof(QuestActivity));
                // pass quest number + 1 to next activity 
                intentQuest.PutExtra("questNumber", questNumber + 1);
                StartActivity(intentQuest);
            };
        }
    }
}
