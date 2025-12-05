
using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class goldplay : MonoBehaviour
{
    public float delay = 1f;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        GetComponent<UIParticle>().Play();
        DOVirtual.DelayedCall(delay, () =>
        {
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();

            StartCoroutine(movetotarget(ps));
        });
    }
    ParticleSystem ptk
    {
        get
        {
            return GetComponentInChildren<ParticleSystem>();
        }
    }
    private void Awake()
    {
        particles = new ParticleSystem.Particle[ptk.main.maxParticles];
    }

    void FixedUpdate() // ����֡���£����ȶ�
    {
        int count = GetComponent<ParticleSystem>().GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            // 1. �������ӵ�Ŀ��ķ���
            Vector3 direction = target.position - particles[i].position;
            // 2. ����������������Խ������ԽС��������ͷ��
            float distance = direction.magnitude;
            if (distance < 0.1f) // ����Ŀ���ֹͣ
            {
                particles[i].velocity = Vector3.zero;
                continue;
            }
            // 3. Ӧ������ + ��ҷ����
            Vector3 force = direction.normalized * attractForce * (1 - distance / 5f); // ����˥��
            particles[i].velocity = particles[i].velocity * (1 - drag * Time.fixedDeltaTime) + force;
        }

        GetComponent<ParticleSystem>().SetParticles(particles, count);
    }
    public float speed = 1f;
    private ParticleSystem.Particle[] particles;
    private float attractForce =1;
    private float drag=1;

    IEnumerator movetotarget(ParticleSystem ps)
    {
        
        if (ps != null && target != null)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
            int count = ps.GetParticles(particles);

            bool end = false;
            // �����������ӣ������Ƿ���Ŀ��
            while (!end)
            {
                end = true;
                for (int i = 0; i < count; i++)
                {
                    var p = particles[i].position;
                    var disv = (target.position - p);
                    if (disv.sqrMagnitude > 0.1f)
                    {
                        end = false;
                        break;
                    }
                    p += disv.normalized * speed;
                    particles[i].position = p;
                }
                ps.SetParticles(particles, count);

                yield return null;
            } 
        }
    }
}
