using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganPuzzle : PuzzlesBase
{
    public Action showingEnded;
    public Action restart;

    private Transform _tubes;
    private Transform _buttons;
    private Transform _lights;

    private int _randomSound = -1;

    public override void Enable()
    {
        base.Enable();

        _randomSound = UnityEngine.Random.Range(0, 3);
        StartCoroutine(ExecuteOrgan());
    }

    public void TriggerOrgan(string btnName)
    {
        restart?.Invoke();

        if (btnName == _randomSound.ToString())
        {
            StartCoroutine(ShowIfCorrectOrIncorrect(btnName, true));
        }
        else
        {
            StartCoroutine(ShowIfCorrectOrIncorrect(btnName, false));
        }
    }

    private IEnumerator ExecuteOrgan()
    {
        _lights.Find("0").gameObject.SetActive(true);
        _lights.Find("1").gameObject.SetActive(true);
        _lights.Find("2").gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        _lights.Find("0").gameObject.SetActive(false);
        _lights.Find("1").gameObject.SetActive(false);
        _lights.Find("2").gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            _tubes.Find(i.ToString()).GetComponent<AudioSource>().Play();
            _lights.Find(i.ToString()).gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            _tubes.Find(i.ToString()).GetComponent<AudioSource>().Stop();
            _lights.Find(i.ToString()).gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);
        _tubes.Find(_randomSound.ToString()).GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);
        _tubes.Find(_randomSound.ToString()).GetComponent<AudioSource>().Stop();

        showingEnded?.Invoke();
    }

    private IEnumerator ShowIfCorrectOrIncorrect(string index, bool correct)
    {
        Color color = correct ? Color.green : Color.red;

        _lights.Find(index).GetComponent<Light>().color = color;
        _lights.Find(index).gameObject.SetActive(true);
        _tubes.Find(index).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(1);

        _lights.Find(index).gameObject.SetActive(false);
        _lights.Find(index).GetComponent<Light>().color = Color.green;
        _tubes.Find(index).GetComponent<AudioSource>().Stop();

        if (!correct)
        {
            _randomSound = -1;
            Enable();
        }
        else
        {
            PuzzleCompleted();
        }
    }

    private void Awake()
    {
        _tubes = transform.Find("Tubes");
        _buttons = transform.Find("Buttons");
        _lights = transform.Find("Lights");
    }
}