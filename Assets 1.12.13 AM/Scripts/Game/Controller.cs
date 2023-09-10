using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class RevolutionariesAndSpies
{
    public Graph g;
    public int m;
    public int r;
    public int s;
}

// Have the UI communicate whose turn is it: https://youtu.be/KF3E-BAUVfM?si=uXAd1_Ze4Ifw91b3
[System.Serializable]
public class Panel 
{
    public Image panel;
    public TMP_Text text;
}

[System.Serializable]
public class PlayerColor
{
    public Color playerColor;
    public Color textColor;
}

[System.Serializable]
public class GameNotation
{
    public GameObject display;
    public TMP_Text game;
    public TMP_Text order;
}

[System.Serializable]
public class MoveDisplay
{
    public GameObject display;
    public GameObject movesMadeDescriptor;
    public GameObject movesLeftDescriptor;
}

public class Controller : MonoBehaviour
{
    public GameObject background;

    // Edges need to appear before the vertices
    [SerializeField]
    private GameObject vBackground;
    public static GameObject v;
    [SerializeField]
    private GameObject eBackground;
    public static GameObject e;

    // Collection of pawns
    public GameObject revBackground;
    public GameObject spyBackground;

    public RevolutionariesAndSpies game;
    public List<Graph> types;
    [SerializeField] private GameNotation n;

    public Player revolutionary;
    public Player spy;
    private List<GameObject> pawns = new List<GameObject>();
    public static List<GameObject> edges = new List<GameObject>();
    private Vector3 pawnScaleFactor;

    // Back And Forth Logic
    private char playerTurn = 'R';
    private int moveCount = 0;
    private int roundsPlayed = 0;
    private int roundsRemaining;
    private System.Random rnd = new System.Random();
    public Panel playerRev;
    public Panel playerSpy;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;
    public GameObject roundTracker;

    [SerializeField] private MoveDisplay moveDisplay;
    private Vector3 displaysScaleFactor;

    // Game Over 
    private bool isGameOver = false;
    public GameObject gameOver;

    public GameObject backToMenu;

    public virtual void Start()
    {
        GetBackgrounds();
        GetGame();

        // Prepare Representation
        game.g.SetController(this);
        game.g.RetrievePositions();
        game.g.PopulateVertices();
        game.g.PrepareDrawing();

        // Prepare Game
        game.g.CreateGraph();
        SetUpGameplay();
        SetMovementOnPawns(GetPlayerTurn());
        
    }

    public void GetBackgrounds()
    {
        v = vBackground;
        e = eBackground;
    }

    public virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))  // Alternating Gameplay
        {
            if (GameOver())
                return;

            if (roundsPlayed == 0)
                return;

            if(PauseGame.IsGamePaused())
                return;

            EndTurn();
        }

        if (PauseGame.IsGamePaused())
        {
            SetMovementOnPawns(false);
        }
        else
        {
            SetMovementOnPawns(GetPlayerTurn());
        }
    }

    // Advanced Feature: For an advanced user, we can have them choose their conflict rule and assignment function
    // Younger Audience: A choice to choose a path, cycle, star, complete graph, subdivided star

    // Take the chosen game, overwrite the previous iteration
    public void GetGame()
    {
        game.g = types.Where(Type => Type.name == DropDown.GetGraphChoice()).SingleOrDefault();
        game.g.SetOrder(GameMenu.GetSize());
        game.m = GameMenu.GetMeetingSize();
        game.r = GameMenu.GetNumberOfRevs();
        game.s = GameMenu.GetNumberOfSpies();
    }

    public void SetUpGameplay()
    {
        roundsRemaining = FindRoundsRemaining();
        SetupProperties();
        SetRoundsRemaining(roundsRemaining);
        UpdateMovesMade(moveCount);
        UpdateMovesRemaining(game.r);
        SetPlayerColors(playerRev, playerSpy);
    }

    int FindRoundsRemaining()
    {
        if(game.g.GetOrder() == 1)
        {
            return 1;
        }
        else if (game.g.GetOrder() <= 5)
        {
            return rnd.Next(game.g.GetOrder() - 1, game.g.GetOrder() + 1);
        }
        else if (game.g.GetOrder() <= 10)
        {
            return rnd.Next(game.g.GetOrder() - 3, game.g.GetOrder() + 1);
        }
        else if (game.g.GetOrder() <= 15)
        {
            return rnd.Next(game.g.GetOrder() - 6, game.g.GetOrder() + 1);
        }
        else
            return 0;
    }


    public static GameObject CreateObject(GameObject prefab, GameObject layout, Vector3 position)
    {
        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity);
        gameObject.transform.SetParent(layout.transform, false);
        return gameObject;
    }

    void Populate(GameObject prefab, GameObject layout, int limit, Vector3 position)
    {
        for (int i = 0; i < limit; i++)
        {
            GameObject pawn = CreateObject(prefab.gameObject, layout.gameObject, new Vector3(position.x, position.y, position.z));
            pawn.GetComponent<Player>().SetGameControllerReference(this);
            pawn.GetComponent<Player>().SetIdentifier(limit - (i + 1)); // IDs in descending order
            pawns.Add(pawn);
        }
    }

    // Set up the game's properties
    void SetupProperties()
    {
        string text = "";
        if (game.g.GetOrder() <= 10)
        {
            text = "RS(" + game.g.GetShortHand() + " ," + game.m + "," + game.r + "," + game.s + ")";  
        }
        else if (game.g.GetOrder() > 10)
        {
            text = "RS(" + game.g.GetShortHand() + "   ," + game.m + "," + game.r + "," + game.s + ")";
        }

         n.game.text = text;
         n.order.text = game.g.GetOrder().ToString();
         Populate(revolutionary.p, revBackground, game.r, new Vector3(-725, 130, 0));
         Populate(spy.p, spyBackground, game.s, new Vector3(-575, 130, 0));
    }

    // Who's turn is it(told explicitly?)
    public void SetPlayerColors(Panel newPlayer, Panel oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.playerColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.playerColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    public char GetPlayerTurn()
    {
        return playerTurn;
    }

    void ChangeSides()
    {
        playerTurn = playerTurn == 'R' ? 'S' : 'R';
        if (playerTurn == 'R')
            SetPlayerColors(playerRev, playerSpy);
        else
            SetPlayerColors(playerSpy, playerRev);

        if (roundsPlayed < 1)
            return;

        ReshufflePawns();

    }

    // Which vertices need to be covered or not?(For turn of spy)
    void UpdateVertices()
    {
        for(int i = 0; i < game.g.GetVertices().Count; i++)
        {
            if (game.g.GetVertices()[i].GetComponent<Vertex>().GetRevCount() < game.m)  
                continue;

            if (game.g.GetVertices()[i].GetComponent<Vertex>().GetSpyCount() >= 1)
                continue;
                
            game.g.GetVertices()[i].GetComponent<Vertex>().PlayWarning(true);
        }
    }

    // Active pawns in front, inactive bestowed behind
    void ReshufflePawns()
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].GetComponentInChildren<TMP_Text>().text == playerTurn.ToString())
                continue;

            pawns[i].transform.SetSiblingIndex(0);
        }
    }

    public List<GameObject> GetPawns()
    {
        return pawns;
    }

    public void SetMovementOnPawns(char playerTurn)
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].GetComponentInChildren<TMP_Text>().text == playerTurn.ToString())
            {
                pawns[i].GetComponent<Player>().SetMovement(true);
            }
            else
            {
                pawns[i].GetComponent<Player>().SetMovement(false);
            }
        }
    }

    public void SetMovementOnPawns(bool toggle)
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            pawns[i].GetComponent<Player>().SetMovement(toggle);
        }
    }

    public void EndTurn()
    {
        if (playerTurn == 'S')  // Have the revs and spies reached their goals?
        {
            roundsPlayed++;
            --roundsRemaining;
            SetRoundsRemaining(roundsRemaining);

            if (isRevWin())
            {
                GameOver("Revs");
                return;
            }
            else if (roundsRemaining <= 0 || isSpyOnEachVertex())
            {
                GameOver("Spies");
                return;
            }        
        }

        UpdateVertices();
        if (isGameOver)
            return;

        ChangeSides();
        SetMovementOnPawns(GetPlayerTurn());

        // Update move counts, set the moves remaining dependent on the current player turn
        moveCount = 0;
        UpdateMovesMade(moveCount);

        if (playerTurn == 'R')
        {
            UpdateMovesRemaining(game.r);
        }
        else
        {
            UpdateMovesRemaining(game.s);
        }
    }

    // Revs make a meeting of m with no spy
    public bool isRevWin()
    {
        for (int i = 0; i < game.g.GetOrder(); i++)
        {
            if (game.g.GetVertices()[i] == null)
                continue;

            if (game.g.GetVertices()[i].GetComponent<Vertex>().GetRevCount() >= game.m)
            {
                if (game.g.GetVertices()[i].GetComponent<Vertex>().GetSpyCount() <= 0)
                    return true;
            }
        }
        return false;
    }

    // Scenario: At initial round, more spies than vertices
    public bool isSpyOnEachVertex()
    {
        for(int i = 0; i < game.g.GetOrder(); i++)
        {
            if (game.g.GetVertices()[i] == null)
                continue;

            if (game.g.GetVertices()[i].GetComponent<Vertex>().GetSpyCount() < 1)
                return false;
        }

        return true;
    }

    public void IncrementMoveCount()
    {
        ++moveCount;
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void SetMoveCount(int num)
    {
        moveCount = num;
    }

    public bool GameOver()
    {
        return isGameOver;
    }

    public void SetGameOver(bool toggle)
    {
        isGameOver = toggle;
    }

    public int GetRoundCount()
    {
        return roundsPlayed;
    }

    // Active player color determines the winner
    void GameOver(string winner)
    {
        SetGameOver(true);
        if (winner == "Revs") 
        {
            SetPlayerColors(playerRev, playerSpy);
        }

        SetMovementOnPawns(false);
        gameOver.SetActive(true);
        gameOver.GetComponentInChildren<TMP_Text>().text = winner + " win!";
        backToMenu.SetActive(true);
    }

    public void SetRoundCount(int num)
    {
        roundsPlayed = num;
    }

    public void SetRoundsRemaining(int num)
    {
        roundTracker.GetComponentInChildren<TMP_Text>().text = "Rounds Left: " + num;
        roundsRemaining = num;
    }

    public int GetRoundsLeft()
    {
        return roundsRemaining;
    }

    public void UpdateMovesRemaining(int curMovesLeft)
    {
        moveDisplay.movesLeftDescriptor.gameObject.GetComponent<TMP_Text>().text = " Left: " + curMovesLeft.ToString();
    }

    public void UpdateMovesMade(int curMovesMade)
    {
        moveDisplay.movesMadeDescriptor.gameObject.GetComponentInChildren<TMP_Text>().text = " Made: " + curMovesMade.ToString();
    }

    // Take away objects from the hierarchy
    void Destroy(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
            Destroy(objects[i]);
    }

    // Prepare for the next game
    public void ClearDrawing()
    {
        Destroy(game.g.GetVertices());
        Destroy(pawns);
        Destroy(edges);
        game.g.GetEdges().Clear();
        game.g.r.hostSet.Clear();
        game.g.r.assignmentFunction.Clear();
        game.g.GetPositions().Clear();
    }

    public GameObject GetVertexBackground()
    {
        return vBackground;
    }

    public GameObject GetEdgeBackground()
    {
        return eBackground;
    }

    public GameObject GetRevBackground()
    {
        return revBackground;
    }

    public GameObject GetSpyBackground()
    {
        return spyBackground;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
