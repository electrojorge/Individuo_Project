using TMPro;
using UnityEngine;

public class MissionHUD : MonoBehaviour
{
    public TextMeshProUGUI mainMission;
    public TextMeshProUGUI[] subMission;

    public void SetMainMission(string text)
    {
        mainMission.text = text;
    }

    public void SetSubMission(int index, string text)
    {
        subMission[index].text = text;
        subMission[index].gameObject.SetActive(true);
    }

    public void ClearSubMission()
    {
        foreach (var m in subMission) m.gameObject.SetActive(false);
    }
}
