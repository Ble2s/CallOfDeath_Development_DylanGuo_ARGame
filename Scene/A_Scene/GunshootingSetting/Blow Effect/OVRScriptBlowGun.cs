using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OVRScriptBloweWindGun : MonoBehaviour
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
    public float WindForceStrenth = 20f;

    // Pick Up
    public OVRInput.RawButton pickupButton; // Button
    public float pickupRange = 2f; // Range area
    private bool isGunPickedUp = false; // Booling value ?

    public Material waterMaterial; // material setting 
    private int shootCount = 0; // shooting account 
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow }; // Color List

    private bool isShootingActive = false; // Flag to track shooting state
    private Color originalColor; // Store the original material color

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = waterMaterial.color;

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

            // Find the particle system nearly
            Collider[] hitColliders = Physics.OverlapSphere(endPoint, 2f); // assume the range of 2 
            foreach (var collider in hitColliders)
        {
            // check if it's particle
            var smokerSpawner = collider.GetComponent<SmokerSpawnerWithARInteraction>();
            if (smokerSpawner != null)
            {
                // caculate the directio from center to outside 
                Vector3 blowDirection = (collider.transform.position - endPoint).normalized;
                float blowStrength = WindForceStrenth; // adjust the strenth
                smokerSpawner.ApplyBlowEffect(blowDirection, blowStrength);
            }
        }

            //check if hit NPC
            // if (hit.collider.CompareTag("SmokerNPC"))
            // {
            //     // get the npc script//
            //     SmokerNPC npc = hit.collider.GetComponent<SmokerNPC>();
            //     if (npc != null)
            //     {
            //         npc.TriggerDeathAnimation(); // trigger the death animation
            //     }
            // }
   

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
    }

    private void ChangeMaterialColor()
    {
        // Check waterMaterial If it's null
        if (waterMaterial == null)
        {
            Debug.LogError("Water Material is not assigned!");
            return;
        }

        // check the attribute of  _Color 
        if (waterMaterial.HasProperty("_Color"))
        {
            // Loop color selection 
            int colorIndex = shootCount % colors.Length; // Loop achieve
            waterMaterial.SetColor("_Color", colors[colorIndex]);

            Debug.Log($"Material color changed to: {colors[colorIndex]}");
        }
        else
        {
            Debug.LogError("Material does not have a _Color property!");
        }
    }
}

