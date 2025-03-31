using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoading : MonoBehaviour
{

    private static List<LoadingTask> loadingTasks = new List<LoadingTask>();
    private static AsyncOperation loadingOperation;

    public static void AddTask(LoadingTask loginTask)
    {
        loadingTasks.Add(loginTask);
    }


    private void Awake()
    {
        //WZ 初始化
        AccountController.InitAccountCtrl();
    }

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());        
    }


    [Button]
    void SelfTest()
    {
        var data = JsonConvert.DeserializeObject<HttpController.HttpResponse<AccountController.WithdrawalObtain>>("{\"code\":\"2000\",\"msg\":\"success\",\"uid\":\"113e53f4-40fc-4c33-958d-00838e33ad36\",\"st\":1739192023370,\"data\":{\"orderId\":1888934370537426945,\"deductionCoin\":1828,\"reduceAmount\":-30.0,\"udCoin\":6969,\"udAmount\":114.34,\"deductionAmount\":30.0,\"reduceCoin\":-1828,\"udAvailableAmount\":114.34,\"udPendingAmount\":0.0,\"currencyCode\":\"IDR\",\"status\":6,\"commissionAmount\":1.35,\"commissionCoin\":82}}");
        Debug.Log(data);
    }

    private static IEnumerator LoadSceneCoroutine()
    {
        float realtimeSinceStartup = Time.realtimeSinceStartup;

        int taskIndex = 0;
        while (taskIndex < loadingTasks.Count)
        {
            if (!loadingTasks[taskIndex].IsActive)
                loadingTasks[taskIndex].Activate();

            if (loadingTasks[taskIndex].IsFinished)
            {
                taskIndex++;
            }

            yield return null;
        }

        //正常流程加载场景
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCount < sceneIndex)
            Debug.LogError("[Loading]: First scene is missing!");

        float MINIMUM_LOADING_TIME = 2f;
        float minimumFinishTime = realtimeSinceStartup + MINIMUM_LOADING_TIME;

        loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone || realtimeSinceStartup < minimumFinishTime)
        {
            yield return null;

            realtimeSinceStartup = Time.realtimeSinceStartup;
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
            }
        }

        UIModule.Instance.InitGameModule();
        GameModule.Instance.InitGameModule();
        SenceModule.Instance.InitGameModule();
     
        /*if (AccountController.Instance.stInfo.earningSwitch == 1)
        {
            Debug.LogError(AccountController.Instance.stInfo.earningSwitch );
            UIController.ShowPage<UIBizzaAAA>();
            Debug.Log("网赚模式开启");
        }
        else
        {
            UIController.HidePage<UIBizzaAAA>();
            Debug.Log("网赚模式关闭");
        }*/
    }
}
