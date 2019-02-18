using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{    
    // A custom class where I can store my countdown method. I may use it to also store other time-related methods.
    public class TimeManager
    {        
        // Private field for time count, it should always start at 0, since we use it to count up in seconds.
        private float timeCount = 0;

        // A simple boolean method that returns true when the desired number of seconds has passed. 
        public bool TimeCount(float seconds)
        {
            timeCount += Time.deltaTime;

            if (timeCount >= seconds)
            {
                timeCount = 0;
                return true;
            }

            else return false;
        }
    }
}
