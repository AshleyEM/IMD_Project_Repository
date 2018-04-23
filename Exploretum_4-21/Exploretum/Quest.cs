using System;

namespace Exploretum
{
    public class Quest
    {
        // information for each quest
        private static string[][] questList = {
            new string[] {"Moth and Magnolia",  
                          "47.591481", // lat  
                          "-122.083228", // long 
                          "[spirit's story]", 
                          "[spirit1 thanks user]", 
                         },
            new string[] {"quest 2",
                          "47.591673 ", 
                          "-122.083251",
                          "[spirit's story]",
                          "[spirit2 thanks user]",  
                         },
 
            // ...
        };
        // sprite images for each quest
        private static int[] spriteImgs = {
            Resource.Drawable.leopard_moth_sprite,
            Resource.Drawable.magnolia_sprite,
        };
        // hot-cold meter images
        public static int[] scopeFrames = {
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
        // return sprite image for Resource.Drawable
        public static int GetSpriteImg(int questNumber){
            return spriteImgs[questNumber];
        }
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
