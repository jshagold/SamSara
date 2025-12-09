using UnityEngine.Localization.Settings;

public static class LocalizationUtils
{
    public static string GetString(string tableName, string key)
    {
        // 아직 로딩이 안 됐거나 에러가 날 경우를 대비해 예외처리나 빈 값 처리를 할 수도 있습니다.
        var op = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

        // 만약 키를 못 찾으면 키 값 자체를 반환하거나 에러 로그를 띄우도록 커스텀 가능
        return string.IsNullOrEmpty(op) ? key : op;
    }
}