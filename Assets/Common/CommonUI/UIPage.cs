using Sirenix.OdinInspector;
using System.Collections;
using OPS.Obfuscator.Attribute;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// UI Page 基类
/// </summary>
[RequireComponent(typeof(RectTransform), typeof(Animation))]
[DoNotRename]
public abstract class UIPageBase : MonoBehaviour, IPoolElement
{
    public abstract EUIPageType PageType { get; }
    private Animation m_animation;
    public Animation Animation
    {
        get
        {
            if (!m_animation)
                m_animation = GetComponent<Animation>();

            return m_animation;
        }
    }

    public float Alpha
    {
        set
        {
            var canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            canvasGroup.alpha = value;
        }
    }

    private void Awake()
    {
        if (!m_animation)
        {
            m_animation = GetComponent<Animation>();
        }

        OnAwake();
    }

    private RectTransform m_rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (!m_rectTransform)
            {
                m_rectTransform = transform as RectTransform;
            }
            return m_rectTransform;
        }
    }

    public void OpenPage_CallByFramework()
    {
        try
        {
            OnOpen();
            ShowPage();
        }
        catch (System.Exception e)
        {
            Debug.LogError("打开界面内部报错: " + e.ToString() + "堆栈:" + e.StackTrace);
        }
    }

    public void ClosePage_CallByFramework()
    {
        try
        {
            OnHide();
            HidePage();
        }
        catch (System.Exception e)
        {
            Debug.LogError("关闭界面内部报错: " + e.ToString() + "堆栈:" + e.StackTrace);
        }
    }

    protected virtual void AfterShow()
    {
        Bizza.Channel.Analytics.Manager.ShowViewEvent(PageType.ToString());
    }

    protected void ShowPage()
    {
        if (!string.IsNullOrEmpty(m_animOnShow))
        {
            PlayAnimation(m_animOnShow);
        }
        else
        {
            OnShow();
            AfterShow();
        }
    }

    protected void HidePage()
    {
        if (!string.IsNullOrEmpty(m_animOnHide))
        {
            PlayAnimation(m_animOnHide);
        }
        else
        {
            OnClose();
            UIModule.Instance.ClosePage_CallByFramework(this);
        }
    }

    protected virtual void OnAwake() { }

    protected abstract void OnOpen();
    protected abstract void OnShow();
    protected abstract void OnHide();
    protected abstract void OnClose();

    #region Animation

    [SerializeField, FoldoutGroup("PageBase")]
    private string m_animOnShow = "";
    [SerializeField, FoldoutGroup("PageBase")]
    private string m_animOnHide = "";

    protected void PlayAnimation(string animation)
    {
        if (!m_animation)
        {
            m_animation = GetComponent<Animation>();
        }

        if (m_animOnHide == animation)
        {
            m_animation.Stop();
        }
        StartCoroutine(PlayAnimationCortious(animation));
    }

    private IEnumerator PlayAnimationCortious(string animation)
    {
        m_animation.Play(animation);
        while (m_animation.IsPlaying(animation))
        {
            yield return null;
        }
        AnimationPlayFinish(animation);
    }
    private void AnimationPlayFinish(string animationName)
    {
        if (animationName == m_animOnHide)
        {
            OnClose();
            UIModule.Instance.ClosePage_CallByFramework(this);
            return;
        }

        if (animationName == m_animOnShow)
        {
            OnShow();
            AfterShow();
            return;
        }

        OnAnimationFinish(animationName);
    }
    protected virtual void OnAnimationFinish(string animationName)
    {

    }

    public void CloseSelf()
    {
        UIModule.Instance.ClosePage(this);
    }

    #endregion

    public virtual void PoolReset()
    {

    }
}
[DoNotRename]
public abstract class UIPageBase<A> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a);

    public void OpenPage_CallByFramework(A a)
    {
        OnOpen(a);
        ShowPage();
    }
}
[DoNotRename]
public abstract class UIPageBase<A, B> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b);

    public void OpenPage_CallByFramework(A a, B b)
    {
        OnOpen(a, b);
        ShowPage();
    }
}
[DoNotRename]
public abstract class UIPageBase<A, B, C> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c);

    public void OpenPage_CallByFramework(A a, B b, C c)
    {
        OnOpen(a, b, c);
        ShowPage();
    }
}
[DoNotRename]
public abstract class UIPageBase<A, B, C, D> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c, D d);
    public void OpenPage_CallByFramework(A a, B b, C c, D d)
    {
        OnOpen(a, b, c, d);
        ShowPage();
    }
}
[DoNotRename]
public abstract class UIPageBase<A, B, C, D, E> : UIPageBase
{
    protected override void OnOpen()
    {
        return;
    }

    protected abstract void OnOpen(A a, B b, C c, D d, E e);
    public void OpenPage_CallByFramework(A a, B b, C c, D d, E e)
    {
        OnOpen(a, b, c, d, e);
        ShowPage();
    }
}