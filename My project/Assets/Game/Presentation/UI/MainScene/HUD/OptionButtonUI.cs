using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake() => button.onClick.AddListener(() => OnClicked?.Invoke());
    public event Action OnClicked;
}
