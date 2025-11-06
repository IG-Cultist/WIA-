//=================================
// タスクの進捗をチェックするスクリプト
// Aouther:y-miura
// Date:2025/11/06
//=================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskCheck : MonoBehaviour
{
    int cubeCnt; //要素の変数
    int checkCnt=1; //目標の要素数
    [SerializeField] List<int> cubeList = new List<int>(); //int型のList

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  　/// <summary>
   /// エリアに触れた時の処理
   /// </summary>
   /// <param name="other">触れたオブジェクト</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cube"))
        {
            //リストに要素を追加する
            cubeList.Add(cubeCnt);

            //キューブのタグを書き換える
            //これにより、キューブが積み重なってもチェックできるようになる
            other.tag = "Goal";

            if(cubeList.Count==checkCnt)
            {//要素数が目標数と同じになったら
                //Debug.Log("クリア");

                SceneManager.LoadScene("01_TitleScene");
            }
        }
    }

    /// <summary>
    /// エリアから離れた時の処理
    /// </summary>
    /// <param name="other">離れたオブジェクト</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Goal"))
        {//オブジェクトがエリアから離れたら
            //リストから要素を削除する
            cubeList.Remove(cubeCnt); 

            //キューブのタグを書き換える
            other.tag = "Cube";
        }
    }
}
