using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveComputer : HackableObject
{
    [SerializeField] private Sprite _checkedSprite;

    protected override void OnLoaded_E()
    {
        onlyShow = true;
        ImgOnCanvas.transform.Find("Image").GetComponent<Image>().sprite = _checkedSprite;
        GameManager.Instance.SaveAllData();

        base.OnLoaded_E();
        isActive = true;
    }
}