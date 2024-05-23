using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SyncTransformAndDisable))]
public class SyncTransformAndDisableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SyncTransformAndDisable script = (SyncTransformAndDisable)target;

        if (GUILayout.Button("Sync and Disable A Objects"))
        {
            SyncAndDisableObjects(script);
        }
    }

    private void SyncAndDisableObjects(SyncTransformAndDisable script)
    {
        if (script.AObjects.Length != script.BObjects.Length)
        {
            Debug.LogError("AObjects and BObjects arrays must be of the same length.");
            return;
        }

        for (int i = 0; i < script.AObjects.Length; i++)
        {
            if (script.AObjects[i] != null && script.BObjects[i] != null)
            {
                // Record the objects to allow undo
                Undo.RecordObject(script.BObjects[i].transform, "Sync Transform");
                Undo.RecordObject(script.AObjects[i], "Disable Object");

                // Copy transform values from A to B
                script.BObjects[i].transform.position = script.AObjects[i].transform.position;
                script.BObjects[i].transform.rotation = script.AObjects[i].transform.rotation;
                script.BObjects[i].transform.localScale = script.AObjects[i].transform.localScale;

                // Disable the A object
                script.AObjects[i].SetActive(false);

                // Mark objects as dirty to ensure changes are saved
                EditorUtility.SetDirty(script.BObjects[i]);
                EditorUtility.SetDirty(script.AObjects[i]);
            }
            else
            {
                Debug.LogWarning($"Element at index {i} is null in either AObjects or BObjects.");
            }
        }

        // Clear the AObjects array
        for (int i = 0; i < script.AObjects.Length; i++)
        {
            script.AObjects[i] = null;
        }

        // Mark the script as dirty to save changes
        EditorUtility.SetDirty(script);
    }
}
