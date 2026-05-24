using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;


[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLables : MonoBehaviour
{

    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.grey;
    [SerializeField] Color exploredColor = Color.black;
    [SerializeField] Color pathColor = new Color(1.0f, 0.5f, 0.0f);

    TextMeshPro label;
    Vector2Int coordinates = new Vector2Int();

    PathManager pathManager;


    void Awake()
    {
        pathManager = FindObjectOfType<PathManager>();
        label = GetComponent<TextMeshPro>();
        // label.enabled = false;

        if (label == null)
        {
            Debug.LogError("TextMeshPro component not found on the GameObject or its children.");
            return;
        }


        DisplayCoordinaes();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            DisplayCoordinaes();
            UpdateGameObjectName();
            label.enabled = true;
        }

        SetLableColors();
        ToggleText();
    }

    void SetLableColors()
    {

        if (pathManager == null) { return; }

        Node node = pathManager.GetNode(coordinates);

        if (node == null) { return; }

        if (!node.isWalkable)
        {
            label.color = blockedColor;
        }
        else if (node.isPath)
        {
            label.color = pathColor;
        }
        else if (node.isExplored)
        {
            label.color = exploredColor;
        }
        else
        {
            label.color = defaultColor;
        }
    }

    void ToggleText()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.IsActive();
        }
    }


    void DisplayCoordinaes()
    {
        float gridX = 1f;
        float gridZ = 1f;

        // coordinates.x = Mathf.RoundToInt(transform.parent.position.x / UnityEditor.EditorSnapSettings.move.x);
        // coordinates.y = Mathf.RoundToInt(transform.parent.position.z / UnityEditor.EditorSnapSettings.move.z);

#if UNITY_EDITOR
        gridX = UnityEditor.EditorSnapSettings.move.x;
        gridZ = UnityEditor.EditorSnapSettings.move.z;
#endif

        coordinates.x = Mathf.RoundToInt(transform.parent.position.x / gridX);
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z / gridZ);

        label.text = coordinates.x + "," + coordinates.y;
    }

    void UpdateGameObjectName()
    {
        transform.parent.name = coordinates.ToString();
    }
}
