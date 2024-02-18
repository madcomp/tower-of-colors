using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMissions : MonoBehaviour
{
    [SerializeField] private MissionUI[] missionUIs;
    public void Setup(Context context, List<Mission> missions)
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
