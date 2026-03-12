using UnityEngine.Localization.Settings;

public static class LocalizationUtils
{
    public const string TextTableName = "UI_TABLE";

    public static string GetString(string key)
    {
        // 아직 로딩이 안 됐거나 에러가 날 경우를 대비해 예외처리나 빈 값 처리를 할 수도 있습니다.
        var op = LocalizationSettings.StringDatabase.GetLocalizedString(TextTableName, key);

        // 만약 키를 못 찾으면 키 값 자체를 반환하거나 에러 로그를 띄우도록 커스텀 가능
        return string.IsNullOrEmpty(op) ? key : op;
    }

    public static string GetString(string tableName, string key)
    {
        // 아직 로딩이 안 됐거나 에러가 날 경우를 대비해 예외처리나 빈 값 처리를 할 수도 있습니다.
        var op = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

        // 만약 키를 못 찾으면 키 값 자체를 반환하거나 에러 로그를 띄우도록 커스텀 가능
        return string.IsNullOrEmpty(op) ? key : op;
    }

    // 변수가 포함된 텍스트 가져오기 (예: "Level {0}")
    // 사용법: LocalizationUtils.GetString("ui_level_text", 5); -> "Level 5"
    public static string GetString(string key, params object[] args)
    {
        // 유니티 Localization은 내부적으로 Smart String(포맷팅)을 지원합니다.
        var op = LocalizationSettings.StringDatabase.GetLocalizedString(TextTableName, key, args);
        return string.IsNullOrEmpty(op) ? key : op;
    }
}