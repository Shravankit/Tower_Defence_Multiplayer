using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;
    [SerializeField] Vector2Int destinationCoordinates;


    Node startNode;
    Node destinationNode;
    Node currentSearchNode;

    Queue<Node> frontier = new Queue<Node>();

    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };


    PathManager pathManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    void Awake()
    {
        pathManager = FindObjectOfType<PathManager>();

        if (pathManager != null)
        {
            grid = pathManager.Grid;
        }
    }

    void Start()
    {
        startNode = pathManager.Grid[startCoordinates];
        destinationNode = pathManager.Grid[destinationCoordinates];

        BredthFirstSearch();
        BuildPath();
    }

    void ExploringNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborCoordinates = currentSearchNode.coordinates + direction;

            if (grid.ContainsKey(neighborCoordinates))
            {
                neighbors.Add(grid[neighborCoordinates]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.coordinates) && neighbor.isWalkable)
            {
                neighbor.connectedTo = currentSearchNode;
                reached.Add(neighbor.coordinates, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }


    //search algorithm for searaching coordinate in the game..............................
    void BredthFirstSearch()
    {
        bool isRunning = true;

        frontier.Enqueue(startNode);

        reached.Add(startCoordinates, startNode);

        while (frontier.Count > 0 && isRunning)
        {
            try
            {
                currentSearchNode = frontier.Dequeue();
                currentSearchNode.isExplored = true;
                ExploringNeighbors();

                if (currentSearchNode.coordinates == destinationCoordinates)
                {
                    isRunning = false;
                }

                // Debug.Log("Code is Working");
            }
            catch (System.Exception)
            {
                Debug.LogError("Exception in reading the code");
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }
}
