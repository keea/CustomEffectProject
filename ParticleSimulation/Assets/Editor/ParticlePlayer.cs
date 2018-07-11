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
    public List<bool> playlist;

    [MenuItem("K_Particle/Player")]
    static void open()
    {
      
       GetWindow<ParticlePlayer>();
    }

    void OnEnable()
    {
        timeControl = new TimeControl();
        timeControl.SetMinMaxTime(0, 9);
        timeControl.speed = 1f;

        GameObject particleManager = Resources.Load("Prefabs/K_ParticleManager/ParticleManager") as GameObject;
        particleControl = particleManager.GetComponent<ParticleControl>();
        particleControl.init();

        playlist = new List<bool>();
        for(int i=0; i<particleControl.particles.Count(); i++)
        {
            playlist.Add(false);
        }
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


        EditorGUILayout.Space();

        //커스텀 타임 라인
        timeControl.currentTime = GUILayout.HorizontalSlider(timeControl.getCurrentTime(),
            timeControl.minTime, timeControl.maxTime, "box", "box", GUILayout.Height(40), GUILayout.ExpandWidth(true));

        var timeLength = timeControl.maxTime - timeControl.minTime; //시간의 길이
        var gridline = timeLength * 2; // 0.5눈금 간격
        var lastRect = GUILayoutUtility.GetLastRect();

        var sliderRect = new Rect(lastRect); //타임 라인 slider의 Rect

        for(int i=1; i<gridline; i++)
        {
            var x = (sliderRect.width / gridline) * i;

            Handles.DrawLine(
                new Vector2(sliderRect.x + x, sliderRect.y),
                new Vector2(sliderRect.x + x, sliderRect.y + sliderRect.height));

            Handles.Label(
                new Vector2(sliderRect.x + x - 10, sliderRect.y - 12),
                (timeLength / gridline * i).ToString("0.0"));
        }

        //GUI갱신
        if (timeControl.IsPlaying)
            Repaint();

        //널이 아닐 경우
        if (particleControl.particles.Count() > 0)
        {
            int max_row = particleControl.amount / 3;

            for(int i=0; i<=max_row; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j=0; j<3; j++)
                    {
                        int idx = (i * 3) + j;
                        if (idx > particleControl.amount-1)
                            break;

                       
                        playlist[idx] =
                        GUILayout.Toggle(playlist[idx], particleControl.particles[idx].gameObject.name,
                        EditorStyles.miniButton,
                        GUILayout.MaxWidth(position.width / 3));
                        if (playlist[idx])
                        {
                            particleControl.particles[idx].Simulate(timeControl.getCurrentTime());
                        }
                        else
                            particleControl.particles[idx].Simulate(0);
                    }
                }
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
