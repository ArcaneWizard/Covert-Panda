using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekDestination : IState
{
    private AI ai;
    private Transform aiBody;
    private AI_FollowPath pathFollower;
    private AI_LookAround lookAround;

    private bool setDestinationManually;
    private Transform possibleDestinations, manualDestination;
    private Vector2 destination;

    public SeekDestination(AI ai, AI_FollowPath AI_pathFollower, Transform possibleDestinations,
            Transform manualDestination, AI_LookAround lookAround)
    {
        this.ai = ai;
        this.pathFollower = AI_pathFollower;
        this.aiBody = ai.transform.GetChild(0);
        this.possibleDestinations = possibleDestinations;
        this.manualDestination = manualDestination;
        this.lookAround = lookAround;

        setDestinationManually = true;
    }

    public void OnEnter() => beginNewJourney();

    public void Tick()
    {
        pathFollower.tick();

        if (getSquaredDistanceBtwnVectors(aiBody.position, destination) < 16f)
        {
            pathFollower.endJourney();
            Debug.Log("journey over");
        }

        if (pathFollower.gotLost())
        {
            beginNewJourney();
            Debug.Log("AI got lost");
        }
    }

    public void OnExit() => pathFollower.endJourney();

    private void beginNewJourney()
    {
        destination = (setDestinationManually)
            ? manualDestination.position
            : possibleDestinations.GetChild(UnityEngine.Random.Range(0, possibleDestinations.childCount)).position;

        ai.StartCoroutine(pathFollower.startJourney(destination));
    }

    private float getSquaredDistanceBtwnVectors(Vector2 a, Vector2 b)
    {
        Vector2 distanceBtwnVectors = a - b;
        return distanceBtwnVectors.x * distanceBtwnVectors.x + distanceBtwnVectors.y * distanceBtwnVectors.y;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the destination position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destination, 1);
    }
#endif
}
