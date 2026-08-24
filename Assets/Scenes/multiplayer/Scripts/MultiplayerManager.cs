using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Collections.Generic;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

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
    [SerializeField] private TMP_Text chatDisplayText;

    public List<string> playersJoined = new List<string>();

    public Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    public Dictionary<string, WayPathMultiplayer> tiles = new Dictionary<string, WayPathMultiplayer>();

    //> Connection Interval
    float networkIntervel = 5f;
    float networkTimeout = 15f;

    float lastMessageSent;
    float lastMessageRecivedTime;

    bool isConnected;

    Queue<GameObject> towerPool = new Queue<GameObject>();
    int poolSize = 20;

    //> Out going message batching
    List<string> outGoingQueue = new List<string>();


    async private void Start()
    {
        PoolTowers();

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

        webSocket.OnMessage += HandleMessages;

        await webSocket.Connect();
    }

    void PoolTowers()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var t = Instantiate(towerGameObject);
            t.SetActive(false);
            towerPool.Enqueue(t);
        }
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

    void HandleMessages(byte[] messages)
    {
        lastMessageRecivedTime = Time.time;
        string json = Encoding.UTF8.GetString(messages);

        Packet packet = JsonUtility.FromJson<Packet>(json);

        switch (packet.type)
        {
            case "joined":
                PlayerStats res = JsonUtility.FromJson<PlayerStats>(json);

                SpawnMyPlayer();

                namePannel.SetActive(false);
                startPannel.SetActive(true);
                break;
            case "playerJoined":
                PlayerStats stats = JsonUtility.FromJson<PlayerStats>(json);
                break;
            case "enemy":
                enemyObjectPool.StartEnemies();
                joinCanvas.SetActive(false);

                EnableTowerPlacement();
                break;
            case "players":
                PlayersList players = JsonUtility.FromJson<PlayersList>(json);

                playersJoined.Clear();

                foreach (string p in players.players)
                    playersJoined.Add(p);
                break;
            case "tower":
                TowerDetails td = JsonUtility.FromJson<TowerDetails>(json);
                Instantiate(towerGameObject, new Vector3(td.x, td.y, td.z), Quaternion.identity);
                break;
            case "tile":
                TileDetails tile = JsonUtility.FromJson<TileDetails>(json);

                if (tiles.TryGetValue(tile.id, out var w))
                    w.IsPlaceable = tile.isPlaced;
                break;
            case "chat":
                ChatSystem chat = JsonUtility.FromJson<ChatSystem>(json);
                OnMessageReceived(chat.playerName + ": " + chat.chatMsg);
                break;
        }
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
        chatDisplayText.text += playerName + ": " + message + "\n";
        chatInputField.text = "";
    }

    void SpawnRemoteTower(TowerDetails td)
    {
        Vector3 pos = new Vector3(td.x, td.y, td.z);
        Instantiate(towerGameObject, pos, Quaternion.identity);
        Debug.Log($"Remote tower placed by {td.playerName} at {pos}");
    }

    void SpawnMyPlayer()
    {
        player = Instantiate(
            playerPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        // player.name = "MyPlayer";
        player.name = playerName;
    }

    void OnMessageReceived(string message)
    {
        Debug.Log("OnMessageReceived: " + message);
        chatDisplayText.text += message + "\n";
    }

    void Update()
    {
        if (webSocket == null) return;

#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.DispatchMessageQueue();
#endif
    }

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

    public class Packet
    {
        public string type;
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
        public PlayerColor playerSelectedColor;
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

    public enum PlayerColor
    {
        Red,
        Orange,
        Yellow,
        Green

    }
}
