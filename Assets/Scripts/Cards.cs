using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cards : MonoBehaviour
{
    public SpriteRenderer render;
    public Transform WayPoint;
    public Sprite Back, front;
    public bool isEnemyCard = false;

    private Vector3 bezierPos;
    void Start()
    {
        if (isEnemyCard)
            render.sprite = Back;
        else
            render.sprite = front;
        
        StartCoroutine(BezireMove());
    }

    IEnumerator BezireMove()
    {
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            // 조절점이 2개일 때 베지어 곡선 공식
            bezierPos = (1 - t) * gameObject.transform.position +
                        t * WayPoint.position;
            bezierPos.z = 0;
            transform.position = bezierPos;
            yield return null;
        }
    }
}