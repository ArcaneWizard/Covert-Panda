
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private Transform seeker;
    public Grid grid;

    public Vector2 randomDistanceWeightRange;

    private Dictionary<float, List<Node>> paths = new Dictionary<float, List<Node>>();
    private List<Node> chosenPath = new List<Node>();

    void Awake()
    {
        seeker = transform.GetChild(0).transform;
    }

    public IEnumerator FindMultiplePaths(float searchTime, Vector2 target)
    {
        paths.Clear();

        //search for a bunch of potential paths (note: paths are stored as end nodes)
        float time = Time.time + searchTime;
        while (Time.time < time)
        {
            FindPath(seeker.position, target);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //take one of the random possible end nodes and build the path out of it
        List<Node> endPoints = paths.ElementAt(UnityEngine.Random.Range(0, paths.Count)).Value;
        List<Node> path = retracePath(endPoints[0], endPoints[1]);

        //show the path visually and share it with state manager script
        grid.path = path;
        chosenPath = path;
    }

    public void FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.getClosestNodeToWorldPosition(startPos, 5);
        Node targetNode = grid.getClosestNodeToWorldPosition(targetPos, 5);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            DebugGUI.debugText4 = currentNode.transform.name + ", " + targetNode.transform.name;
            if (Transform.Equals(currentNode.transform, targetNode.transform))
            {
                bool saved = savePath(startNode, currentNode);
                //just to debug it in console
                if (saved) retracePath(startNode, currentNode);
                return;
            }

            foreach (Node neighbour in grid.getNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                float newMovementCostToNeighbour = currentNode.gCost + Random.Range(0.2f, 1);

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getWeightedDistanceBtwnNodes(neighbour, targetNode, Random.Range(randomDistanceWeightRange.x, randomDistanceWeightRange.y));
                    neighbour.pathID = currentNode.pathID + neighbour.transform.GetSiblingIndex();
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

            //yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private bool savePath(Node startNode, Node endNode)
    {
        if (!paths.ContainsKey(endNode.pathID) && paths.Count < 10)
        {
            paths.Add(endNode.pathID, new List<Node> { startNode, endNode });
            return true;
        }
        return false;
    }

    private List<Node> retracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        DebugGUI.debugText5 = $"hCost: {currentNode.hCost}, gCost: {currentNode.gCost}";

        while (!Transform.Equals(currentNode.transform, startNode.transform))
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;

            if (path.Count > 20)
            {
                debugPathInConsole(path);
                Debug.LogWarning("Out of Memory Exception about to happen");
                return null;
            }
        }

        path.Reverse();
        debugPathInConsole(path);

        return path;
    }

    public void debugPathInConsole(List<Node> path)
    {
        if (path.Count == 0)
            return;

        string s = $"{path[0].transform.name}";
        for (int i = 1; i < path.Count; i++)
            s += $", {path[i].transform.name}";

        Debug.Log(s);
    }

    public List<Node> getChosenPath()
    {
        return chosenPath;
    }

    private float getWeightedDistanceBtwnNodes(Node a, Node b, float multiplier)
    {
        Vector2 distanceBtwnVectors = a.transform.position - b.transform.position;
        return Mathf.Sqrt(distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y) * multiplier;
    }
}