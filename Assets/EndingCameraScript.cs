using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameCamera.GetCamera().StartFade(2f, 0f, () =>
        {
            
        });
    }
    
}
