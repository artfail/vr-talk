using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReactionCube : MonoBehaviour
{
    public Color[] colors;
    public Renderer cube;

    public TextMeshPro outText;

    private void Start()
    {
        outText.text = "Loaded";
    }

    public void GestureOn(int type)
    {
        cube.material.color = colors[type];
        if (type == 1) outText.text = "Grabbing";
        if (type == 2) outText.text = "Fist";
    }

    public void GestureOff()
    {
        cube.material.color = colors[0];
        outText.text = "No Gesture";
    }
}
