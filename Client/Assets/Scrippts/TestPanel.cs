using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
