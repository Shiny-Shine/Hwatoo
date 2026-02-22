using System;
using UnityEngine;
using UnityEngine.EventSystems;

// 카드 1장 렌더링
// 카드 앞/뒤 표시 / 클릭 가능 여부 제어 / 클릭 시 Card를 상위 프레젠터로 전달
[RequireComponent(typeof(BoxCollider))]
public class CardView : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private SpriteRenderer frontRenderer;
	[SerializeField] private SpriteRenderer backRenderer;
	[SerializeField] private BoxCollider hitCollider;

	private Card boundCard;
	private Action<Card> clickHandler;
	private bool clickable;
	
	void Reset()
	{
		if (hitCollider == null)
			hitCollider = GetComponent<BoxCollider>();
	}

	public void Bind(Card card, Sprite front, Sprite back, bool faceUp, bool canClick, Action<Card> onClick)
	{
		boundCard = card;
		clickHandler = onClick;
		clickable = canClick;

		frontRenderer.sprite = front;
		backRenderer.sprite = back;

		frontRenderer.gameObject.SetActive(faceUp);
		backRenderer.gameObject.SetActive(!faceUp);

		if (hitCollider != null)
			hitCollider.enabled = canClick;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData != null && eventData.button != PointerEventData.InputButton.Left) return;
		if (!clickable || boundCard == null) return;

		clickHandler?.Invoke(boundCard);
	}
}