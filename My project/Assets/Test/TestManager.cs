using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    public HoTBuff buff;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.gameObject.GetComponent<BuffController>().AddBuff(enemy,buff);
        }
    }
}
