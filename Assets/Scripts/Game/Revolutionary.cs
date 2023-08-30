using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Revolutionary : Player
{
    // Rev Collides with Vertex
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
                this.transform.localPosition = Vector3.zero;
                this.transform.SetSiblingIndex(GetComponentInParent<Vertex>().GetSpyCount());
                this.transform.parent.GetComponent<Vertex>().AddRevolutionary();
                this.transform.parent.GetComponent<Vertex>().DisplayCounts();
                hasLeftStartingVertex = false;
                canMove = false;

                controller.IncrementMoveCount();
                int movesMade = controller.GetMoveCount();
                controller.UpdateMovesMade(movesMade);
                controller.UpdateMovesRemaining(controller.game.r - movesMade);

                if (movesMade != controller.game.r)
                {
                    return;
                }
                   
                controller.EndTurn();
            }
        }

        collisionDetected = true;
    }

    // Update counts when I leave my starting vertex once and that's it
    void OnTriggerExit2D(Collider2D collision)
    {
        if (controller.GameOver())
            return;

        if (controller.GetPlayerTurn() != 'R')
            return;

        if (collision.gameObject.tag != "Vertex")
            return;

        if (letGo)
            return;
 
        collisionDetected = false;
        this.transform.SetParent(controller.revBackground.transform, false);

        if (controller.GetRoundCount() == 0)
            return;

        if (hasLeftStartingVertex)
            return;

        hasLeftStartingVertex = true;
        collision.GetComponent<Vertex>().TakeAwayRevolutionary();

        if (collision.GetComponent<Vertex>().GetRevCount() < controller.game.m)
        {
            collision.GetComponent<Vertex>().PlayWarning(false);
        }

        collision.GetComponent<Vertex>().DisplayCounts();
        GetPlayerInfo(collision);
        
    }

    // Rev Movement
    public void OnMouseDrag(BaseEventData data)
    {
        if (controller.GetPlayerTurn() != 'R')
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

        if (controller.GetPlayerTurn() != 'R')
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
        this.transform.SetSiblingIndex(GetComponentInParent<Vertex>().GetSpyCount());
        this.transform.parent.GetComponent<Vertex>().AddRevolutionary();
        this.transform.parent.GetComponent<Vertex>().DisplayCounts();
        this.playerID = transform.parent.GetComponent<Vertex>().GetVertID();
        hasLeftStartingVertex = false;
        canMove = false;

        controller.IncrementMoveCount();
        int movesMade = controller.GetMoveCount();
        controller.UpdateMovesMade(movesMade);
        controller.UpdateMovesRemaining(controller.game.r - movesMade);

        if (movesMade != controller.game.r)
        {
            return;
        }

        controller.EndTurn();

    }
    
}
