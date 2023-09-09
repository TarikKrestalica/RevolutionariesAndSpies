using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Player : MonoBehaviour
{
    public GameObject p;
    [SerializeField]
    private TMP_Text identifier;  // Identify pawn by its index

    // Which vertex is the pawn currently located?
    protected int playerID;
    protected HashSet<int> m_subset = new HashSet<int>();

    protected Vector3 startPosition;
    protected Controller controller;

    // Movement Logic
    protected bool canMove = true;
    protected bool collisionDetected = false;
    protected bool hasLeftStartingVertex = false;  // Obtain info once and only once
    protected bool letGo = false;  // Pawn interaction

    void Start()
    {
        startPosition = this.p.transform.position;
    }

    protected HashSet<int> GetSubsetID()
    {
        return m_subset;
    }

    public void SetGameControllerReference(Controller c)
    {
        controller = c;
    }

    public void SetMovement(bool toggle)
    {
        canMove = toggle;
    }

    public void SetIdentifier(int aID)
    {
        identifier.text = aID.ToString();
    }

    public void SetCollisionDetected(bool toggle)
    {
        collisionDetected = false;
    }

    protected void Move(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position = pointerData.position;
        this.transform.position = position;
    }

    protected void GetPlayerInfo(Collider2D other)
    {
        m_subset = other.GetComponent<Vertex>().GetSubset();
        startPosition = other.GetComponent<RectTransform>().position;
    }

    // Must collide with adjacent or same vertex
    protected bool IsValidCollision(Collider2D collider)
    {
        HashSet<int> result = new HashSet<int>(collider.GetComponent<Vertex>().GetSubset());
        result.IntersectWith(m_subset);

        if (result.Count < controller.game.g.r.conflictRule)
            return false;

        return true;
    }

}
