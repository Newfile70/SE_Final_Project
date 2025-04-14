using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightValueManager : MonoBehaviour
{
    private Light lightVal;
    private string LightValue;// Key of brightness value in PlayerPrefs

    private void Start()
    {
        lightVal = GetComponent<Light>();
    }


    void Update()
    {
        if (gameObject.tag == "Value 0.4")//Standard size is 0.4f
        {
            lightVal.intensity = PlayerPrefs.GetFloat("LightValue", 60f) / 60 * 0.4f;//Multiply the value passed in from the Setting interface by the standard brightness size
        }
        if (gameObject.tag == "Value 1")//Standard size is 1f
        {
            lightVal.intensity = PlayerPrefs.GetFloat("LightValue", 60f) / 60 * 1f;//Multiply the value passed in from the Setting interface by the standard brightness size
        }

    }

}
