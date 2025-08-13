using System.Collections.Generic;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private Transform appleParent;

    [SerializeField] private int width = 8, height = 14;
    [SerializeField] private int spacing = 15;
    private List<Apple> appleList = new List<Apple>();
    public List<Apple> AppleList { get {  return appleList; } }

    void Start()
    {
        SpawnApples();
    }

    private void SpawnApples()
    {
        Vector2 size = applePrefab.GetComponent<RectTransform>().sizeDelta;
        size += new Vector2(spacing, spacing);

        int sum = 0;

        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                GameObject clone = Instantiate(applePrefab, appleParent);
                RectTransform rect = clone.GetComponent<RectTransform>();

                float px = (-width * 0.5f + 0.5f + x) * size.x;
                float py = (height * 0.5f - 0.5f - y) * size.y;
                rect.anchoredPosition = new Vector2(px, py);

                Apple apple = clone.GetComponent<Apple>();
                apple.Number = Random.Range(1, 10);

                if(y == height - 1 && x == width - 2)
                {
                    var exclusionNumber = 10 - (sum % 10);

                    if(apple.Number ==  exclusionNumber)
                    {
                        if(apple.Number == 1)
                        {
                            apple.Number = Random.Range(2, 10);
                        }
                        else if(apple.Number == 9)
                        {
                            apple.Number = Random.Range(1, 9);
                        }
                        else
                        {
                            if(apple.Number < 6)
                            {
                                apple.Number = Random.Range(1, exclusionNumber);
                            }
                            else
                            {
                                apple.Number = Random.Range(exclusionNumber + 1, 10);
                            }
                        }
                    }
                }

                if(y == height - 1 && x == width - 1)
                {
                    apple.Number = 10 - (sum % 10);
                }

                sum += apple.Number;

                appleList.Add(apple);
            }
        }
    }

    public void DestroyApple(Apple removeApple)
    {
        AppleList.Remove(removeApple);
        Destroy(removeApple.gameObject);
    }
}
