using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

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

public class ParticlePlayer : EditorWindow{

    ParticleControl particleControl;
    TimeControl timeControl;

    [MenuItem("K_Particle/Player")]
    static void open()
    {
      
       GetWindow<ParticlePlayer>();
    }

    void OnEnable()
    {
        timeControl = new TimeControl();
        timeControl.SetMinMaxTime(0, 10);
        timeControl.speed = 0.1f;

        GameObject particleManager = Resources.Load("Prefabs/K_ParticleManager/ParticleManager") as GameObject;
        particleControl = particleManager.GetComponent<ParticleControl>();
        particleControl.init();
    }

    void OnGUI()
    {
        if (particleControl == null)
            return;

        //플레이버튼
        var buttonText = timeControl.IsPlaying ? "Pause" : "Play";
        if (GUILayout.Button(buttonText))
        {
            if (timeControl.IsPlaying)
                timeControl.Pause();
            else
                timeControl.Play();
        }

        timeControl.currentTime = GUILayout.HorizontalSlider(timeControl.getCurrentTime(),
            timeControl.minTime, timeControl.maxTime, "box", "box", GUILayout.Height(40), GUILayout.ExpandWidth(true));

        //GUI갱신
        if (timeControl.IsPlaying)
            Repaint();

        if (particleControl.arr_name.Count() > 0)
        {
            //널이 아닐 경우
            for (int i = 0; i < particleControl.amount; i++)
            {
                particleControl.playlist[i] = 
                    GUILayout.Toggle(particleControl.playlist[i], particleControl.arr_name[i],
                    EditorStyles.miniButton,
                    GUILayout.MaxWidth(position.width / 3));
                if (particleControl.playlist[i])
                {
                    particleControl.particles[i].Simulate(timeControl.getCurrentTime());
                }
                else
                    particleControl.particles[i].Simulate(0);
            }
        }

        //화살표 키로 시간이동
        if(Event.current.type == EventType.KeyDown)
        {
            //재생 중이면 일시정지함.
            timeControl.Pause();

            if (Event.current.keyCode == KeyCode.RightArrow)
                timeControl.currentTime += 0.01f;

            if (Event.current.keyCode == KeyCode.LeftArrow)
                timeControl.currentTime -= 0.01f;

            GUI.changed = true;
            Event.current.Use();
            Repaint();
        }
    }
}
