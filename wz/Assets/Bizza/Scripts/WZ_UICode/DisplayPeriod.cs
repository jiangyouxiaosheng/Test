using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPeriod : MonoBehaviour
{
    public float Delay = 20;
    public float Period = 30;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ToggleEnable", Delay, Period);

        gameObject.SetActive(false);
    }

    void ToggleEnable()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
