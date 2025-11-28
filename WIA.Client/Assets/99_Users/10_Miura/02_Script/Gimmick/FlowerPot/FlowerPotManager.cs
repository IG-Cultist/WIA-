using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.UIElements;

public class FlowerPotManager : MonoBehaviour
{
    [SerializeField] public GameObject potObj; //A–Ø”«ƒIƒuƒWƒFƒNƒg
    [SerializeField] public GameObject potFragmentObj; //A–Ø”«‚Ì”j•ĞƒIƒuƒWƒFƒNƒg

    public List<GameObject> potList = new List<GameObject>(); //A–Ø”«‚Ì¶¬ŒÂ”‚ğŠi”[‚·‚éƒŠƒXƒg
    public List<GameObject> randomSpawnPoint =new List<GameObject>(); //A–Ø”«‚ªƒXƒ|[ƒ“‚·‚éêŠ‚ÌƒŠƒXƒg
    public List<int> nowSpawnList=new List<int>();

    public bool isThreePot = false; //A–Ø”«‚ª3ŒÂ‘¶İ‚·‚é‚©‚Ç‚¤‚©‚Ì•Ï”
    public int generatNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //A–Ø”«‚ğ¶¬‚·‚é
        GeneratePot();
    }

    // Update is called once per frame
    void Update()
    {
        if (potList.Count >= 3)
        {//potList‚Ì—v‘f‚ª3ˆÈã‚¾‚Á‚½‚ç
            isThreePot = true; // A–Ø”«‚ª3ŒÂ‚ ‚éó‘Ô‚É‚·‚é
            RemoveList(generatNumber);
        }
        if (potList.Count <= 2)
        {//potList‚Ì—v‘f‚ª2ˆÈ‰º‚¾‚Á‚½‚ç
            GeneratePot(); //A–Ø”«‚ğ¶¬‚·‚é
            isThreePot = false; // A–Ø”«‚ª3ŒÂ‘¶İ‚µ‚È‚¢ó‘Ô
        }
    }

    /// <summary>
    /// A–Ø”«‚ğ¶¬‚·‚éˆ—
    /// </summary>
    private void GeneratePot()
    {
        while (true)
        {        
            // ¶¬ˆÊ’u‚ğŒˆ‚ß‚é
            generatNumber = Random.Range(0, randomSpawnPoint.Count); // ¶¬ˆÊ’u‚ğspawnPointPos‚Ì’†‚©‚çŒˆ‚ß‚é

            if (!nowSpawnList.Contains(generatNumber))
            {
                break;
            }
        }
        nowSpawnList.Add(generatNumber);

        // potObj‚ğ¶¬‚·‚é
        Instantiate(potObj, randomSpawnPoint[generatNumber].transform.position, randomSpawnPoint[generatNumber].transform.rotation); // randomSpawnPoint‚ÉŠi”[‚³‚ê‚½gameObject‚ÌgeneratNumber‚ÌêŠ‚É¶¬

        //potList‚É—v‘f‚ğ’Ç‰Á‚·‚é
        potList.Add(potObj);
    }

    public void RemoveList(int generateNumber)
    {
        if(nowSpawnList.Contains(generateNumber))
        {
            nowSpawnList.Remove(generateNumber);
        }
    }
}