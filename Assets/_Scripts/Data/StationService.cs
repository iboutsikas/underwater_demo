using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class StationService
{
    private static string api = "https://bay-db.irc.umbc.edu";
    //private static string api = "http://localhost:8080";

    public List<Station> GetAllStations()
    {
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(api + "/api/V1/stations");
        request.Method = "GET";
        request.Accept = "application/json";

        try
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<List<Station>>(json);

                return result;
                

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return new List<Station>();
        }
    }

    public List<BayData> GetSamples(Guid guid, DateTimeOffset from)
    {

        if (guid == Guid.Empty)
        {
            Debug.DebugBreak();
        }

        var dateString = from.ToString("yyyy-MM-ddThh:mm:sszzzz");

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(api + $"/api/V1/stations/{guid}?include_measurements=true" +
            $"&start_date={Uri.EscapeDataString(dateString)}"
        );

        request.Method = "GET";
        request.Accept = "application/json";

        try
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<Station>(json);

                return result.Samples;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            Debug.LogError($"Url was {request.RequestUri}");
            return null;
        }
    }
}
