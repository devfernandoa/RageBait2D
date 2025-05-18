using UnityEngine;
using UnityEngine.UI;

public class TipPopupUI : MonoBehaviour
{
    public GameObject panel;   // The UI Panel that shows the tip
    public Text tipText;       // The text component inside the panel

    public void ShowTip(string message)
    {
        tipText.text = message;
        panel.SetActive(true);
    }

    public void HideTip()
    {
        panel.SetActive(false);
    }
}