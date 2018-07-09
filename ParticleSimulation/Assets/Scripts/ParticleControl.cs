using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour {
    public  int amount;
    public  List<string> arr_name;
    public  List<bool> playlist;
    public List<ParticleSystem> particles;

    public ParticleControl()
    {
        arr_name = new List<string>();
        playlist = new List<bool>();
        particles = new List<ParticleSystem>();
        amount = 0; 
    }

    public void init()
    {
        amount = 0;
        arr_name.Clear();
        playlist.Clear();

        //씬 내의 ParticleSystem를 모두 얻어오기
        var particleSystems = FindObjectsOfType<ParticleSystem>();
        foreach (var particleSystem in particleSystems)
        {
            if (IsRoot(particleSystem))
            {
                arr_name.Add(particleSystem.name);
                playlist.Add(false);
                particles.Add(particleSystem);
                amount++;
            }
        }
    }


    //부모의 ParticleSystem인지 아닌지를 판단하기
    bool IsRoot(ParticleSystem ps)
    {
        var parent = ps.transform.parent;
        //부모 없는 particleSystem이면 경로
        if (parent == null)
            return true;

        //부모가 있어도 ParticleSystem 컴포넌트가 아니면 루트
        return parent.GetComponent<ParticleSystem>() == false;
    }
}
