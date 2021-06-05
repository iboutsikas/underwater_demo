using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BayAnimator))]
public class BayAnimatorEditor : Editor
{
    SerializedProperty thisScript;

    SerializedProperty overrideDataFeed;
    // ================== Temperature ==================
    SerializedProperty coldTemperature;
    SerializedProperty coldColor;
    SerializedProperty warmTemperature;
    SerializedProperty warmColor;
    SerializedProperty temperatureOverride;
    SerializedProperty temperatureBias;
    SerializedProperty visualizeTemperature;
    bool showTemperature = true;

    // ==================== Oxygen =====================
    SerializedProperty oxygenOverride;
    SerializedProperty oxygenUpperBound;
    SerializedProperty visualizeOxygen;
    bool showOxygen = true;

    // ======================= pH ======================
    SerializedProperty visualizePH;
    SerializedProperty pHOverride;
    SerializedProperty pHLowOffset;
    SerializedProperty pHHighOffset;
    bool showPH = true;

    // ================= Chlorophyll ===================
    SerializedProperty visualizeChlorophyll;
    SerializedProperty chlorophyllOverride;
    SerializedProperty minimumAlpha;
    SerializedProperty maximumAlpha;
    SerializedProperty minChlorophyll;
    SerializedProperty maxChlorophyll;
    bool showChlorophyll = true;

    // =================== Salinity ====================
    SerializedProperty visualizeSalinity;
    SerializedProperty minSalinity;
    SerializedProperty minRotationSpeed;
    SerializedProperty maxSalinity;
    SerializedProperty maxRotationSpeed;
    SerializedProperty salinityOverride;
    bool showSalinity = true;

#pragma warning disable 0414
    // =================== Turbidity ===================
    //SerializedProperty overrideTurbidity;
    //SerializedProperty visualizeTurbidity;
    bool showTurbidity = true;
#pragma warning restore 0414

    internal class Styles
    {
        public static GUIContent coldTempLabel = EditorGUIUtility.TrTextContent(
            "Cold temperature (°C)", 
            "The model will be using the cold color when temperature is below this value"
        );

        public static GUIContent coldColorLabel = EditorGUIUtility.TrTextContent(
            "Cold color",
            "The color value used when temperature is below the limit"
        );

        public static GUIContent warmTempLabel = EditorGUIUtility.TrTextContent(
            "Warm temperature (°C)",
            "The model will be using the warm color when temperature is above this value"
        );

        public static GUIContent warmColorLabel = EditorGUIUtility.TrTextContent(
            "Warm color",
            "The color value used when temperature is above the limit"
        );

        public static GUIContent temperatureBiasLabel = EditorGUIUtility.TrTextContent(
            "Temperature Bias",
            "Configures how much the color should shift around a given temperature"
        );

        public static GUIContent visualizeTempLabel = EditorGUIUtility.TrTextContent(
            "Visualize Temperature",
            "Whether temperature should be animated (i.e change color)"
        );

        public static GUIContent visualizeOxygenLabel = EditorGUIUtility.TrTextContent(
            "Visualize Oxygen",
            "Whether oxygen should be animated (i.e change scale)"
        );

        public static GUIContent oxygenUpperBoundLabel = EditorGUIUtility.TrTextContent(
            "Oxygen upper bound",
            "If oxygen is above this value, the animation will play at it's fullest"
        );

        public static GUIContent oxygenOverrideLabel = EditorGUIUtility.TrTextContent(
            "Oxygen override (μg/L)",
            "If oxygen is above this value, the animation will play at it's fullest"
        );

        public static GUIContent visualizePhLabel = EditorGUIUtility.TrTextContent(
            "Visualize pH",
            "Whether pH should be animated (i.e change main texture offset)"
        );

        public static GUIContent pHOverrideLabel = EditorGUIUtility.TrTextContent(
            "Override pH",
            "This value will be used to replace whatever value is coming in from the data stream"
        );

        public static GUIContent visualizeClfLabel = EditorGUIUtility.TrTextContent(
            "Visualize Chlorophyll",
            "Whether chlorophyll should be animated (i.e. change transparency)"
        );

        public static GUIContent minimumAlphaLabel = EditorGUIUtility.TrTextContent(
            "Minimum Opacity",
            "This is the minimum opacity the model will get to"
        );

        public static GUIContent maximummAlphaLabel = EditorGUIUtility.TrTextContent(
            "Maximum Opacity",
            "This is the maximum opacity the model will get to"
        );

        public static GUIContent minimumClfLabel = EditorGUIUtility.TrTextContent(
            "Minimum Chlorophyll",
            "If chlorophyll is bellow this number, the model will always have its minimum opacity"
        );

        public static GUIContent maximumClfLabel = EditorGUIUtility.TrTextContent(
            "Maximum Chlorophyll",
            "If chlorophyll is above this number, the model will always have its maximum opacity"
        );
        
        public static GUIContent visualizeSalinityLabel = EditorGUIUtility.TrTextContent(
            "Visualize Salinity",
            "Whether salinity should be animated (i.e. change rotation speed)"
        );

        public static GUIContent minSalinityLabel = EditorGUIUtility.TrTextContent(
            "Minimum Salinity",
            "If salinity is below this level, the model will have its minimum rotation speed"
        );
        
        public static GUIContent minSpeedLabel = EditorGUIUtility.TrTextContent(
            "Minimum Rotation Speed",
            "Rotation speed will not go bellow this value. 0 to turn off, 1 to leave it to animation's default"
        );
        
        public static GUIContent maxSalinityLabel = EditorGUIUtility.TrTextContent(
            "Maximum Salinity",
            "If salinity is above this threshold, the model will use its maximum rotation speed"
        );
        
        public static GUIContent maxSpeedLabel = EditorGUIUtility.TrTextContent(
            "Maximum Rotation Speed",
            "Rotation speed will not go above this value"
        );
    }

    void OnEnable()
    {
        thisScript = serializedObject.FindProperty("m_Script");

        overrideDataFeed = serializedObject.FindProperty(nameof(BayAnimator.OverrideDataFeed));

        visualizeTemperature = serializedObject.FindProperty(nameof(BayAnimator.VisualizeTemperature));
        coldTemperature = serializedObject.FindProperty(nameof(BayAnimator.ColdTemperature));
        coldColor = serializedObject.FindProperty(nameof(BayAnimator.ColdColor));
        warmTemperature = serializedObject.FindProperty(nameof(BayAnimator.WarmTemperature));
        warmColor = serializedObject.FindProperty(nameof(BayAnimator.WarmColor));
        temperatureBias = serializedObject.FindProperty(nameof(BayAnimator.TemperatureBias));
        temperatureOverride = serializedObject.FindProperty(nameof(BayAnimator.TemperatureOverride));


        visualizeOxygen = serializedObject.FindProperty(nameof(BayAnimator.VisualizeOxygen));
        oxygenUpperBound = serializedObject.FindProperty(nameof(BayAnimator.OxygenUpperBound));
        oxygenOverride = serializedObject.FindProperty(nameof(BayAnimator.OxygenOverride));

        visualizePH = serializedObject.FindProperty(nameof(BayAnimator.VisualizePH));
        pHOverride = serializedObject.FindProperty(nameof(BayAnimator.PHOverride));
        pHLowOffset = serializedObject.FindProperty(nameof(BayAnimator.LowOffset));
        pHHighOffset = serializedObject.FindProperty(nameof(BayAnimator.HighOffset));

        visualizeChlorophyll = serializedObject.FindProperty(nameof(BayAnimator.VisualizeChrolophyll));
        minimumAlpha = serializedObject.FindProperty(nameof(BayAnimator.MinAlpha));
        maximumAlpha = serializedObject.FindProperty(nameof(BayAnimator.MaxAlpha));
        minChlorophyll = serializedObject.FindProperty(nameof(BayAnimator.MinChlf));
        maxChlorophyll = serializedObject.FindProperty(nameof(BayAnimator.MaxChlf));
        chlorophyllOverride = serializedObject.FindProperty(nameof(BayAnimator.ClorophyllOverride));

        visualizeSalinity = serializedObject.FindProperty(nameof(BayAnimator.VisualizeSalinity));
        minSalinity = serializedObject.FindProperty(nameof(BayAnimator.MinSalinity));
        minRotationSpeed = serializedObject.FindProperty(nameof(BayAnimator.MinRotationSpeed));
        maxSalinity = serializedObject.FindProperty(nameof(BayAnimator.MaxSalinity));
        maxRotationSpeed = serializedObject.FindProperty(nameof(BayAnimator.MaxRotationSpeed));
        salinityOverride = serializedObject.FindProperty(nameof(BayAnimator.SalinityOverride));
    }

    public override void OnInspectorGUI()
    {
        BayAnimator anim = (BayAnimator)target;
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(thisScript, true, new GUILayoutOption[0]);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(overrideDataFeed);

        EditorStyles.foldout.fontStyle = FontStyle.Bold;
        // ================== Temperature ==================
        showTemperature = EditorGUILayout.Foldout(showTemperature, "Temperature", true);
        if (showTemperature)
        {
            EditorGUILayout.PropertyField(visualizeTemperature, Styles.visualizeTempLabel);

            EditorGUILayout.PropertyField(coldTemperature, Styles.coldTempLabel);
            EditorGUILayout.PropertyField(coldColor, Styles.coldColorLabel);

            EditorGUILayout.PropertyField(warmTemperature, Styles.warmTempLabel);
            EditorGUILayout.PropertyField(warmColor, Styles.warmColorLabel);
            EditorGUILayout.PropertyField(temperatureBias, Styles.temperatureBiasLabel);

            if (overrideDataFeed.boolValue)
            {
                temperatureOverride.floatValue = EditorGUILayout.Slider("Override temperature (°C)", temperatureOverride.floatValue, coldTemperature.floatValue, warmTemperature.floatValue);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);
        
        // ==================== Oxygen =====================
        showOxygen = EditorGUILayout.Foldout(showOxygen, "Oxygen", true);
        if (showOxygen)
        {
            EditorGUILayout.PropertyField(visualizeOxygen, Styles.visualizeOxygenLabel);
            EditorGUILayout.PropertyField(oxygenUpperBound, Styles.oxygenUpperBoundLabel);

            if (overrideDataFeed.boolValue)
            {
                EditorGUILayout.PropertyField(oxygenOverride, Styles.oxygenOverrideLabel);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // ======================= pH ======================
        showPH = EditorGUILayout.Foldout(showPH, "pH", true);
        if (showPH)
        {
            EditorGUILayout.PropertyField(visualizePH, Styles.visualizePhLabel);
            EditorGUILayout.PropertyField(pHLowOffset);
            EditorGUILayout.PropertyField(pHHighOffset);
            
            if (overrideDataFeed.boolValue)
            {
                EditorGUILayout.PropertyField(pHOverride, Styles.pHOverrideLabel);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // ================= Chlorophyll ====================
        showChlorophyll = EditorGUILayout.Foldout(showChlorophyll, "Chlorophyll", true);
        if (showChlorophyll)
        {
            EditorGUILayout.PropertyField(visualizeChlorophyll, Styles.visualizeClfLabel);

            EditorGUILayout.PropertyField(minimumAlpha, Styles.minimumAlphaLabel);
            EditorGUILayout.PropertyField(minChlorophyll, Styles.minimumClfLabel);
            EditorGUILayout.PropertyField(maximumAlpha, Styles.maximummAlphaLabel);
            EditorGUILayout.PropertyField(maxChlorophyll, Styles.maximumClfLabel);


            if (overrideDataFeed.boolValue)
            {
                EditorGUILayout.PropertyField(chlorophyllOverride);
            }
        }
        EditorGUILayout.Separator();
        DrawHorizontalLine(2);

        // =================== Salinity ====================
        showSalinity = EditorGUILayout.Foldout(showSalinity, "Salinity", true);
        if (showSalinity)
        {
            EditorGUILayout.PropertyField(visualizeSalinity, Styles.visualizeSalinityLabel);
            EditorGUILayout.PropertyField(minSalinity, Styles.minSalinityLabel);
            EditorGUILayout.PropertyField(minRotationSpeed, Styles.minSpeedLabel);
            EditorGUILayout.PropertyField(maxSalinity, Styles.maxSalinityLabel);
            EditorGUILayout.PropertyField(maxRotationSpeed, Styles.maxSpeedLabel);

            if (overrideDataFeed.boolValue)
            {
                EditorGUILayout.PropertyField(salinityOverride);
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
