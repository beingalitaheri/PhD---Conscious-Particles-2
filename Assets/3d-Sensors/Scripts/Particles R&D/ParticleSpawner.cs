using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public bool useVFX = true;
    public GameObject particlePrefab,particleVFXPrefab;      // 
    public Transform centerTransform;      // 
    public int particleCount = 100;        // 
    public float spawnRadius = 5f;         //
    public float initialSpeed = 2f;        // 
    GameObject particleVFX;
    ParticlesVFX pbVFX;
   //
   [Header(" ( Particles values ) ")]
    [SerializeField] private bool isMeshRenderer = true;
    [SerializeField] private Vector2 particleForce = new Vector2(20f , 30f);
    [SerializeField] private Vector2 particleForceMagnitude = new Vector2(2f, 4f);
    [SerializeField] private Vector2 particleNoiseScale = new Vector2(0.1f, 1f);
    [SerializeField] private Vector2 particleSeprationDistance = new Vector2(0.05f, 0.1f);

    void Start()
    {
        for (int i = 0; i < particleCount; i++)
        {
            Vector3 randomPos = centerTransform.position + Random.insideUnitSphere * spawnRadius;
            GameObject particle = Instantiate(particlePrefab, randomPos, Quaternion.identity);
            particle.gameObject.GetComponent<MeshRenderer>().enabled = isMeshRenderer;
            if (useVFX)
            {
                particleVFX = Instantiate(particleVFXPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            }
            Rigidbody rb = particle.GetComponent<Rigidbody>();

            Vector3 randomDirection = Random.onUnitSphere;
            rb.velocity = randomDirection * initialSpeed;

            ParticleBehavior pb = particle.AddComponent<ParticleBehavior>();
            if (particleVFX !=null)
            { 
                pbVFX = particleVFX.GetComponent<ParticlesVFX>(); 
            }
            pb.centerTransform = centerTransform;
            if (particleVFX != null)
            {
                pbVFX.target = particle.transform;
            }
            pb.attractionForce = Random.Range(particleForce.x, particleForce.y);
            pb.randomForceMagnitude = Random.Range(particleForceMagnitude.x, particleForceMagnitude.y);
            pb.noiseScale = Random.Range(particleNoiseScale.x , particleNoiseScale.y);
            pb.separationDistance = Random.Range(particleSeprationDistance.x , particleSeprationDistance.y);
        }
    }
}
