using UnityEngine;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs; //NpCPackage
    public ParticleSystem smokeEffect; // Prefab
    public Vector2 spawnAreaMin = new Vector2(-10f, -10f); // MInRange
    public Vector2 spawnAreaMax = new Vector2(10f, 10f);   // MaxRange
    public float spawnInterval = 5f; // Iteration
    public float smokeDelay = 2f; // Smoking delay
    public int maxNPCCount = 5; // MaxNUmber OF SMOker
    public float npcScale = 1f; // NPC Size Scale (Adjustable in Inspector)
    private int currentNPCCount = 0; // CurrentNumber_SMoker

    private void Start()
    {
        StartCoroutine(SpawnNPCWithInterval());
    }

    private IEnumerator SpawnNPCWithInterval()
    {
        while (true)
        {
            SpawnNPC();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnNPC()
    {
        // Select Npc
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Random Postion
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0, // ，y = 0
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

         // RandomRotation
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        // Instantiate NPC Smoker 
        GameObject npc = Instantiate(npcPrefab, spawnPosition, randomRotation);

         // Set NPC scale
        npc.transform.localScale = Vector3.one * npcScale;

        //Add Number OF SMoker
        currentNPCCount++;

        // RandomizeTheANimation
        Animator animator = npc.GetComponent<Animator>();
        if (animator != null)
        {
            float randomStartTime = Random.Range(0f, animator.GetCurrentAnimatorStateInfo(0).length);
            animator.Play(0, -1, randomStartTime);
        }

        // DelaySmoking_TRrigger
        StartCoroutine(SpawnSmokeEffectAfterDelay(npc));

        // Destroy
        npc.GetComponent<NPCController>().OnNPCDestroyed += HandleNPCDestroyed;
    }

    private IEnumerator SpawnSmokeEffectAfterDelay(GameObject npc)
    {
        yield return new WaitForSeconds(smokeDelay);

        // NpcLogic
        if (npc != null)
        {
            // Create the Smoking which connect to the SMoker
            ParticleSystem smoke = Instantiate(smokeEffect, npc.transform.position, Quaternion.identity);
            smoke.transform.SetParent(npc.transform); // Bindind to the smoker 
            smoke.Play();
        }
    }

      private void HandleNPCDestroyed()
    {
        currentNPCCount--;
    }
}
