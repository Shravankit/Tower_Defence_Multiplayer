using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Collections.Generic;
using TMPro;

public class MultiplayerManager : MonoBehaviour
{
    WebSocket webSocket;
    public string url = "ws://localhost:8080";
    public GameObject playerPrefab;
    public GameObject towerGameObject;
    GameObject player;
    public GameObject joinCanvas;

    public enemyObjectPool enemyObjectPool;

    public Dictionary<string, WayPathMultiplayer> tiles = new Dictionary<string, WayPathMultiplayer>();

    async private void Start()
    {
        webSocket = new WebSocket(url);

        webSocket.OnOpen += () =>
        {
            Debug.Log("connected");
            SpawnMyPlayer();
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log(e);
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);
        };

        webSocket.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);

            if (json.Contains("\"type\":\"enemy\""))
            {
                enemyObjectPool.StartEnemies();
                joinCanvas.SetActive(false);
            }

            if (json.Contains("\"type\":\"tower\""))
            {
                TowerDetails td = JsonUtility.FromJson<TowerDetails>(json);
                Instantiate(towerGameObject, new Vector3(td.x, td.y, td.z), Quaternion.identity);
            }

            if (json.Contains("\"type\":\"tile\""))
            {
                TileDetails tile = JsonUtility.FromJson<TileDetails>(json);

                if (tiles.TryGetValue(tile.id, out var w))
                    w.IsPlaceable = tile.isPlaced;
            }
        };

        await webSocket.Connect();
    }

    public void SendTowerPlacement(string towerType, Vector3 pos)
    {
        TowerDetails td = new TowerDetails
        {
            type = "tower",
            playerName = player.name,
            x = pos.x,
            y = pos.y,
            z = pos.z,
        };

        string json = JsonUtility.ToJson(td);
        webSocket.SendText(json);
    }

    public void SendTileDetails(string id, bool isPlaced)
    {
        TileDetails tile = new TileDetails
        {
            type = "tile",
            id = id,
            isPlaced = isPlaced
        };
        string json = JsonUtility.ToJson(tile);
        webSocket.SendText(json);
    }

    public void SendStartStatus()
    {
        enemyObjectPool.StartEnemies();
        joinCanvas.SetActive(false);

        EnemyPoolStats enemyPool = new EnemyPoolStats
        {
            type = "enemy",
        };

        string json = JsonUtility.ToJson(enemyPool);
        webSocket.SendText(json);
    }

    void SpawnRemoteTower(TowerDetails td)
    {
        Vector3 pos = new Vector3(td.x, td.y, td.z);
        Instantiate(towerGameObject, pos, Quaternion.identity);
        Debug.Log($"Remote tower placed by {td.playerName} at {pos}");
    }

    void SpawnMyPlayer()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(-5f, 5f),
            0f,
            Random.Range(-5f, 5f)
        );

        player = Instantiate(
            playerPrefab,
            spawnPos,
            Quaternion.identity
        );

        player.name = "MyPlayer";
    }

    void Update()
    {
#if !UNITY_WEBGL || !UNITY_EDITOR
        webSocket.DispatchMessageQueue();
#endif
    }

    async void OnApplicationQuit()
    {
        await webSocket.Close();
    }

    public class TowerDetails
    {
        public string type;
        public string playerName;
        public float x;
        public float y;
        public float z;
    }

    public class TileDetails
    {
        public string type;
        public string id;
        public bool isPlaced;
    }

    public class PlayerStats
    {
        public string type;
        public string name;
    }

    public class EnemyPoolStats
    {
        public string type;
    }
}
