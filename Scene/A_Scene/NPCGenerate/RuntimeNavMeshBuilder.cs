using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Meta.XR.MRUtilityKit;
using System.Collections;
public class RuntimeNavMeshBuilder : MonoBehaviour
{

    private NavMeshSurface navmeshSurface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navmeshSurface = GetComponent<NavMeshSurface>();
        MRUK.Instance.RegisterSceneLoadedCallback(BuidNavmesh);
    }

    public void BuidNavmesh()
    {
        StartCoroutine(BuidNavmeshRoutine());
    }
    
    public IEnumerator BuidNavmeshRoutine()
    {
        yield return new WaitForEndOfFrame();
        navmeshSurface.BuildNavMesh();
    }
}
