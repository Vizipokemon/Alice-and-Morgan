using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Clock : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dateText;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateClock), 0f, 60f);
    }

    private void UpdateClock()
    {
        System.DateTime theTime = System.DateTime.Now;
        timeText.text = theTime.ToString("HH:mm");
        dateText.text = theTime.ToString("yyyy.MM.dd.");
    }
}
