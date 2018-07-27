using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeControl
{
    private double m_lastFrameEditorTime = 0;

    //재생중인지 판단
    public bool IsPlaying { get; private set; }
    //시간의 하한
    public float minTime { get; private set; }
    //시간의 상한
    public float maxTime { get; private set; }

    //재생속도
    public float speed { get; set; }

    public float currentTime;

    public float getCurrentTime()
    {
        return Mathf.Repeat(currentTime, maxTime);

    }

    public void SetMinMaxTime(float minTime, float maxTime)
    {
        this.minTime = minTime;
        this.maxTime = maxTime;
    }


    public TimeControl()
    {
        EditorApplication.update += TimeUpdate;
    }

    public void TimeUpdate()
    {
        if (IsPlaying)
        {
            var timeSinceStartup = EditorApplication.timeSinceStartup;
            var deltaTime = timeSinceStartup - m_lastFrameEditorTime;
            m_lastFrameEditorTime = timeSinceStartup;

            currentTime += (float)deltaTime * speed;
        }
    }

    //재생
    public void Play()
    {
        IsPlaying = true;
        m_lastFrameEditorTime = EditorApplication.timeSinceStartup;
    }

    //일시 정지
    public void Pause()
    {
        IsPlaying = false;
    }

    //정지
    public void Stop()
    {
        IsPlaying = false;
        currentTime = 0;
    }
}
