using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest", order = 1)]
public class QuestSO : ScriptableObject
{
    [SerializeField] Sprite _icon;
    [SerializeField] string _description;

    public Sprite GetIcon()
    {
        return _icon;
    }

    public string GetString()
    {
        return _description;
    }

}
