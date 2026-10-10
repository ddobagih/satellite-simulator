using System;
using UnityEngine;

[Serializable]
public struct GeodeticCoordinate
{
    public double latitudeDegrees;
    public double longitudeDegrees;
    public double altitudeKm;

    public GeodeticCoordinate(double latitudeDegrees, double longitudeDegrees, double altitudeKm)
    {
        this.latitudeDegrees = latitudeDegrees;
        this.longitudeDegrees = longitudeDegrees;
        this.altitudeKm = altitudeKm;
    }

    public override string ToString()
    {
        return latitudeDegrees.ToString("F4") + "°, " + longitudeDegrees.ToString("F4") + "°, " + altitudeKm.ToString("F1") + " km";
    }
}

/// <summary>
/// 위성 좌표 변환 유틸리티.
///
/// 단위:
/// - ECI/ECEF 위치: km
/// - ECI/ECEF 속도: km/s
/// - 위도/경도: degrees
/// - Unity 위치: project earth radius를 기준으로 한 Unity world unit
///
/// 좌표계:
/// - ECEF는 표준 WGS84 축을 사용한다. X=위도 0/경도 0, Y=위도 0/경도 90E, Z=북극.
/// - Unity 지구 표현은 기존 프로젝트 SurfacePosition과 맞춰 X=경도 0, Y=북극, Z=경도 90E로 매핑한다.
/// - ECI->ECEF는 GMST 기반 근사 변환이며 nutation/polar motion/TEME 보정은 포함하지 않는다.
/// </summary>
public static class SatelliteCoordinateTransforms
{
    public const string EciToEcefModelName = "GMST-only ECI/ECEF approximation";
    public const string EciToEcefModelWarning = "TEME nutation, polar motion, UT1-UTC 보정을 포함하지 않습니다.";

    public const double EarthRadiusEquatorialKm = 6378.137;
    public const double EarthFlattening = 1.0 / 298.257223563;
    public const double EarthRotationRateRadSec = 7.2921150e-5;
    public const float DefaultUnityEarthRadius = 3f;

    private const double TwoPi = Math.PI * 2.0;

    public static Vector3 EciToEcef(Vector3 positionEciKm, DateTime utc)
    {
        utc = NormalizeUtc(utc);
        double theta = GreenwichMeanSiderealTimeRadians(utc);
        return RotateAroundZ(positionEciKm, -theta);
    }

    public static string EciToEcefStatusMessage()
    {
        return EciToEcefModelName + " - " + EciToEcefModelWarning;
    }

    public static Vector3 EciVelocityToEcef(Vector3 positionEciKm, Vector3 velocityEciKmSec, DateTime utc)
    {
        utc = NormalizeUtc(utc);
        Vector3 omegaCrossR = new Vector3(
            (float)(-EarthRotationRateRadSec * positionEciKm.y),
            (float)(EarthRotationRateRadSec * positionEciKm.x),
            0f);
        Vector3 rotatingVelocity = velocityEciKmSec - omegaCrossR;
        double theta = GreenwichMeanSiderealTimeRadians(utc);
        return RotateAroundZ(rotatingVelocity, -theta);
    }

    public static GeodeticCoordinate EcefToGeodetic(Vector3 positionEcefKm)
    {
        double a = EarthRadiusEquatorialKm;
        double f = EarthFlattening;
        double e2 = f * (2.0 - f);

        double x = positionEcefKm.x;
        double y = positionEcefKm.y;
        double z = positionEcefKm.z;
        double longitudeRad = Math.Atan2(y, x);
        double p = Math.Sqrt(x * x + y * y);

        if (p < 1e-9)
        {
            double poleLatitude = z >= 0.0 ? 90.0 : -90.0;
            double polarRadius = a * (1.0 - f);
            return new GeodeticCoordinate(poleLatitude, 0.0, Math.Abs(z) - polarRadius);
        }

        double latitudeRad = Math.Atan2(z, p * (1.0 - e2));
        double altitudeKm = 0.0;
        for (int i = 0; i < 8; i++)
        {
            double sinLat = Math.Sin(latitudeRad);
            double cosLat = Math.Cos(latitudeRad);
            double n = a / Math.Sqrt(1.0 - e2 * sinLat * sinLat);
            altitudeKm = p / Math.Max(1e-12, cosLat) - n;
            latitudeRad = Math.Atan2(z, p * (1.0 - e2 * n / (n + altitudeKm)));
        }

        double finalSinLat = Math.Sin(latitudeRad);
        double finalCosLat = Math.Cos(latitudeRad);
        double finalN = a / Math.Sqrt(1.0 - e2 * finalSinLat * finalSinLat);
        altitudeKm = p / Math.Max(1e-12, finalCosLat) - finalN;

        return new GeodeticCoordinate(
            RadiansToDegrees(latitudeRad),
            NormalizeLongitudeDegrees(RadiansToDegrees(longitudeRad)),
            altitudeKm);
    }

    public static Vector3 GeodeticToEcef(double latitudeDegrees, double longitudeDegrees, double altitudeKm)
    {
        double a = EarthRadiusEquatorialKm;
        double f = EarthFlattening;
        double e2 = f * (2.0 - f);
        double lat = DegreesToRadians(latitudeDegrees);
        double lon = DegreesToRadians(longitudeDegrees);
        double sinLat = Math.Sin(lat);
        double cosLat = Math.Cos(lat);
        double n = a / Math.Sqrt(1.0 - e2 * sinLat * sinLat);

        double x = (n + altitudeKm) * cosLat * Math.Cos(lon);
        double y = (n + altitudeKm) * cosLat * Math.Sin(lon);
        double z = (n * (1.0 - e2) + altitudeKm) * sinLat;
        return new Vector3((float)x, (float)y, (float)z);
    }

    public static Vector3 GeodeticToUnityPosition(double latitudeDegrees, double longitudeDegrees, double altitudeKm, float earthRadiusUnity)
    {
        return EcefToUnityPosition(GeodeticToEcef(latitudeDegrees, longitudeDegrees, altitudeKm), earthRadiusUnity, Vector3.zero);
    }

    public static Vector3 GeodeticToUnityPosition(double latitudeDegrees, double longitudeDegrees, double altitudeKm, float earthRadiusUnity, Vector3 earthCenter)
    {
        return EcefToUnityPosition(GeodeticToEcef(latitudeDegrees, longitudeDegrees, altitudeKm), earthRadiusUnity, earthCenter);
    }

    public static Vector3 EcefToUnityPosition(Vector3 positionEcefKm, float earthRadiusUnity)
    {
        return EcefToUnityPosition(positionEcefKm, earthRadiusUnity, Vector3.zero);
    }

    public static Vector3 EcefToUnityPosition(Vector3 positionEcefKm, float earthRadiusUnity, Vector3 earthCenter)
    {
        float scale = earthRadiusUnity / (float)EarthRadiusEquatorialKm;
        return earthCenter + EcefToUnityVector(positionEcefKm) * scale;
    }

    public static Vector3 EcefToUnityVector(Vector3 vectorEcef)
    {
        return new Vector3(vectorEcef.x, vectorEcef.z, vectorEcef.y);
    }

    public static Vector3 EcefToUnityDirection(Vector3 vectorEcef)
    {
        Vector3 unity = EcefToUnityVector(vectorEcef);
        if (unity.sqrMagnitude < 1e-12f)
        {
            return Vector3.zero;
        }
        return unity.normalized;
    }

    public static Vector3 EciToUnityPosition(Vector3 positionEciKm, DateTime utc, float earthRadiusUnity)
    {
        return EciToUnityPosition(positionEciKm, utc, earthRadiusUnity, Vector3.zero);
    }

    public static Vector3 EciToUnityPosition(Vector3 positionEciKm, DateTime utc, float earthRadiusUnity, Vector3 earthCenter)
    {
        Vector3 ecef = EciToEcef(positionEciKm, utc);
        return EcefToUnityPosition(ecef, earthRadiusUnity, earthCenter);
    }

    public static Vector3 EciVelocityToUnityVector(Vector3 positionEciKm, Vector3 velocityEciKmSec, DateTime utc, float earthRadiusUnity)
    {
        Vector3 ecefVelocity = EciVelocityToEcef(positionEciKm, velocityEciKmSec, utc);
        float scale = earthRadiusUnity / (float)EarthRadiusEquatorialKm;
        return EcefToUnityVector(ecefVelocity) * scale;
    }

    public static Vector3 EciVelocityToUnityDirection(Vector3 positionEciKm, Vector3 velocityEciKmSec, DateTime utc)
    {
        Vector3 ecefVelocity = EciVelocityToEcef(positionEciKm, velocityEciKmSec, utc);
        return EcefToUnityDirection(ecefVelocity);
    }

    public static GeodeticCoordinate EciToGeodetic(Vector3 positionEciKm, DateTime utc)
    {
        return EcefToGeodetic(EciToEcef(positionEciKm, utc));
    }

    public static double GreenwichMeanSiderealTimeRadians(DateTime utc)
    {
        double jd = JulianDate(utc);
        double t = (jd - 2451545.0) / 36525.0;
        double gmstDegrees = 280.46061837
            + 360.98564736629 * (jd - 2451545.0)
            + 0.000387933 * t * t
            - t * t * t / 38710000.0;
        return NormalizeRadians(DegreesToRadians(gmstDegrees));
    }

    public static double JulianDate(DateTime utc)
    {
        utc = NormalizeUtc(utc);
        int year = utc.Year;
        int month = utc.Month;
        double day = utc.Day + utc.TimeOfDay.TotalDays;

        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }

        int a = year / 100;
        int b = 2 - a + a / 4;
        return Math.Floor(365.25 * (year + 4716))
            + Math.Floor(30.6001 * (month + 1))
            + day + b - 1524.5;
    }

    public static double NormalizeLongitudeDegrees(double longitudeDegrees)
    {
        double normalized = (longitudeDegrees + 180.0) % 360.0;
        if (normalized < 0.0)
        {
            normalized += 360.0;
        }
        return normalized - 180.0;
    }

    private static Vector3 RotateAroundZ(Vector3 vector, double radians)
    {
        double cos = Math.Cos(radians);
        double sin = Math.Sin(radians);
        double x = cos * vector.x - sin * vector.y;
        double y = sin * vector.x + cos * vector.y;
        return new Vector3((float)x, (float)y, vector.z);
    }

    private static DateTime NormalizeUtc(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }
        if (dateTime.Kind == DateTimeKind.Unspecified)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
        return dateTime;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }

    private static double NormalizeRadians(double radians)
    {
        double normalized = radians % TwoPi;
        if (normalized < 0.0)
        {
            normalized += TwoPi;
        }
        return normalized;
    }
}
