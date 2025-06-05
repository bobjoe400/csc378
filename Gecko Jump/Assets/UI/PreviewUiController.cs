using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class PreviewUIController : MonoBehaviour 
{
    [Header("UI References")]
    public Image fillImage;
    public GameObject previewUi;
    public GameObject aliveUi;
    
    [Header("Settings")]
    public float holdDuration = 1.5f;
    
    [Header("Visual Effects")]
    public Color startColor = Color.white;
    public Color endColor = Color.green;
    public AnimationCurve fillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // Event for when skip is completed
    public event Action OnSkipComplete;
    
    private float holdTime = 0f;
    private bool isHolding = false;
    private bool canSkip = false;
    private Coroutine fillCoroutine;
    
    // Input Action for any key press
    private InputAction anyKeyAction;

    void Awake()
    {
        // Create an input action that responds to any key press
        anyKeyAction = new InputAction("AnyKey", binding: "*/<Button>");
        anyKeyAction.performed += OnAnyKeyPressed;
        anyKeyAction.canceled += OnAnyKeyReleased;
    }

    void OnEnable()
    {
        anyKeyAction?.Enable();
    }

    void OnDisable()
    {
        anyKeyAction?.Disable();
    }

    void OnDestroy()
    {
        anyKeyAction?.Dispose();
    }

    void Start()
    {
        ResetVisuals();
        HideSkipUI();
    }

    void OnAnyKeyPressed(InputAction.CallbackContext context)
    {
        if (canSkip && !isHolding)
        {
            StartHolding();
        }
    }

    void OnAnyKeyReleased(InputAction.CallbackContext context)
    {
        if (isHolding)
        {
            StopHolding();
        }
    }

    public void ShowSkipUI()
    {
        canSkip = true;
        if (previewUi != null)
        {
            previewUi.SetActive(true);
        }
        if (aliveUi != null)
        {
            aliveUi.SetActive(false);
        }
    }

    public void HideSkipUI()
    {
        canSkip = false;
        StopHolding();
        if (previewUi != null)
        {
            previewUi.SetActive(false);
        }
        if (aliveUi != null)
        {
            aliveUi.SetActive(true);
        }
    }

    void StartHolding()
    {
        if (!canSkip) return;
        
        isHolding = true;
        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        fillCoroutine = StartCoroutine(FillSliderSmooth());
    }

    void StopHolding()
    {
        isHolding = false;
        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        StartCoroutine(ResetSliderSmooth());
    }

    IEnumerator FillSliderSmooth()
    {
        while (isHolding && holdTime < holdDuration && canSkip)
        {
            holdTime += Time.deltaTime;
            float progress = holdTime / holdDuration;
            float curvedProgress = fillCurve.Evaluate(progress);
            
            if (fillImage != null)
            {
                fillImage.fillAmount = curvedProgress;
                fillImage.color = Color.Lerp(startColor, endColor, progress);
            }
            
            if (holdTime >= holdDuration)
            {
                CompleteSkip();
                yield break;
            }
            
            yield return null;
        }
    }

    IEnumerator ResetSliderSmooth()
    {
        float startFill = fillImage?.fillAmount ?? 0f;
        Color startCol = fillImage?.color ?? startColor;
        float resetTime = 0f;
        float resetDuration = 0.2f;
        
        while (resetTime < resetDuration)
        {
            resetTime += Time.deltaTime;
            float progress = resetTime / resetDuration;
            
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Lerp(startFill, 0f, progress);
                fillImage.color = Color.Lerp(startCol, startColor, progress);
            }
            
            yield return null;
        }
        
        ResetVisuals();
    }

    void ResetVisuals()
    {
        holdTime = 0f;
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
            fillImage.color = startColor;
        }
    }

    void CompleteSkip()
    {
        OnSkipComplete?.Invoke();
        HideSkipUI();
    }
}