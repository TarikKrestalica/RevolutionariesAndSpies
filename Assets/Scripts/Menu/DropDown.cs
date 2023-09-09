using UnityEngine;

public class DropDown : MonoBehaviour
{

    // Setup and store the choice of graph they want: https://youtu.be/bNOPiPIp_W8?si=oAh4GG1ojrYN_zbB
    private static string graphChoice;

    void Start()
    {
        graphChoice = "Path";
    }

    public void DropDownSample(int index)
    {
        switch (index)
        {
            case 0:
                graphChoice = "Path";
                break;

            case 1:
                graphChoice = "Cycle";
                break;

            case 2:
                graphChoice = "Star";
                break;

            case 3:
                graphChoice = "Complete Graph";
                break;

            default:
                break;
        }
    }

    public static string GetGraphChoice()
    {
        return graphChoice;
    }
}
