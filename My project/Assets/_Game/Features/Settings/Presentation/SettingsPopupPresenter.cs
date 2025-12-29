using UnityEngine;

public class SettingsPopupPresenter
{
    private readonly SettingsPopupView _view;
    private readonly GameContext _context;

    public SettingsPopupPresenter(SettingsPopupView view, GameContext context)
    {
        _view = view;
        _context = context;

        ConnectEvents();
    }

    private void ConnectEvents()
    {
        _view.SetEvents(
            onClose: () =>
            {
                _view.ClosePopup();
            },
            onBgmChanged: (vol) =>
            {
                _context.Settings.bgmVolume = vol;
                _context.IsDirty = true;
                Debug.Log($"BGM: {vol}");
            },
            onSfxChanged: (vol) =>
            {
                _context.Settings.sfxVolume = vol;
                _context.IsDirty = true;
                Debug.Log($"SFX: {vol}");
            },
            onAutoBattleChanged: (isOn) =>
            {
                _context.Settings.autoBattle = isOn;
                _context.IsDirty = true;
                Debug.Log($"AutoBattle: {isOn}");
            },
            onQteChanged: (isOn) =>
            {
                _context.Settings.qteEnabled = isOn;
                _context.IsDirty = true;
                Debug.Log($"QTE: {isOn}");
            }
        );
    }

    public void OpenPopup()
    {
        _view.InitView(_context.Settings);
        _view.OpenPopup();
    }
}