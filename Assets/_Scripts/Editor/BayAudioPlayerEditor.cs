using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;



[CustomEditor(typeof(BayAudioPlayer))]
public class BayAudioPlayerEditor : Editor
{
    const string turbidityHelpText = "Audio will play at \"Min Speed\" when turbidity is below \"Min Turbidity\","+
        "and at \"Max Speed\" when it's above \"Min Turbidity\". The value is interpolated otherwise.";

    const string phHelpText = "Audio will play at \"Min Volume\" when pH is below \"Min pH\" and at " +
        "\"Max Volume\" when above \"Max pH\". The value is interpolated otherwise.";

    SerializedProperty overrideDataFeed;
    SerializedProperty audioClip;

    // ==================== Turbidity =====================
    SerializedProperty minTurbidity;
    SerializedProperty minSpeed;
    SerializedProperty maxTurbidity;
    SerializedProperty maxSpeed;
    SerializedProperty turbidityOverride;

    // ======================= pH =========================
    SerializedProperty minPH;
    SerializedProperty maxPH;
    SerializedProperty minVolume;
    SerializedProperty maxVolume;
    SerializedProperty phOverride;

    internal static class Styles
    {
        public static GUIContent minTurbidityLabel = EditorGUIUtility.TrTextContent("Minimum Turbidity", "Audio will play at minimum speed if turbidity is below this value");
        public static GUIContent maxTurbidityLabel = EditorGUIUtility.TrTextContent("Maximum Turbidity", "Audio will play at maximum speed if turbidity is above this value");
        public static GUIContent minSpeedLabel = EditorGUIUtility.TrTextContent("Minimum Speed", "Audio playback speed will not drop below this value");
        public static GUIContent maxSpeedLabel = EditorGUIUtility.TrTextContent("Maximum Speed", "Audio playback speed will not go above this value");

        public static GUIContent minPHLabel = EditorGUIUtility.TrTextContent("Minimum pH", "Audio will play at minimum volume if pH is below this value");
        public static GUIContent maxPHLabel = EditorGUIUtility.TrTextContent("Maximum pH", "Audio will play at maximum volume if pH is above this value");
        public static GUIContent minVolumeLabel = EditorGUIUtility.TrTextContent("Minimum Volume", "Audio playback volume will not drop below this value");
        public static GUIContent maxVolumeLabel = EditorGUIUtility.TrTextContent("Maximum Volume", "Audio playback volume will not go above this value");
    }



    void OnEnable()
    {
        overrideDataFeed = serializedObject.FindProperty("OverrideDataFeed");
        audioClip = serializedObject.FindProperty("audioClip");

        minTurbidity = serializedObject.FindProperty("m_MinTurbidity");
        maxTurbidity = serializedObject.FindProperty("m_MaxTurbidity");
        minSpeed = serializedObject.FindProperty("m_MinSpeed");
        maxSpeed = serializedObject.FindProperty("m_MaxSpeed");

        turbidityOverride = serializedObject.FindProperty("TurbidityOverride");

        minPH = serializedObject.FindProperty("m_MinPh");
        maxPH = serializedObject.FindProperty("m_MaxPh");
        minVolume = serializedObject.FindProperty("m_MinVolume");
        maxVolume = serializedObject.FindProperty("m_MaxVolume");

        phOverride = serializedObject.FindProperty("PHOverride");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BayAudioPlayer player = (BayAudioPlayer)target;
        
        EditorGUILayout.PropertyField(audioClip);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(overrideDataFeed);

        EditorGUILayout.LabelField("Turbidity", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(turbidityHelpText, EditorStyles.helpBox);
        EditorGUILayout.PropertyField(minTurbidity, Styles.minTurbidityLabel);
        EditorGUILayout.PropertyField(minSpeed, Styles.minSpeedLabel);
        EditorGUILayout.PropertyField(maxTurbidity, Styles.maxTurbidityLabel);
        EditorGUILayout.PropertyField(maxSpeed, Styles.maxSpeedLabel);
        if (overrideDataFeed.boolValue)
        {
            turbidityOverride.floatValue = EditorGUILayout.Slider("Turbidity Override", turbidityOverride.floatValue, minTurbidity.floatValue, maxTurbidity.floatValue);
        }

        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        EditorGUILayout.LabelField("pH", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(phHelpText, EditorStyles.helpBox);
        EditorGUILayout.PropertyField(minPH, Styles.minPHLabel);
        EditorGUILayout.PropertyField(minVolume, Styles.minVolumeLabel);
        EditorGUILayout.PropertyField(maxPH, Styles.maxPHLabel);
        EditorGUILayout.PropertyField(maxVolume, Styles.maxVolumeLabel);
        if (overrideDataFeed.boolValue)
        {
            phOverride.floatValue = EditorGUILayout.Slider("pH Override", phOverride.floatValue, minPH.floatValue, maxPH.floatValue);
        }

        serializedObject.ApplyModifiedProperties();

    }

    private static void DrawHorizontalLine(int height)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);

        rect.height = height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
