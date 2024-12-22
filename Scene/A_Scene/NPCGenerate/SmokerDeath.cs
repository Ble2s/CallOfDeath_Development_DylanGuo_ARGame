using UnityEngine;
using UnityEngine.UI;


public class SmokerNPC : MonoBehaviour
{
    private Animator animator; //  Animator
    private bool isDead = false; // avoid trigger multiple time 
    private ParticleSystem smokeParticles; // Reference to smoke particle
    private KillCounterUI killCounter;

    void Start()
    {
        animator = GetComponent<Animator>();

        //find the smoke particle system(child or nearby)
        smokeParticles = GetComponentInChildren<ParticleSystem>();

        killCounter = Object.FindFirstObjectByType<KillCounterUI>();

    }

    // animation trigger
    public void TriggerDeathAnimation()
    {
        if (isDead) return; // if die , return
        isDead = true;

        if (smokeParticles != null)
        {
            smokeParticles.Stop(); // Stop emitting particles
            Destroy(smokeParticles.gameObject, 2f); // Optional: Destroy smoke after a delay
        }

        // paly the animation
        if (animator != null)
        {
            animator.SetTrigger("Death");
            animator.SetBool("isDead", true);
        }

         // Find KillCounterUI and add a kill
         if (killCounter != null)
        {
            killCounter.AddKill();
        }

        // destroy delay 
        Destroy(gameObject, 2f); // AfterANimation
    }
}
