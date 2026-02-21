using System;
using UnityEngine;

// 카드 1장 렌더링
// 카드 앞/뒤 표시 / 클릭 가능 여부 제어 / 클릭 시 Card를 상위 프레젠터로 전달
[RequireComponent(typeof(BoxCollider))]
public class CardView : MonoBehaviour
{
	[SerializeField] private SpriteRenderer frontRenderer;
	[SerializeField] private SpriteRenderer backRenderer;
	[SerializeField] private BoxCollider hitCollider;

	private Card boundCard;
	private Action<Card> clickHandler;
	private bool clickable;

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

	private void OnMouseUpAsButton()
	{
		if (!clickable || boundCard == null) return;
		clickHandler?.Invoke(boundCard);
	}
}