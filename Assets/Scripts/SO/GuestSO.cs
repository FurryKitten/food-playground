using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guest", menuName = "ScriptableObjects/Guest", order = 1)]
public class GuestSO : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<string> _phrases; 
    [SerializeField] private List<string> _phrasesEnd;
    [SerializeField] private List<string> _phrasesBegin;

    public int Id => _id;
    public Sprite Icon => _icon;
}
