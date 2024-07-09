using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControl : MonoBehaviour
{
    public Image image;
    public SoundManager soundManager;
    public bool isMute;
    public Button muteButton;

    public Slider slider;

    public int type;

    void Start()
    {
        if (gameObject.name == "Sound1")
        {
            type = 0;
        }
        else
        {
            type = 1;
        }
    }

    void Update()
    {

    }

    public void ChangeFillAmount()
    {
        image.fillAmount = slider.value;
    }

    public void ChangeRealSound()
    {
        if (isMute)
        {
            return;
        }

        soundManager.ChangeVolume(slider.value, type);
    }

    public void ClickMute()
    {
        if (isMute)
        {
            muteButton.image.sprite = Resources.Load<Sprite>("UserInterface/Flat game GUI pack/ui/PNG/Icons/icons/sound_green");
            soundManager.ChangeVolume(image.fillAmount, type);
            isMute = false;
        }
        else
        {
            muteButton.image.sprite = Resources.Load<Sprite>("UserInterface/Flat game GUI pack/ui/PNG/Icons/icons/sound_off_green");
            soundManager.ChangeVolume(0, type);
            isMute = true;
        }
    }
}
