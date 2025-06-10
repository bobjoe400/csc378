using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; // Adjusted for smoother scroll
    private ScrollRect scrollRect;
    
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    
    void Update()
    {
        scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
        
        if (scrollRect.verticalNormalizedPosition >= 1f)
        {
            scrollRect.verticalNormalizedPosition = 0f; // Loop back to bottom
        }
    }
}
