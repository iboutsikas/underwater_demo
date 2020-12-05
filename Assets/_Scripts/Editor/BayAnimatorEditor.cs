using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BayAnimator))]
public class BayAnimatorEditor : Editor
{
    SerializedProperty overrideDataFeed;
    // ================== Temperature ==================
    SerializedProperty coldTemperature;
    SerializedProperty coldColor;

    SerializedProperty warmTemperature;
    SerializedProperty warmColor;
    SerializedProperty overrideTemperature;
    SerializedProperty visualizeTemperature;
    bool showTemperature = true;

    // ==================== Oxygen =====================
    SerializedProperty overrideOxygen;
    SerializedProperty visualizeOxygen;
    bool showOxygen = true;
#pragma warning disable 0414
    // =================== Turbidity ===================
    //SerializedProperty overrideTurbidity;
    //SerializedProperty visualizeTurbidity;
    bool showTurbidity = true;

    // =================== Salinity ====================
    bool visualizeSalinity;
    bool showSalinity = true;

    // ======================= pH ======================
    bool visualizePH;
    bool showPH = true;

    // ================= Chlorophyll ===================
    bool visualizeChlorophyll;
    bool showChlorophyll = true;
#pragma warning restore 0414

    internal class Styles
    {
        public static GUIContent coldTempLabel = EditorGUIUtility.TrTextContent(
            "Cold temperature (°F)", 
            "The model will be using the cold color when temperature is below this value"
        );

        public static GUIContent coldColorLabel = EditorGUIUtility.TrTextContent(
            "Cold color",
            "The color value used when temperature is below the limit"
        );

        public static GUIContent warmTempLabel = EditorGUIUtility.TrTextContent(
            "Warm temperature (°F)",
            "The model will be using the warm color when temperature is above this value"
        );

        public static GUIContent warmColorLabel = EditorGUIUtility.TrTextContent(
            "Warm color",
            "The color value used when temperature is above the limit"
        );

        public static GUIContent visualizeTempLabel = EditorGUIUtility.TrTextContent(
            "Visualize Temperature",
            "Whether temperature should be animated (i.e change color)"
        );

        public static GUIContent visualizeOxygenLabel = EditorGUIUtility.TrTextContent(
            "Visualize Oxygen",
            "Whether oxygen should be animated (i.e change scale)"
        );
    }

    void OnEnable()
    {
        overrideDataFeed = serializedObject.FindProperty(nameof(BayAnimator.OverrideDataFeed));

        coldTemperature = serializedObject.FindProperty(nameof(BayAnimator.ColdTemperature));
        coldColor = serializedObject.FindProperty(nameof(BayAnimator.ColdColor));
        warmTemperature = serializedObject.FindProperty(nameof(BayAnimator.WarmTemperature));
        warmColor = serializedObject.FindProperty(nameof(BayAnimator.WarmColor));
        visualizeTemperature = serializedObject.FindProperty(nameof(BayAnimator.VisualizeTemperature));
        overrideTemperature = serializedObject.FindProperty(nameof(BayAnimator.TemperatureOverride));

        visualizeOxygen = serializedObject.FindProperty(nameof(BayAnimator.VisualizeOxygen));
        overrideOxygen = serializedObject.FindProperty(nameof(BayAnimator.OxygenOverride));
    }

    public override void OnInspectorGUI()
    {
        BayAnimator anim = (BayAnimator)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(overrideDataFeed);

        EditorStyles.foldout.fontStyle = FontStyle.Bold;
        // ================== Temperature ==================
        showTemperature = EditorGUILayout.Foldout(showTemperature, "Temperature", true);
        if (showTemperature)
        {
            EditorGUILayout.PropertyField(coldTemperature, Styles.coldTempLabel);
            EditorGUILayout.PropertyField(coldColor, Styles.coldColorLabel);

            EditorGUILayout.PropertyField(warmTemperature, Styles.warmTempLabel);
            EditorGUILayout.PropertyField(warmColor, Styles.warmColorLabel);

            EditorGUILayout.PropertyField(visualizeTemperature, Styles.visualizeTempLabel);

            if (overrideDataFeed.boolValue)
            {
                overrideTemperature.floatValue = EditorGUILayout.Slider("Override temperature (°F)", overrideTemperature.floatValue, coldTemperature.floatValue, warmTemperature.floatValue);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);
        // ==================== Oxygen =====================
        showOxygen = EditorGUILayout.Foldout(showOxygen, "Oxygen", true);
        if (showOxygen)
        {
            EditorGUILayout.PropertyField(visualizeOxygen, Styles.visualizeOxygenLabel);

            if (overrideDataFeed.boolValue)
            {
                overrideOxygen.floatValue = EditorGUILayout.FloatField("Override oxygen (mg/L)",overrideOxygen.floatValue);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);
#if false

        // =================== Turbidity ===================
        showTurbidity = EditorGUILayout.Foldout(showTurbidity, "Turbidity", true);
        if (showTurbidity)
        {
            if (overrideDataFeed)
            {
                overrideTurbidity = EditorGUILayout.FloatField("Override turbidity (NTU)", overrideTurbidity);
            }
            visualizeTurbidity = EditorGUILayout.Toggle("Visualize turbidity", anim.AnimateTurbidity);
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // =================== Salinity ====================
        showSalinity = EditorGUILayout.Foldout(showSalinity, "Salinity", true);
        if (showSalinity)
        {
            EditorGUILayout.LabelField("Not implemented yet");
            visualizeSalinity = EditorGUILayout.Toggle("Visualize salinity", visualizeSalinity);
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // ======================= pH ======================
        showPH = EditorGUILayout.Foldout(showPH, "pH", true);
        if (showPH)
        {
            EditorGUILayout.LabelField("Not implemented yet");
            visualizePH = EditorGUILayout.Toggle("Visualize pH", visualizePH);
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // ================= Chlorophyll ====================
        showChlorophyll = EditorGUILayout.Foldout(showChlorophyll, "Chlorophyll", true);
        if (showChlorophyll)
        {
            EditorGUILayout.LabelField("Not implemented yet");
            visualizeChlorophyll = EditorGUILayout.Toggle("Visualize Chlorophyll", visualizeChlorophyll);
        }

        // These are all down here so we can add any checks in groups instead of
        // doing it when the gui is updated.

        anim.ColdTemperature = coldTemperature;
        anim.ColdColor = coldColor;
        anim.WarmTemperature = warmTemperature;
        anim.WarmColor = warmColor;
        anim.VisualizeTemperature = visualizeTemperature;
        anim.VisualizeOxygen = visualizeOxygen;
        anim.AnimateTurbidity = visualizeTurbidity;

        anim.OverrideDataFeed = overrideDataFeed;

        if (overrideDataFeed)
        {
            anim.TemperatureOverride = overrideTemperature;
            anim.OxygenOverride = overrideOxygen;
            anim.TurbidityOverride = overrideTurbidity;
        }
#endif
        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawHorizontalLine(int height)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);

        rect.height = height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
    
}
