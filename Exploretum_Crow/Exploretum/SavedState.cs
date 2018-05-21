using System;
namespace Exploretum
{
    public class SavedState
    {
        private static int[] savedStops = {0, 0, 0}; // 0: Moth, 1: Pine, 2: Hummingbird

        // get saved stop number for this quest
        public static int getStateOf(int qn) 
        {
            if (qn == 0){
                return savedStops[0];
            }else if (qn == 1){
                return savedStops[1];
            }else if (qn == 2){
                return savedStops[2];
            }else{
                return -1; // will throw error
            }
        }

        // save current stop number of this quest
        public static void setStateOf(int qn, int sn)
        {
            for (int i = 0; i < savedStops.Length; i++)
            {
                if (qn == i){
                    savedStops[i] = sn;   
                }
            }
        }
     

      
    }
}
