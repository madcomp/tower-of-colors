using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private Image imageRewardIcon;
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private TMP_Text textRewardValue;
    
    public void Setup(Context context, Mission mission)
    {
        textDescription.text = mission.GetDescription(context);
        mission.Reward.SetupUI(imageRewardIcon, textRewardValue);
    }
}
