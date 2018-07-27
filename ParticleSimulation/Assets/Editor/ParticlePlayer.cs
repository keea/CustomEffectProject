using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

public class ParticlePlayer : EditorWindow
{

    ParticleControl particleControl;
    TimeControl timeControl;
    public List<TimeControl> timeControls;
    public List<bool> playlist;
    Vector2 scrollPos;
    bool syncTime;

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
        timeControls = new List<TimeControl>();
        syncTime = false;
        for (int i = 0; i < particleControl.particles.Count(); i++)
        {
            playlist.Add(false);
            TimeControl temp = new TimeControl();
            temp.SetMinMaxTime(0, 9);
            temp.speed = 1f;
            timeControls.Add(temp);
        }
    }

    void OnGUI()
    {
        if (particleControl == null)
            return;

        Repaint();
       
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //타임라인
        CreateTimeLine(timeControl, 40);

        using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(position.width / 3)))
        {
            var text = timeControl.IsPlaying ? "Pause" : "Play";
            if (GUILayout.Button(text, EditorStyles.miniButton, GUILayout.MaxWidth(position.width / 8)))
            {
                if (timeControl.IsPlaying)
                {
                    timeControl.Pause();
                    //파티클 재생을 멈춘다.
                    Stop();
                }
                else
                {
                    timeControl.Play();
                    //파티클 재생한다.
                    SyncAllPlay();
                }
            }

            syncTime = EditorGUILayout.ToggleLeft("Sync", syncTime);
        }
       
        //플레이시간 보기
        ShowPlayTime(timeControl);

        using (var h = new EditorGUILayout.HorizontalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollView.scrollPosition;

                for (int i = 0; i < particleControl.particles.Count(); i++)
                {
                    playlist[i] = EditorGUILayout.ToggleLeft(
                        particleControl.particles[i].gameObject.name,
                        playlist[i]);

                    if (playlist[i])
                    {
                        if (syncTime)
                        {
                            if (timeControl.IsPlaying)
                                particleControl.particles[i].Simulate(timeControl.getCurrentTime());
                            else
                                particleControl.particles[i].Stop();
                        }
                        else
                        {
                            EditorGUILayout.Space();
                            CreateTimeLine(timeControls[i], 20);

                            CommandBtn(timeControls[i]);
                            ShowPlayTime(timeControls[i]);

                            //if (timeControls[i].IsPlaying)
                                particleControl.particles[i].Simulate(timeControls[i].getCurrentTime());

                           // else
                               // particleControl.particles[i].Stop();
                        }
                    }
                }
            }
        }
    }

    //파티클 싱크재생 - 전부
    void SyncAllPlay()
    {
        for(int i=0; i<particleControl.particles.Count(); i++)
        {
            if (playlist[i])
                timeControls[i].Play();
        }
    }

    //파티클 재생 정지
    void Stop()
    {
        for (int i = 0; i < particleControl.particles.Count(); i++)
            timeControls[i].Pause();
    }

    void CreateTimeLine(TimeControl timeControl, int height)
    {   
        //커스텀 타임 라인
        timeControl.currentTime = GUILayout.HorizontalSlider(timeControl.getCurrentTime(),
            timeControl.minTime, timeControl.maxTime, "box", "box", GUILayout.Height(height), GUILayout.ExpandWidth(true));

        DrawTicks();
    }

    //버튼 동작
    void CommandBtn(TimeControl timeControl)
    {
        var text = timeControl.IsPlaying ? "Pause" : "Play";
        if (GUILayout.Button(text, EditorStyles.miniButton, GUILayout.MaxWidth(position.width / 8))) 
        {
            if (timeControl.IsPlaying)
                timeControl.Pause();
            else
                timeControl.Play();
        }
    }

    //시간 위치 확인
    void ShowPlayTime(TimeControl timeControl)
    {
        var lastRect = GUILayoutUtility.GetLastRect();
        float time = 
        EditorGUI.FloatField(new Rect((position.width / 2) - 40,
            lastRect.y, 80 ,EditorGUIUtility.singleLineHeight) ,timeControl.currentTime);

        timeControl.currentTime = time;
    }

    //눈금 만들기
    void DrawTicks()
    {
        var timeLength = timeControl.maxTime - timeControl.minTime; //시간의 길이
        var gridline = timeLength * 2; // 0.5눈금 간격
        var lastRect = GUILayoutUtility.GetLastRect();

        var sliderRect = new Rect(lastRect); //타임 라인 slider의 Rect

        for (int i = 1; i < gridline; i++)
        {
            var x = (sliderRect.width / gridline) * i;

            Handles.DrawLine(
                new Vector2(sliderRect.x + x, sliderRect.y),
                new Vector2(sliderRect.x + x, sliderRect.y + sliderRect.height));

            Handles.Label(
                new Vector2(sliderRect.x + x - 10, sliderRect.y - 12),
                (timeLength / gridline * i).ToString("0.0"));
        }
    }
}
