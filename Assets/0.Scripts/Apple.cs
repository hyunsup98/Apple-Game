using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    [SerializeField] private TMP_Text textNumber;
    public RectTransform rect;
    private Image image;

    private int number;
    public int Number
    {
        get { return number; }
        set
        {
            number = value;
            textNumber.text = number.ToString();
        }
    }

    public Vector3 position { get { return rect.position; } }

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void OnSelected()
    {
        image.color = Color.blue;
    }

    public void OnDeselected()
    {
        image.color = Color.red;
    }
}
