using UnityEngine;

public class ProjectMeshParticleInteraction : MonoBehaviour
{
    public Mesh[] targetMeshes; // Array of target meshes from the Project
    public ParticleSystem[] particleSystems; // Array of particle systems to be affected
    public float influenceRadius = 5f; // Radius of influence
    public float baseInfluenceStrength = 1f; // Base strength of influence
    public float speedMultiplier = 1f; // Multiplier for influence strength based on speed

    private Vector3[] previousPositions; // Array to store previous positions of target objects
    private Vector3[] velocities; // Array to store velocities of target objects

    private void Start()
    {
        previousPositions = new Vector3[targetMeshes.Length];
        velocities = new Vector3[targetMeshes.Length];

        for (int i = 0; i < targetMeshes.Length; i++)
        {
            if (targetMeshes[i] != null)
            {
                previousPositions[i] = Vector3.zero; // Initialize with zero since these are project assets
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < targetMeshes.Length; i++)
        {
            Mesh mesh = targetMeshes[i];
            if (mesh == null) continue;

            // Assuming you have a way to get the position of the mesh in the scene
            Vector3 meshPosition = GetMeshPositionInScene(mesh);
            Vector3 currentPosition = meshPosition;
            velocities[i] = (currentPosition - previousPositions[i]) / Time.deltaTime;
            previousPositions[i] = currentPosition;

            foreach (ParticleSystem ps in particleSystems)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
                int numParticlesAlive = ps.GetParticles(particles);

                for (int j = 0; j < numParticlesAlive; j++)
                {
                    Vector3 direction = particles[j].position - meshPosition;
                    float distance = direction.magnitude;

                    if (distance < influenceRadius)
                    {
                        float influence = (influenceRadius - distance) / influenceRadius * baseInfluenceStrength;
                        influence += velocities[i].magnitude * speedMultiplier;
                        particles[j].velocity += direction.normalized * influence;

                        // Debug information
                        Debug.Log($"Particle {j} influenced with strength {influence} and velocity {particles[j].velocity}");
                    }
                }

                ps.SetParticles(particles, numParticlesAlive);
            }
        }
    }

    private Vector3 GetMeshPositionInScene(Mesh mesh)
    {
        // Implement logic to get the position of the mesh in the scene
        // This is a placeholder and needs to be replaced with actual logic
        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < targetMeshes.Length; i++)
        {
            Mesh mesh = targetMeshes[i];
            if (mesh == null) continue;

            Vector3 meshPosition = GetMeshPositionInScene(mesh);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(meshPosition, influenceRadius);
        }
    }
}
