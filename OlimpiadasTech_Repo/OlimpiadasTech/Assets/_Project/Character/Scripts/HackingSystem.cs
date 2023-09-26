using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HackingSystem : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask _wallsMask;
    [SerializeField] private Transform _grabButton;
    [SerializeField] private Transform _grabTransform;
    [SerializeField] private Rig _rightHandIK;

    [HideInInspector] public bool isGrabbingSomething = false;

    private List<HackableObject> _hackableObjects = new List<HackableObject>();
    private Transform _plrCenter;
    private StarterAssetsInputs _input;
    private ThirdPersonController _movement;
    private Animator _anim;
    private CharacterController _cc;
    private KillSystem _killSystem;
    private HackableObject _focusedObject = null;

    private int _hackingHash = Animator.StringToHash("Hacking");
    private int _carryingHash = Animator.StringToHash("Carrying");

    private bool IsCameraBlocked(Vector3 objPos)
    {
        bool isBlocked = Physics.Linecast(objPos, _cam.transform.position, _wallsMask);

        return isBlocked;
    }

    private bool IsCloseToCenterOfCamera(Vector3 objPos)
    {
        Vector2 objPosOnScreen = _cam.WorldToScreenPoint(objPos);

        // Obtenemos la distancia en relación al centro
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float dist = Vector2.Distance(center, objPosOnScreen);
        // Retornamos si está lo suficientemente cerca
        return dist <= 300f;
    }

    private void GetAllHackableObjectsOnScene()
    {
        _hackableObjects = GameObject.FindObjectsOfType<HackableObject>(true).ToList();
    }

    private void GetAvailableObject()
    {
        _focusedObject = null;

        if (_hackableObjects.Count <= 0) return;

        foreach (HackableObject obj in _hackableObjects)
        {
            float dist = Vector3.Distance(obj.transform.position, _plrCenter.position);

            bool isGameObjectActive = obj.gameObject.activeInHierarchy;
            bool isInRange = dist <= obj.range;

            Vector3 unit = (obj.transform.position - _cam.transform.position);
            unit.Normalize();

            Vector3 fwd = _cam.transform.forward;
            fwd.Normalize();

            float dot = Vector3.Dot(fwd, unit);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (isGameObjectActive && isInRange && !IsCameraBlocked(obj.transform.position) && obj.isActive && !obj.isBeingGrabbed && !isGrabbingSomething && angle <= 60f)
            {
                obj.DisplayIcon(true);

                obj.ImgOnCanvas.transform.position = _cam.WorldToScreenPoint(obj.IconPosition.position);

                if (IsCloseToCenterOfCamera(obj.transform.position) && _focusedObject == null && !obj.onlyShow)
                {
                    _focusedObject = obj;
                }
                else
                {
                    obj.UnfocusIcon();
                }
            }
            else
            {
                obj.DisplayIcon(false);

                if (obj.canBeGrabbed) _grabButton.gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        _plrCenter = transform.Find("Center");
        _input = GetComponent<StarterAssetsInputs>();
        _movement = GetComponent<ThirdPersonController>();
        _anim = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
        _killSystem = GetComponent<KillSystem>();

        _input.onKillButtonPressed += OnGrabPressed;

        GetAllHackableObjectsOnScene();
    }

    private void OnDisable()
    {
        _input.onKillButtonPressed -= OnGrabPressed;
    }

    private void Update()
    {
        if (!_killSystem.IsKilling)
        {
            _movement.canMove = true;
            _anim.SetBool(_hackingHash, false);

            if (_input.hack && _focusedObject != null)
            {
                _movement.canMove = false;
                _focusedObject.Load();
                _anim.SetBool(_hackingHash, true && _focusedObject.playAnimation);
            }
            else if (_focusedObject != null)
            {
                _focusedObject.Unload();
            }
        }
    }

    private void LateUpdate()
    {
        _grabButton.gameObject.SetActive(false);
        GetAvailableObject();
        if (_focusedObject != null)
        {
            _focusedObject.FocusIcon();

            if (_focusedObject.canBeGrabbed)
            {
                _grabButton.gameObject.SetActive(true);
                _grabButton.position = _cam.WorldToScreenPoint(_focusedObject.transform.position);
            }
        }
    }

    #region Callbacks
    private void OnGrabPressed()
    {
        bool isCarryingSomething = _grabTransform.childCount > 0;

        if (_focusedObject != null && _focusedObject.canBeGrabbed && !isCarryingSomething)
        {
            Physics.IgnoreCollision(_focusedObject.GetComponent<Collider>(), _cc, true);

            _focusedObject.transform.SetParent(_grabTransform);
            _focusedObject.GetComponent<Rigidbody>().isKinematic = true;
            _focusedObject.transform.localPosition = Vector3.zero;
            _focusedObject.transform.localRotation = Quaternion.identity;
            _focusedObject.isBeingGrabbed = true;
            isCarryingSomething = true;
            isGrabbingSomething = true;

            _anim.SetBool(_carryingHash, true);
            _rightHandIK.weight = 1f;
        }
        else if (isCarryingSomething)
        {
            Physics.IgnoreCollision(_grabTransform.GetChild(0).GetComponent<Collider>(), _cc, false);
            _grabTransform.GetChild(0).GetComponent<HackableObject>().isBeingGrabbed = false;
            _grabTransform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
            _grabTransform.GetChild(0).SetParent(null);
            _anim.SetBool(_carryingHash, false);
            _rightHandIK.weight = 0f;

            isGrabbingSomething = false;
        }
    }
    #endregion
}