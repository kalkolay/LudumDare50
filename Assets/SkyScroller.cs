using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyScroller : MonoBehaviour
{
    public Image sky1;
    public Image sky2;
    float x1;
    float x2;
    
    void Start()
    {
        x1 = 43;
        x2 = -667;
        sky1.transform.localPosition = new Vector3(x1, sky1.transform.localPosition.y, sky1.transform.localPosition.z);
        sky2.transform.localPosition = new Vector3(x2, sky2.transform.localPosition.y, sky2.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        x1 += 0.08f;
        x2 += 0.08f;
        sky1.transform.localPosition = new Vector3(x1, sky1.transform.localPosition.y, sky1.transform.localPosition.z);
        sky2.transform.localPosition = new Vector3(x2, sky2.transform.localPosition.y, sky2.transform.localPosition.z);
        if (x1 > 650)
            x1 = x2 - 712;
        if (x2 > 650)
            x2 = x1 - 712;
    }
}

