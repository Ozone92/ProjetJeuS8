using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BalanceBar : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Key decreaseKey = Key.A;
    [SerializeField] private Key increaseKey = Key.D;

    [Header("Balance")]
    [SerializeField, Range(0f, 100f)] private float startValue = 50f;
    [SerializeField] private float driftSpeed = 18f;
    [SerializeField] private float correctionSpeed = 35f;
    [SerializeField] private float driftDirectionChangeInterval = 1.2f;
    [SerializeField, Range(0f, 100f)] private float stableMin = 40f;
    [SerializeField, Range(0f, 100f)] private float stableMax = 60f;

    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color stableColor = new Color(0.25f, 0.85f, 0.35f);
    [SerializeField] private Color unstableColor = new Color(0.9f, 0.25f, 0.2f);

    private float value;
    private float driftDirection;
    private float nextDirectionChangeAt;
    private bool active;

    public bool IsStable => value >= stableMin && value <= stableMax;
    public float Value => value;

    private void Awake()
    {
        if (!slider)
        {
            slider = GetComponentInChildren<Slider>();
        }
    }

    private void Start()
    {
        ResetBar();
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        UpdateDriftDirection();
        ApplyInput();
        value = Mathf.Clamp(value + driftDirection * driftSpeed * Time.deltaTime, 0f, 100f);
        UpdateVisuals();
    }

    public void Begin()
    {
        ResetBar();
        active = true;
    }

    public void Stop()
    {
        active = false;
    }

    public void Configure(Slider sliderReference, Image fillReference, Key decrease, Key increase)
    {
        slider = sliderReference;
        fillImage = fillReference;
        decreaseKey = decrease;
        increaseKey = increase;
        ResetBar();
    }

    public void SetDifficulty(float driftMultiplier, float correctionMultiplier)
    {
        driftSpeed *= driftMultiplier;
        correctionSpeed *= correctionMultiplier;
    }

    private void ResetBar()
    {
        active = false;
        value = Mathf.Clamp(startValue, 0f, 100f);
        PickNewDirection();
        UpdateVisuals();
    }

    private void UpdateDriftDirection()
    {
        if (Time.time >= nextDirectionChangeAt)
        {
            PickNewDirection();
        }
    }

    private void PickNewDirection()
    {
        driftDirection = driftDirection == 0f ? (Random.value < 0.5f ? -1f : 1f) : -driftDirection;
        nextDirectionChangeAt = Time.time + Random.Range(driftDirectionChangeInterval * 0.6f, driftDirectionChangeInterval * 1.4f);
    }

    private void ApplyInput()
    {
        float input = 0f;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard[decreaseKey].isPressed)
        {
            input -= 1f;
        }

        if (keyboard[increaseKey].isPressed)
        {
            input += 1f;
        }

        value = Mathf.Clamp(value + input * correctionSpeed * Time.deltaTime, 0f, 100f);
    }

    private void UpdateVisuals()
    {
        if (slider)
        {
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = value;
        }

        if (fillImage)
        {
            fillImage.color = IsStable ? stableColor : unstableColor;
        }
    }
}
