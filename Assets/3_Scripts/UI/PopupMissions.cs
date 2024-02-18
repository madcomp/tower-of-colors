using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMissions : MonoBehaviour
{
    [SerializeField] private CanvasGroup content;
    [SerializeField] private MissionUI[] missionUIs;
    
    public void Setup(Context context, List<Mission> missions)
    {
        if (gameObject.activeSelf)
        {
            LeanTween.cancel(content.gameObject);
            content.alpha = 1;
            LeanTween.alphaCanvas(content, 0, 0.5f)
                .setOnComplete(() =>
                {
                    Refresh(context, missions);
                    LeanTween.alphaCanvas(content, 1, 0.5f);
                });
        }
        else
        {
            Refresh(context, missions);
        }
    }

    void Refresh(Context context, List<Mission> missions)
    {
        for (int i = 0; i < missionUIs.Length; i++)
        {
            var missionUI = missionUIs[i];
            if (i < missions.Count)
            {
                missionUI.Setup(context, missions[i]);
                missionUI.gameObject.SetActive(true);
            }
            else
            {
                missionUI.gameObject.SetActive(false);
            }
        }
    }
}
