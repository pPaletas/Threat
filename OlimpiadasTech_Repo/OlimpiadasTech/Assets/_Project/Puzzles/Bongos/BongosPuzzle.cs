using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class BongosPuzzle : PuzzlesBase
{
    public Action showingEnded;
    public Action restart;

    private List<int> _showOrder = new List<int>() { 0, 1, 2 };
    private List<int> _order = new List<int>();

    private int _currentTriggeredBongos = 0;

    private IEnumerator ExecuteBongos()
    {
        transform.Find("0").gameObject.SetActive(true);
        transform.Find("1").gameObject.SetActive(true);
        transform.Find("2").gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        transform.Find("0").gameObject.SetActive(false);
        transform.Find("1").gameObject.SetActive(false);
        transform.Find("2").gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            int randomNum = -1;

            int randIndex = UnityEngine.Random.Range(0, _showOrder.Count);
            Debug.Log(randIndex);
            randomNum = _showOrder[randIndex];
            _showOrder.RemoveAt(randIndex);

            _order.Add(randomNum);
            transform.Find(randomNum.ToString()).gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);

            transform.Find(randomNum.ToString()).gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

        }

        showingEnded?.Invoke();
    }

    public override void Enable()
    {
        base.Enable();


        StartCoroutine(ExecuteBongos());
    }

    public void TriggerBongo(string bongoName)
    {
        int bongoIndex = Int32.Parse(bongoName[1].ToString());

        // Desactivamos los botones
        restart?.Invoke();

        if (_order[_currentTriggeredBongos] == bongoIndex)
        {
            StartCoroutine(ShowIfCorrectOrIncorrect(bongoIndex, true));

            _currentTriggeredBongos++;
        }
        else
        {
            // Volvemos a empezar
            StartCoroutine(ShowIfCorrectOrIncorrect(bongoIndex, false));
        }
    }

    private IEnumerator ShowIfCorrectOrIncorrect(int index, bool correct)
    {
        Color color = correct ? Color.green : Color.red;

        transform.Find(index.ToString()).GetComponent<Light>().color = color;
        transform.Find(index.ToString()).gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        transform.Find(index.ToString()).gameObject.SetActive(false);
        transform.Find(index.ToString()).GetComponent<Light>().color = Color.green;

        if (!correct)
        {
            _currentTriggeredBongos = 0;
            _order.Clear();
            _showOrder = new List<int>() { 0, 1, 2 };

            StartCoroutine(ExecuteBongos());
        }
        else
        {
            if (_currentTriggeredBongos == 3)
            {
                PuzzleCompleted();
            }
            else
            {
                showingEnded?.Invoke();
            }
        }
    }
}