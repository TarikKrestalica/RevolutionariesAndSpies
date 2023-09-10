using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Slider functionality : https://youtu.be/nTLgzvklgU8?si=C22b9qs77ensPljI
[System.Serializable]
public class Toggler
{
    public Slider slider;
    public TextMeshProUGUI classifier;
    public TextMeshProUGUI sliderText;
}

public class GameMenu : MonoBehaviour
{
    public TMP_Dropdown Graph;
    public Toggler Order;
    public Toggler Revolutionary;
    public Toggler Spy;
    public Toggler MeetSize;

    private static int chosenGraphOrder = 1;
    private static int chosenRevs = 1;
    private static int chosenSpies = 1;
    private static int chosenMeetingSize = 1;

    private static bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        SetActivation(true);
        SetDefaultValues();
        Order.sliderText.text = Order.slider.value.ToString();
        Order.slider.onValueChanged.AddListener((v) =>
        {
            Order.sliderText.text = v.ToString();
            chosenGraphOrder = (int)v;
        });

        Revolutionary.sliderText.text = Revolutionary.slider.value.ToString();
        Revolutionary.slider.onValueChanged.AddListener((v) =>
        {
            Revolutionary.sliderText.text = v.ToString();
            chosenRevs = (int)v;
        });

        Spy.sliderText.text = Spy.slider.value.ToString();
        Spy.slider.onValueChanged.AddListener((v) =>
        {
            Spy.sliderText.text = v.ToString();
            chosenSpies = (int)v;
        });

        MeetSize.sliderText.text = MeetSize.slider.value.ToString();
        MeetSize.slider.onValueChanged.AddListener((v) =>
        {
            MeetSize.sliderText.text = v.ToString();
            chosenMeetingSize = (int)v;
        });
    }

    void Update()  
    {
        if (DropDown.GetGraphChoice() == "Cycle") // For cycles, must have minimum of 3 vertices
        {
            Order.slider.minValue = 3;
        }
        else
        {
            Order.slider.minValue = 1;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
            SetActivation(false);
        }
    }

    public static int GetSize()
    {
        return chosenGraphOrder;
    }

    public static int GetNumberOfRevs()
    {
        return chosenRevs;
    }

    public static int GetNumberOfSpies()
    {
        return chosenSpies;
    }

    public static int GetMeetingSize()
    {
        return chosenMeetingSize;
    }

    void SetDefaultValues()
    {
        chosenGraphOrder = 1;
        chosenRevs = 1;
        chosenSpies = 1;
        chosenMeetingSize = 1;
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public static void SetActivation(bool toggle)
    {
        isActivated = toggle;
    }

    public static bool IsActive()
    {
        return isActivated;
    }
}
