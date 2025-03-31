
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SenceModule : BaseGameModule<SenceModule>
{
   private int _playerSelectedLv = 1;
   private int _currentLv;
   public int NowLv = 1;
   public override void InitGameModule()
   {
       _currentLv = 1;
       
   }

   public override void ReleaseGameModule()
   {
      
   }
   
   public void LoadSceneByIndex(int sceneIndex)
   {
       
        SceneManager.LoadScene(sceneIndex); // 通过场景索引加载
   }

   public void ChangeLv()
   {
       
   }

   public void NextScene()
   {
       _playerSelectedLv++;
       NowLv++;
       PlayerPrefs.SetInt("NowLv", NowLv);
       UIModule.Instance.ClosePage(EUIPageType.BattlePage);
       ChangeLv();
   }

   public void ReStartGame()
   {
       StartCoroutine(LoadSceneAsync(_currentLv));
       
   }
   private IEnumerator LoadSceneAsync(int sceneIndex)
   {
       Debug.LogError(sceneIndex);
       AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

      
       while (!asyncLoad.isDone)
       {
           yield return null; // 等待下一帧
       }
       //GameModule.Instance.FailPage();
       
   }
  
}
