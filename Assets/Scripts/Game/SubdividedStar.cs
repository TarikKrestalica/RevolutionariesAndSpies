using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubdividedStar : MonoBehaviour
{
    [SerializeField] private int numOfLeaves;
    [SerializeField] private int lengthPerPath;

    private System.Random ran = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpStar()
    {
        Debug.Log("Star is in setup mode!");
    }

    public void SetNumOfLeaves(int num)
    {
        numOfLeaves = num;
    }

    void SetLengthOfExtendedPath()
    {
        lengthPerPath = ran.Next(1, 3);
    }
}
