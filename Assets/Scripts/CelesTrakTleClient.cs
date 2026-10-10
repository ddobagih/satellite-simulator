using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public enum CelesTrakTleGroup
{
    Stations,
    Active,
    Starlink
}

public class CelesTrakTleFetchResult
{
    public bool success;
    public bool fromCache;
    public bool fromFallback;
    public string url;
    public string cacheKey;
    public string error;
    public string warning;
    public string rawText;
    public string dataFormat;
    public bool usedRateLimitGuard;
    public DateTime fetchedAtUtc;
    public DateTime oldestEpochUtc;
    public DateTime newestEpochUtc;
    public double oldestEpochAgeDays;
    public double newestEpochAgeDays;
    public bool isEpochStale;
    public List<RealSatelliteTle> satellites = new List<RealSatelliteTle>();
}

public class CelesTrakTleClient : MonoBehaviour
{
    public const int IssNoradId = 25544;
    public const int DefaultMinimumCacheSeconds = 2 * 60 * 60;
    public const string StationsGroupName = "STATIONS";
    public const string ActiveGroupName = "ACTIVE";
    public const string StarlinkGroupName = "STARLINK";

    private const string BaseUrl = "https://celestrak.org/NORAD/elements/gp.php";
    private const string IssCacheKey = "catnr-25544";
    private const string CachePrefix = "OrbitShare.RealSatelliteTle.Memory.";
    private const string CacheFolderName = "OrbitShareTleCache";

    private static readonly Dictionary<string, CacheEntry> MemoryCache = new Dictionary<string, CacheEntry>();
    private static readonly Dictionary<string, DateTime> LastNetworkRequestUtcByKey = new Dictionary<string, DateTime>();

    [Tooltip("같은 TLE 요청을 다시 보내기 전 기다릴 최소 시간(초). 기본값은 2시간입니다.")]
    public int minimumCacheSeconds = DefaultMinimumCacheSeconds;
    [Tooltip("캐시가 만료됐더라도 같은 키로 네트워크 요청을 너무 자주 보내지 않기 위한 최소 간격(초).")]
    public int minimumNetworkRequestIntervalSeconds = 15;
    [Tooltip("TLE epoch가 이 일수보다 오래되면 stale 경고를 결과에 표시합니다.")]
    public double staleTleEpochWarningDays = 7.0;
    public int requestTimeoutSeconds = 8;
    public bool usePersistentCache = true;
    public bool allowStaleCacheOnFailure = true;
    public bool allowBuiltInIssFallback = true;

    public static string BuildIssUrl()
    {
        return BuildCatalogNumberUrl(IssNoradId);
    }

    public static string BuildCatalogNumberUrl(int catalogNumber)
    {
        return BaseUrl + "?CATNR=" + catalogNumber + "&FORMAT=TLE";
    }

    public static string BuildIssOmmJsonUrl()
    {
        return BuildCatalogNumberOmmJsonUrl(IssNoradId);
    }

    public static string BuildIssOmmCsvUrl()
    {
        return BuildCatalogNumberOmmCsvUrl(IssNoradId);
    }

    public static string BuildCatalogNumberOmmJsonUrl(int catalogNumber)
    {
        return BaseUrl + "?CATNR=" + catalogNumber + "&FORMAT=JSON";
    }

    public static string BuildCatalogNumberOmmCsvUrl(int catalogNumber)
    {
        return BaseUrl + "?CATNR=" + catalogNumber + "&FORMAT=CSV";
    }

    public static string BuildGroupUrl(CelesTrakTleGroup group)
    {
        if (group == CelesTrakTleGroup.Stations)
        {
            return BuildGroupUrl(StationsGroupName);
        }
        if (group == CelesTrakTleGroup.Starlink)
        {
            return BuildGroupUrl(StarlinkGroupName);
        }

        return BuildGroupUrl(ActiveGroupName);
    }

    public static string BuildGroupUrl(string groupName)
    {
        return BaseUrl + "?GROUP=" + NormalizeGroupName(groupName) + "&FORMAT=TLE";
    }

    public static string BuildGroupOmmJsonUrl(CelesTrakTleGroup group)
    {
        if (group == CelesTrakTleGroup.Stations)
        {
            return BuildGroupOmmJsonUrl(StationsGroupName);
        }
        if (group == CelesTrakTleGroup.Starlink)
        {
            return BuildGroupOmmJsonUrl(StarlinkGroupName);
        }

        return BuildGroupOmmJsonUrl(ActiveGroupName);
    }

    public static string BuildGroupOmmJsonUrl(string groupName)
    {
        return BaseUrl + "?GROUP=" + NormalizeGroupName(groupName) + "&FORMAT=JSON";
    }

    public static string BuildGroupOmmCsvUrl(CelesTrakTleGroup group)
    {
        if (group == CelesTrakTleGroup.Stations)
        {
            return BuildGroupOmmCsvUrl(StationsGroupName);
        }
        if (group == CelesTrakTleGroup.Starlink)
        {
            return BuildGroupOmmCsvUrl(StarlinkGroupName);
        }

        return BuildGroupOmmCsvUrl(ActiveGroupName);
    }

    public static string BuildGroupOmmCsvUrl(string groupName)
    {
        return BaseUrl + "?GROUP=" + NormalizeGroupName(groupName) + "&FORMAT=CSV";
    }

    public IEnumerator FetchIss(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchUrl(BuildIssUrl(), IssCacheKey, onComplete, forceRefresh);
    }

    public IEnumerator FetchIssOmmJson(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchUrl(BuildIssOmmJsonUrl(), "omm-catnr-25544", onComplete, forceRefresh);
    }

    public IEnumerator FetchIssOmmCsv(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchUrl(BuildIssOmmCsvUrl(), "omm-csv-catnr-25544", onComplete, forceRefresh);
    }

    public IEnumerator FetchActiveGroup(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchGroup(CelesTrakTleGroup.Active, onComplete, forceRefresh);
    }

    public IEnumerator FetchStationsGroup(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchGroup(CelesTrakTleGroup.Stations, onComplete, forceRefresh);
    }

    public IEnumerator FetchStarlinkGroup(Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        yield return FetchGroup(CelesTrakTleGroup.Starlink, onComplete, forceRefresh);
    }

    public IEnumerator FetchGroup(CelesTrakTleGroup group, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string groupName = group == CelesTrakTleGroup.Stations
            ? StationsGroupName
            : group == CelesTrakTleGroup.Starlink ? StarlinkGroupName : ActiveGroupName;
        yield return FetchGroup(groupName, onComplete, forceRefresh);
    }

    public IEnumerator FetchGroup(string groupName, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string normalizedGroup = NormalizeGroupName(groupName);
        yield return FetchUrl(BuildGroupUrl(normalizedGroup), "group-" + normalizedGroup.ToLowerInvariant(), onComplete, forceRefresh);
    }

    public IEnumerator FetchGroupOmmJson(CelesTrakTleGroup group, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string groupName = group == CelesTrakTleGroup.Stations
            ? StationsGroupName
            : group == CelesTrakTleGroup.Starlink ? StarlinkGroupName : ActiveGroupName;
        yield return FetchGroupOmmJson(groupName, onComplete, forceRefresh);
    }

    public IEnumerator FetchGroupOmmJson(string groupName, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string normalizedGroup = NormalizeGroupName(groupName);
        yield return FetchUrl(BuildGroupOmmJsonUrl(normalizedGroup), "omm-json-group-" + normalizedGroup.ToLowerInvariant(), onComplete, forceRefresh);
    }

    public IEnumerator FetchGroupOmmCsv(CelesTrakTleGroup group, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string groupName = group == CelesTrakTleGroup.Stations
            ? StationsGroupName
            : group == CelesTrakTleGroup.Starlink ? StarlinkGroupName : ActiveGroupName;
        yield return FetchGroupOmmCsv(groupName, onComplete, forceRefresh);
    }

    public IEnumerator FetchGroupOmmCsv(string groupName, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh = false)
    {
        string normalizedGroup = NormalizeGroupName(groupName);
        yield return FetchUrl(BuildGroupOmmCsvUrl(normalizedGroup), "omm-csv-group-" + normalizedGroup.ToLowerInvariant(), onComplete, forceRefresh);
    }

    public bool HasFreshCache(string cacheKey)
    {
        CacheEntry entry;
        return TryGetAnyCache(cacheKey, out entry) && IsFresh(entry);
    }

    public void ClearMemoryCache()
    {
        MemoryCache.Clear();
    }

    public bool TryGetCachedTles(string cacheKey, out List<RealSatelliteTle> satellites)
    {
        satellites = null;

        CacheEntry entry;
        if (!TryGetAnyCache(cacheKey, out entry))
        {
            return false;
        }

        satellites = ParseSatellites(entry.rawText);
        return satellites.Count > 0;
    }

    private IEnumerator FetchUrl(string url, string cacheKey, Action<CelesTrakTleFetchResult> onComplete, bool forceRefresh)
    {
        CacheEntry entry;
        if (!forceRefresh && TryGetAnyCache(cacheKey, out entry) && IsFresh(entry))
        {
            Complete(onComplete, BuildResult(true, true, false, url, cacheKey, null, entry.rawText, entry.fetchedAtUtc));
            yield break;
        }

        int waitSeconds;
        if (!CanStartNetworkRequest(cacheKey, out waitSeconds))
        {
            string intervalError = "CelesTrak 재요청 간격 보호 중입니다. 약 " + waitSeconds + "초 후 다시 시도하세요.";
            if (TryGetAnyCache(cacheKey, out entry) && !string.IsNullOrWhiteSpace(entry.rawText))
            {
                CelesTrakTleFetchResult guardedResult = BuildResult(true, true, false, url, cacheKey, intervalError, entry.rawText, entry.fetchedAtUtc);
                guardedResult.usedRateLimitGuard = true;
                Complete(onComplete, guardedResult);
            }
            else
            {
                Complete(onComplete, BuildResult(false, false, false, url, cacheKey, intervalError, null, DateTime.UtcNow));
            }
            yield break;
        }

        MarkNetworkRequest(cacheKey);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = Mathf.Max(1, requestTimeoutSeconds);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success && !string.IsNullOrWhiteSpace(request.downloadHandler.text))
            {
                string rawText = request.downloadHandler.text;
                DateTime fetchedAtUtc = DateTime.UtcNow;
                SetCache(cacheKey, rawText, fetchedAtUtc);
                Complete(onComplete, BuildResult(true, false, false, url, cacheKey, null, rawText, fetchedAtUtc));
                yield break;
            }

            string error = request.error;
            if (string.IsNullOrWhiteSpace(error))
            {
                error = "CelesTrak 응답이 비어 있습니다.";
            }

            if (allowStaleCacheOnFailure && TryGetAnyCache(cacheKey, out entry) && !string.IsNullOrWhiteSpace(entry.rawText))
            {
                Complete(onComplete, BuildResult(true, true, false, url, cacheKey, error, entry.rawText, entry.fetchedAtUtc));
                yield break;
            }

            if (allowBuiltInIssFallback && cacheKey == IssCacheKey)
            {
                Complete(onComplete, BuildResult(true, false, true, url, cacheKey, error, BuiltInIssSampleTle(), DateTime.UtcNow));
                yield break;
            }

            Complete(onComplete, BuildResult(false, false, false, url, cacheKey, error, null, DateTime.UtcNow));
        }
    }

    private CelesTrakTleFetchResult BuildResult(bool success, bool fromCache, bool fromFallback, string url, string cacheKey, string error, string rawText, DateTime fetchedAtUtc)
    {
        CelesTrakTleFetchResult result = new CelesTrakTleFetchResult
        {
            success = success,
            fromCache = fromCache,
            fromFallback = fromFallback,
            url = url,
            cacheKey = cacheKey,
            error = error,
            rawText = rawText,
            dataFormat = DetectDataFormat(rawText),
            fetchedAtUtc = NormalizeUtc(fetchedAtUtc)
        };

        result.satellites = ParseSatellites(rawText);
        PopulateEpochMetadata(result);
        if (success && result.satellites.Count == 0)
        {
            result.success = false;
            result.error = string.IsNullOrWhiteSpace(error) ? "위성 데이터 파싱 결과가 비어 있습니다." : error;
        }

        return result;
    }

    private static void Complete(Action<CelesTrakTleFetchResult> onComplete, CelesTrakTleFetchResult result)
    {
        if (onComplete != null)
        {
            onComplete(result);
        }
    }

    private bool IsFresh(CacheEntry entry)
    {
        int seconds = Mathf.Max(0, minimumCacheSeconds);
        return (DateTime.UtcNow - entry.fetchedAtUtc).TotalSeconds < seconds;
    }

    private bool CanStartNetworkRequest(string cacheKey, out int waitSeconds)
    {
        waitSeconds = 0;
        int minimumSeconds = Mathf.Max(0, minimumNetworkRequestIntervalSeconds);
        if (minimumSeconds == 0 || string.IsNullOrWhiteSpace(cacheKey))
        {
            return true;
        }

        DateTime lastRequestUtc;
        if (!LastNetworkRequestUtcByKey.TryGetValue(CachePrefix + cacheKey, out lastRequestUtc))
        {
            return true;
        }

        double elapsedSeconds = (DateTime.UtcNow - lastRequestUtc).TotalSeconds;
        if (elapsedSeconds >= minimumSeconds)
        {
            return true;
        }

        waitSeconds = Mathf.CeilToInt((float)(minimumSeconds - elapsedSeconds));
        return false;
    }

    private static void MarkNetworkRequest(string cacheKey)
    {
        if (!string.IsNullOrWhiteSpace(cacheKey))
        {
            LastNetworkRequestUtcByKey[CachePrefix + cacheKey] = DateTime.UtcNow;
        }
    }

    private bool TryGetAnyCache(string cacheKey, out CacheEntry entry)
    {
        if (TryGetMemoryCache(cacheKey, out entry))
        {
            return true;
        }

        if (TryGetPersistentCache(cacheKey, out entry))
        {
            SetMemoryCache(cacheKey, entry.rawText, entry.fetchedAtUtc);
            return true;
        }

        entry = null;
        return false;
    }

    private static bool TryGetMemoryCache(string cacheKey, out CacheEntry entry)
    {
        entry = null;
        return !string.IsNullOrWhiteSpace(cacheKey) && MemoryCache.TryGetValue(CachePrefix + cacheKey, out entry);
    }

    private void SetCache(string cacheKey, string rawText, DateTime fetchedAtUtc)
    {
        SetMemoryCache(cacheKey, rawText, fetchedAtUtc);
        SetPersistentCache(cacheKey, rawText, fetchedAtUtc);
    }

    private static void SetMemoryCache(string cacheKey, string rawText, DateTime fetchedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(rawText))
        {
            return;
        }

        MemoryCache[CachePrefix + cacheKey] = new CacheEntry
        {
            rawText = rawText,
            fetchedAtUtc = NormalizeUtc(fetchedAtUtc)
        };
    }

    private bool TryGetPersistentCache(string cacheKey, out CacheEntry entry)
    {
        entry = null;
        if (!usePersistentCache || string.IsNullOrWhiteSpace(cacheKey))
        {
            return false;
        }

        try
        {
            string rawPath = GetPersistentRawPath(cacheKey);
            if (!File.Exists(rawPath))
            {
                return false;
            }

            string rawText = File.ReadAllText(rawPath);
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return false;
            }

            DateTime fetchedAtUtc = File.GetLastWriteTimeUtc(rawPath);
            string metaPath = GetPersistentMetaPath(cacheKey);
            if (File.Exists(metaPath))
            {
                DateTime parsedUtc;
                if (DateTime.TryParse(File.ReadAllText(metaPath).Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedUtc))
                {
                    fetchedAtUtc = parsedUtc;
                }
            }

            entry = new CacheEntry
            {
                rawText = rawText,
                fetchedAtUtc = NormalizeUtc(fetchedAtUtc)
            };
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("TLE 파일 캐시 읽기 실패: " + exception.Message);
            return false;
        }
    }

    private void SetPersistentCache(string cacheKey, string rawText, DateTime fetchedAtUtc)
    {
        if (!usePersistentCache || string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(rawText))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(GetPersistentCacheDirectory());
            File.WriteAllText(GetPersistentRawPath(cacheKey), rawText);
            File.WriteAllText(GetPersistentMetaPath(cacheKey), NormalizeUtc(fetchedAtUtc).ToString("o", CultureInfo.InvariantCulture));
        }
        catch (Exception exception)
        {
            Debug.LogWarning("TLE 파일 캐시 저장 실패: " + exception.Message);
        }
    }

    private string GetPersistentCacheDirectory()
    {
        return Path.Combine(Application.persistentDataPath, CacheFolderName);
    }

    private string GetPersistentRawPath(string cacheKey)
    {
        return Path.Combine(GetPersistentCacheDirectory(), SanitizeCacheKey(cacheKey) + ".cache");
    }

    private string GetPersistentMetaPath(string cacheKey)
    {
        return Path.Combine(GetPersistentCacheDirectory(), SanitizeCacheKey(cacheKey) + ".utc");
    }

    private static string SanitizeCacheKey(string cacheKey)
    {
        string safe = string.IsNullOrWhiteSpace(cacheKey) ? "unknown" : cacheKey.Trim();
        char[] invalidChars = Path.GetInvalidFileNameChars();
        for (int i = 0; i < invalidChars.Length; i++)
        {
            safe = safe.Replace(invalidChars[i], '_');
        }
        safe = safe.Replace(' ', '_');
        return safe;
    }

    private List<RealSatelliteTle> ParseSatellites(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return new List<RealSatelliteTle>();
        }

        string trimmed = rawText.TrimStart();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            return RealSatelliteTle.ParseOmmJson(rawText);
        }
        if (LooksLikeOmmCsv(trimmed))
        {
            return RealSatelliteTle.ParseOmmCsv(rawText);
        }

        return RealSatelliteTle.ParseMany(rawText);
    }

    private static string DetectDataFormat(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return string.Empty;
        }

        string trimmed = rawText.TrimStart();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            return "OMM_JSON";
        }
        if (LooksLikeOmmCsv(trimmed))
        {
            return "OMM_CSV";
        }
        return "TLE";
    }

    private static bool LooksLikeOmmCsv(string trimmedText)
    {
        if (string.IsNullOrWhiteSpace(trimmedText))
        {
            return false;
        }

        int firstLineEnd = trimmedText.IndexOf('\n');
        string firstLine = firstLineEnd >= 0 ? trimmedText.Substring(0, firstLineEnd) : trimmedText;
        return firstLine.IndexOf("NORAD_CAT_ID", StringComparison.OrdinalIgnoreCase) >= 0
            && firstLine.IndexOf("MEAN_MOTION", StringComparison.OrdinalIgnoreCase) >= 0
            && firstLine.IndexOf(',') >= 0;
    }

    private void PopulateEpochMetadata(CelesTrakTleFetchResult result)
    {
        if (result == null || result.satellites == null || result.satellites.Count == 0)
        {
            return;
        }

        DateTime oldest = DateTime.MaxValue;
        DateTime newest = DateTime.MinValue;
        for (int i = 0; i < result.satellites.Count; i++)
        {
            DateTime epoch = NormalizeUtc(result.satellites[i].epoch);
            if (epoch == DateTime.MinValue)
            {
                continue;
            }

            if (epoch < oldest)
            {
                oldest = epoch;
            }
            if (epoch > newest)
            {
                newest = epoch;
            }
        }

        if (newest == DateTime.MinValue)
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        result.oldestEpochUtc = oldest;
        result.newestEpochUtc = newest;
        result.oldestEpochAgeDays = Math.Max(0.0, (now - oldest).TotalDays);
        result.newestEpochAgeDays = Math.Max(0.0, (now - newest).TotalDays);

        double staleDays = Math.Max(0.0, staleTleEpochWarningDays);
        result.isEpochStale = staleDays > 0.0 && result.newestEpochAgeDays > staleDays;
        if (result.isEpochStale)
        {
            result.warning = "최신 TLE epoch가 " + result.newestEpochAgeDays.ToString("F1", CultureInfo.InvariantCulture) + "일 지났습니다.";
        }
    }

    private static string NormalizeGroupName(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return ActiveGroupName;
        }

        return groupName.Trim().ToUpperInvariant();
    }

    private static string BuiltInIssSampleTle()
    {
        return "ISS (ZARYA)\n"
            + "1 25544U 98067A   24153.48714594  .00016717  00000+0  30490-3 0  9997\n"
            + "2 25544  51.6416  77.0477 0006096  38.0818  39.5539 15.50381565455373\n";
    }

    private class CacheEntry
    {
        public string rawText;
        public DateTime fetchedAtUtc;
    }

    private static DateTime NormalizeUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }
        return dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime;
    }
}
