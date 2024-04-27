using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject prefab;
    public Sprite[] flowerCards;
    public Tuple<int, int>[] map;
    public const int CardLen = 52;

    void Start()
    {
    }

    public void cardSet()
    {
        shuffle();
        for (int i = 0; i < CardLen; i++)
        {
            int x = map[i].Item1;
            int y = map[i].Item2;
            GameObject temp = Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
            temp.GetComponent<SpriteRenderer>().sprite = flowerCards[i];
        }
    }

    void swap(int x, int y)
    {
        Tuple<int, int> temp = map[x];
        map[x] = map[y];
        map[y] = temp;
    }

    public void shuffle()
    {
        map = new Tuple<int, int>[CardLen];
        map[0] = new Tuple<int, int>(18, 8);
        map[1] = new Tuple<int, int>(18, 4);

        // 순서대로 생성해둔 뒤 랜덤한 위치로 스왑한다.
        int x = -15, y = -8;
        for (int i = 2; i < map.Length; i++)
        {
            map[i] = new Tuple<int, int>(x, y);
            x += 3;
            if (x == 15)
            {
                x = -15;
                y += 4;
            }
        }

        for (int i = 0; i < 100; i++)
        {
            int first = Random.Range(1, map.Length);
            int second = Random.Range(1, map.Length);
            swap(first, second);
        }
    }
}