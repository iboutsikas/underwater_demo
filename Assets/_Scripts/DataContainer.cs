using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public struct BayData
{
    public DateTime SampleDate;
    public float Oxygen;
    public float Temperature;
    public float Salinity;
    public float Turbidity;
    public float PH;
    public float Chlorophyll;
};

public class DataContainer
{
#if UNITY_EDITOR
    public static bool UseLiveData = false;
    private static int mSelectedDay = 0;
    public static int SelectedDay
    {
        get
        {
            return mSelectedDay;
        }

        set
        {
            if (value < HardcodedData.Length && value != mSelectedDay)
            {
                mSelectedDay = value;
                currentData = HardcodedData[value];
            }
        }
    }

    private static BayData[] HardcodedData =
    {
       new BayData() { SampleDate = DateTime.Parse("07/01/20 10:00"), Salinity = -1.0f,  PH = 7.34f, Oxygen = 7.35f, Turbidity = -1.0f, Chlorophyll = -1.0f, Temperature = 80.366f },
       new BayData() { SampleDate = DateTime.Parse("07/02/20 10:00"), Salinity =  7.67f, PH = 8.35f, Oxygen = 7.04f, Turbidity =  8.7f, Chlorophyll =  9.8f, Temperature = 80.186f },
       new BayData() { SampleDate = DateTime.Parse("07/03/20 10:00"), Salinity =  8.1f,  PH = 8.01f, Oxygen = 6.05f, Turbidity =  8.6f, Chlorophyll =  9.4f, Temperature = 80.618f },
       new BayData() { SampleDate = DateTime.Parse("07/04/20 10:00"), Salinity =  7.6f,  PH = 8.01f, Oxygen = 6.10f, Turbidity =  8.9f, Chlorophyll =  8.8f, Temperature = 80.366f },
       new BayData() { SampleDate = DateTime.Parse("07/05/20 10:00"), Salinity =  9.93f, PH = 9.97f, Oxygen = 5.03f, Turbidity =  6.2f, Chlorophyll =  6.5f, Temperature = 81.230f },
       new BayData() { SampleDate = DateTime.Parse("07/06/20 10:00"), Salinity = 10.1f,  PH = 7.96f, Oxygen = 5.10f, Turbidity =  5.6f, Chlorophyll =  7.1f, Temperature = 79.592f },
       new BayData() { SampleDate = DateTime.Parse("07/07/20 10:00"), Salinity =  9.81f, PH = 8.08f, Oxygen = 6.02f, Turbidity =  8.8f, Chlorophyll =  7.4f, Temperature = 79.448f },
       new BayData() { SampleDate = DateTime.Parse("07/08/20 10:00"), Salinity =  9.79f, PH = 8.25f, Oxygen = 7.08f, Turbidity =  5.7f, Chlorophyll = 16.4f, Temperature = 79.016f },
       new BayData() { SampleDate = DateTime.Parse("07/09/20 10:00"), Salinity =  9.39f, PH = 7.87f, Oxygen = 6.06f, Turbidity =  8.5f, Chlorophyll =  9.9f, Temperature = 80.222f },
       new BayData() { SampleDate = DateTime.Parse("07/10/20 10:00"), Salinity = -1.0f,  PH = 7.83f, Oxygen = 6.33f, Turbidity = -1.0f, Chlorophyll = -1.0f, Temperature = 81.626f },
       new BayData() { SampleDate = DateTime.Parse("07/11/20 10:00"), Salinity = -1.0f,  PH = 7.67f, Oxygen = 6.22f, Turbidity = -1.0f, Chlorophyll = -1.0f, Temperature = 81.752f },
       new BayData() { SampleDate = DateTime.Parse("07/12/20 10:00"), Salinity = -1.0f,  PH = 7.57f, Oxygen = 6.01f, Turbidity = -1.0f, Chlorophyll = -1.0f, Temperature = 81.680f },
       new BayData() { SampleDate = DateTime.Parse("07/13/20 10:00"), Salinity = -1.0f,  PH = 7.53f, Oxygen = 5.96f, Turbidity = -1.0f, Chlorophyll = -1.0f, Temperature = 81.662f },
       new BayData() { SampleDate = DateTime.Parse("07/14/20 10:00"), Salinity =  9.87f, PH = 7.98f, Oxygen = 6.61f, Turbidity = 22.5f, Chlorophyll = 17.7f, Temperature = 81.626f },
       new BayData() { SampleDate = DateTime.Parse("07/15/20 10:00"), Salinity =  9.6f,  PH = 8.18f, Oxygen = 7.66f, Turbidity = 12.7f, Chlorophyll = 22.8f, Temperature = 82.814f },
       new BayData() { SampleDate = DateTime.Parse("07/16/20 10:00"), Salinity =  9.64f, PH = 8.17f, Oxygen = 6.47f, Turbidity = 19.4f, Chlorophyll = 15.8f, Temperature = 84.218f },
       new BayData() { SampleDate = DateTime.Parse("07/17/20 10:00"), Salinity =  9.81f, PH = 8.07f, Oxygen = 6.30f, Turbidity = 13.0f, Chlorophyll = 13.6f, Temperature = 82.940f },
       new BayData() { SampleDate = DateTime.Parse("07/18/20 10:00"), Salinity =  9.8f,  PH = 7.85f, Oxygen = 5.60f, Turbidity =  6.8f, Chlorophyll =  9.9f, Temperature = 81.752f },
       new BayData() { SampleDate = DateTime.Parse("07/19/20 10:00"), Salinity =  9.18f, PH = 8.19f, Oxygen = 7.50f, Turbidity =  7.6f, Chlorophyll = 18.4f, Temperature = 82.148f },
       new BayData() { SampleDate = DateTime.Parse("07/20/20 10:00"), Salinity =  9.33f, PH = 7.82f, Oxygen = 5.67f, Turbidity = 10.7f, Chlorophyll = 17.0f, Temperature = 84.254f },
    };
    public static int DataPoints => HardcodedData.Length;
#endif

    static DataContainer()
    {
    }

    private static BayData currentData = new BayData()
    {
        SampleDate = DateTime.Parse("07/01/20 10:00"),
        Salinity = -1.0f,
        PH = 7.34f,
        Oxygen = 7.35f,
        Turbidity = -1.0f,
        Chlorophyll = -1.0f,
        Temperature = 80.366f
    };

    public static BayData GetData()
    {
        return currentData;
    }

    public static float Oxygen
    {
        get => currentData.Oxygen;
        set
        {
            if (currentData.Oxygen != value)
            {
                currentData.Oxygen = value;
            }
        }
    }

    public static float Temperature {
        get => currentData.Temperature;
        set
        {
            if (currentData.Temperature != value)
            {
                currentData.Temperature = value;
            }
        }
    }

    public static float Salinity
    {
        get => currentData.Salinity;
        set
        {
            if (currentData.Salinity != value)
            {
                currentData.Salinity = value;
            }
        }
    }

    public static float Turbidity
    {
        get => currentData.Turbidity;
        set
        {
            if (currentData.Turbidity != value)
            {
                currentData.Turbidity = value;
            }
        }
    }

    public static float PH
    {
        get => currentData.PH;
        set
        {
            if (currentData.PH != value)
            {
                currentData.PH = value;
            }
        }
    }

    public static float Chlorophyll
    {
        get => currentData.Chlorophyll;
        set
        {
            if (currentData.Chlorophyll != value)
            {
                currentData.Chlorophyll = value;
            }
        }
    }


}
