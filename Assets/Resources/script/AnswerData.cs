using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class AnswerData : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Text infoTextObject;
    [SerializeField] Image toggle;

    [Header("Textures")]
    [SerializeField] Sprite unCheckedToggle;
    [SerializeField] Sprite checkedTogglele;

    [Header("References")]
    [SerializeField] GameEvents events;

    private RectTransform _rect;
    public RectTransform Rect
    {
        get
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
            return _rect;
        }
    }

    private int _answerIndex = -1;
    public int AnswerIndex { get { return _answerIndex; } }

    [SerializeField] public bool Checked = false;

    public void UpdateData(string info, int index)
    {
        infoTextObject.text = info;
        _answerIndex = index;
    }

    public void Reset()
    {
        Checked = false;
        UpdateUI();
    }


    public void SwitchState()
    {
        Checked = !Checked;
        UpdateUI();

        if (events.updateQuestionAnswer != null)
        {
            events.updateQuestionAnswer(this);
        }
    }

    void UpdateUI()
    {

        if (toggle == null) return;
        toggle.sprite = (Checked) ? checkedTogglele : unCheckedToggle;
    }
}
