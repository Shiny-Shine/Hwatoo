using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cards : MonoBehaviour, IPointerClickHandler
{
    public Animator ani;
    public SpriteRenderer render;
    public Transform WayPoint;
    public Sprite front;
    public bool isEnemyCard = false;
    public BoxCollider box;

    private Vector3 bezierPos;

    void Start()
    {
        render.sprite = front;
        StartCoroutine(CardMove(transform.position, WayPoint.position, 5f));
    }

    public void Move(Vector3 start, Vector3 end, float speed)
    {
        StartCoroutine(CardMove(start, end, speed));
    }

    IEnumerator CardMove(Vector3 start, Vector3 end, float speed)
    {
        for (float t = 0; t < 1; t += Time.deltaTime * speed)
        {
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        transform.position = Vector3.Lerp(start, end, 1);

        if (!isEnemyCard)
            ani.SetTrigger("Flip");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ani.SetTrigger("Click");
    }
}