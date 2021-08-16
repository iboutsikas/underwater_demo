using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class UnderTheBayWindow : EditorWindow
{
    private static string[] StationNames;
    
    [MenuItem("Under the Bay/Data stream")]
    public static void ShowWindow()
    {
        GetWindow<UnderTheBayWindow>(true, "Under the Bay Datastream", false);
        UpdateStations();        
    }

    private int selectedStation = 0;
    //private int selectedSample = 0;

    private void OnGUI()
    {
        if (StationNames == null || StationNames.Length == 0)
        {
            UpdateStations();
        }

        DataContainer dc = DataContainer.instance;

        if (GUILayout.Button("Fetch Data"))
        {
            UpdateStations();
            var s = dc.SelectStation(0);
            selectedStation = 0;
            dc.GetSamples(s, true);
            dc.SelectSample(0);
        }

        EditorGUILayout.LabelField("Stream configuration", EditorStyles.boldLabel);

        var newSelection = EditorGUILayout.Popup("Station", selectedStation, StationNames);
        Station station = null;
        if (newSelection != selectedStation)
        {
            selectedStation = newSelection;
            station = dc.SelectStation(selectedStation);
            dc.GetSamples(station, true);
            dc.SelectSample(0);
        }
        else
        {
            station = dc.SelectStation(selectedStation);
            dc.SelectSample(0);
        }

        EditorGUILayout.LabelField($"Last updated: {station.LastUpdate.ToString()}");
        
        EditorGUILayout.Space(15);
        EditorGUILayout.Separator();

        var data = dc.CurrentSample;

        if (data == null)
            return;

        EditorGUILayout.LabelField($"Preview date: {data.SampleDate}");
        EditorGUILayout.LabelField($"Oxygen Saturation: {data.Oxygen} mg/L");
        EditorGUILayout.LabelField($"Temperature: {data.Temperature} °C");
        EditorGUILayout.LabelField($"Salinity: {data.Salinity} ppt");
        EditorGUILayout.LabelField($"Turbidity: {data.Turbidity} NTU");
        EditorGUILayout.LabelField($"pH: {data.PH}");
        EditorGUILayout.LabelField($"Chlorophyll: {data.Chlorophyll} μg/L");

    }        

    private static void UpdateStations()
    {
        var stations = DataContainer.instance.Stations;
        var names = new List<string>();
        foreach (var station in stations)
            names.Add(station.Name);

        StationNames = names.ToArray();
    }

}
