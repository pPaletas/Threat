using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackableObject : MonoBehaviour
{
    public float range;
    public bool canBeGrabbed = false;
    [SerializeField] private float _loadSpeed = 20f;
    [SerializeField] private float _unloadSpeed = 10f;
    [SerializeField] private Transform _iconPosition;
    [SerializeField] protected Sprite _sprite;
    [SerializeField] private string _iconAssetName = "";

    [HideInInspector] public bool isActive = true;
    [HideInInspector] public bool isBeingGrabbed = false;
    [HideInInspector] public bool onlyShow = false;

    private Transform _iconsContainer;
    private GameObject _imgOnCanvas;
    private Image _fillIcon;

    private float _loadedAmount = 0f;

    protected bool isFocused = false;
    protected bool isIconDisplaying = false;

    public GameObject ImgOnCanvas { get => _imgOnCanvas; }
    public Transform IconPosition { get => _iconPosition; }

    public bool Load()
    {
        _loadedAmount += Time.deltaTime * _loadSpeed;
        _loadedAmount = Mathf.Clamp(_loadedAmount, 0f, 100f);

        _fillIcon.fillAmount = _loadedAmount / 100f;

        if (_loadedAmount >= 100)
        {
            _loadedAmount = 0;
            OnLoaded_E();
            DisplayIcon(false);
            return true;
        }

        return false;
    }

    public void Unload()
    {
        _loadedAmount -= Time.deltaTime * _unloadSpeed;
        _loadedAmount = Mathf.Clamp(_loadedAmount, 0f, 100f);

        _fillIcon.fillAmount = _loadedAmount / 100f;
    }

    public virtual void DisplayIcon(bool display)
    {
        if (isIconDisplaying != display)
        {
            isIconDisplaying = display;
            _imgOnCanvas.SetActive(display);

            if (!display) UnfocusIcon();
        }
    }

    public virtual void FocusIcon()
    {
        if (!isFocused)
        {
            isFocused = true;
            _imgOnCanvas.transform.localScale = Vector3.one * 1.4f;
        }
    }

    public void UnfocusIcon()
    {
        if (isFocused)
        {
            isFocused = false;
            if (!onlyShow) _imgOnCanvas.transform.localScale = Vector3.one;
            else _imgOnCanvas.transform.localScale = Vector3.one * 1.4f;
            _loadedAmount = 0f;
            _fillIcon.fillAmount = 0f;
        }
    }

    protected virtual void OnLoaded_E()
    {
        isActive = false;
        _imgOnCanvas.transform.localScale = Vector3.one;
    }

    protected virtual void Awake()
    {
        _iconsContainer = GameObject.Find("HUD/IconsContainer").transform;
        _iconPosition = _iconPosition == null ? transform : _iconPosition;

        CreateIconOnCanvas();
    }

    private void CreateIconOnCanvas()
    {
        string assetName = string.IsNullOrEmpty(_iconAssetName) ? "HackableIcon" : _iconAssetName;
        GameObject iconPrefab = Resources.Load<GameObject>(assetName);

        _imgOnCanvas = Instantiate(iconPrefab, _iconsContainer);
        _fillIcon = _imgOnCanvas.transform.Find("Bar").GetComponent<Image>();

        if (_sprite != null)
            _imgOnCanvas.transform.Find("Image").GetComponent<Image>().sprite = _sprite;

        _imgOnCanvas.SetActive(false);
    }
}
