using System;
using UnityEngine;

public class WayPathMultiplayer : MonoBehaviour
{
    [SerializeField] bool isPlaceable;
    [SerializeField] TowerMultiplayer towerMultiplayer;

    [SerializeField] MultiplayerManager multiplayerManager;

    [Header("MultiplayerId")]
    [SerializeField] string ID;

    public bool IsPlaceable
    {
        get
        {
            return isPlaceable;
        }
        set
        {
            isPlaceable = value;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = this.gameObject.name;

            multiplayerManager.tiles.Add(ID, this);
        }
    }


    private void OnMouseDown()
    {
        if (!IsPlaceable) return;

        bool isTowerPlaced = towerMultiplayer.TowerInstantiate(towerMultiplayer, transform.position);

        if (isTowerPlaced)
        {
            IsPlaceable = false;

            multiplayerManager.SendTowerPlacement(towerMultiplayer.name, transform.position);
            multiplayerManager.SendTileDetails(ID, IsPlaceable);
        }
    }
}