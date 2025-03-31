using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPopBtn : MonoBehaviour
{
  private void Start()
  {
    gameObject.SetActive(!GameModule.Instance.isReviewMode);
  }

  public void OpenPopWindow()
  {
    UIModule.Instance.OpenPage(EUIPageType.PopWindowPage);
  }
}
