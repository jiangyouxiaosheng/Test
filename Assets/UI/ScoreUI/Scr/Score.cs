using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    private Animator _anim;
    public Sprite[] _sprites;
    public Image _image;
    public TMP_Text comboTxt;
    public AudioClip[] _sounds;
    public AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
      
        comboTxt.text = $"X{GameModule.Instance.comboCount}";
        switch (GameModule.Instance.comboCount)
        {
            case 3:
                _image.sprite = _sprites[1];
                _audio.clip = _sounds[1];
                break;
            case 5:
                _image.sprite = _sprites[2];
                _audio.clip = _sounds[2];
                break;
            case 7:
                _image.sprite = _sprites[3];
                _audio.clip = _sounds[3];
                break;
            default:
                _image.sprite = _sprites[0];
                _audio.clip = _sounds[0];
                break;
        }
        _audio.Play();
    }


    public void AnimPlayOver()
    {
        Destroy(gameObject);
    }

   
}
