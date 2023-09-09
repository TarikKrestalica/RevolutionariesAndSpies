using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveMenu : MonoBehaviour
{
    [SerializeField] private GameObject revObjective;
    [SerializeField] private GameObject spyObjective;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            this.gameObject.SetActive(false);
            revObjective.SetActive(true); 
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            this.gameObject.SetActive(false);
            spyObjective.SetActive(true);
        }
    }
}
