using UnityEngine;

public class ParticleBehavior : MonoBehaviour
{
    public Transform centerTransform;      // 
    public float attractionForce = 5f;     //
    public float randomForceMagnitude = 1f;// 
    public float noiseScale = 0.5f;        //
    public float separationDistance = 1f;  

    private Rigidbody rb;
    private Vector3 noiseOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        noiseOffset = new Vector3(Random.value * 10f, Random.value * 10f, Random.value * 10f);
        gameObject.tag = "Particle"; 
    }

    void FixedUpdate()
    {
        Vector3 toCenter = centerTransform.position - transform.position;

        Vector3 attraction = toCenter.normalized * attractionForce;

        // 
        float noiseX = Mathf.PerlinNoise(Time.time * noiseScale + noiseOffset.x, noiseOffset.y) - 0.5f;
        float noiseY = Mathf.PerlinNoise(Time.time * noiseScale + noiseOffset.y, noiseOffset.z) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(Time.time * noiseScale + noiseOffset.z, noiseOffset.x) - 0.5f;
        Vector3 randomForce = new Vector3(noiseX, noiseY, noiseZ) * randomForceMagnitude;

        // 
        Vector3 separationForce = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, separationDistance);
        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.CompareTag("Particle"))
            {
                Vector3 toNeighbor = transform.position - neighbor.transform.position;
                if (toNeighbor.sqrMagnitude > 0f)
                {
                    separationForce += toNeighbor.normalized / toNeighbor.sqrMagnitude;
                }
            }
        }

        
        rb.AddForce(attraction + randomForce + separationForce);
    }
}
