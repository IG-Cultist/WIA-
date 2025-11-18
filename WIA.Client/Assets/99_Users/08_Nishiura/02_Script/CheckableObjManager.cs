/// ------------------------------
/// 調査可能オブジェクトマネージャー
/// Author:Nishiura Date:25/11/18
/// ------------------------------
using System.Collections.Generic;
using UnityEngine;

public class CheckableObjManager : MonoBehaviour
{
    // 調査可能オブジェクトリスト
    [SerializeField] List<GameObject> checkableObjList;

    // キー内蔵オブジェクト番号
    int keyObjectNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 0からリストの長さ分までの乱数を設定
        keyObjectNum = Random.Range(0, checkableObjList.Count);
        Debug.Log("鍵が隠されているのは"+ checkableObjList[keyObjectNum].name);
    }

    /// <summary>
    /// オブジェクト調査処理
    /// </summary>
    /// <param name="obj"></param>
    public void CheckObject(GameObject obj)
    {
        // 調べたオブジェクトにキーが入っていた場合
        if(obj == checkableObjList[keyObjectNum])
        {
            Debug.Log("鍵を見つけた");
        }
        else
        {
            Debug.Log("なにもなかった");
        }
    }
}
