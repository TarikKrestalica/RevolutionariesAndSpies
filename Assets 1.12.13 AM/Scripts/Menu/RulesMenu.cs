using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesMenu : MonoBehaviour
{
    [SerializeField] private GameObject initialRoundUI;
    [SerializeField] private GameObject postInitialRoundUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (initialRoundUI.activeInHierarchy)
            {
                initialRoundUI.SetActive(false);
            }
            else if (postInitialRoundUI.activeInHierarchy)
            {
                postInitialRoundUI.SetActive(false);
            }
        }
    }
}
