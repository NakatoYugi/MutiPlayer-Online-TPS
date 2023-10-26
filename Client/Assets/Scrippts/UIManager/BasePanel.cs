using System;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public string skinPath;
    public GameObject skin;
    public PanelManager.Layer layer = PanelManager.Layer.Panel;
    public void Init()
    {
        GameObject skinPrefab = Resources.Load(skinPath) as GameObject;
        skin = Instantiate(skinPrefab) as GameObject;
    }

    public void Close()
    {
        string name = this.GetType().ToString();
        PanelManager.Close(name);
    }

    public virtual void OnInit() { }
    public virtual void OnShow(params object[] para) { }
    public virtual void OnClose() { }
}

