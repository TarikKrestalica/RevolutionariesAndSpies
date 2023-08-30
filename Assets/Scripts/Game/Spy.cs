using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Spy : Player
{
    // Spy Collides with Vertex
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (controller.GameOver())
            return;

        if (collision.gameObject.tag != "Vertex")
            return;

        this.transform.SetParent(collision.transform, false);
        if (controller.GetRoundCount() >= 1)
        {
            if (!IsValidCollision(collision))
                return;

            if (letGo)  // Accounting for when the pawn does not snap back appropriately in place
            {
                // Snap pawn and update vertex counts
                this.transform.localPosition = Vector3.zero;
                this.transform.SetSiblingIndex(GetComponentInParent<Vertex>().GetRevCount());
                this.transform.parent.GetComponent<Vertex>().AddSpy();
                this.transform.parent.GetComponent<Vertex>().DisplayCounts();
                hasLeftStartingVertex = false;
                canMove = false;

                // If at least m revs on a vertex
                if (this.transform.parent.GetComponent<Vertex>().GetRevCount() >= controller.game.m)
                {
                    this.transform.parent.GetComponent<Vertex>().PlayWarning(false);
                }

                controller.IncrementMoveCount();
                int movesMade = controller.GetMoveCount();
                controller.UpdateMovesMade(movesMade);
                controller.UpdateMovesRemaining(controller.game.s - movesMade);

                if (movesMade != controller.game.s)
                {
                    return;
                }
                    
                controller.EndTurn();
            }
        }

        collisionDetected = true;
    }

    // Update counts and retrieve information when I leave the vertex the pawn started with
    void OnTriggerExit2D(Collider2D collision)
    {
        if (controller.GameOver())
            return;

        if (controller.GetPlayerTurn() != 'S')
            return;

        if (collision.gameObject.tag != "Vertex")
            return;

        if (letGo)
            return;

        collisionDetected = false;
        this.transform.SetParent(controller.spyBackground.transform, false);

        if (controller.GetRoundCount() == 0)
            return;

        if (hasLeftStartingVertex)
            return;

        collision.GetComponent<Vertex>().TakeAwaySpy();
        collision.GetComponent<Vertex>().DisplayCounts();
        GetPlayerInfo(collision);
        hasLeftStartingVertex = true;
    }

    // Spy Movement
    public void OnMouseDrag(BaseEventData data)
    {
        if (controller.GetPlayerTurn() != 'S')
            return;

        if (!canMove)
            return;

        letGo = false;
        Move(data);
    }

    // Where will my rev end up?
    public void OnMouseUp()
    {
        if (controller.GameOver())
            return;

        if (controller.GetPlayerTurn() != 'S')
            return;

        letGo = true;
        if (!canMove)
            return;

        if (controller.GetRoundCount() >= 1)
        {
            if (!hasLeftStartingVertex)
            {
                this.transform.localPosition = Vector3.zero;
                return;
            }
        }

        if (!collisionDetected) 
        {
            this.transform.position = startPosition;
            return;
        }

        // Snap pawn and update vertex counts
        this.transform.localPosition = Vector3.zero;
        this.transform.SetSiblingIndex(GetComponentInParent<Vertex>().GetRevCount());
        this.transform.parent.GetComponent<Vertex>().AddSpy();
        this.transform.parent.GetComponent<Vertex>().DisplayCounts();
        this.playerID = transform.parent.GetComponent<Vertex>().GetVertID();
        hasLeftStartingVertex = false;
        canMove = false;

        // If at least m revs on a vertex
        if (this.transform.parent.GetComponent<Vertex>().GetRevCount() >= controller.game.m)
        {
            this.transform.parent.GetComponent<Vertex>().PlayWarning(false);
        }

        controller.IncrementMoveCount();
        int movesMade = controller.GetMoveCount();
        controller.UpdateMovesMade(movesMade);
        controller.UpdateMovesRemaining(controller.game.s - movesMade);

        if (controller.GetMoveCount() != controller.game.s)
        {
            return;
        }
           
        controller.EndTurn();
    }
}
