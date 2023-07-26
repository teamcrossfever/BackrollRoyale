using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RendersOff))]
[CanEditMultipleObjects]
public class RendererOffEditor : Editor {

    public static bool renderToggle = false;
    public override void OnInspectorGUI()
    {
        //Get the Button style we want
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        GUIContent toggleRenderOff = new GUIContent("Show/Hide");



        base.OnInspectorGUI(); //Original GUI here

        RendersOff t = target as RendersOff;

        //Place the button for toggle Lap points on the Inspector
        if (GUILayout.Button(toggleRenderOff, buttonStyle)) {
            renderToggle = !renderToggle;
            t.ToggleRenderer(renderToggle);
        }
        ToggleAllRendersButton();
    }

    public static void ToggleAllRendersButton()
    {
        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
        GUIContent toggleAllRenderOff = new GUIContent("Show/Hide ALL");

        if (GUILayout.Button(toggleAllRenderOff, buttonStyle))
        {
            renderToggle = !renderToggle;
            RendersOff[] r = FindObjectsOfType<RendersOff>();
            for (int i = 0; i < r.Length; i++)
            {
                r[i].ToggleRenderer(renderToggle);
            }
        }
    }
}