using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickCustom : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    static Vector2 posInput;
    [SerializeField] Image joyPos;

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joyPos.rectTransform, eventData.position, eventData.pressEventCamera, out posInput))
        {
            posInput.x /= joyPos.rectTransform.sizeDelta.x;
            posInput.y /= joyPos.rectTransform.sizeDelta.y;
            posInput = posInput.normalized;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        posInput = Vector2.zero;
    }
    public static float Horizontal()
    {
        if (posInput.x != 0)
            return posInput.x;
        else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized.x;
    }
    public static float Vertical()
    {
        if (posInput.y != 0)
            return posInput.y;
        else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized.y;
    }
}
