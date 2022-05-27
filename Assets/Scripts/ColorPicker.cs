﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler {

    [SerializeField] RectTransform picker;
    [SerializeField] Image pickedColorImage;
    [SerializeField] Material colorWheelMat;
    [SerializeField] int totalNumberofColors = 24;
    [SerializeField] int wheelsCount = 2;

    [SerializeField][Range(0, 360)] float startingAngle = 0;
    [SerializeField] bool controlSV = false;
    [SerializeField] bool inertia = true;
    [SerializeField] float decelerationRate = 0.135f;
    [SerializeField] bool wholeSegment = false;

    [SerializeField][Range(0.5f, 0.001f)] float minimumSatValStep = 0.01f;
    [SerializeField][Range(0, 1)] float minimumSaturation = 0.25f;
    [SerializeField][Range(0, 1)] float maximumSaturation = 1;

    float minimumValue = 0;
    float maximumValue = 2;

    bool dragging = false;
    float satValAmount = 1;
    float omega = 0;
    float previousTheta;
    float theta;

    float previousDiscretedH;
    float sat = 1, val = 1;
    Color selectedColor;

    public ColorChangeEvent OnColorChange;
    public HueChangeEvent OnHueChange;

    Vector2 centerPoint;
    float paletteRadius;
    float pickerHueOffset;

    void Awake() {
        CalculatePresets();
        UpdateMaterialInitialValues();
        UpdateMaterial();
        UpdateColor();
    }

    void Update() {
        float deltaTime = Time.unscaledDeltaTime;
        if (dragging && inertia) {
            float newOmega = (theta - previousTheta) / Time.deltaTime;
            omega = Mathf.Lerp(omega, newOmega, deltaTime * 10);
            previousTheta = theta;
        }
        if (!dragging && omega != 0) {
            omega *= Mathf.Pow(decelerationRate, deltaTime);
            if (Mathf.Abs(omega) < 1)
                omega = 0;
            float dtheta = omega * deltaTime;
            Hue += dtheta / 360;
            if (Hue < 0) Hue += wheelsCount;
            UpdateHue();
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            satValAmount += Input.GetAxis("Mouse ScrollWheel");
            satValAmount = Mathf.Clamp(satValAmount, 0, 2);
            CalculateSaturationAndValue(satValAmount);
        }
    }

    public Color SelectedColor {
        get {
            return selectedColor;
        }
        private set {
            if (value != selectedColor) {
                selectedColor = value;
                OnColorChange?.Invoke(SelectedColor);
            }
        }
    }

    public float Hue { get; private set; } = 0;

    public float Value {
        get { return val; }
        set {
            float newVal = Mathf.Clamp(value, minimumValue, maximumValue);
            if (Mathf.Abs(val - newVal) > minimumSatValStep) {
                val = newVal;
                UpdateMaterial();
                UpdateColor();
            }
        }
    }

    public float Saturation {
        get { return sat; }
        set {
            float newSat = Mathf.Clamp(value, minimumSaturation, maximumSaturation);
            if (Mathf.Abs(sat - newSat) > minimumSatValStep) {
                sat = newSat;
                UpdateMaterial();
                UpdateColor();
            }
        }
    }

    void UpdateMaterialInitialValues() {
        colorWheelMat.SetFloat("_StartingAngle", startingAngle);
        colorWheelMat.SetInt("_ColorsCount", totalNumberofColors);
        colorWheelMat.SetInt("_WheelsCount", wheelsCount);

    }

    void CalculatePresets() {
        centerPoint = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        RectTransform rect = GetComponent<RectTransform>();
        paletteRadius = rect.sizeDelta.x / 2;
        Vector3 pickerLocalPosition = picker.localPosition;
        float angle = Vector2.SignedAngle(Vector2.right, pickerLocalPosition);
        if (angle < 0) angle += 360;
        pickerHueOffset = angle / 360;

    }

    void CalculateSaturationAndValue(float amount) {
        if (amount > 1 && amount < 2) {
            val = 1;
            sat = 2 - amount;
        } else if (amount < 1) {
            sat = 1;
            val = amount;
        } else {
            val = 2;
            sat = 2 - amount;
        }
        sat = Mathf.Clamp(sat, minimumSaturation, maximumSaturation);
        val = Mathf.Clamp(val, minimumValue, maximumValue);
        UpdateMaterial();
        UpdateColor();
    }

    void UpdateHue() {
        UpdateMaterial();
        UpdateColor();
    }

    void UpdateMaterial() {
        if (wholeSegment) {
            float discretedHue = ((int)((Hue + startingAngle / 360.0f) * totalNumberofColors)) / (1.0f * (totalNumberofColors));
            colorWheelMat.SetFloat("_Hue", discretedHue);
        } else {
            colorWheelMat.SetFloat("_Hue", Hue);
        }
        if (controlSV) {
            colorWheelMat.SetFloat("_Sat", sat);
            colorWheelMat.SetFloat("_Val", val);
        }

    }

    void UpdateColor() {

        float shiftedH = (pickerHueOffset + startingAngle / 360.0f + Hue % wheelsCount) / wheelsCount;
        shiftedH = shiftedH % 1.0f;
        float discretedH = ((int)(shiftedH * totalNumberofColors)) / (1.0f * (totalNumberofColors - 1));
        Color color;
        color = Color.HSVToRGB(discretedH, sat, val);
        if (previousDiscretedH != discretedH)
            OnHueChange?.Invoke(discretedH);
        if (pickedColorImage) pickedColorImage.color = color;
        SelectedColor = color;
        previousDiscretedH = discretedH;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!dragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector2 dragVec = eventData.delta;
        Vector2 currentPos = eventData.position;
        Vector2 prevPos = currentPos - dragVec;

        float dtheta = Vector2.SignedAngle(currentPos - centerPoint, prevPos - centerPoint);
        theta += dtheta;

        Hue += dtheta / 360;
        if (Hue < 0) Hue += wheelsCount;

        UpdateHue();

    }
    public void OnInitializePotentialDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        omega = 0;
    }
    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        dragging = true;
        omega = 0;
    }
    public void OnEndDrag(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        dragging = false;
    }
}

[System.Serializable]
public class ColorChangeEvent : UnityEvent<Color> { }
[System.Serializable]
public class HueChangeEvent : UnityEvent<float> { }
