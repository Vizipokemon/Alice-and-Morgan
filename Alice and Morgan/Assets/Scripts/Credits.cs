using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private GameObject creditsGO;

    public void EnableCredits(bool enable)
    {
        creditsGO.SetActive(enable);
    }
}
