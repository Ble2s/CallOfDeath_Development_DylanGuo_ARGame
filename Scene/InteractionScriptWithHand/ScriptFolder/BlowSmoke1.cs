using UnityEngine;

public class BlowSmokeWithCamera : MonoBehaviour
{
    public ParticleSystem blowerParticle; // ParticleSystem
    public float repelRadius = 5f; // MouseRange
    public float repelForce = 50f; // TheForceSzie
    public Camera mainCamera; // MainCamera
    public float angleThreshold = 45f; // Angle of Camera 

    private void Start()
    {
        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        // Get the World position of Mouse
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 10f)); // 假设粒子系统在 z=10 附近

        // Get forward
        Vector3 mouseForward = mainCamera.ScreenPointToRay(mouseScreenPosition).direction;

        // Get particle
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[blowerParticle.particleCount];
        int particleCount = blowerParticle.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 toMouse = mouseWorldPosition - particles[i].position;

            // Check the logic
            if (toMouse.magnitude < repelRadius)
            {
                // using dot product to detect the angle 
                float angle = Vector3.Angle(toMouse.normalized, mouseForward);
                if (angle < angleThreshold)
                {
                    // BLowing Direction
                    Vector3 forceDirection = toMouse.normalized;
                    particles[i].velocity -= forceDirection * repelForce * Time.deltaTime;
                }
            }
        }

        // Update  particle
        blowerParticle.SetParticles(particles, particleCount);
    }
}
