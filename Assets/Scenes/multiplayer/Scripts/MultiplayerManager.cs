using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Collections.Generic;
using TMPro;

public class MultiplayerManager : MonoBehaviour
{
    //discovery reference
    [SerializeField] ServerDiscovery serverDiscovery;

    //websocket
    WebSocket webSocket;
    public string url = "ws://localhost:8080";
    public GameObject playerPrefab;
    public GameObject towerGameObject;
    GameObject player;
    public GameObject joinCanvas;
    public GameObject namePannel;
    public GameObject startPannel;

    public enemyObjectPool enemyObjectPool;

    public string playerName;

    public TMP_InputField nameInputField;

    //chat input
    [Header("Chat Input Field")]
    [SerializeField] private TMP_InputField chatInputField;

    public List<string> playersJoined = new List<string>();

    public Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    public Dictionary<string, WayPathMultiplayer> tiles = new Dictionary<string, WayPathMultiplayer>();

    async private void Start()
    {
        string serverIp = await serverDiscovery.FindServer();

        url = $"ws://{serverIp}:8080";

        webSocket = new WebSocket(url);

        Debug.Log(url);

        webSocket.OnOpen += () =>
        {
            Debug.Log("connected");
            // SpawnMyPlayer();
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

            Debug.Log(json);

            if (json.Contains("\"type\":\"joined\""))
            {
                PlayerStats res = JsonUtility.FromJson<PlayerStats>(json);

                SpawnMyPlayer();

                namePannel.SetActive(false);
                startPannel.SetActive(true);

                Debug.Log(json);
            }

            if (json.Contains("\"type\":\"playerJoined\""))
            {
                PlayerStats stats = JsonUtility.FromJson<PlayerStats>(json);

                Debug.Log(stats.name + " has joined Room");
            }

            if (json.Contains("\"type\":\"enemy\""))
            {
                enemyObjectPool.StartEnemies();
                joinCanvas.SetActive(false);

                EnableTowerPlacement();
            }

            if (json.Contains("\"type\":\"players\""))
            {
                PlayersList players = JsonUtility.FromJson<PlayersList>(json);

                playersJoined.Clear();

                foreach (string p in players.players)
                    playersJoined.Add(p);
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

            if (json.Contains("\"type\":\"chat\""))
            {
                ChatSystem chat = JsonUtility.FromJson<ChatSystem>(json);
                Debug.Log(chat.playerName + ": " + chat.chatMsg);
                OnMessageReceived(chat.playerName + ": " + chat.chatMsg);
            }
        };

        await webSocket.Connect();
    }

    public void Join()
    {
        playerName = nameInputField.text;

        if (string.IsNullOrEmpty(nameInputField.text))
        {
            Debug.LogError("Input field is empty");
            return;
        }

        PlayerStats playerStats = new PlayerStats
        {
            type = "join",
            name = playerName,
        };

        string json = JsonUtility.ToJson(playerStats);

        webSocket.SendText(json);
    }

    public void SendTowerPlacement(string towerType, Vector3 pos)
    {
        TowerDetails td = new TowerDetails
        {
            type = "tower",
            playerName = playerName,
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

        EnableTowerPlacement();

        EnemyPoolStats enemyPool = new EnemyPoolStats
        {
            type = "enemy",
        };

        string json = JsonUtility.ToJson(obj: enemyPool);
        webSocket.SendText(message: json);
    }

    public void SendChatMessage()
    {
        string message = chatInputField.text;
        ChatSystem chat = new ChatSystem
        {
            type = "chat",
            playerName = playerName,
            chatMsg = message
        };

        string json = JsonUtility.ToJson(chat);
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

        // player.name = "MyPlayer";
        player.name = playerName;
    }

    void OnMessageReceived(string message)
    {
        Debug.Log("OnMessageReceived: " + message);
    }

    //     void Update()
    //     {
    // #if !UNITY_WEBGL || !UNITY_EDITOR
    //         webSocket.DispatchMessageQueue();
    // #endif
    //     }

    void Update()
    {
        if (webSocket == null) return;

#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.DispatchMessageQueue();
#endif
    }

    // async void OnApplicationQuit()
    // {
    //     if (webSocket == null) return;
    //     await webSocket.Close();
    // }

    public void EnableTowerPlacement()
    {
        foreach (WayPathMultiplayer tile in tiles.Values)
        {
            tile.CanStartPlacing = true;
        }
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

    public class PlayersList
    {
        public string[] players;
    }

    public class ChatSystem
    {
        public string type;
        public string playerName;
        public string chatMsg;
    }
}
