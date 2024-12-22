using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OVRScriptRayGun : MonoBehaviour
{
    public LayerMask layerMask;
    public OVRInput.RawButton shootingButton;
    public LineRenderer linePrefab;
    public GameObject rayImpactPrefab;
    public Transform shootingPoint;
    public float maxLineDistance = 5f;
    public float lineShowTimer = 0.3f;
    public AudioSource source;
    public AudioClip shootingAudioClip;

    public AudioSource DeathSound;
    public AudioClip DeathAudioClip;

    // Pick Up
    public OVRInput.RawButton pickupButton; // Button
    public float pickupRange = 2f; // Range area
    private bool isGunPickedUp = false; // Booling value ?

    public Material waterMaterial; // Material Setting
    private int shootCount = 0; // Shooting Account
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow }; // Material 

    private bool isShootingActive = false; // Flag to track shooting state
    private Color originalColor; // Store the original material color

    public ParticleSystem particleSystemPrefab;
    private ParticleSystem activeParticleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = waterMaterial.color;


        if (particleSystemPrefab != null)
        {
        // Instantiate the particle system at the shooting point with matching position and rotation
        activeParticleSystem = Instantiate(particleSystemPrefab, shootingPoint.position, shootingPoint.rotation);
    
        // Set the particle system as a child of the shooting point for hierarchy organization
        activeParticleSystem.transform.SetParent(shootingPoint);
    
        // Stop the particle system immediately after creation to allow manual triggering later
        activeParticleSystem.Stop();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //  if (OVRInput.GetDown(pickupButton) && !isGunPickedUp)
        // {
        // TryPickupGun();
        // }

        // if (isGunPickedUp && OVRInput.GetDown(shootingButton))
        // {
        // Shoot();
        // }

        if(OVRInput.GetDown(shootingButton))
        {
            Shoot();
        }


        if (activeParticleSystem != null && !OVRInput.Get(shootingButton))
        {
            activeParticleSystem.Stop();
            activeParticleSystem = null;
        }

        

        // If shooting is not active, reset the material color to the original
        if (isShootingActive && !OVRInput.Get(shootingButton))
        {
            waterMaterial.color = originalColor;
            isShootingActive = false;
        }
    }


    // private void TryPickupGun()
    // {
    // RaycastHit hit;
    // if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
    //     {
    //     if (hit.collider.CompareTag("BubbleGun"))
    //         {
    //         // Binding to player
    //         hit.collider.transform.SetParent(transform);
    //         hit.collider.transform.localPosition = Vector3.zero;
    //         hit.collider.transform.localRotation = Quaternion.identity;
    //         isGunPickedUp = true;
    //         }
    //     }
    // }

    public void Shoot()
    {
        source.PlayOneShot(shootingAudioClip);

         // Set the material color to red
        waterMaterial.color = Color.red;
        isShootingActive = true;

        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, maxLineDistance, layerMask);

        Vector3 endPoint = Vector3.zero;

        if(hasHit)
        {
            // stop the ray 
            endPoint = hit.point;

            //check if hit NPC
            if (hit.collider.CompareTag("SmokerNPC"))
            {
                // get the npc script
                SmokerNPC npc = hit.collider.GetComponent<SmokerNPC>();
                if (npc != null)
                {
                    npc.TriggerDeathAnimation(); // trigger the death animation
                    DeathSound.PlayOneShot(DeathAudioClip);
                }
            }
   

            // SmokerSpawnerWithARInteraction smokerPrefabs = hit.transform.GetComponentInParent<SmokerSpawnerWithARInteraction>();
            
            Quaternion rayImpactRotation = Quaternion.LookRotation(-hit.normal);

            GameObject rayImpact = Instantiate(rayImpactPrefab, hit.point,rayImpactRotation);
            Destroy(rayImpact, 1);
        }
        else
        {
            endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance;
        }

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, shootingPoint.position);


        line.SetPosition(1,endPoint);

        Destroy(line.gameObject, lineShowTimer);

        ChangeMaterialColor();

        TriggerParticleSystem();
    }

    
   private void TriggerParticleSystem()
{
    if (particleSystemPrefab != null)
    {
        // Check if no active particle system exists
        if (activeParticleSystem == null)
        {
            // Instantiate the particle system at the shooting point
            activeParticleSystem = Instantiate(particleSystemPrefab, shootingPoint.position, Quaternion.identity);
            activeParticleSystem.transform.SetParent(shootingPoint); // Set it as a child of the shooting point
        }

        // Play the particle system
        activeParticleSystem.Play();
    }
}




    private void ChangeMaterialColor()
    {
        // check materail null
        if (waterMaterial == null)
        {
            Debug.LogError("Water Material is not assigned!");
            return;
        }

        // Check attribute of _Color 
        if (waterMaterial.HasProperty("_Color"))
        {
            // Loop color
            int colorIndex = shootCount % colors.Length; // Loop
            waterMaterial.SetColor("_Color", colors[colorIndex]);

            Debug.Log($"Material color changed to: {colors[colorIndex]}");
        }
        else
        {
            Debug.LogError("Material does not have a _Color property!");
        }
    }
  
}
