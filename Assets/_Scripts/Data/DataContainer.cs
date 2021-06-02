using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System.Timers;
using UnityEditor;
using Newtonsoft.Json;

[Serializable]
public class BayData
{
    public DateTimeOffset SampleDate;
    [JsonProperty("dissolvedOxygen")]
    public float Oxygen;
    [JsonProperty("waterTemperature")]
    public float Temperature;
    public float Salinity;
    public float Turbidity;
    public float PH;
    public float Chlorophyll;
};

public class DataContainer : ScriptableObjectSingleton<DataContainer>
{
    private StationService _StationService;
    private TimeSpan _UpdateFrequency;
    private List<Station> _Stations;
    private List<BayData> _Samples;
    private Timer _Timer;

    public Station CurrentStation { get; private set; } = null;
    public BayData CurrentSample { get; private set; } = null;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        DataContainer.Initialize();
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void OnEditorMethodLoad()
    {
        DataContainer.Initialize();
    }
#endif

    DataContainer()
    {
        _StationService = new StationService();
        _Stations = _StationService.GetAllStations();
        GetSamples(_Stations[0], true);
        _UpdateFrequency = TimeSpan.FromMinutes(15);
        _Timer = new Timer(5 * 60 * 1000);
        //_Timer = new Timer(5000);

        _Timer.Elapsed += new ElapsedEventHandler(On_TimerEllapsed);
        _Timer.Start();
    }

    protected override void Awake() {
        base.Awake();
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
#if UNITY_EDITOR
       
#endif
    }


    public List<Station> Stations
    {
        get
        {
            if (_Stations == null)
                _Stations = _StationService.GetAllStations();
            return _Stations;
        }
    }

    public Station SelectStation(int index)
    {
        if (index >= 0 || index < _Stations.Count)
        {
            CurrentStation = _Stations[index];
            CurrentSample = null;
        }

        if (index == -1)
            CurrentStation = null;

        return CurrentStation;
    }

    public BayData SelectSample(int index)
    {
        if (index >= 0 && index < _Samples.Count)
            CurrentSample = _Samples[index];

        if (index == -1)
            CurrentSample = null;

        return CurrentSample;
    }

    private void On_TimerEllapsed(object sender, ElapsedEventArgs e)
    {
        _Stations = _StationService.GetAllStations();

        if (CurrentStation != null)
        {
            _Samples = _StationService.GetSamples(CurrentStation.Id, CurrentStation.LastAttempt);
            CurrentStation.LastAttempt = DateTimeOffset.Now;
            GetSamples(CurrentStation);
        }
        Debug.Log("On_TimerEllapsed");
    }


    public List<BayData> GetSamples(Station station, bool force = false)
    {
        var diff = DateTimeOffset.Now - station.LastUpdate; 
        var diff2 = DateTimeOffset.Now - station.LastAttempt;

        if (diff <= _UpdateFrequency && diff2 <= _UpdateFrequency && !force)
            return _Samples;

        var temp = _StationService.GetSamples(station.Id, station.LastUpdate);
        if (temp != null)
        {
            _Samples = temp;
            // Samples are in date descending order
            CurrentSample = _Samples[0];
        }

        station.LastAttempt = DateTimeOffset.Now;
        return _Samples;
    }

    public float Oxygen
    {
        get { return CurrentSample.Oxygen; }
        private set { }     
    }

    public float Temperature
    {
        get => CurrentSample.Temperature;
        private set { }        
    }

    public float Salinity
    {
        get => CurrentSample.Salinity;
        private set { }
    }

    public float Turbidity
    {
        get => CurrentSample.Turbidity;
        private set { }
    }

    public float PH
    {
        get => CurrentSample.PH;
        private set { }
    }

    public float Chlorophyll
    {
        get => CurrentSample.Chlorophyll;
        private set { }
    }

}
