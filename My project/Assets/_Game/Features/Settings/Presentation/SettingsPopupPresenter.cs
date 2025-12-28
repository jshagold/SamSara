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
        _view.OnCloseClicked += () =>
        {
            _view.Close();
        };

        _view.OnBgmChanged += (vol) =>
        {
            _context.Settings.bgmVolume = vol;
            _context.IsDirty = true; 

            // (나중에 여기에 사운드 매니저 호출 코드 추가: SoundManager.Instance.SetBGM(vol))
            Debug.Log($"BGM 볼륨 변경됨: {vol}");
        };

        // SFX 변경
        _view.OnSfxChanged += (vol) =>
        {
            _context.Settings.sfxVolume = vol;
            _context.IsDirty = true;
            Debug.Log($"SFX 볼륨 변경됨: {vol}");
        };

        // 자동전투 변경
        _view.OnAutoBattleChanged += (isOn) =>
        {
            _context.Settings.autoBattle = isOn;
            _context.IsDirty = true;
            Debug.Log($"자동전투 설정: {isOn}");
        };

        // QTE 변경
        _view.OnQteChanged += (isOn) =>
        {
            _context.Settings.qteEnabled = isOn;
            _context.IsDirty = true;
            Debug.Log($"QTE 설정: {isOn}");
        };
    }

    // 외부(메인화면 버튼)에서 이 함수를 호출해서 팝업을 염
    public void OpenPopup()
    {
        // 데이터에 저장된 값으로 뷰를 초기화 (싱크 맞추기)
        _view.InitView(_context.Settings);

        // 화면에 띄우기
        _view.Open();
    }
}