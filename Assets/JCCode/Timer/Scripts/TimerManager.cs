using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JCCode
{
    namespace Timer
    {
        public class TimerManager : MonoBehaviour
        {
            private static TimerManager _instance;
            public static TimerManager instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new GameObject("TimerManager").AddComponent<TimerManager>();
                        _instance.Initialize();
                    }
                    return _instance;
                }
            }

            List<Timer> timerList;

            void Initialize()
            {
                timerList = new List<Timer>();
            }

            private int _id = 0;
            public static Timer CreateTimer(Action start, Action callback, Action finish, float repeatTime, int repeatCounter = -1)
            {
                Timer timer = new Timer(instance._id++, start, callback, finish, repeatTime, repeatCounter);
                instance.timerList.Add(timer);
                return timer;
            }

            void Update()
            {
                timerList.RemoveAll((x) => x.isReadyToDelete);

                for (int i = 0; i < timerList.Count; i++)
                {
                    timerList[i].Tick(Time.deltaTime);
                }
            }

            public static void RemoveTimerWithId(int id)
            {
                instance.timerList.RemoveAll((x) => x.id == id);
            }
        }

        public class Timer
        {
            // 
            private int _id = -1;
            public int id
            {
                set
                {
                    _id = value;
                }
                get
                {
                    return _id;
                }
            }

            //time
            private float _repeatTimer;
            private float _repeatTime;
            private int _MaxCounter;
            private int _CurrentCounter;

            // action
            private Action _onStart;
            private Action _onCallBack;
            private Action _onFinish;

            //status
            bool isRun = false;
            private bool _isReadyToDelete = false;
            public bool isReadyToDelete
            {
                get
                {
                    return _isReadyToDelete;
                }
            }

            public Timer(int id, Action start, Action callback, Action finish, float repeatTime, int repeatCounter = -1)
            {
                //set status
                isRun = false;
                _isReadyToDelete = false;

                //set private data
                this.id = id;

                _repeatTime = repeatTime;
                _repeatTimer = 0.0f;
                _MaxCounter = repeatCounter;
                _CurrentCounter = repeatCounter;

                _onStart = start;
                _onCallBack = callback;
                _onFinish = finish;
            }

            public void BeginTimer()
            {
                if (_onStart != null
                    && !isRun)
                    _onStart();

                isRun = true;
            }

            public void UnPauseTimer()
            {
                isRun = true;
            }

            public void PauseTimer()
            {
                isRun = false;
            }

            public void KillTimer(bool isCallFinish = false)
            {
                isRun = false;
                SetToRemove();
                _repeatTimer = 0.0f;

                if (isCallFinish)
                {
                    if (_onFinish != null)
                        _onFinish();
                }
            }

            public void RemoveTimer(bool isCallFinish = false)
            {
                isRun = false;

                _repeatTimer = 0.0f;

                if (isCallFinish)
                {
                    if (_onFinish != null)
                        _onFinish();
                }
            }

            public void ReStart(bool isRunStart)
            {
                if (isRunStart)
                {
                    if (_onStart != null)
                        _onStart();
                }

                isRun = true;
                _repeatTimer = 0.0f;
                _CurrentCounter = _MaxCounter;
            }

            public void Tick(float delta)
            {
                if (!isReadyToDelete)
                {
                    if (isRun)
                    {
                        _repeatTimer += delta;
                        if (_repeatTimer >= _repeatTime)
                        {
                            _repeatTimer = 0.0f;
                            if (_CurrentCounter == 0)
                            {
                                if (_onFinish != null)
                                    _onFinish();

                                SetToRemove();
                            }
                            else
                            {
                                if (_onCallBack != null)
                                    _onCallBack();

                                _CurrentCounter--;
                            }
                        }
                    }
                }
            }

            void SetToRemove()
            {
                _isReadyToDelete = true;
            }
        }
    }
}
