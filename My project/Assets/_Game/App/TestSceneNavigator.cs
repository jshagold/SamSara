using Samsara.App;
using Samsara.Core.Navigation;
using UnityEngine;

public class TestSceneNavigator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var navigator = new SceneNavigator();
        await navigator.NavigateToAsync(SceneKey.Bootstrap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
