using UnityEngine;
using Meta.XR.MRUtilityKit;

public class SmokerSpawnerWithARInteraction : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] smokerPrefabs; // Support multiple NPC prefabs
    public float spawnTimer = 1f;
    public float minEdgeDistance = 0.3f;
    public float normalOffset = 0.1f;
    public int spawnTry = 1000;

    [Header("Smoke Settings")]
    public GameObject smokePrefab;
    public float smokeDelayMin = 2f; // Minimum delay for smoke appearance
    public float smokeDelayMax = 5f; // Maximum delay for smoke appearance

    [Header("Size Control")]
    [Range(0.05f, 2f)]
    public float npcSizeScale = 1f; // Slider to uniformly control NPC size

    public MRUKAnchor.SceneLabels spawnLabels;
    private float timer;

    private GameObject currentSmoke; // Store the data of current smoke
    public float blowStrenth = 100;

    void Update()
    {
        if (MRUK.Instance == null || !MRUK.Instance.IsInitialized) 
            return;

        timer += Time.deltaTime;
        if (timer > spawnTimer)
        {
            SpawnSmoker();
            timer -= spawnTimer;
        }
    }

    public void SpawnSmoker()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        int currentTry = 0;

        while (currentTry < spawnTry)
        {
            bool hasFoundPosition = room.GenerateRandomPositionOnSurface(
                MRUK.SurfaceType.VERTICAL, 
                minEdgeDistance, 
                new LabelFilter(spawnLabels), 
                out Vector3 pos, 
                out Vector3 norm
            );

            if (hasFoundPosition)
            {
                Vector3 randomPositionNormalOffset = pos + norm * normalOffset;
                randomPositionNormalOffset.y = 0;

                // Randomly select NPC prefab
                GameObject selectedPrefab = smokerPrefabs[Random.Range(0, smokerPrefabs.Length)];

                // Generate random rotation around Y-axis
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);


                GameObject spawnedNPC = Instantiate(selectedPrefab, randomPositionNormalOffset, Quaternion.identity);
                spawnedNPC.transform.rotation = randomRotation;


                // Uniformly control NPC size
                spawnedNPC.transform.localScale = Vector3.one * npcSizeScale;

                // Delayed smoke generation
                StartCoroutine(SpawnSmokeDelayed(spawnedNPC));

                return;
            }
            else
            {
                currentTry++;
            }
        }
    }

    private System.Collections.IEnumerator SpawnSmokeDelayed(GameObject npc)
    {
        // Wait for random delay before generating smoke
        float delay = Random.Range(smokeDelayMin, smokeDelayMax);
        yield return new WaitForSeconds(delay);

        if (smokePrefab != null && npc != null)// make sure npc did not disappear 
        {
             // Try to find SmokeAnchor in NPC's children
        //     Transform smokeAnchor = npc.transform.Find("SmokeAnchor");
            
        //     Vector3 smokePosition;
        // if (smokeAnchor != null)
        // {
        //     // Use SmokeAnchor position if it exists
        //     smokePosition = smokeAnchor.position;
        // }
        // else
        // {
        //     // Default to NPC position with an upward offset
        //     smokePosition = npc.transform.position + Vector3.up * 2f;
        // }

        //      // Instantiate smoke at the calculated position
        //     GameObject smoke = Instantiate(smokePrefab, smokePosition, Quaternion.identity);
        //     smoke.transform.SetParent(npc.transform); // Bind smoke to NPC
        //     currentSmoke = smoke; // Store reference to the smoke

            // Spawn smoke near NPC
            Vector3 smokePosition = npc.transform.position + Vector3.up * 1.5f; // Adjust height
            GameObject smoke = Instantiate(smokePrefab, smokePosition, Quaternion.identity);
            smoke.transform.SetParent(npc.transform); // binding to the smoke 

        }
    }

 
    /// <param name="direction">Direction</param>
    /// <param name="strength">SwingStrenth</param>
    public void ApplyBlowEffect(Vector3 direction, float strength)
    {
        strength = 10f;
        if (currentSmoke == null) return; // Make sure the smoke exist

        ParticleSystem particles = currentSmoke.GetComponent<ParticleSystem>();
        if (particles != null)
        {

            // var main = particles.main;
            // main.startSpeed = strength * blowStrenth;

            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;

            // Vector3 randomizedDirection = direction.normalized + new Vector3(
            // Random.Range(-0.2f, 0.2f), 
            // Random.Range(-0.2f, 0.2f), 
            // Random.Range(-0.2f, 0.2f)
            // );
            // Setting up the direction and Speed
            Vector3 randomizedDirection = direction + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), Random.Range(-0.5f, 0.5f)).normalized;
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(randomizedDirection.x * strength * blowStrenth);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(randomizedDirection.y * strength * blowStrenth);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(randomizedDirection.z * strength * blowStrenth);
        }
    }

    
}