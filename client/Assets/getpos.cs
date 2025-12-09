using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getpos : MonoBehaviour
{
    private ParticleSystem.Particle[] particles;
    public Transform tg;
    // Start is called before the first frame update
    void Start()
    {
        particles = new ParticleSystem.Particle[p.main.maxParticles];
    }

    ParticleSystem p
    {
        get
        {
            return GetComponent<ParticleSystem>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        int count = p.GetParticles(particles);
        if(count > 0)
        {
            tg.transform.position = particles[0].position;
        }
    }
}
