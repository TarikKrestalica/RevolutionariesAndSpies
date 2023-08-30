using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// What's inside my vertex
struct Contents
{
    public int numOfRevs;
    public int numOfSpies;

    public Contents(int aNumOfRevs, int aNumOfSpies)
    {
        numOfRevs = aNumOfRevs;
        numOfSpies = aNumOfSpies;
    }
}

public class Vertex : MonoBehaviour
{
    // Gameplay & Graph Construction Setup
    public GameObject node;
    [SerializeField] private TMP_Text revText;
    [SerializeField] private TMP_Text spyText;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private GameObject warning;
    private Animator anim;

    // Properties
    private HashSet<int> m_subset = new HashSet<int>();
    private int vertID;

    private Contents m_contents;

    void Start()
    {
        m_contents = new Contents(0, 0);
        anim = GetComponent<Animator>();
    }

    public HashSet<int> GetSubset()  // Used to compare pairs of subsets for edges
    {
        return m_subset;
    }

    public void SetSubset(HashSet<int> subset)
    {
        m_subset = subset;
    }

    public void SetVertID(int num)
    {
        vertID = num;
        idText.text = num.ToString();
    }

    public int GetVertID()
    {
        return vertID;
    }

    public void AddRevolutionary()
    {
        m_contents.numOfRevs += 1;
    }

    public void AddSpy()
    {
        m_contents.numOfSpies += 1;
    }

    public void TakeAwayRevolutionary()
    {
        m_contents.numOfRevs -= 1;
    }

    public void TakeAwaySpy()
    {
        m_contents.numOfSpies -= 1;
    }

    public int GetRevCount()
    {
        return m_contents.numOfRevs;
    }

    public int GetSpyCount()
    {
        return m_contents.numOfSpies;
    }

    public void UpdateScaleForWarning(Vector3 scaleFactor) 
    {
        warning.GetComponent<RectTransform>().localScale += scaleFactor;
    }

    // Update Counts
    public void DisplayCounts()
    {
        revText.text = $"{m_contents.numOfRevs}";
        spyText.text = $"{m_contents.numOfSpies}";
    }

    public void PlayWarning(bool toggle) 
    {
        warning.SetActive(toggle);
        anim.SetBool("mRevs", toggle);
    }
}

