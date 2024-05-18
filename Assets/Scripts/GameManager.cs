using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;

    // void Awake()
    // {
    //     if (null == singleton)
    //     {
    //         singleton = this;
    //         DontDestroyOnLoad(this.gameObject);
    //     }
    //     else
    //         Destroy(this.gameObject);
    // }

    public GameObject prefab, cardList, hand, Ehand;
    public Transform[] wayPoints;
    public Sprite[] flowerCards;

    [SerializeField]private int idx = 1;
    private Tuple<int, int>[] map;
    private const int CardLen = 52;

    public void btnShuffle()
    {
        StartCoroutine(cardSet());
    }

    IEnumerator cardSet()
    {
        shuffle();
        
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Card");
        foreach (var i in tmp)
        {
            Destroy(i.gameObject);
        }
        
        idx = 1;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++) // 바닥 패 4장
            {
                GameObject temp = Instantiate(prefab, new Vector3(-10, 0, 0), Quaternion.identity);
                Cards tmpCard = temp.GetComponent<Cards>();
                //temp.transform.SetParent(cardList.transform);
                tmpCard.front = flowerCards[idx];
                tmpCard.WayPoint = wayPoints[idx - 1];
                idx++;
            }

            yield return new WaitForSeconds(0.33f);

            for (int j = 0; j < 10; j++) // 플레이어 & 상대 패 5장씩
            {
                GameObject temp = Instantiate(prefab, Vector2.zero, Quaternion.identity);
                Cards tmpCard = temp.GetComponent<Cards>();
                tmpCard.front = flowerCards[idx];
                tmpCard.WayPoint = wayPoints[idx - 1];
                if (j >= 5)
                {
                    tmpCard.isEnemyCard = true;
                    temp.transform.SetParent(Ehand.transform);
                }
                else
                    temp.transform.SetParent(hand.transform);
                idx++;
            }

            yield return new WaitForSeconds(0.33f);
        }
    }

    void swap(int x, int y)
    {
        Sprite temp = flowerCards[x];
        flowerCards[x] = flowerCards[y];
        flowerCards[y] = temp;
    }

    public void shuffle()
    {
        for (int i = 0; i < 100; i++)
        {
            int first = Random.Range(1, flowerCards.Length);
            int second = Random.Range(1, flowerCards.Length);
            swap(first, second);
        }
    }
}