using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainContentView : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;

    public void AddStartListener(UnityAction action)
    {
        _startGameButton.onClick.AddListener(action);
    }
}
