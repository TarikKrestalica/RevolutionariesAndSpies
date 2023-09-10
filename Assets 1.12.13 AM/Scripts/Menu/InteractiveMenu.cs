using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractiveMenu: MonoBehaviour
{
    [SerializeField] private GameObject backArrow;
    [SerializeField] private GameObject frontArrow;

    [SerializeField] private List<GameObject> displays;
    private int currentDisplay = 0;

    void Start()
    {
        SetDisplay(currentDisplay);
        backArrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (currentDisplay < displays.Count - 1)
                MoveForward();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (currentDisplay > 0)
                MoveBack();
        }
    }

    void SetDisplay(int index)
    {
        for(int i = 0; i < displays.Count; i++)
        {
            if(i == index)
            {
                displays[i].SetActive(true);
            }
            else
            {
                displays[i].SetActive(false);
            }
        }
    }

    public void MoveForward()
    {
        ++currentDisplay;

        if (currentDisplay >= displays.Count - 1)
        {
            frontArrow.SetActive(false);
            backArrow.SetActive(true);
        }
        else
        {
            frontArrow.SetActive(true);
        }
            
        SetDisplay(currentDisplay);
    }

    public void MoveBack()
    {
        --currentDisplay;

        if (currentDisplay <= 0)
        {
            backArrow.SetActive(false);
            frontArrow.SetActive(true);
        } 
        else
            backArrow.SetActive(true);

        SetDisplay(currentDisplay);
    }
}
