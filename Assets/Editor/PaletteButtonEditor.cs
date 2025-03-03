using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PaletteButton))]
public class PaletteButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //doing this so it lists the custom fields I want
        //because I guess the Button class has its own custom editor
        DrawDefaultInspector();
    }
}
