using UnityEngine;

public class CharacterDetailBootstrapper: MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(CharacterDetailBootstrapper)}]";

    public void Initialize()
    {

        Debug.Log($"{_logClass} Initialize");
    }

    private void OnDestroy()
    {
        
    }
}