using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class KillSystem : MonoBehaviour
{
    [SerializeField] private float _distance = 1.5f;
    [SerializeField] private float _angle = 45f;
    [SerializeField] private float _smoothness = 5f;
    [SerializeField] private float _killOffset = 0.519f;
    [SerializeField] private RectTransform _killButton;
    [SerializeField] private Camera _cam;
    [SerializeField] private AudioClip _abrirTapaClip;
    [SerializeField] private AudioSource _fabricAudio;

    private StarterAssetsInputs _input;
    private GameObject _closestRobot;
    private Animator _anim;
    private ThirdPersonController _movement;
    private HackingSystem _hacking;
    private HealthSystem _health;

    private bool _isKilling = false;
    private int _killHash = Animator.StringToHash("Kill");

    public bool IsKilling { get => _isKilling; }

    private IEnumerator SetupAnimation()
    {
        Vector3 unit = (_closestRobot.transform.position - transform.position);
        unit.y = 0f;
        unit.Normalize();

        Vector3 targetPos = transform.position + unit * _killOffset;
        bool ready = false;

        Quaternion targetRot = Quaternion.LookRotation(unit);

        while (!ready)
        {
            bool isRobotInPlace = Vector3.Distance(_closestRobot.transform.position, targetPos) <= 0.05f;
            bool isPlrInDirection = Quaternion.Angle(transform.rotation, targetRot) <= 0.5f;
            bool isRobotInDirection = Quaternion.Angle(_closestRobot.transform.rotation, targetRot) <= 0.5f;

            if (!isRobotInPlace || !isPlrInDirection || !isRobotInDirection)
            {
                _closestRobot.transform.position = Vector3.Lerp(_closestRobot.transform.position, targetPos, Time.deltaTime * _smoothness);
                _closestRobot.transform.rotation = Quaternion.Slerp(_closestRobot.transform.rotation, targetRot, Time.deltaTime * _smoothness);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _smoothness);
            }
            else ready = true;

            // Siguiente frame
            yield return new WaitForEndOfFrame();
        }

        NoiseSystem.Instance.GenerateNoise(5f, 100f);
        _anim.SetTrigger(_killHash);
        _closestRobot.GetComponent<RobotStateMachine>().Kill();
    }

    private void SetupSnappedAnimation()
    {
        Vector3 unit = (_closestRobot.transform.position - transform.position);
        unit.y = 0f;
        unit.Normalize();

        Vector3 targetPos = transform.position + unit * _killOffset;
        Quaternion targetRot = Quaternion.LookRotation(unit);

        _closestRobot.transform.position = targetPos;
        _closestRobot.transform.rotation = targetRot;
        transform.rotation = targetRot;

        NoiseSystem.Instance.GenerateNoise(5f, 100f);
        _anim.SetTrigger(_killHash);
        _closestRobot.GetComponent<RobotStateMachine>().Kill();
    }

    private void SetClosestRobot()
    {
        // Prevenimos que el robot actual se establezca como nulo mientras lo matamos
        if (_isKilling) return;

        foreach (GameObject robot in SceneData.Instance.Robots)
        {
            bool isVulnerable = robot.GetComponent<RobotStateMachine>().isVulnerable;
            bool isCloseEnough = Vector3.Distance(transform.position, robot.transform.position) <= _distance;

            Vector3 unit = (robot.transform.position - transform.position);
            unit.y = 0f;
            unit.Normalize();

            Vector3 fwd = robot.transform.forward;
            fwd.y = 0f;
            fwd.Normalize();

            float dot = Vector3.Dot(fwd, unit);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            bool isInAngle = angle <= _angle;

            if (isVulnerable && isCloseEnough && isInAngle)
            {
                _killButton.gameObject.SetActive(true);
                _closestRobot = robot;
                Vector3 wiresPos = robot.transform.Find("Armature/Body/Wires").position;
                Vector3 worldToScreen = _cam.WorldToScreenPoint(wiresPos);
                _killButton.position = Vector3.Lerp(_killButton.position, worldToScreen, 20f * Time.deltaTime);

                return;
            }
        }

        _killButton.gameObject.SetActive(false);
        _closestRobot = null;
    }

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _input.onKillButtonPressed += KillButton;

        _anim = GetComponent<Animator>();
        _movement = GetComponent<ThirdPersonController>();
        _hacking = GetComponent<HackingSystem>();
        _health = GetComponent<HealthSystem>();
    }

    private void OnDisable()
    {
        _input.onKillButtonPressed -= KillButton;
    }

    private void LateUpdate()
    {
        SetClosestRobot();
    }

    #region Callbacks
    public void OnAnimationTrigger(string animName)
    {
        if (animName == "AbrirTapa")
        {
            AudioSource.PlayClipAtPoint(_abrirTapaClip, transform.position);
        }
        else if (animName == "Fabric")
        {
            _fabricAudio.Play();
        }
    }

    public void OnKillAnimationFinished()
    {
        _movement.canMove = true;
        _isKilling = false;
        _health.AddChips(1);
    }

    private void KillButton()
    {
        if (_closestRobot != null && !_hacking.isGrabbingSomething && !_isKilling)
        {
            _isKilling = true;
            _closestRobot.GetComponent<RobotStateMachine>().SetupAnimation();
            _movement.canMove = false;

            _killButton.gameObject.SetActive(false);

            // StartCoroutine(SetupAnimation());
            SetupSnappedAnimation();
        }
    }
    #endregion
}