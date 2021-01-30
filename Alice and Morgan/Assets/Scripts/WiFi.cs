using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiFi : MonoBehaviour
{
    [SerializeField] private GameObject wifiStrength3;
    [SerializeField] private GameObject noWifiSignal;
    private const float minWaitTimeInSeconds = 15f;
    private const float maxWaitTimeInSeconds = 40f;
    public int currentWifiStrength = 2;

    private void Start()
    {
        StartCoroutine(nameof(SwitchWifiStrength));
    }

    private IEnumerator SwitchWifiStrength()
    {
        //1 in 1000 chance
        if(Random.Range(0, 1000) == 1)
        {
            SetWifiStrength(0);
        }
        else
        {
            if (currentWifiStrength == 2)
            {
                SetWifiStrength(3);
            }
            else
            {
                SetWifiStrength(2);
            }
        }

        float howMuchToWaitBeforeNextSwitch = Random.Range(minWaitTimeInSeconds, maxWaitTimeInSeconds);
        yield return new WaitForSeconds(howMuchToWaitBeforeNextSwitch);
        StartCoroutine(nameof(SwitchWifiStrength));
    }

    private void SetWifiStrength(int strength)
    {
        switch (strength)
        {
            case 0:
                wifiStrength3.SetActive(false);
                noWifiSignal.SetActive(true);
                currentWifiStrength = 0;
                break;
            case 2:
                wifiStrength3.SetActive(false);
                noWifiSignal.SetActive(false);
                currentWifiStrength = 2;
                break;
            case 3:
                wifiStrength3.SetActive(true);
                noWifiSignal.SetActive(false);
                currentWifiStrength = 3;
                break;
            default:
                wifiStrength3.SetActive(false);
                noWifiSignal.SetActive(false);
                currentWifiStrength = 2;
                break;
        }
    }
}
