using System;
using System.Globalization;
using UnityEngine;

/// <summary>
/// TLE 기반 위성 상태 전파 유틸리티.
/// 현재 구현은 Unity 시각화용 near-earth Kepler + 1차 J2 세차 baseline이다.
/// Vallado SGP4와 같은 입력/출력 흐름을 따르지만, deep-space/drag 세부 보정까지 포함한 정밀 SGP4는 아니다.
/// CelesTrak OMM JSON처럼 line1/line2가 없는 데이터도 같은 평균 궤도 요소가 있으면 전파할 수 있다.
/// 출력 단위는 위치 km, 속도 km/s다.
/// </summary>
public static class Sgp4Propagator
{
    public const bool UsesValladoSgp4 = false;
    public const string PropagationModelName = "Kepler + first-order J2 visual approximation";
    public const string PropagationModelWarning = "정밀 Vallado SGP4가 아니며 deep-space/drag 세부 보정을 포함하지 않습니다.";

    public const double EarthMuKm3Sec2 = 398600.4418;
    public const double EarthRadiusKm = 6378.137;
    public const double EarthJ2 = 1.08262668e-3;

    private const double TwoPi = Math.PI * 2.0;
    private const double SecondsPerDay = 86400.0;

    private struct TleElements
    {
        public DateTime epochUtc;
        public double inclinationRad;
        public double raanRad;
        public double eccentricity;
        public double argumentOfPerigeeRad;
        public double meanAnomalyRad;
        public double meanMotionRadSec;
    }

    public static bool TryPropagate(RealSatelliteTle tle, DateTime utc, out Vector3 positionEciKm, out Vector3 velocityEciKmSec, out string error)
    {
        positionEciKm = Vector3.zero;
        velocityEciKmSec = Vector3.zero;
        error = string.Empty;

        TleElements elements;
        if (!TryParseTle(tle, out elements, out error))
        {
            return false;
        }

        utc = NormalizeUtc(utc);
        double elapsedSeconds = (utc - elements.epochUtc).TotalSeconds;
        double n = elements.meanMotionRadSec;
        if (n <= 0.0 || double.IsNaN(n) || double.IsInfinity(n))
        {
            error = "TLE 평균 운동량(mean motion)이 유효하지 않습니다.";
            return false;
        }

        double semiMajorAxisKm = Math.Pow(EarthMuKm3Sec2 / (n * n), 1.0 / 3.0);
        double e = Math.Max(0.0, Math.Min(0.999999, elements.eccentricity));
        double oneMinusESquared = Math.Max(1e-8, 1.0 - e * e);
        double p = semiMajorAxisKm * oneMinusESquared;
        if (p <= 0.0)
        {
            error = "TLE 이심률/장반경으로 유효한 궤도 반장축을 만들 수 없습니다.";
            return false;
        }

        double cosI = Math.Cos(elements.inclinationRad);
        double sinI = Math.Sin(elements.inclinationRad);
        double j2Scale = EarthJ2 * Math.Pow(EarthRadiusKm / p, 2.0);
        double raanDot = -1.5 * j2Scale * n * cosI;
        double argumentOfPerigeeDot = 0.75 * j2Scale * n * (5.0 * cosI * cosI - 1.0);
        double meanAnomalyDot = n + 0.75 * j2Scale * n * Math.Sqrt(oneMinusESquared) * (3.0 * cosI * cosI - 1.0);

        double raan = NormalizeRadians(elements.raanRad + raanDot * elapsedSeconds);
        double argumentOfPerigee = NormalizeRadians(elements.argumentOfPerigeeRad + argumentOfPerigeeDot * elapsedSeconds);
        double meanAnomaly = NormalizeRadians(elements.meanAnomalyRad + meanAnomalyDot * elapsedSeconds);

        double eccentricAnomaly = SolveKepler(meanAnomaly, e);
        double cosE = Math.Cos(eccentricAnomaly);
        double sinE = Math.Sin(eccentricAnomaly);
        double sqrtOneMinusESquared = Math.Sqrt(oneMinusESquared);
        double radiusKm = semiMajorAxisKm * (1.0 - e * cosE);
        if (radiusKm <= 0.0)
        {
            error = "전파 결과 반지름이 유효하지 않습니다.";
            return false;
        }

        double xPerifocal = semiMajorAxisKm * (cosE - e);
        double yPerifocal = semiMajorAxisKm * sqrtOneMinusESquared * sinE;
        double velocityFactor = Math.Sqrt(EarthMuKm3Sec2 * semiMajorAxisKm) / radiusKm;
        double vxPerifocal = -velocityFactor * sinE;
        double vyPerifocal = velocityFactor * sqrtOneMinusESquared * cosE;

        double cosRaan = Math.Cos(raan);
        double sinRaan = Math.Sin(raan);
        double cosArg = Math.Cos(argumentOfPerigee);
        double sinArg = Math.Sin(argumentOfPerigee);

        double m11 = cosRaan * cosArg - sinRaan * sinArg * cosI;
        double m12 = -cosRaan * sinArg - sinRaan * cosArg * cosI;
        double m21 = sinRaan * cosArg + cosRaan * sinArg * cosI;
        double m22 = -sinRaan * sinArg + cosRaan * cosArg * cosI;
        double m31 = sinArg * sinI;
        double m32 = cosArg * sinI;

        positionEciKm = new Vector3(
            (float)(m11 * xPerifocal + m12 * yPerifocal),
            (float)(m21 * xPerifocal + m22 * yPerifocal),
            (float)(m31 * xPerifocal + m32 * yPerifocal));
        velocityEciKmSec = new Vector3(
            (float)(m11 * vxPerifocal + m12 * vyPerifocal),
            (float)(m21 * vxPerifocal + m22 * vyPerifocal),
            (float)(m31 * vxPerifocal + m32 * vyPerifocal));
        return true;
    }

    public static string ModelStatusMessage()
    {
        return PropagationModelName + " - " + PropagationModelWarning;
    }

    public static bool TryGetEpochUtc(RealSatelliteTle tle, out DateTime epochUtc, out string error)
    {
        epochUtc = DateTime.MinValue;
        TleElements elements;
        if (!TryParseTle(tle, out elements, out error))
        {
            return false;
        }
        epochUtc = elements.epochUtc;
        return true;
    }

    public static bool TryParseMeanMotionRevPerDay(RealSatelliteTle tle, out double meanMotionRevPerDay, out string error)
    {
        meanMotionRevPerDay = 0.0;
        TleElements elements;
        if (!TryParseTle(tle, out elements, out error))
        {
            return false;
        }
        meanMotionRevPerDay = elements.meanMotionRadSec * SecondsPerDay / TwoPi;
        return true;
    }

    private static bool TryParseTle(RealSatelliteTle tle, out TleElements elements, out string error)
    {
        elements = new TleElements();
        error = string.Empty;
        if (tle == null)
        {
            error = "TLE 데이터가 없습니다.";
            return false;
        }

        string line1 = tle.line1;
        string line2 = tle.line2;
        bool hasLines = !string.IsNullOrWhiteSpace(line1) && !string.IsNullOrWhiteSpace(line2);
        bool hasFieldElements = tle.epoch != DateTime.MinValue && tle.meanMotion > 0.0;
        if (!hasLines && !hasFieldElements)
        {
            error = "TLE line1/line2 또는 OMM 평균 궤도 요소가 없습니다.";
            return false;
        }

        if (hasLines)
        {
            line1 = line1.TrimEnd();
            line2 = line2.TrimEnd();
            if (line1.Length < 61 || line2.Length < 63)
            {
                error = "TLE 행 길이가 부족합니다.";
                return false;
            }
            if (line1.Length >= 69 && line2.Length >= 69 && (!HasValidChecksum(line1) || !HasValidChecksum(line2)))
            {
                error = "TLE checksum이 유효하지 않습니다.";
                return false;
            }
            if (line1[0] != '1' || line2[0] != '2')
            {
                error = "TLE line 번호가 올바르지 않습니다.";
                return false;
            }
            if (line1.Length >= 7 && line2.Length >= 7 && !string.Equals(line1.Substring(2, 5), line2.Substring(2, 5), StringComparison.Ordinal))
            {
                error = "TLE line1/line2 NORAD ID가 일치하지 않습니다.";
                return false;
            }
        }

        try
        {
            double meanMotionRevPerDay;
            if (hasLines)
            {
                elements.epochUtc = ParseEpochUtc(line1.Substring(18, 2), line1.Substring(20, 12));
                elements.inclinationRad = DegreesToRadians(ParseDouble(line2.Substring(8, 8)));
                elements.raanRad = DegreesToRadians(ParseDouble(line2.Substring(17, 8)));
                elements.eccentricity = ParseImpliedDecimal(line2.Substring(26, 7));
                elements.argumentOfPerigeeRad = DegreesToRadians(ParseDouble(line2.Substring(34, 8)));
                elements.meanAnomalyRad = DegreesToRadians(ParseDouble(line2.Substring(43, 8)));
                meanMotionRevPerDay = ParseDouble(line2.Substring(52, 11));
            }
            else
            {
                elements.epochUtc = NormalizeUtc(tle.epoch);
                elements.inclinationRad = DegreesToRadians(tle.inclination);
                elements.raanRad = DegreesToRadians(tle.raan);
                elements.eccentricity = tle.eccentricity;
                elements.argumentOfPerigeeRad = DegreesToRadians(tle.argPerigee);
                elements.meanAnomalyRad = DegreesToRadians(tle.meanAnomaly);
                meanMotionRevPerDay = tle.meanMotion;
            }

            elements.meanMotionRadSec = meanMotionRevPerDay * TwoPi / SecondsPerDay;
        }
        catch (Exception exception)
        {
            error = "TLE/OMM 파싱 실패: " + exception.Message;
            return false;
        }

        if (elements.eccentricity < 0.0 || elements.eccentricity >= 1.0)
        {
            error = "TLE/OMM 이심률이 near-earth 타원 궤도 범위를 벗어났습니다.";
            return false;
        }
        if (elements.meanMotionRadSec <= 0.0)
        {
            error = "TLE/OMM 평균 운동량(mean motion)이 유효하지 않습니다.";
            return false;
        }
        return true;
    }

    private static DateTime ParseEpochUtc(string yearText, string dayOfYearText)
    {
        int shortYear = int.Parse(yearText.Trim(), CultureInfo.InvariantCulture);
        int fullYear = shortYear < 57 ? 2000 + shortYear : 1900 + shortYear;
        double dayOfYear = ParseDouble(dayOfYearText);
        return new DateTime(fullYear, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(dayOfYear - 1.0);
    }

    private static double ParseDouble(string text)
    {
        return double.Parse(text.Trim(), CultureInfo.InvariantCulture);
    }

    private static double ParseImpliedDecimal(string text)
    {
        string trimmed = text.Trim();
        return string.IsNullOrEmpty(trimmed) ? 0.0 : double.Parse("0." + trimmed, CultureInfo.InvariantCulture);
    }

    private static bool HasValidChecksum(string line)
    {
        if (string.IsNullOrEmpty(line) || line.Length < 69 || !char.IsDigit(line[68]))
        {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < 68; i++)
        {
            char c = line[i];
            if (c >= '0' && c <= '9')
            {
                sum += c - '0';
            }
            else if (c == '-')
            {
                sum += 1;
            }
        }
        return sum % 10 == line[68] - '0';
    }

    private static double SolveKepler(double meanAnomalyRad, double eccentricity)
    {
        double eccentricAnomaly = eccentricity < 0.8 ? meanAnomalyRad : Math.PI;
        for (int i = 0; i < 12; i++)
        {
            double f = eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly) - meanAnomalyRad;
            double fp = 1.0 - eccentricity * Math.Cos(eccentricAnomaly);
            if (Math.Abs(fp) < 1e-12)
            {
                break;
            }
            double delta = f / fp;
            eccentricAnomaly -= delta;
            if (Math.Abs(delta) < 1e-11)
            {
                break;
            }
        }
        return eccentricAnomaly;
    }

    private static DateTime NormalizeUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }
        return dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) : dateTime;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double NormalizeRadians(double radians)
    {
        double normalized = radians % TwoPi;
        return normalized < 0.0 ? normalized + TwoPi : normalized;
    }
}
