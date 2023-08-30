using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : Controller
{
    [SerializeField] private GameObject gameplayTrigger;
    [SerializeField] private GameObject initialRoundRevWinTrigger;
    [SerializeField] private GameObject postInitialRoundTrigger;
    private bool hasGamePlayReset = false;

    private DialogueManager dm;
    
    public override void Start()
    {
        gameplayTrigger.GetComponent<DialogueTrigger>().TriggerDialogue();
        dm = FindObjectOfType<DialogueManager>();
        GetBackgrounds();

        // Prepare Representation
        game.g.SetController(this);
        game.g.RetrievePositions();
        game.g.PopulateVertices();
        game.g.PrepareDrawing();

        // Prepare Game
        game.g.CreateGraph();
        SetUpGameplay();
        SetMovementOnPawns(false);
    }

    // Control when alternate gameplay occurs(When revs and spies have permission to move, when gameplay explaination is done)
    public override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))  // Alternating Gameplay
        {
            if (GetRoundCount() == 0)
                return;

            if (!postInitialRoundTrigger.activeInHierarchy)
                return;

            if (dm.GetCurrentLine() < 4)
                return;

            if (dm.GetCurrentLine() == 4 || dm.GetCurrentLine() == 5)
            {
                EndTurn();
                return;
            }

            if (dm.StoryFinished())
            {
                EndTurn();
                return;
            }   
        }

        if (gameplayTrigger.activeInHierarchy)
        {
            // Flashing vertices
            if (dm.GetCurrentLine() == 2)
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 2);
            }
            // Stop flashing vertices, flashing edges
            else if (dm.GetCurrentLine() == 3)
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 3);
                SetAnimationOnCollection(edges, "CurrentLine", 3);
            }
            // Stop flashing edges
            else if (dm.GetCurrentLine() == 4)
            {
                SetAnimationOnCollection(edges, "CurrentLine", 4);
            }
            // Highlight Vertex ID
            else if (dm.GetCurrentLine() == 5)
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 5);
            }
            // Highlight Revolutionary Count, stop ID animation
            else if (dm.GetCurrentLine() == 6)
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 6);
            }
            // Highlight Spy Count, stop Revolutionary Count animation
            else if (dm.GetCurrentLine() == 7)
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 7);
            }
            else if (dm.GetCurrentLine() == 8)  // Stop spy Count animation
            {
                SetAnimationOnCollection(game.g.GetVertices(), "CurrentLine", 8);
            }

            else if (dm.GetCurrentLine() == 11)  // Better explain the objective
            {
                dm.SetTextBox("");
                string sentence = "In this case, we seek " + game.m.ToString() + " revolutionaries alone at the end of the round.";
                dm.SetTextBox(sentence);
                dm.GetContinueButton().SetActive(true);
            }

            // Prompt rev movement before continuing
            else if (dm.GetCurrentLine() == 17)
            {
                SetMovementOnPawns(true);
                dm.GetContinueButton().SetActive(false);
                if (GetPlayerTurn() != 'R')
                {
                    dm.DisplaySentence();  // Display next sentence
                }
            }

            // Prompt spy movement before continuing
            else if (dm.GetCurrentLine() == 18)
            {
                dm.GetDoneButton().SetActive(false);
                if (GetRoundCount() < 1)
                {
                    return;
                }

                gameplayTrigger.SetActive(false);
                if (GameOver()) // Unoptimal gameplay
                {
                    initialRoundRevWinTrigger.SetActive(true);
                    initialRoundRevWinTrigger.GetComponent<DialogueTrigger>().TriggerDialogue();
                    return;
                }

                postInitialRoundTrigger.SetActive(true);
                postInitialRoundTrigger.GetComponent<DialogueTrigger>().TriggerDialogue();
            }
        }

        else if (initialRoundRevWinTrigger.activeInHierarchy)  // Reset Gameplay, warn the team of spies that a better move is ahead
        {
            if (dm.GetCurrentLine() == 1)
                backToMenu.SetActive(false);

            if (dm.GetCurrentLine() < 4)
                return;

            if(!hasGamePlayReset)  
            {
                dm.EndDialogue();
                ResetGameplay();
                hasGamePlayReset = true;
            }

            if (GetRoundCount() < 1)
            {
                return;
            }

            if (GameOver())  // Spy still fails to guard the meeting of m
            {
                hasGamePlayReset = false;
                initialRoundRevWinTrigger.GetComponent<DialogueTrigger>().TriggerDialogue();
                return;
            }

            initialRoundRevWinTrigger.SetActive(false);
            postInitialRoundTrigger.SetActive(true);
            postInitialRoundTrigger.GetComponent<DialogueTrigger>().TriggerDialogue();
            dm.GetDoneButton().SetActive(false);
        }

        else if (postInitialRoundTrigger.activeInHierarchy)
        {
            if(dm.GetCurrentLine() == 1 || dm.GetCurrentLine() == 6)
                SetMovementOnPawns(false);

            // Prompt post-initial rev movement before continuing
            if (dm.GetCurrentLine() == 4)
            {
                dm.GetContinueButton().SetActive(false);
                SetMovementOnPawns(true);
                if (GetPlayerTurn() == 'R')
                    return;

                dm.DisplaySentence();
            }

            // Prompt post-initial spy movement before continuing
            else if (dm.GetCurrentLine() == 5)
            {
                dm.GetContinueButton().SetActive(false);
                if (GetPlayerTurn() == 'S')
                {
                    return;
                }
                dm.DisplaySentence();  
            }

            else if (dm.GetCurrentLine() == 8)
            {
                if (!dm.StoryFinished())
                    return;

                SetMovementOnPawns(true);
                dm.GetBox().SetActive(false);
            }
        }  
    }

    void SetAnimationOnCollection(List<GameObject> collection, string animation, int parameter)
    {
        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i].GetComponent<Animator>() == null)
                continue;

            if (!collection[i].GetComponent<Animator>().isActiveAndEnabled)
                continue;

            collection[i].GetComponent<Animator>().SetInteger(animation, parameter);
        }
    }

    void ResetGameplay()  // Failed to guard a meeting of m, reset the game to accomodate the spy moving again
    {
        SetGameOver(false);
        SetPlayerColors(playerSpy, playerRev);
        SetMoveCount(0);
        gameOver.GetComponentInChildren<TMP_Text>().text = "";
        gameOver.SetActive(false);
        backToMenu.SetActive(false);
        SetRoundCount(0);
        SetRoundsRemaining(GetRoundsLeft() + 1);
        SendSpiesBack(GetPawns());
    }


    void SendSpiesBack(List<GameObject> pawns)  // Back to starting positions, reset vertex, etc.
    {
        for(int i = 0; i < pawns.Count; i++)
        {
            if(pawns[i].GetComponentInChildren<TMP_Text>().text != "S")
            {
                continue;
            }

            // We go to the parent, and retrieve the current revolutionary count on each vertex. If the count is at least m, stay, if not, send that spy back
            pawns[i].GetComponent<RectTransform>().localPosition = new Vector3(-600, 130, 0);
            pawns[i].transform.parent.GetComponent<Vertex>().TakeAwaySpy();
            pawns[i].transform.parent.GetComponent<Vertex>().DisplayCounts();
            pawns[i].transform.SetParent(GetSpyBackground().transform, false);
            pawns[i].GetComponent<Spy>().SetCollisionDetected(false);
            pawns[i].GetComponent<Spy>().SetMovement(true);
        }
    }
}


