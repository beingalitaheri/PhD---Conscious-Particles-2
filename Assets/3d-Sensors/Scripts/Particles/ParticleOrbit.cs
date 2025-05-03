using UnityEngine;

public class ParticleOrbit : MonoBehaviour
{
    public Transform centerTransform;
    public float radius = 1f;
    public float speed = 1f;
    public float randomMovement = 0.1f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

        var main = ps.main;
        main.loop = true;
    }

    void LateUpdate()
    {
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            float angle = speed * Time.time + i;
            Vector3 newPos = centerTransform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            newPos += Random.insideUnitSphere * randomMovement;
            particles[i].position = newPos;
        }

        ps.SetParticles(particles, numParticlesAlive);
    }
}
