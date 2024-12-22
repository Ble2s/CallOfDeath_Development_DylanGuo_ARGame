using UnityEngine;
using Meta.XR.MRUtilityKit;
public class SmokerSpawnerWithARINteraction : MonoBehaviour
{
    public float spawnTimer = 1;
    public GameObject prefabToSpawn;
    
    public float minEdgeDistance = 0.3f;
    public MRUKAnchor.SceneLabels spawnLabels;
    public float normalOffset;

    public int spawnTry = 1000;
    private float timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!MRUK.Instance && MRUK.Instance.IsInitialized) 
            return;

        timer += Time.deltaTime;
        if (timer > spawnTimer)
        {
            SpawnGhost();
            timer -= spawnTimer;
        }
    }

    public void SpawnGhost()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();

        int currentTry = 0;

        while(currentTry < spawnTry)
        {
            bool hasFoundPosition = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, minEdgeDistance, new LabelFilter(spawnLabels), out Vector3 pos, out Vector3 norm);

            if(hasFoundPosition)
            {
                Vector3 randomPositionNormalOffest = pos + norm  * normalOffset;
                randomPositionNormalOffest.y = 0;

                Instantiate(prefabToSpawn,randomPositionNormalOffest,Quaternion.identity);

                return; 
            }
            else
            {
                currentTry++;
            }
        }

        
      
    }
}
