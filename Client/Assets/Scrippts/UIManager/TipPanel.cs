using TMPro;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    private TextMeshProUGUI tipText;
    private Button closeButton;

    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    public override void OnShow(params object[] para)
    {
        tipText = skin.transform.Find("TipText").GetComponent<TextMeshProUGUI>();
        closeButton = skin.transform.Find("Close Button").GetComponent<Button>();
        closeButton.onClick.AddListener(Close);
        if (para.Length == 1)
            tipText.text = (string)para[0];
    }

    public override void OnClose()
    {
        
    }
}