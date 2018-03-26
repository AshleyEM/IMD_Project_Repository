using System;


namespace app_test
{
    public class Quest
    {
        // information for each quest
        private static string[][] questList = {
            new string[] {"quest 1",  
                          "47.591673", // lat  
                          "-122.083251", // long 
                          "[spirit's story]",
                          "[spirit1 thanks user]"   
                         },
            new string[] {"quest 2",
                          "47.591481", 
                          "-122.083228",
                          "[spirit's story]",
                          "[spirit2 thanks user]"  
                         },
            new string[] {"quest 3", 
                          "47.591349",  
                          "-122.082417",
                          "[spirit's story]",
                          "[spirit3 thanks user]",   
                         },
            new string[] {"quest 4", 
                          "47.591546",
                          "-122.082891",
                          "[spirit's story]",
                          "[spirit4 thanks user]",   
                         },
            // ...
        };
    
        // return the coordinates { latitude, longitude } of given quest
        public static string[] QuestCoords(int questNumber){
            string[] coords = new string[2];
            coords[0] = questList[questNumber][1];
            coords[1] = questList[questNumber][2];
            return coords;
        }
        // return the number of total quests
        public static int QuestLimit()
        {
            return questList.Length-1;
        }
        // return quest name based on questNumber
        public static string GetQuestName(int questNumber)
        {
            return questList[questNumber][0];
        }
        public static string GetQuestStory(int questNumber)
        {
            return questList[questNumber][3];
        }
        public static string GetQuestCompleted(int questNumber)
        {
            return questList[questNumber][4];
        }
        // for debugging
        public void printQuests()
        {
            foreach (string[] s in questList)
            {
                Console.WriteLine(s[0]);
            }
        }

    }
}
