using UnityEngine;

public static class SatelliteRouteEstimator
{
    private const float EarthRadiusKm = 6371f;
    private const int ContactAllowanceBufferSeconds = 3600;
    private const int FallbackImmediateContactSeconds = 300;

    public static float EstimateSurfaceAngleDegrees(SatelliteData satellite, Transform satelliteTransform, string targetName, Transform targetTransform)
    {
        if (satelliteTransform != null && targetTransform != null)
        {
            Vector3 center = Vector3.zero;
            Vector3 satelliteDirection = (satelliteTransform.position - center).normalized;
            Vector3 targetDirection = (targetTransform.position - center).normalized;
            if (satelliteDirection.sqrMagnitude > 0.001f && targetDirection.sqrMagnitude > 0.001f)
            {
                return Vector3.Angle(satelliteDirection, targetDirection);
            }
        }

        int hash = StableHash((satellite.satelliteName ?? string.Empty) + ":" + (targetName ?? string.Empty));
        float pseudoAngle = Mathf.Abs(hash % 120);
        return Mathf.Clamp(satellite.nextWindowMinutes * 3.2f + pseudoAngle * 0.45f, 0f, 180f);
    }

    public static int EstimateWindowSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond)
    {
        return EstimateWindowSeconds(satellite, routeAngleDegrees, hasDynamicTransform, degreesPerSecond, 0f);
    }

    public static int EstimateWindowSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond, float routeAngleTrendDegreesPerSecond)
    {
        return EstimateWindowSeconds(satellite, routeAngleDegrees, hasDynamicTransform, degreesPerSecond, routeAngleTrendDegreesPerSecond, string.Empty);
    }

    public static int EstimateWindowSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond, float routeAngleTrendDegreesPerSecond, string endpointName)
    {
        if (!hasDynamicTransform)
        {
            return Mathf.Max(0, satellite.nextWindowMinutes) * 60;
        }

        float contactAngle = ContactAngleDegrees(satellite, endpointName);
        float gapDegrees = Mathf.Max(0f, routeAngleDegrees - contactAngle);
        if (gapDegrees <= 0f)
        {
            return 0;
        }

        float safeDegreesPerSecond = EffectiveDegreesPerSecond(degreesPerSecond, routeAngleTrendDegreesPerSecond);
        int directSeconds = Mathf.CeilToInt(gapDegrees / safeDegreesPerSecond);
        if (routeAngleTrendDegreesPerSecond > safeDegreesPerSecond * 0.12f)
        {
            int cycleSeconds = EstimateOrbitCycleSeconds(safeDegreesPerSecond);
            return Mathf.Max(1, cycleSeconds - Mathf.Min(cycleSeconds - 1, directSeconds));
        }
        return directSeconds;
    }

    public static int EstimateAvailableForSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond)
    {
        return EstimateAvailableForSeconds(satellite, routeAngleDegrees, hasDynamicTransform, degreesPerSecond, 0f);
    }

    public static int EstimateAvailableForSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond, float routeAngleTrendDegreesPerSecond)
    {
        return EstimateAvailableForSeconds(satellite, routeAngleDegrees, hasDynamicTransform, degreesPerSecond, routeAngleTrendDegreesPerSecond, string.Empty);
    }

    public static int EstimateAvailableForSeconds(SatelliteData satellite, float routeAngleDegrees, bool hasDynamicTransform, float degreesPerSecond, float routeAngleTrendDegreesPerSecond, string endpointName)
    {
        if (satellite == null)
        {
            return 0;
        }
        if (!hasDynamicTransform)
        {
            return Mathf.Max(0, satellite.nextWindowMinutes) <= 0 ? FallbackImmediateAvailableSeconds(satellite) : 0;
        }

        float contactAngle = ContactAngleDegrees(satellite, endpointName);
        if (routeAngleDegrees > contactAngle)
        {
            return 0;
        }

        float safeDegreesPerSecond = EffectiveDegreesPerSecond(degreesPerSecond, routeAngleTrendDegreesPerSecond);
        float remainingDegrees = routeAngleTrendDegreesPerSecond < -safeDegreesPerSecond * 0.12f
            ? contactAngle + Mathf.Max(0f, routeAngleDegrees)
            : contactAngle - Mathf.Max(0f, routeAngleDegrees);
        return Mathf.CeilToInt(Mathf.Max(0f, remainingDegrees) / safeDegreesPerSecond);
    }

    private static float EffectiveDegreesPerSecond(float degreesPerSecond, float routeAngleTrendDegreesPerSecond)
    {
        float fallbackSpeed = Mathf.Max(0.01f, degreesPerSecond);
        float trendSpeed = Mathf.Abs(routeAngleTrendDegreesPerSecond);
        if (trendSpeed < fallbackSpeed * 0.12f)
        {
            return fallbackSpeed;
        }
        return Mathf.Clamp(trendSpeed, fallbackSpeed * 0.35f, fallbackSpeed * 1.8f);
    }

    private static int EstimateOrbitCycleSeconds(float degreesPerSecond)
    {
        return Mathf.Clamp(Mathf.CeilToInt(360f / Mathf.Max(0.01f, degreesPerSecond)), 20, 7200);
    }

    private static int FallbackImmediateAvailableSeconds(SatelliteData satellite)
    {
        int offset = Mathf.Abs(StableHash(satellite.satelliteName ?? string.Empty)) % FallbackImmediateContactSeconds;
        int elapsed = Mathf.FloorToInt(Mathf.Repeat(Time.unscaledTime + offset, FallbackImmediateContactSeconds));
        return Mathf.Max(1, FallbackImmediateContactSeconds - elapsed);
    }

    public static float ContactAngleDegrees(SatelliteData satellite)
    {
        return ContactAngleDegrees(satellite, string.Empty);
    }

    public static float ContactAngleDegrees(SatelliteData satellite, string endpointName)
    {
        if (satellite == null)
        {
            return 18f;
        }

        int hash = Mathf.Abs(StableHash((satellite.satelliteName ?? string.Empty) + ":" + (endpointName ?? string.Empty)));
        float altitudeKm = Mathf.Max(120f, satellite.altitudeKm);
        float minimumElevationDegrees = satellite.altitudeKm < 1200f
            ? 2f + (hash % 13) * 0.75f
            : satellite.altitudeKm < 20000f ? 3f + (hash % 10) * 0.8f : 4f + (hash % 8) * 0.75f;
        float elevationRadians = minimumElevationDegrees * Mathf.Deg2Rad;
        float radiusRatio = EarthRadiusKm / (EarthRadiusKm + altitudeKm);
        float centralAngleRadians = Mathf.Acos(Mathf.Clamp(radiusRatio * Mathf.Cos(elevationRadians), -1f, 1f)) - elevationRadians;
        float baseAngle = Mathf.Max(1f, centralAngleRadians * Mathf.Rad2Deg);
        float linkMargin = (((hash / 17) % 9) - 4) * 0.35f;
        if (satellite.orbitSlot == "Polar-C")
        {
            linkMargin += 0.8f;
        }
        if (satellite.orbitSlot == "MEO-B")
        {
            linkMargin += 1.4f;
        }
        if (satellite.orbitSlot == "LEO-TLE")
        {
            linkMargin += (((hash / 37) % 7) - 3) * 0.25f;
        }
        float maxAngle = satellite.altitudeKm < 1200f ? 28f : satellite.altitudeKm < 20000f ? 34f : 36f;
        return Mathf.Clamp(baseAngle + linkMargin, 6f, maxAngle);
    }

    public static int ContactAllowanceSeconds(int requestedDelayMinutes)
    {
        return Mathf.Max(ContactAllowanceBufferSeconds, Mathf.Max(0, requestedDelayMinutes) * 60 + ContactAllowanceBufferSeconds);
    }

    public static SatelliteCommunicationWindowState CommunicationWindowState(bool canServe, int windowSeconds, int availableForSeconds)
    {
        if (!canServe)
        {
            return SatelliteCommunicationWindowState.Unavailable;
        }
        return windowSeconds <= 0 ? SatelliteCommunicationWindowState.Remaining : SatelliteCommunicationWindowState.AvailableLater;
    }

    public static int EstimatePayloadSeconds(int payloadKb)
    {
        return Mathf.CeilToInt(Mathf.Max(1, payloadKb) / 48f);
    }

    public static float CalculateRouteScore(SatelliteData satellite, SatelliteRouteEstimateRecord estimate, int payloadKb)
    {
        float price = estimate.estimatedPriceUsd > 0f ? estimate.estimatedPriceUsd : EstimateRoutePriceUsd(satellite, payloadKb, estimate.estimatedDeliverySeconds, estimate.routeAngleDegrees);
        float unsupportedPenalty = estimate.supportsTarget ? 0f : 10000f;
        float unavailablePenalty = estimate.canServe ? 0f : 2500f;
        float targetAffinity = Mathf.Abs(StableHash(satellite.satelliteName + ":" + estimate.targetName) % 100) * 0.05f;
        return unsupportedPenalty
            + unavailablePenalty
            + estimate.estimatedWindowSeconds * 2.2f
            + estimate.routeAngleDegrees * 1.4f
            + satellite.latencySeconds * 5f
            + price * 12f
            + satellite.congestionPercent * 0.8f
            - satellite.RecentSuccessPercent * 0.55f
            + targetAffinity;
    }

    public static float EstimateRoutePriceUsd(SatelliteData satellite, int payloadKb, int deliverySeconds, float routeAngleDegrees)
    {
        if (satellite == null)
        {
            return 0f;
        }

        return satellite.EstimatePriceUsd(payloadKb);
    }

    public static string BuildAvailabilitySummary(SatelliteData satellite, SatelliteRouteEstimateRecord estimate, int requestedDelayMinutes)
    {
        if (estimate == null || !estimate.canServe)
        {
            return "통신 불가";
        }

        if (estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining || estimate.estimatedWindowSeconds <= 0)
        {
            int availableSeconds = estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedAvailableForSeconds;
            return FormatSeconds(availableSeconds) + " 남음";
        }
        int waitSeconds = Mathf.Max(0, estimate.communicationWindowSeconds);
        if (waitSeconds <= 0)
        {
            waitSeconds = estimate.estimatedWindowSeconds;
        }
        return FormatSeconds(waitSeconds) + " 후 가능";
    }

    public static string FormatSeconds(int totalSeconds)
    {
        int safeSeconds = Mathf.Max(0, totalSeconds);
        if (safeSeconds >= 60)
        {
            return (safeSeconds / 60) + "분 " + (safeSeconds % 60) + "초";
        }
        return safeSeconds + "초";
    }

    public static int StableHash(string text)
    {
        unchecked
        {
            int hash = 23;
            if (text == null)
            {
                return hash;
            }
            for (int i = 0; i < text.Length; i++)
            {
                hash = hash * 31 + text[i];
            }
            return hash;
        }
    }
}
