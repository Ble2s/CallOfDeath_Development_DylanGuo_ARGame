using UnityEngine;
using System;

public class NPCController : MonoBehaviour
{
    public event Action OnNPCDestroyed; // Destroy

    private void OnDestroy()
    {
        OnNPCDestroyed?.Invoke();
    }
}