using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextField : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _textObject;
    [SerializeField] private Scrollbar _scrollbar;
    
    private readonly List<string> _messages = new();

    public event Action<string> CurrentSelectedUsername;

    public void OnPointerClick(PointerEventData eventData)
    {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(_textObject, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            var linkInfo = _textObject.textInfo.linkInfo[linkIndex].GetLinkID();
            Debug.Log(linkInfo);
            CurrentSelectedUsername?.Invoke($"{linkInfo} ");
        }
    }

    public void ReceiveMessage(string message)
    {
        _messages.Add(message);

        var value = (_messages.Count - 1) * _scrollbar.value;

        _scrollbar.value = Mathf.Clamp(value, 0, 1);
        UpdateText();
    }

    private void UpdateText()
    {
        var text = "";
        var index = (int)(_messages.Count * _scrollbar.value);

        for (int i = index; i < _messages.Count; i++)
        {
            text += $"{_messages[i]}\n";
        }

        _textObject.text = text;
    }
}