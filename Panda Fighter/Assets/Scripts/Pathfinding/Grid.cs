using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour
{
    private Dictionary<Transform, Node> grid = new Dictionary<Transform, Node>();

    [SerializeField]
    private Transform nodeCollection, alien;
    [SerializeField]
    private LayerMask decisionZoneMask;

    private float leastDistanceSoFar, colliderDistance;
    private Transform closestNode;

    void Awake()
    {
        foreach (Transform transform in nodeCollection)
            grid.Add(transform, new Node(transform));
    }

    public Node getClosestNodeToWorldPosition(Vector2 entityPos, float radiusCheck)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(entityPos, radiusCheck, decisionZoneMask);
        leastDistanceSoFar = 400;

        foreach (Collider2D hitCollider in hitColliders)
        {
            colliderDistance = getSquaredDistanceBtwnVectors(hitCollider.transform.position, entityPos);

            if (colliderDistance < leastDistanceSoFar)
            {
                leastDistanceSoFar = colliderDistance;
                closestNode = hitCollider.transform;
            }
        }

        if (leastDistanceSoFar == 400)
            return getClosestNodeToWorldPosition(entityPos, radiusCheck + 5);
        else
        {
            if (grid.ContainsKey(closestNode.transform))
                return grid[closestNode.transform];
            else
            {
                Debug.Log(closestNode.transform.name);
                return grid[closestNode.transform];
            }
        }
    }

    public List<Node> getNeighbors(Node node)
    {
        List<Node> connectedNodes = new List<Node>();
        foreach (Transform child in node.transform)
        {
            if (child.gameObject.activeSelf)
                connectedNodes.Add(grid[child.transform.GetComponent<TestingTrajectories>().chainedDirectionZone.transform]);
        }

        return connectedNodes;
    }

    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Node alienNode = getClosestNodeToWorldPosition(alien.position, 5);
            foreach (KeyValuePair<Transform, Node> n in grid)
            {
                Gizmos.color = Color.black;
                if (path != null)
                {
                    foreach (Node pathNode in path)
                    {
                        if (Transform.Equals(pathNode.transform, n.Value.transform))
                            Gizmos.color = Color.white;
                    }
                }
                if (Transform.Equals(alienNode.transform, n.Value.transform))
                    Gizmos.color = Color.cyan;
                Gizmos.DrawCube(n.Value.transform.position, new Vector2(0.5f, 0.5f));
            }
        }
    }
}