using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://youtu.be/_nRzoTzeyxU

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public List<string> sentences;
}
