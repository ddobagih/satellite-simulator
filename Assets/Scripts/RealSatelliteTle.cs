using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class RealSatelliteTle
{
    public const double DefaultStaleWarningDays = 14.0;
    public const double DefaultVeryStaleWarningDays = 30.0;

    public string name;
    public int noradId;
    public DateTime epoch;
    public double inclination;
    public double raan;
    public double eccentricity;
    public double argPerigee;
    public double meanAnomaly;
    public double meanMotion;
    public double bstar;
    public string line1;
    public string line2;
    public bool checksumValid;
    public bool lineNoradIdsMatch;
    public string sourceFormat;

    public string SatelliteName
    {
        get { return name; }
        set { name = value; }
    }

    public string Line1
    {
        get { return line1; }
        set { line1 = value; }
    }

    public string Line2
    {
        get { return line2; }
        set { line2 = value; }
    }

    public static bool TryParse(string name, string line1, string line2, out RealSatelliteTle tle)
    {
        tle = null;

        if (string.IsNullOrWhiteSpace(line1) || string.IsNullOrWhiteSpace(line2))
        {
            return false;
        }

        line1 = line1.TrimEnd();
        line2 = line2.TrimEnd();

        if (!line1.StartsWith("1 ", StringComparison.Ordinal) || !line2.StartsWith("2 ", StringComparison.Ordinal))
        {
            return false;
        }

        if (line1.Length < 69 || line2.Length < 69)
        {
            return false;
        }

        if (!HasValidChecksum(line1) || !HasValidChecksum(line2))
        {
            return false;
        }

        int noradId;
        int line2NoradId;
        DateTime epoch;
        double inclination;
        double raan;
        double eccentricity;
        double argPerigee;
        double meanAnomaly;
        double meanMotion;
        double bstar;

        if (!TryParseInt(SafeSubstring(line1, 2, 5), out noradId)
            || !TryParseInt(SafeSubstring(line2, 2, 5), out line2NoradId)
            || !TryParseEpoch(SafeSubstring(line1, 18, 14), out epoch)
            || !TryParseDouble(SafeSubstring(line2, 8, 8), out inclination)
            || !TryParseDouble(SafeSubstring(line2, 17, 8), out raan)
            || !TryParseEccentricity(SafeSubstring(line2, 26, 7), out eccentricity)
            || !TryParseDouble(SafeSubstring(line2, 34, 8), out argPerigee)
            || !TryParseDouble(SafeSubstring(line2, 43, 8), out meanAnomaly)
            || !TryParseDouble(SafeSubstring(line2, 52, 11), out meanMotion))
        {
            return false;
        }

        if (noradId != line2NoradId)
        {
            return false;
        }

        TryParseCompactExponent(SafeSubstring(line1, 53, 8), out bstar);

        tle = new RealSatelliteTle
        {
            name = CleanName(name, noradId),
            noradId = noradId,
            epoch = epoch,
            inclination = inclination,
            raan = raan,
            eccentricity = eccentricity,
            argPerigee = argPerigee,
            meanAnomaly = meanAnomaly,
            meanMotion = meanMotion,
            bstar = bstar,
            line1 = line1,
            line2 = line2,
            checksumValid = true,
            lineNoradIdsMatch = true,
            sourceFormat = "TLE"
        };
        return true;
    }

    public static List<RealSatelliteTle> ParseMany(string tleText)
    {
        List<RealSatelliteTle> satellites = new List<RealSatelliteTle>();
        if (string.IsNullOrWhiteSpace(tleText))
        {
            return satellites;
        }

        string[] rawLines = tleText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        List<string> lines = new List<string>();
        for (int i = 0; i < rawLines.Length; i++)
        {
            string line = rawLines[i].Trim();
            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(line);
            }
        }

        int index = 0;
        while (index < lines.Count)
        {
            RealSatelliteTle tle;
            if (IsLine1(lines[index]) && index + 1 < lines.Count && IsLine2(lines[index + 1]))
            {
                if (TryParse(null, lines[index], lines[index + 1], out tle))
                {
                    satellites.Add(tle);
                }
                index += 2;
                continue;
            }

            if (index + 2 < lines.Count && IsLine1(lines[index + 1]) && IsLine2(lines[index + 2]))
            {
                if (TryParse(lines[index], lines[index + 1], lines[index + 2], out tle))
                {
                    satellites.Add(tle);
                }
                index += 3;
                continue;
            }

            index++;
        }
        return satellites;
    }

    public static List<RealSatelliteTle> ParseOmmJson(string jsonText)
    {
        List<RealSatelliteTle> satellites = new List<RealSatelliteTle>();
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            return satellites;
        }

        string trimmed = jsonText.Trim();
        if (!trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            return satellites;
        }

        try
        {
            CelesTrakOmmRecordList records = JsonUtility.FromJson<CelesTrakOmmRecordList>("{\"items\":" + trimmed + "}");
            if (records == null || records.items == null)
            {
                return satellites;
            }

            for (int i = 0; i < records.items.Length; i++)
            {
                RealSatelliteTle tle;
                if (TryFromOmmRecord(records.items[i], out tle))
                {
                    satellites.Add(tle);
                }
            }
        }
        catch
        {
            satellites.Clear();
        }

        return satellites;
    }

    public static List<RealSatelliteTle> ParseOmmCsv(string csvText)
    {
        List<RealSatelliteTle> satellites = new List<RealSatelliteTle>();
        if (string.IsNullOrWhiteSpace(csvText))
        {
            return satellites;
        }

        string[] rawLines = csvText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        if (rawLines.Length < 2)
        {
            return satellites;
        }

        List<string> headers = SplitCsvLine(rawLines[0]);
        Dictionary<string, int> headerIndices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < headers.Count; i++)
        {
            if (!headerIndices.ContainsKey(headers[i]))
            {
                headerIndices.Add(headers[i], i);
            }
        }

        for (int i = 1; i < rawLines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rawLines[i]))
            {
                continue;
            }

            List<string> fields = SplitCsvLine(rawLines[i]);
            CelesTrakOmmRecord record;
            if (TryBuildOmmRecordFromCsv(headerIndices, fields, out record))
            {
                RealSatelliteTle tle;
                if (TryFromOmmRecord(record, out tle))
                {
                    satellites.Add(tle);
                }
            }
        }

        return satellites;
    }

    public string EpochIsoUtc()
    {
        return epoch.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }

    public double EpochAgeDays(DateTime utc)
    {
        return Math.Abs((NormalizeUtc(utc) - NormalizeUtc(epoch)).TotalDays);
    }

    public TleEpochFreshness GetEpochFreshness(DateTime utc)
    {
        return GetEpochFreshness(utc, DefaultStaleWarningDays, DefaultVeryStaleWarningDays);
    }

    public TleEpochFreshness GetEpochFreshness(DateTime utc, double staleDays, double veryStaleDays)
    {
        if (epoch == DateTime.MinValue)
        {
            return TleEpochFreshness.Unknown;
        }

        double ageDays = EpochAgeDays(utc);
        if (ageDays >= Math.Max(staleDays, veryStaleDays))
        {
            return TleEpochFreshness.VeryStale;
        }
        if (ageDays >= staleDays)
        {
            return TleEpochFreshness.Stale;
        }
        return TleEpochFreshness.Fresh;
    }

    public string EpochFreshnessMessage(DateTime utc)
    {
        TleEpochFreshness freshness = GetEpochFreshness(utc);
        if (freshness == TleEpochFreshness.Fresh)
        {
            return string.Empty;
        }
        if (freshness == TleEpochFreshness.Unknown)
        {
            return "TLE epoch을 확인할 수 없습니다.";
        }
        return "TLE epoch이 " + EpochAgeDays(utc).ToString("F1", CultureInfo.InvariantCulture) + "일 떨어져 있어 전파 정확도가 낮아질 수 있습니다.";
    }

    private static bool IsLine1(string line)
    {
        return !string.IsNullOrEmpty(line) && line.StartsWith("1 ", StringComparison.Ordinal);
    }

    private static bool IsLine2(string line)
    {
        return !string.IsNullOrEmpty(line) && line.StartsWith("2 ", StringComparison.Ordinal);
    }

    private static bool HasValidChecksum(string line)
    {
        if (string.IsNullOrEmpty(line) || line.Length < 69)
        {
            return false;
        }

        int expected;
        if (!TryParseInt(line[68].ToString(), out expected))
        {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < 68; i++)
        {
            char character = line[i];
            if (character >= '0' && character <= '9')
            {
                sum += character - '0';
            }
            else if (character == '-')
            {
                sum += 1;
            }
        }

        return sum % 10 == expected;
    }

    private static string CleanName(string value, int noradId)
    {
        return string.IsNullOrWhiteSpace(value) ? "NORAD " + noradId : value.Trim();
    }

    private static bool TryParseEpoch(string value, out DateTime epoch)
    {
        epoch = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(value) || value.Length < 5)
        {
            return false;
        }

        int year;
        double dayOfYear;
        if (!TryParseInt(value.Substring(0, 2), out year) || !TryParseDouble(value.Substring(2), out dayOfYear))
        {
            return false;
        }

        int fullYear = year < 57 ? 2000 + year : 1900 + year;
        epoch = new DateTime(fullYear, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(dayOfYear - 1.0);
        return true;
    }

    private static bool TryParseEccentricity(string value, out double eccentricity)
    {
        eccentricity = 0d;
        return !string.IsNullOrWhiteSpace(value) && TryParseDouble("0." + value.Trim(), out eccentricity);
    }

    private static bool TryParseCompactExponent(string value, out double result)
    {
        result = 0d;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string compact = value.Trim();
        int exponentSignIndex = compact.LastIndexOf('+');
        if (exponentSignIndex <= 0)
        {
            exponentSignIndex = compact.LastIndexOf('-');
        }
        if (exponentSignIndex <= 0 || exponentSignIndex >= compact.Length - 1)
        {
            return false;
        }

        string mantissa = compact.Substring(0, exponentSignIndex);
        string exponent = compact.Substring(exponentSignIndex);
        int sign = 1;
        if (mantissa.StartsWith("-", StringComparison.Ordinal))
        {
            sign = -1;
            mantissa = mantissa.Substring(1);
        }
        else if (mantissa.StartsWith("+", StringComparison.Ordinal))
        {
            mantissa = mantissa.Substring(1);
        }
        if (mantissa.Length == 0)
        {
            return false;
        }

        double mantissaValue;
        int exponentValue;
        if (!TryParseDouble("0." + mantissa, out mantissaValue) || !TryParseInt(exponent, out exponentValue))
        {
            return false;
        }

        result = sign * mantissaValue * Math.Pow(10d, exponentValue);
        return true;
    }

    private static bool TryFromOmmRecord(CelesTrakOmmRecord record, out RealSatelliteTle tle)
    {
        tle = null;
        if (record == null || record.NORAD_CAT_ID <= 0 || record.MEAN_MOTION <= 0.0)
        {
            return false;
        }

        DateTime epoch;
        if (!TryParseOmmEpoch(record.EPOCH, out epoch))
        {
            return false;
        }

        if (record.ECCENTRICITY < 0.0 || record.ECCENTRICITY >= 1.0)
        {
            return false;
        }

        tle = new RealSatelliteTle
        {
            name = CleanName(record.OBJECT_NAME, record.NORAD_CAT_ID),
            noradId = record.NORAD_CAT_ID,
            epoch = epoch,
            inclination = record.INCLINATION,
            raan = record.RA_OF_ASC_NODE,
            eccentricity = record.ECCENTRICITY,
            argPerigee = record.ARG_OF_PERICENTER,
            meanAnomaly = record.MEAN_ANOMALY,
            meanMotion = record.MEAN_MOTION,
            bstar = record.BSTAR,
            line1 = string.Empty,
            line2 = string.Empty,
            checksumValid = false,
            lineNoradIdsMatch = true,
            sourceFormat = "OMM"
        };
        return true;
    }

    private static bool TryBuildOmmRecordFromCsv(Dictionary<string, int> headerIndices, List<string> fields, out CelesTrakOmmRecord record)
    {
        record = null;
        int noradId;
        double inclination;
        double raan;
        double eccentricity;
        double argPerigee;
        double meanAnomaly;
        double meanMotion;
        double bstar;

        if (!TryParseInt(GetCsvField(headerIndices, fields, "NORAD_CAT_ID"), out noradId)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "INCLINATION"), out inclination)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "RA_OF_ASC_NODE"), out raan)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "ECCENTRICITY"), out eccentricity)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "ARG_OF_PERICENTER"), out argPerigee)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "MEAN_ANOMALY"), out meanAnomaly)
            || !TryParseDouble(GetCsvField(headerIndices, fields, "MEAN_MOTION"), out meanMotion))
        {
            return false;
        }

        TryParseDouble(GetCsvField(headerIndices, fields, "BSTAR"), out bstar);

        record = new CelesTrakOmmRecord
        {
            OBJECT_NAME = GetCsvField(headerIndices, fields, "OBJECT_NAME"),
            NORAD_CAT_ID = noradId,
            EPOCH = GetCsvField(headerIndices, fields, "EPOCH"),
            INCLINATION = inclination,
            RA_OF_ASC_NODE = raan,
            ECCENTRICITY = eccentricity,
            ARG_OF_PERICENTER = argPerigee,
            MEAN_ANOMALY = meanAnomaly,
            MEAN_MOTION = meanMotion,
            BSTAR = bstar
        };
        return true;
    }

    private static string GetCsvField(Dictionary<string, int> headerIndices, List<string> fields, string header)
    {
        int index;
        if (headerIndices == null || fields == null || !headerIndices.TryGetValue(header, out index) || index < 0 || index >= fields.Count)
        {
            return string.Empty;
        }

        return fields[index].Trim();
    }

    private static List<string> SplitCsvLine(string line)
    {
        List<string> fields = new List<string>();
        if (line == null)
        {
            fields.Add(string.Empty);
            return fields;
        }

        bool inQuotes = false;
        string current = string.Empty;
        for (int i = 0; i < line.Length; i++)
        {
            char character = line[i];
            if (character == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (character == ',' && !inQuotes)
            {
                fields.Add(current);
                current = string.Empty;
            }
            else
            {
                current += character;
            }
        }
        fields.Add(current);
        return fields;
    }

    private static bool TryParseOmmEpoch(string value, out DateTime epoch)
    {
        epoch = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        DateTime parsed;
        if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsed))
        {
            return false;
        }

        epoch = parsed.ToUniversalTime();
        return true;
    }

    private static bool TryParseDouble(string value, out double result)
    {
        return double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseInt(string value, out int result)
    {
        return int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }

    private static string SafeSubstring(string value, int startIndex, int length)
    {
        if (string.IsNullOrEmpty(value) || startIndex >= value.Length)
        {
            return string.Empty;
        }
        int safeLength = Math.Min(length, value.Length - startIndex);
        return value.Substring(startIndex, safeLength);
    }

    private static DateTime NormalizeUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }
        return dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime;
    }

    [Serializable]
    private class CelesTrakOmmRecordList
    {
        public CelesTrakOmmRecord[] items;
    }

    [Serializable]
    private class CelesTrakOmmRecord
    {
        public string OBJECT_NAME;
        public int NORAD_CAT_ID;
        public string EPOCH;
        public double INCLINATION;
        public double RA_OF_ASC_NODE;
        public double ECCENTRICITY;
        public double ARG_OF_PERICENTER;
        public double MEAN_ANOMALY;
        public double MEAN_MOTION;
        public double BSTAR;
    }
}

public enum TleEpochFreshness
{
    Unknown,
    Fresh,
    Stale,
    VeryStale
}
