using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class WayPath : MonoBehaviour
{
    [SerializeField] bool isPlaceable;

    [SerializeField] Tower tower;

    [SerializeField] MultiplayerManager multiplayerManager;

    public bool IsPlaceable
    {
        get
        {
            return isPlaceable;
        }
    }


    void OnMouseDown()
    {

        // if (isPlaceable)
        // {
        //     bool isPlaced = tower.TowerInstantiate(tower, transform.position);
        //     isPlaceable = !isPlaced;
        // }

        if (!isPlaceable) return;

        bool isPlaced = tower.TowerInstantiate(tower, transform.position);

        if (isPlaced)
        {
            isPlaceable = false;

            // ✅ NEW: broadcast to all other players
            multiplayerManager?.SendTowerPlacement(tower.name, transform.position);
        }
    }
}
