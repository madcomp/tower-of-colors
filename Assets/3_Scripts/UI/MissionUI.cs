using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private Color colorNormal1;
    [SerializeField] private Color colorNormal2;
    [SerializeField] private Color colorToCollect1;
    [SerializeField] private Color colorToCollect2;
    [SerializeField] private Color colorCollected1;
    [SerializeField] private Color colorCollected2;
    [SerializeField] private Image background1;
    [SerializeField] private Image background2;
    
    [SerializeField] private GameObject contentChecked;
    [SerializeField] private GameObject contentReward;
    [SerializeField] private Image imageRewardIcon;
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private TMP_Text textRewardLabel;
    [SerializeField] private TMP_Text textRewardValue;

    private Mission _mission;
    
    public void OnClick()
    {
        if (_mission.IsComplete() && !_mission.Reward.IsCollected())
        {
            _mission.Reward.Collect();
            Refresh();
        }
    }
    
    public void Setup(Context context, Mission mission)
    {
        _mission = mission;
        
        Refresh();
        
        textDescription.text = _mission.GetDescription(context);
        _mission.Reward.SetupUI(imageRewardIcon, textRewardValue);
    }
    
    void Refresh()
    {
        if (_mission.IsComplete())
        {
            var collected = _mission.Reward.IsCollected();
            contentReward.SetActive(!collected);
            contentChecked.SetActive(collected);
            if (collected)
            {
                SetupColors(colorCollected1, colorCollected2);
            }
            else
            {
                SetupColors(colorToCollect1, colorToCollect2);
            }
        }
        else
        {
            contentReward.SetActive(true);
            contentChecked.SetActive(false);
            SetupColors(colorNormal1, colorNormal2);
        }
    }
    
    void SetupColors(Color color1, Color color2)
    {
        background1.color = color1;
        textRewardLabel.color = color1;
        textRewardValue.color = color1;
        textDescription.color = color2;
        background2.color = color2;
    }
}
