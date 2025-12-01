using UnityEngine;

public class TopHUDPresenter
{
    private readonly TopHUDView _view;

    public TopHUDPresenter(TopHUDView view)
    {
        _view = view;
    }

    public void Initialize()
    {
        _view.OnOptionClicked += HandleOptionClick;
        _view.UpdateStatus("Ready");
    }

    private void HandleOptionClick()
    {
        Debug.Log("[HUD] 옵션 버튼 클릭됨 -> 팝업을 띄우거나 씬 이동");
        _view.UpdateStatus("Paused");
    }
}
