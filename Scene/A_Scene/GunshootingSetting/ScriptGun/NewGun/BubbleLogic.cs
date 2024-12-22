using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    public OVRInput.RawButton shootingButton; // Button to trigger the particle system
    private new ParticleSystem particleSystem; // Use 'new' keyword to hide inherited member

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Stop(); // Stop the particle system at the start
        }
    }

    void Update()
    {
        if (particleSystem != null)
        {
            if (OVRInput.GetDown(shootingButton))
            {
                particleSystem.Play(); // Play the particle system when the button is pressed
            }
            else if (OVRInput.GetUp(shootingButton))
            {
                particleSystem.Stop(); // Stop the particle system when the button is released
            }
        }
    }
}
