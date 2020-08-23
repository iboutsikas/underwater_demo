using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class UnderTheBayWindow : EditorWindow
{
    private static bool useLiveData = false;
    private static int selectedDay = 1;
    [MenuItem("Under the Bay/Data stream")]
    public static void ShowWindow()
    {
        GetWindow<UnderTheBayWindow>(true, "DNR data stream", false);
    }

    private void OnGUI()
    {
        useLiveData = EditorGUILayout.Toggle("Use \"live\" data", useLiveData);
        EditorGUILayout.Space(15);

        if (useLiveData)
        {
            selectedDay = EditorGUILayout.IntSlider("Day of data", selectedDay, 1, DataContainer.DataPoints);
            DataContainer.SelectedDay = selectedDay - 1;
            BayData data = DataContainer.GetData();

            EditorGUILayout.LabelField($"Preview date: {data.SampleDate}");
            EditorGUILayout.LabelField($"Oxygen Saturation: {data.Oxygen} mg/L");
            EditorGUILayout.LabelField($"Temperature: {data.Temperature} °F");
            EditorGUILayout.LabelField($"Salinity: {data.Salinity} ppt");
            EditorGUILayout.LabelField($"Turbidity: {data.Turbidity} NTU");
            EditorGUILayout.LabelField($"pH: {data.PH}");
            EditorGUILayout.LabelField($"Chlorophyll: {data.Chlorophyll} μg/L");
        }
        else
        {
            DataContainer.Oxygen = EditorGUILayout.Slider("Oxygen (mg/L)", DataContainer.Oxygen, 0.0f, 15.0f);
            DataContainer.Temperature = EditorGUILayout.Slider("Temperature (°F)", DataContainer.Temperature, 0.0f, 100.0f);
            DataContainer.Salinity = EditorGUILayout.Slider("Salinity (ppt)", DataContainer.Salinity, 0.0f, 32.0f);
            DataContainer.Turbidity = EditorGUILayout.Slider("Turbidity (NTU)", DataContainer.Turbidity, 0.0f, 100.0f);
            DataContainer.PH = EditorGUILayout.Slider("pH", DataContainer.PH, 1.0f, 14.0f);
            DataContainer.Chlorophyll = EditorGUILayout.Slider("Chlorophyll (μg/L)", DataContainer.Chlorophyll, 0.0f, 100.0f);
        }
    }
}
