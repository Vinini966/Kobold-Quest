//Code by Vincent Kyne

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Timer
{

    private float maxTime;
    private float timeLeft;
    private bool started = false;



    public Timer(float mT)
    {
        maxTime = mT;
        timeLeft = maxTime;
    }

    public void ResetTimer()
    {
        started = false;
        timeLeft = maxTime;
    }

    public void SetTimer(float nMT)
    {
        maxTime = nMT;
    }

    public void StartTimer()
    {
        ResetTimer();
        started = true;
    }

    public void ZeroTimer()
    {
        timeLeft = 0;
    }

    public void ResumeTimer()
    {
        started = true;
    }

    public void StopTimer()
    {
        started = false;
    }

    public bool isTimerStarted()
    {
        return started;
    }

    public void TimerUpdate()
    {
        if (started)
            timeLeft -= Time.deltaTime;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public float GetPercent()
    {
        return 1 - timeLeft / maxTime;
    }

    public bool CheckTimer()
    {
        if (timeLeft <= 0)
        {
            started = false;
            return true;
        }
        else
        {
            return false;
        }
    }

}
