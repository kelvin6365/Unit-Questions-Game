using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsTrack : MonoBehaviour
{

    public void OnButtonClick(string btnName)
    {
        Debug.Log("[AnalyticsTrack] OnButtonClick: " + btnName);
        Analytics.CustomEvent("ButtonEvent", new Dictionary<string, object>
        {
            { "Name", btnName },
            { "remark", "" }
        });

    }
}