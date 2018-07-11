using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour {
    public  int amount;
    public List<ParticleSystem> particles;

    public ParticleControl()
    {
        particles = new List<ParticleSystem>();
        amount = 0; 
    }

    public void init()
    {
        amount = 0;
        particles.Clear();

        //씬 내의 ParticleSystem를 모두 얻어오기
        var particleSystems = FindObjectsOfType<ParticleSystem>();
        foreach (var particleSystem in particleSystems)
        {
            if (IsRoot(particleSystem))
            {
                particles.Add(particleSystem);
                amount++;
            }
        }

        //파티클 이름 순으로 정렬
        particles.Sort(delegate (ParticleSystem p1, ParticleSystem p2)
        {
            return p1.gameObject.name.CompareTo(p2.name);
        });
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
