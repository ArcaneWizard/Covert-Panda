

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

    private Dictionary<float, List<Node>> pathsFound = new Dictionary<float, List<Node>>();
    private List<Node> chosenPath = new List<Node>();
    private HashSet<Node> universalClosedSet = new HashSet<Node>();
    private System.Random random;

    void Awake()
    {
        seeker = transform.GetChild(0).transform;
        random = new System.Random();
    }

    public List<Node> getChosenPath() => chosenPath;

    public IEnumerator FindMultiplePaths(float searchTime, Vector2 target)
    {
        pathsFound.Clear();
        universalClosedSet.Clear();
        chosenPath = null;

        //search for a bunch of potential paths (note: paths are stored as end nodes)
        float time = Time.time + searchTime;
        while (Time.time < time)
        {
            findOnePath(seeker.position, target);
            yield return new WaitForSeconds(0.01f);
        }

        List<Node> path = pathsFound.ElementAt(random.Next(0, pathsFound.Count)).Value;

        //show the path visually and share it with state manager script
        showPathOnGrid(path);
        chosenPath = path;
    }

    private void findOnePath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = grid.getClosestNodeToWorldPosition(startPos, 5);
        Node targetNode = grid.getClosestNodeToWorldPosition(targetPos, 5);

        Debug.Log(targetNode.transform.name);

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

            if (Transform.Equals(currentNode.transform, targetNode.transform))
            {
                savePathIfUnique(startNode, currentNode);
                universalClosedSet.UnionWith(closedSet);
                return;
            }

            foreach (Node neighbour in grid.getNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                //the very first neighbor nodes of the startNode should have the same g Cost (equal probability)
                float newMovementCostToNeighbour = !Transform.Equals(currentNode, startNode)
                    ? currentNode.gCost + Random.Range(0.2f, 0.4f)
                    : currentNode.gCost + 0.1f;

                //incentivize choosing nodes that haven't been used before in previously found paths
                if (!universalClosedSet.Contains(neighbour) && pathsFound.Count > 2)
                    newMovementCostToNeighbour = -0.1f;

                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getWeightedDistanceBtwnNodes(neighbour, targetNode, Random.Range(randomDistanceWeightRange.x, randomDistanceWeightRange.y));

                    if (!universalClosedSet.Contains(neighbour) && pathsFound.Count > 2)
                        neighbour.hCost = 0.02f;

                    neighbour.pathID = currentNode.pathID + neighbour.transform.GetSiblingIndex();
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    private void savePathIfUnique(Node startNode, Node endNode)
    {
        if (!pathsFound.ContainsKey(endNode.pathID) && pathsFound.Count < 10)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (!Transform.Equals(currentNode.transform, startNode.transform))
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;

                if (path.Count > 20)
                    return;
            }

            path.Reverse();
            pathsFound.Add(endNode.pathID, path);
            showPathOnGrid(path);
        }
    }

    public void debugPathInConsole(List<Node> path)
    {
        if (path.Count == 0)
            return;

        string s = $"{path[0].transform.name}";
        for (int i = 1; i < path.Count; i++)
            s += $", {path[i].transform.name}";

        Debug.Log(s);
        // Debug.Log($"{path[path.Count - 1].gCost} and {path[path.Count - 1].hCost}");
    }

    private void showPathOnGrid(List<Node> path)
    {
        grid.path = path;
    }

    private float getWeightedDistanceBtwnNodes(Node a, Node b, float multiplier)
    {
        Vector2 distanceBtwnVectors = a.transform.position - b.transform.position;
        return Mathf.Sqrt(distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y) * multiplier;
    }


    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }

}