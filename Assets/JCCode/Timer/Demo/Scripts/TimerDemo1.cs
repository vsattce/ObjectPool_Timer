using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCCode
{
    namespace Timer
    {
        public class TimerDemo1 : MonoBehaviour
        {
            void Awake()
            {
                Timer timer = TimerManager.CreateTimer(TimerStart, TimerUpdate, TimerExit, 1.0f);
                timer.BeginTimer();
            }

            void TimerStart()
            {
                Debug.Log("TimerStart");
            }

            void TimerUpdate()
            {
                Debug.Log("TimerUpdate");
            }
            void TimerExit()
            {
                Debug.Log("TimerExit");
            }
        }
    }
}
