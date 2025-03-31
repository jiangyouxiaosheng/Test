using UnityEngine;

public class CoinAnim : MonoBehaviour
{
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }


    public void GetAnimCoin()
    {
        if (_anim.IsInTransition(0) || _anim.GetCurrentAnimatorStateInfo(0).IsName("GetCoinAnim"))
        {
            // 如果动画正在播放，从头开始播放
            _anim.Play("GetCoinAnim", 0, 0f);
        }
        else
        {
            // 如果动画未播放，直接播放
            _anim.SetTrigger("Play");
        }
    }
}
