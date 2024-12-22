using UnityEngine;
using Meta.XR.MRUtilityKit;

public class SmokerSpawnerWithARInteractionNew : MonoBehaviour
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
    public float blowStrength = 400;

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

        GameObject spawnedNPC = null; // Declare the variable outside the loop

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

                spawnedNPC = Instantiate(selectedPrefab, randomPositionNormalOffset, Quaternion.identity);
                spawnedNPC.transform.rotation = randomRotation;

                // Uniformly control NPC size
                spawnedNPC.transform.localScale = Vector3.one * npcSizeScale;

                break; // Exit loop once a valid position is found and NPC is spawned
            }
            else
            {
                currentTry++;
            }
        }

        if (spawnedNPC != null)
        {
            StartCoroutine(SpawnSmokeDelayed(spawnedNPC));
        }
    }

    private System.Collections.IEnumerator SpawnSmokeDelayed(GameObject npc)
    {
        // Wait for random delay before generating smoke
        float delay = Random.Range(smokeDelayMin, smokeDelayMax);
        yield return new WaitForSeconds(delay);

        if (smokePrefab != null && npc != null) // Ensure NPC has not been destroyed
        {
            // Spawn smoke near NPC
            Vector3 smokePosition = npc.transform.position + Vector3.up * 1.5f; // Adjust height
            GameObject smoke = Instantiate(smokePrefab, smokePosition, Quaternion.identity);
            smoke.transform.SetParent(npc.transform); // Bind smoke to NPC
            currentSmoke = smoke; // Store reference to the smoke
        }
    }

    /// <summary>
    /// Applies a blow effect to the current smoke particles.
    /// </summary>
    /// <param name="direction">The direction of the blow.</param>
    /// <param name="strength">The strength of the blow.</param>
    public void ApplyBlowEffect(Vector3 direction, float strength)
    {
        if (currentSmoke == null) return; // Ensure smoke exists

        ParticleSystem particles = currentSmoke.GetComponent<ParticleSystem>();

        
        if (particles != null)
        {
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;

            // Add randomness to the blow direction for a more natural effect
            Vector3 randomizedDirection = direction + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.5f, 0.5f)
            ).normalized;

            // Apply the blow force to the particle velocity
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(randomizedDirection.x * strength * blowStrength*10);
            velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(randomizedDirection.y * strength * blowStrength*10);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(randomizedDirection.z * strength * blowStrength*10);
        }
    }

    
}
