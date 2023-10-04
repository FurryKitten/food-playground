using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderCounterUIService : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUIs;

    void Start()
    {
        ServiceLocator.Current.Get<GameState>()._onOrderNumberChange.AddListener((int x) =>
        {
            foreach (var t in textMeshProUGUIs)
                t.text = $"#{x}";
        });
    }
}
