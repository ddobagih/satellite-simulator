using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum MissionPhase
{
    Idle,
    Reserved,
    Running,
    Observing,
    ObservationComplete,
    Transmitting,
    Complete,
    Failed
}

public enum SatelliteCommunicationStep
{
    Draft,
    Reserved,
    WaitingForUplink,
    Uplink,
    LeoReceived,
    InterSatelliteLink,
    DestinationRelaySelection,
    Downlink,
    FinalReceive,
    Complete,
    Failed,
    WaitingForDownlink
}

[Serializable]
public class MessageAttachmentRecord
{
    public string fileName;
    public int sizeKb;
    public int sizeBytes;

    public MessageAttachmentRecord()
    {
    }

    public MessageAttachmentRecord(string fileName, int sizeKb)
        : this(fileName, sizeKb, 0)
    {
    }

    public MessageAttachmentRecord(string fileName, int sizeKb, long sizeBytes)
    {
        this.fileName = string.IsNullOrWhiteSpace(fileName) ? string.Empty : fileName.Trim();
        long safeBytes = Math.Max(0L, sizeBytes);
        this.sizeBytes = safeBytes > int.MaxValue ? int.MaxValue : (int)safeBytes;
        this.sizeKb = Mathf.Max(0, sizeKb);
        if (this.sizeBytes <= 0 && this.sizeKb > 0)
        {
            long bytesFromKb = (long)this.sizeKb * 1024L;
            this.sizeBytes = bytesFromKb > int.MaxValue ? int.MaxValue : (int)bytesFromKb;
        }
        if (this.sizeKb <= 0 && this.sizeBytes > 0)
        {
            this.sizeKb = Mathf.CeilToInt(this.sizeBytes / 1024f);
        }
    }
}

[Serializable]
public class SentMessageRecord
{
    public string trackingId;
    public string receiptId
    {
        get { return trackingId; }
        set { trackingId = value; }
    }
    public string deliveredAt;
    public string scheduledLabel;
    public string originName;
    public string originLabel;
    public string targetName;
    public string destinationLabel;
    public string satelliteName;
    public string messageText;
    public string attachmentName;
    public int attachmentSizeKb;
    public List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
    public int messageTextBytes;
    public int payloadBytes;
    public int payloadKb;
    public float priceUsd;
    public float baseFeeUsd;
    public float payloadFeeUsd;
    public float urgencyFeeUsd;
    public int elapsedSeconds;
    public int successProbabilityPercent;
    public string destinationRelaySatelliteName;
    public string transmissionLog;
    public string securitySummary;
    public string checksumLabel;
    public bool isComplete = true;
    public bool isActiveTransferRecord;
    public string statusLabel;
}

[Serializable]
public class QueuedMessageRouteDetail
{
    public bool isAvailable;
    public string satelliteName;
    public string scheduledLabel;
    public int requestedSendDelayMinutes;
    public int estimatedDeliverySeconds;
    public string estimatedDeliveryLabel;
    public float priceUsd;
    public float baseFeeUsd;
    public float payloadFeeUsd;
    public float urgencyFeeUsd;
    public int successProbabilityPercent;
    public string routeSummary;
    public string recommendationReason;
    public string statusSummary;
    public string attachmentSummary;
    public int payloadKb;
    public bool canEdit;
    public bool canCancel;
    public bool canStartNow;
    public string actionSummary;
}

[Serializable]
public class QueuedMessageRecord
{
    public string queuedAt;
    public string originName;
    public string originLabel;
    public string targetName;
    public string destinationLabel;
    public string messageText;
    public int requestedSendDelayMinutes;
    public int scheduledDayOffset;
    public int scheduledHour;
    public int scheduledMinute;
    public string scheduledLabel;
    public string attachmentName;
    public int attachmentSizeKb;
    public List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
    public int messageTextBytes;
    public int payloadKb;
    public float priceUsd;
    public float baseFeeUsd;
    public float payloadFeeUsd;
    public float urgencyFeeUsd;
    public string preferredSatelliteName;
    public bool isHeld;
}

[Serializable]
public class MessageTemplateRecord
{
    public string templateId;
    public string templateName;
    public string createdAt;
    public string originName;
    public string originLabel;
    public string targetName;
    public string destinationLabel;
    public string messageText;
    public int requestedSendDelayMinutes;
    public string scheduledLabel;
    public string attachmentName;
    public int attachmentSizeKb;
    public List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
    public int messageTextBytes;
    public int payloadKb;
}

public class MissionManager : MonoBehaviour
{
    private const int MaxScheduledDayOffset = 365;
    [Serializable]
    private class ActiveMissionPersistenceState
    {
        public bool hasActiveMission;
        public int phase;
        public string resultMessage;
        public string completionSummary;
        public string originName;
        public string targetName;
        public string satelliteName;
        public string messageText;
        public int requestedSendDelayMinutes;
        public int scheduledDayOffset;
        public int scheduledHour;
        public int scheduledMinute;
        public string scheduledLabel;
        public string attachmentName;
        public int attachmentSizeKb;
        public List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        public int messageTextBytes;
        public int messageSizeKb;
        public bool payloadTooLarge;
        public int communicationStep;
        public float observationProgress;
        public float transferProgress;
        public float interSatelliteProgress;
        public float relaySelectionProgress;
        public float downlinkProgress;
        public float finalReceiveProgress;
        public float sendWaitProgress;
        public float scheduledWaitElapsedSeconds;
        public float plannedWaitSeconds;
        public float transferElapsedSeconds;
        public float plannedTransferSeconds;
        public float plannedDownlinkReadySeconds;
        public float plannedIslSeconds;
        public float plannedRelaySelectionSeconds;
        public float plannedDownlinkSeconds;
        public float plannedFinalReceiveSeconds;
        public float remainingWaitSeconds;
        public float remainingTransferSeconds;
        public int estimatedDeliverySeconds;
        public float estimatedPriceUsd;
        public float priceBaseUsd;
        public float pricePayloadUsd;
        public float priceUrgencyUsd;
        public int successProbabilityPercent;
        public string destinationRelaySatelliteName;
        public bool deliveryRecorded;
        public string activeSentMessageTrackingId;
        public bool reservationAlertLogged;
        public List<string> missionEventLog = new List<string>();
    }

    [Serializable]
    private class MissionPersistenceState
    {
        public int version = 3;
        public List<SentMessageRecord> sentMessages = new List<SentMessageRecord>();
        public List<QueuedMessageRecord> queuedMessages = new List<QueuedMessageRecord>();
        public List<MessageTemplateRecord> messageTemplates = new List<MessageTemplateRecord>();
        public List<string> favoriteTargetNames = new List<string>();
        public List<string> favoriteSatelliteNames = new List<string>();
        public List<string> recentTargetNames = new List<string>();
        public bool autoStartQueuedMessages;
        public int successfulTransmissionCount;
        public int failedTransmissionCount;
        public ActiveMissionPersistenceState activeMission = new ActiveMissionPersistenceState();
        public bool hasDraft;
        public string draftSummary;
        public string draftOriginName;
        public string draftTargetName;
        public int draftDelayMinutes;
        public string draftMessageText;
        public int draftMessageSizeKb;
        public string draftAttachmentName;
        public int draftAttachmentSizeKb;
        public List<MessageAttachmentRecord> draftAttachments = new List<MessageAttachmentRecord>();
    }

    public event Action OnMissionChanged;

    public SatelliteManager satelliteManager;
    public SimulationManager simulationManager;
    public Transform earth;
    public SatelliteOrbit satelliteOrbit;
    public GroundStation groundStation;
    public ConnectionLine connectionLine;

    public readonly List<ObservationTarget> Targets = new List<ObservationTarget>();

    public ObservationTarget SelectedOrigin { get; private set; }
    public ObservationTarget SelectedTarget { get; private set; }
    public MissionPhase Phase { get; private set; }
    public SatelliteCommunicationStep CommunicationStep { get; private set; }
    public float ObservationProgress { get; private set; }
    public float TransferProgress { get; private set; }
    public float InterSatelliteProgress { get; private set; }
    public float RelaySelectionProgress { get; private set; }
    public float DownlinkProgress { get; private set; }
    public float FinalReceiveProgress { get; private set; }
    public bool CommunicationAvailable { get; private set; }
    public string ResultMessage { get; private set; }
    public string CompletionSummary { get; private set; }
    public float EstimatedPassSeconds { get; private set; }
    public float EstimatedDownlinkSeconds { get; private set; }
    public int MissionQualityScore { get; private set; }
    public int ServiceReliabilityScore { get; private set; }
    public int RequestedSendDelayMinutes { get; private set; }
    public int MessageSizeKb { get; private set; }
    public int MessagePayloadBytes { get; private set; }
    public int EstimatedPriceCredits { get; private set; }
    public float EstimatedPriceUsd { get; private set; }
    public int SuccessProbabilityPercent { get; private set; }
    public string MessageText { get; private set; }
    public int MessageTextBytes { get; private set; }
    public string AttachmentName { get; private set; }
    public int AttachmentSizeKb { get; private set; }
    public int AttachmentCount { get; private set; }
    public int TotalAttachmentSizeKb { get; private set; }
    public string AttachmentSummary { get; private set; }
    public bool PayloadTooLarge { get; private set; }
    public int MaxPayloadKb { get { return 512; } }
    public int ScheduledDayOffset { get; private set; }
    public int ScheduledHour { get; private set; }
    public int ScheduledMinute { get; private set; }
    public string ScheduledLabel { get; private set; }
    public float SendWaitProgress { get; private set; }
    public int EstimatedDeliverySeconds { get; private set; }
    public bool AutoStartQueuedMessages { get; private set; }
    public bool SimulateFailureNextSend { get; private set; }
    public bool IsReplayingRoute { get; private set; }
    public float RouteReplayProgress { get; private set; }
    public float AutoQueueCountdown { get; private set; }
    public float RemainingWaitSeconds { get; private set; }
    public float RemainingTransferSeconds { get; private set; }
    public float PriceBaseUsd { get; private set; }
    public float PricePayloadUsd { get; private set; }
    public float PriceUrgencyUsd { get; private set; }
    public string LastFailureReason { get; private set; }
    public string SuggestedRetrySatelliteName { get; private set; }
    public int SuggestedRetryDelayMinutes { get; private set; }
    public string SuggestedRetryHint { get; private set; }
    public string SecurityStatusLabel { get; private set; }
    public string SecurityChecksumLabel { get; private set; }
    public bool HasDraft { get; private set; }
    public string DraftSummary { get; private set; }
    public bool HasExplicitOriginSelection { get; private set; }
    public bool HasExplicitDestinationSelection { get; private set; }
    public bool HasExplicitRouteSelection { get { return HasExplicitOriginSelection && HasExplicitDestinationSelection; } }
    public string OriginName { get { return SelectedOrigin != null ? SelectedOrigin.targetName : OrbitShareDestinations.Seoul; } }
    public string OriginLabel { get { return CityLabel(OriginName); } }
    public string DestinationName { get { return SelectedTarget != null ? SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName; } }
    public string DestinationDisplayLabel { get { return CityLabel(DestinationName); } }
    public string DestinationRelaySatelliteName { get; private set; }
    public string CommunicationStepLabel { get { return CommunicationStepLabelFor(CommunicationStep); } }
    public string CommunicationFlowSummary { get { return BuildCommunicationFlowSummary(); } }
    public Transform DestinationRelayTransform { get { return DestinationRelaySatelliteTransform(); } }
    public bool ShouldUseInterSatelliteLink { get { return UsesInterSatelliteLink(); } }
    public int InterSatelliteHopCount { get { return Mathf.Max(0, interSatelliteRouteNames.Count - 1); } }
    public string InterSatelliteRouteSummary { get { return BuildInterSatelliteRouteSummary(); } }
    public IReadOnlyList<MessageAttachmentRecord> AttachmentFiles { get { return attachmentFiles; } }
    public readonly List<string> FavoriteTargetNames = new List<string>();
    public readonly List<string> FavoriteSatelliteNames = new List<string>();
    public readonly List<string> RecentTargetNames = new List<string>();
    public readonly List<string> MissionEventLog = new List<string>();
    public readonly List<SentMessageRecord> SentMessages = new List<SentMessageRecord>();
    public readonly List<QueuedMessageRecord> QueuedMessages = new List<QueuedMessageRecord>();
    private string activeSentMessageTrackingId = string.Empty;
    public readonly List<MessageTemplateRecord> MessageTemplates = new List<MessageTemplateRecord>();
    public int QueuedMessageCount { get { return QueuedMessages.Count; } }
    public int MessageTemplateCount { get { return MessageTemplates.Count; } }
    public int TotalTransmissionCount { get; private set; }
    public int SuccessfulTransmissionCount { get; private set; }
    public int FailedTransmissionCount { get; private set; }
    public float AverageCostUsd { get; private set; }
    public float AverageDelaySeconds { get; private set; }
    public string MostUsedSatelliteName { get; private set; }
    public float AverageSuccessRatePercent { get; private set; }
    public float AverageSuccessProbabilityPercent { get; private set; }
    public string StatisticsSummary { get; private set; }
    public bool IsRouteReplayPaused { get; private set; }
    public float RouteReplaySpeed { get; private set; }
    public bool IsDemoRouteReplayEnabled { get; private set; }
    public string SelectedSatelliteName
    {
            get
            {
                SatelliteData selectedSatellite = CurrentUplinkSatellite();
                return selectedSatellite != null ? selectedSatellite.satelliteName : string.Empty;
            }
        }

    private const string PersistenceKey = "OrbitShare.MissionManager.State.v1";
    private const float RequiredRelayLockSeconds = 1.2f;
    private const float QueuedMessageBufferPercent = 42f;
    private const int AttachmentNameSummaryMaxChars = 24;
    private const float RouteOriginScoreWeight = 0.45f;
    private const float DemoDefaultTimeScale = 120f;
    private const float DemoMaxTimeScale = 600f;
    private const float EarthRadiusKm = 6371f;
    private const float InterSatelliteRouteRefreshSeconds = 1f;
    private const float ActiveMissionNotifyIntervalSeconds = 0.12f;
    private bool demoAssist;
    private bool suppressPersistenceSave;
    private bool suppressMissionChangedNotification;
    private bool suppressSatelliteChangedNotification;
    private float nextActiveMissionNotifyTime;
    private MissionPhase lastNotifiedPhase = MissionPhase.Idle;
    private SatelliteCommunicationStep lastNotifiedStep = SatelliteCommunicationStep.Draft;
    private string lastNotifiedResultMessage = string.Empty;
    private float scheduledWaitElapsedSeconds;
    private float plannedWaitSeconds;
    private float transferElapsedSeconds;
    private float plannedTransferSeconds = 6f;
    private float plannedDownlinkReadySeconds;
    private float plannedIslSeconds = 3f;
    private float plannedRelaySelectionSeconds = 1.5f;
    private float plannedDownlinkSeconds = 4f;
    private float plannedFinalReceiveSeconds = 0.8f;
    private string plannedDestinationRelaySatelliteName = string.Empty;
    private float plannedIslRouteCost;
    private float nextInterSatelliteRouteRefreshTime;
    private readonly List<string> interSatelliteRouteNames = new List<string>();
    private bool deliveryRecorded;
    private int lastAutoCountdownSecond = -1;
    private bool reservationAlertLogged;
    private readonly List<MessageAttachmentRecord> attachmentFiles = new List<MessageAttachmentRecord>();
    private int successfulTransmissionTotal;
    private string draftTargetName;
    private string draftOriginName;
    private int draftDelayMinutes;
    private string draftMessageText;
    private int draftMessageSizeKb;
    private string draftAttachmentName;
    private int draftAttachmentSizeKb;
    private List<MessageAttachmentRecord> draftAttachments = new List<MessageAttachmentRecord>();

    private void Start()
    {
        suppressPersistenceSave = true;
        MessageText = string.Empty;
        AttachmentName = string.Empty;
        AttachmentSizeKb = 0;
        AttachmentSummary = "첨부 없음";
        RouteReplaySpeed = 1f;
        CommunicationStep = SatelliteCommunicationStep.Draft;
        DestinationRelaySatelliteName = string.Empty;
        MostUsedSatelliteName = "없음";
        StatisticsSummary = "전송 기록 없음";
        attachmentFiles.Clear();
        SetSendDelayMinutes(0);
        UpdatePayloadSize();
        ApplySelectedOriginSilently(OrbitShareDestinations.Seoul);
        if (Targets.Count > 0)
        {
            ApplySelectedTargetSilently(Targets.Find(target => target.targetName == OrbitShareDestinations.DefaultTargetName) != null
                ? OrbitShareDestinations.DefaultTargetName
                : Targets[0].targetName);
        }
        HasExplicitOriginSelection = false;
        HasExplicitDestinationSelection = false;
        LastFailureReason = string.Empty;
        SuggestedRetryHint = string.Empty;
        SecurityStatusLabel = "보안 확인 전";
        SecurityChecksumLabel = "CHK-0000";
        AddMissionEvent("요청 작성 시작");
        ResultMessage = "시간, 내용, 발신지/목적지를 순서대로 선택하세요.";
        CompletionSummary = "전송이 끝나면 결과가 표시됩니다.";
        suppressPersistenceSave = false;
        LoadPersistentState();
        NotifyChanged();
    }

    private void Update()
    {
        if (earth == null || CurrentSatelliteTransform() == null)
        {
            return;
        }

        UpdateCommunicationAvailability();
        UpdateMissionEstimate();

        if (Phase == MissionPhase.Running || Phase == MissionPhase.Observing)
        {
            UpdateObservation();
        }
        else if (Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting)
        {
            UpdateTransmission();
        }

        UpdateRouteReplay();
        UpdateAutoQueue();
    }

    private void OnApplicationQuit()
    {
        SavePersistentState();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePersistentState();
        }
    }

    public void SelectOrigin(string originName)
    {
        if (Phase != MissionPhase.Idle)
        {
            ResultMessage = "전송 중에는 발신지를 바꿀 수 없습니다.";
            NotifyChanged();
            return;
        }

        if (!ApplySelectedOriginSilently(originName))
        {
            ResultMessage = "발신지를 선택하세요.";
            NotifyChanged();
            return;
        }

        HasExplicitOriginSelection = true;
        RefreshTargetSelectionVisuals();
        SelectRecommendedSatelliteIfNeeded();
        ResultMessage = "발신지 " + OriginLabel + " 선택됨 · 위성망 추천을 다시 계산했습니다.";
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SelectTarget(string targetName)
    {
        if (IsActiveMissionPhase(Phase))
        {
            ResultMessage = "전송 중에는 목적지를 바꿀 수 없습니다.";
            NotifyChanged();
            return;
        }

        ObservationTarget selected = Targets.Find(target => target.targetName == targetName);
        if (selected == null)
        {
            return;
        }

        SelectedTarget = selected;
        HasExplicitDestinationSelection = true;
        RefreshTargetSelectionVisuals();

        AddRecentTarget(targetName);
        SelectRecommendedSatelliteIfNeeded();
        if (Phase == MissionPhase.Idle)
        {
            ResultMessage = "목적지 " + DestinationLabel(SelectedTarget.targetName) + " 선택됨 · 위성을 다시 추천했습니다.";
        }

        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SetSendDelayMinutes(int minutes)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        RequestedSendDelayMinutes = Mathf.Max(0, minutes);
        System.DateTime scheduled = System.DateTime.Now.AddMinutes(RequestedSendDelayMinutes);
        ScheduledDayOffset = Mathf.Clamp((scheduled.Date - System.DateTime.Now.Date).Days, 0, MaxScheduledDayOffset);
        ScheduledHour = scheduled.Hour;
        ScheduledMinute = scheduled.Minute - scheduled.Minute % 5;
        ScheduledLabel = RequestedSendDelayMinutes == 0 ? "지금 보내기" : scheduled.ToString("MM/dd HH:mm") + " 보내기";
        SelectRecommendedSatelliteIfNeeded();
        ResultMessage = RequestedSendDelayMinutes == 0
            ? "지금 보낼 수 있는 위성을 추천했습니다."
            : "보낼 시간 " + ScheduledLabel + " 기준으로 위성을 다시 추천했습니다.";
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SetScheduledDateTime(int dayOffset, int hour, int minute)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        System.DateTime now = System.DateTime.Now;
        System.DateTime scheduled = now.Date.AddDays(Mathf.Clamp(dayOffset, 0, MaxScheduledDayOffset)).AddHours(Mathf.Clamp(hour, 0, 23)).AddMinutes(Mathf.Clamp(minute, 0, 59));
        int delayMinutes = Mathf.Max(0, Mathf.CeilToInt((float)(scheduled - now).TotalMinutes));
        if (delayMinutes == 0)
        {
            scheduled = now;
        }
        RequestedSendDelayMinutes = delayMinutes;
        ScheduledDayOffset = Mathf.Clamp((scheduled.Date - now.Date).Days, 0, MaxScheduledDayOffset);
        ScheduledHour = scheduled.Hour;
        ScheduledMinute = scheduled.Minute - scheduled.Minute % 5;
        ScheduledLabel = delayMinutes == 0 ? "지금 보내기" : scheduled.ToString("MM/dd HH:mm") + " 보내기";
        SelectRecommendedSatelliteIfNeeded();
        ResultMessage = delayMinutes == 0
            ? "지난 시간이라 지금 보내기로 설정했습니다."
            : ScheduledLabel + " 기준으로 위성을 다시 추천했습니다.";
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SetMessageSizeKb(int sizeKb)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        MessageSizeKb = Mathf.Max(1, sizeKb);
        MessagePayloadBytes = KilobytesToBytes(MessageSizeKb);
        PayloadTooLarge = MessageSizeKb > MaxPayloadKb;
        ResultMessage = PayloadTooLarge
            ? "크기가 " + PayloadKbText(MaxPayloadKb) + "를 넘었습니다."
            : "크기를 다시 계산했습니다.";
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SetMessageText(string text)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        MessageText = text ?? string.Empty;
        UpdatePayloadSize();
        ResultMessage = PayloadTooLarge
            ? "크기가 너무 큽니다. 메시지를 줄이거나 파일을 제거하세요."
            : "메시지 크기 " + FormatBytes(MessageTextBytes) + " · 총 " + PayloadKbText(MessageSizeKb);
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SetAttachment(string fileName, int sizeKb)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        attachmentFiles.Clear();
        if (!string.IsNullOrEmpty(fileName) && sizeKb > 0)
        {
            attachmentFiles.Add(new MessageAttachmentRecord(fileName, sizeKb));
        }
        UpdateAttachmentSummary();
        UpdatePayloadSize();
        ResultMessage = PayloadTooLarge
            ? "파일을 포함한 크기가 " + PayloadKbText(MaxPayloadKb) + "를 넘었습니다."
            : (AttachmentSizeKb > 0 ? AttachmentSummary + " 첨부 완료 · 총 " + PayloadKbText(MessageSizeKb) : "파일을 제거했습니다.");
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void ClearAttachment()
    {
        SetAttachment(string.Empty, 0);
    }

    public void SetAttachments(List<MessageAttachmentRecord> attachments)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        SetAttachmentsInternal(attachments);
        ResultMessage = PayloadTooLarge
            ? "파일을 포함한 크기가 " + PayloadKbText(MaxPayloadKb) + "를 넘었습니다."
            : (AttachmentCount > 0 ? AttachmentSummary + " 첨부 완료 · 총 " + PayloadKbText(MessageSizeKb) : "파일을 제거했습니다.");
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void AddAttachment(string fileName, int sizeKb)
    {
        AddAttachment(fileName, sizeKb, 0);
    }

    public void AddAttachment(string fileName, int sizeKb, long sizeBytes)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }
        if (string.IsNullOrEmpty(fileName) || sizeKb <= 0)
        {
            ResultMessage = "추가할 파일을 선택하세요.";
            NotifyChanged();
            return;
        }

        attachmentFiles.Add(new MessageAttachmentRecord(fileName, sizeKb, sizeBytes));
        UpdateAttachmentSummary();
        UpdatePayloadSize();
        ResultMessage = PayloadTooLarge
            ? "파일을 포함한 크기가 " + PayloadKbText(MaxPayloadKb) + "를 넘었습니다."
            : CompactAttachmentFileName(fileName) + " 첨부 완료 · " + AttachmentSummary + " · 총 " + PayloadKbText(MessageSizeKb);
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void RemoveAttachmentAt(int index)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }
        if (index < 0 || index >= attachmentFiles.Count)
        {
            return;
        }

        attachmentFiles.RemoveAt(index);
        UpdateAttachmentSummary();
        UpdatePayloadSize();
        ResultMessage = AttachmentCount > 0 ? "첨부를 삭제했습니다. · " + AttachmentSummary + " · 총 " + PayloadKbText(MessageSizeKb) : "파일을 제거했습니다.";
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public string GetAttachmentSummary()
    {
        return AttachmentSummary;
    }

    public string GetAttachmentSummary(List<MessageAttachmentRecord> attachments)
    {
        return AttachmentSummaryText(attachments, string.Empty, 0);
    }

    public int GetAttachmentTotalSizeKb(List<MessageAttachmentRecord> attachments)
    {
        return TotalAttachmentSize(attachments);
    }

    public int GetAttachmentTotalSizeBytes(List<MessageAttachmentRecord> attachments)
    {
        return TotalAttachmentBytes(attachments);
    }

    public int CalculateMessageTextBytes(string text)
    {
        return GetUtf8ByteCount(text);
    }

    public int CalculateMessagePayloadBytes(string text, List<MessageAttachmentRecord> attachments)
    {
        return CalculatePayloadBytes(GetUtf8ByteCount(text), attachments);
    }

    public int CalculateMessageSizeKb(string text, List<MessageAttachmentRecord> attachments)
    {
        return CalculatePayloadKbFromBytes(CalculateMessagePayloadBytes(text, attachments));
    }

    public int CalculateMessageSizeKb(string text, string attachmentName, int attachmentSizeKb)
    {
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        if (attachmentSizeKb > 0)
        {
            attachments.Add(new MessageAttachmentRecord(string.IsNullOrEmpty(attachmentName) ? "첨부 파일" : attachmentName, attachmentSizeKb));
        }
        return CalculateMessageSizeKb(text, attachments);
    }

    public void RecalculateMessageSize()
    {
        UpdateAttachmentSummary();
        UpdatePayloadSize();
        ResultMessage = PayloadTooLarge
            ? "크기 초과: " + PayloadKbText(MessageSizeKb) + " / " + PayloadKbText(MaxPayloadKb)
            : "메시지 크기 " + FormatBytes(MessageTextBytes) + " + 첨부 " + PayloadKbText(TotalAttachmentSizeKb) + " = " + PayloadKbText(MessageSizeKb);
        UpdateMissionEstimate();
        SaveDraftSnapshot();
        NotifyChanged();
    }

    public void SaveDraft()
    {
        SaveDraftSnapshot();
        ResultMessage = HasDraft ? "임시 저장 완료" : "저장할 내용이 없습니다.";
        NotifyChanged();
    }

    public bool RestoreDraft()
    {
        if (!HasDraft)
        {
            ResultMessage = "불러올 임시 저장이 없습니다.";
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle && Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            ResultMessage = "전송 중에는 임시 저장을 불러올 수 없습니다.";
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        if (!string.IsNullOrEmpty(draftTargetName))
        {
            ApplyExplicitOriginSelection(string.IsNullOrEmpty(draftOriginName) ? OrbitShareDestinations.Seoul : draftOriginName);
            SelectTarget(draftTargetName);
        }
        SetSendDelayMinutes(draftDelayMinutes);
        SetMessageText(draftMessageText);
        if (draftAttachments != null && draftAttachments.Count > 0)
        {
            SetAttachments(draftAttachments);
        }
        else
        {
            SetAttachment(draftAttachmentName, draftAttachmentSizeKb);
        }
        if (draftMessageSizeKb > 0)
        {
            SetMessageSizeKb(draftMessageSizeKb);
        }
        ResultMessage = "임시 저장을 불러왔습니다.";
        NotifyChanged();
        return true;
    }

    public void ToggleFavoriteTarget(string targetName)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }
        if (FavoriteTargetNames.Contains(targetName))
        {
            FavoriteTargetNames.Remove(targetName);
            ResultMessage = DestinationLabel(targetName) + " 즐겨찾기를 해제했습니다.";
        }
        else
        {
            FavoriteTargetNames.Insert(0, targetName);
            ResultMessage = DestinationLabel(targetName) + " 즐겨찾기에 추가했습니다.";
        }
        SavePersistentState();
        NotifyChanged();
    }

    public bool IsFavoriteTarget(string targetName)
    {
        return FavoriteTargetNames.Contains(targetName);
    }

    public void ToggleFavoriteSatellite(string satelliteName)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return;
        }

        if (FavoriteSatelliteNames.Contains(satelliteName))
        {
            FavoriteSatelliteNames.Remove(satelliteName);
            ResultMessage = satelliteName + " 즐겨찾기를 해제했습니다.";
        }
        else
        {
            FavoriteSatelliteNames.Insert(0, satelliteName);
            ResultMessage = satelliteName + " 즐겨찾기에 추가했습니다.";
        }
        SavePersistentState();
        NotifyChanged();
    }

    public bool IsFavoriteSatellite(string satelliteName)
    {
        return FavoriteSatelliteNames.Contains(satelliteName);
    }

    public bool QueueMessage(string messageText, string targetName, string attachmentName, int attachmentSizeKb, out string errorMessage)
    {
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        if (attachmentSizeKb > 0)
        {
            attachments.Add(new MessageAttachmentRecord(string.IsNullOrEmpty(attachmentName) ? "첨부 파일" : attachmentName, attachmentSizeKb));
        }
        return QueueMessage(messageText, targetName, attachments, out errorMessage);
    }

    public bool QueueMessage(string messageText, string targetName, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        return QueueMessage(messageText, targetName, 0, attachments, out errorMessage);
    }

    public bool QueueMessage(string messageText, string targetName, int requestedSendDelayMinutes, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        QueuedMessageRecord record;
        if (!TryBuildQueuedMessageRecord(messageText, targetName, attachments, requestedSendDelayMinutes, out record, out errorMessage))
        {
            return false;
        }
        QueuedMessages.Add(record);
        ResultMessage = BuildQueuedMessageAddedStatus(record);
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool QueueMessage(string messageText, string targetName, int scheduledDayOffset, int scheduledHour, int scheduledMinute, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        QueuedMessageRecord record;
        if (!TryBuildQueuedMessageRecord(messageText, targetName, attachments, scheduledDayOffset, scheduledHour, scheduledMinute, out record, out errorMessage))
        {
            return false;
        }
        QueuedMessages.Add(record);
        ResultMessage = BuildQueuedMessageAddedStatus(record);
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool QueueMessageFromOrigin(string messageText, string originName, string targetName, int scheduledDayOffset, int scheduledHour, int scheduledMinute, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        QueuedMessageRecord record;
        if (!TryBuildQueuedMessageRecord(messageText, originName, targetName, attachments, scheduledDayOffset, scheduledHour, scheduledMinute, out record, out errorMessage))
        {
            return false;
        }
        QueuedMessages.Add(record);
        ResultMessage = BuildQueuedMessageAddedStatus(record);
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool StartNextQueuedMessage()
    {
        if (QueuedMessages.Count == 0)
        {
            ResultMessage = "대기 중인 메시지가 없습니다.";
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle && Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            ResultMessage = "현재 전송이 끝난 뒤 시작할 수 있습니다.";
            NotifyChanged();
            return false;
        }

        int startIndex = FindNextStartableQueuedMessageIndex();
        if (startIndex < 0)
        {
            ResultMessage = "시작 가능한 메시지가 없습니다.";
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord queued = QueuedMessages[startIndex];
        if (Targets.Find(item => item.targetName == queued.targetName) == null)
        {
            ResultMessage = "메시지의 목적지를 확인할 수 없습니다.";
            NotifyChanged();
            return false;
        }
        if (!string.IsNullOrEmpty(queued.originName) && Targets.Find(item => item.targetName == queued.originName) == null)
        {
            ResultMessage = "메시지의 발신지를 확인할 수 없습니다.";
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        ApplyExplicitOriginSelection(string.IsNullOrEmpty(queued.originName) ? OrbitShareDestinations.Seoul : queued.originName);
        SelectTarget(queued.targetName);
        SetMessageText(queued.messageText);
        ApplyQueuedAttachments(queued);
        ApplyQueuedSchedule(queued);

        SatelliteData bestSatellite = FindRecommendedSatelliteForQueuedMessage(startIndex);
        if (bestSatellite != null && satelliteManager != null)
        {
            satelliteManager.SelectSatellite(bestSatellite.satelliteName);
        }

        bool canStart = satelliteManager != null
            && SelectedTarget != null
            && CanServeRoute(satelliteManager.SelectedSatellite, OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb);
        if (!canStart)
        {
            ResultMessage = "메시지에 맞는 위성을 찾지 못했습니다.";
            NotifyChanged();
            return false;
        }

        QueuedMessages.RemoveAt(startIndex);
        SavePersistentState();
        ReserveMission();
        StartMission();
        return Phase != MissionPhase.Idle;
    }

    public void SetAutoStartQueuedMessages(bool enabled)
    {
        AutoStartQueuedMessages = enabled;
        AutoQueueCountdown = -1f;
        lastAutoCountdownSecond = -1;
        ResultMessage = enabled ? "메시지 자동 전송을 켰습니다." : "메시지 자동 전송을 껐습니다.";
        SavePersistentState();
        NotifyChanged();
    }

    public void SetSimulateFailureNextSend(bool enabled)
    {
        SimulateFailureNextSend = enabled;
        ResultMessage = enabled ? "실패 테스트를 켰습니다." : "실패 테스트를 껐습니다.";
        NotifyChanged();
    }

    public void MoveQueuedMessage(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= QueuedMessages.Count || toIndex < 0 || toIndex >= QueuedMessages.Count || fromIndex == toIndex)
        {
            return;
        }

        QueuedMessageRecord item = QueuedMessages[fromIndex];
        QueuedMessages.RemoveAt(fromIndex);
        QueuedMessages.Insert(toIndex, item);
        ResultMessage = "메시지 목록 순서를 바꿨습니다.";
        SavePersistentState();
        NotifyChanged();
    }

    public void RemoveQueuedMessage(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            return;
        }

        QueuedMessages.RemoveAt(index);
        ResultMessage = "메시지를 삭제했습니다.";
        SavePersistentState();
        NotifyChanged();
    }

    public bool CancelQueuedMessage(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            ResultMessage = "삭제할 대기 메시지가 없습니다.";
            NotifyChanged();
            return false;
        }

        QueuedMessages.RemoveAt(index);
        ResultMessage = "대기 메시지를 삭제했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool ToggleQueuedMessageHold(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            ResultMessage = "대기 메시지가 없습니다.";
            NotifyChanged();
            return false;
        }

        return SetQueuedMessageHold(index, false);
    }

    public bool SetQueuedMessageHold(int index, bool isHeld)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            ResultMessage = "대기 메시지가 없습니다.";
            NotifyChanged();
            return false;
        }

        QueuedMessages[index].isHeld = false;
        ResultMessage = "대기 메시지는 계속 전송 대기 상태입니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool UpdateQueuedMessage(int index, string messageText, string targetName, string attachmentName, int attachmentSizeKb, out string errorMessage)
    {
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        if (attachmentSizeKb > 0)
        {
            attachments.Add(new MessageAttachmentRecord(string.IsNullOrEmpty(attachmentName) ? "첨부 파일" : attachmentName, attachmentSizeKb));
        }
        return UpdateQueuedMessage(index, messageText, targetName, attachments, out errorMessage);
    }

    public bool UpdateQueuedMessage(int index, string messageText, string targetName, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        return UpdateQueuedMessage(index, messageText, targetName, 0, attachments, out errorMessage);
    }

    public bool UpdateQueuedMessage(int index, string messageText, string targetName, int requestedSendDelayMinutes, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= QueuedMessages.Count)
        {
            errorMessage = "수정할 메시지가 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord updated;
        if (!TryBuildQueuedMessageRecord(messageText, targetName, attachments, requestedSendDelayMinutes, out updated, out errorMessage))
        {
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord previous = QueuedMessages[index];
        updated.isHeld = false;
        updated.preferredSatelliteName = previous.preferredSatelliteName;
        QueuedMessages[index] = updated;
        ResultMessage = "메시지를 수정했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool UpdateQueuedMessage(int index, string messageText, string targetName, int scheduledDayOffset, int scheduledHour, int scheduledMinute, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= QueuedMessages.Count)
        {
            errorMessage = "수정할 메시지가 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord updated;
        if (!TryBuildQueuedMessageRecord(messageText, targetName, attachments, scheduledDayOffset, scheduledHour, scheduledMinute, out updated, out errorMessage))
        {
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        QueuedMessageRecord previous = QueuedMessages[index];
        updated.isHeld = false;
        updated.preferredSatelliteName = previous.preferredSatelliteName;
        QueuedMessages[index] = updated;
        ResultMessage = "메시지를 수정했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool UpdateQueuedMessageFromOrigin(int index, string messageText, string originName, string targetName, int scheduledDayOffset, int scheduledHour, int scheduledMinute, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= QueuedMessages.Count)
        {
            errorMessage = "수정할 메시지가 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord updated;
        if (!TryBuildQueuedMessageRecord(messageText, originName, targetName, attachments, scheduledDayOffset, scheduledHour, scheduledMinute, out updated, out errorMessage))
        {
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        QueuedMessageRecord previous = QueuedMessages[index];
        updated.isHeld = false;
        updated.preferredSatelliteName = previous.preferredSatelliteName;
        QueuedMessages[index] = updated;
        ResultMessage = "메시지를 수정했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool LoadQueuedMessageForEditing(int index, out string errorMessage)
    {
        return LoadQueuedMessageForEditing(index, false, out errorMessage);
    }

    public bool LoadQueuedMessageForEditing(int index, bool removeFromQueue, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= QueuedMessages.Count)
        {
            errorMessage = "불러올 메시지가 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle && Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            errorMessage = "전송 중에는 메시지를 작성 화면으로 불러올 수 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        if (Targets.Find(item => item.targetName == queued.targetName) == null)
        {
            errorMessage = "메시지의 목적지를 확인할 수 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        ApplyExplicitOriginSelection(string.IsNullOrEmpty(queued.originName) ? OrbitShareDestinations.Seoul : queued.originName);
        SelectTarget(queued.targetName);
        SetMessageText(queued.messageText);
        ApplyQueuedAttachments(queued);
        ApplyQueuedSchedule(queued);
        if (removeFromQueue)
        {
            QueuedMessages.RemoveAt(index);
        }
        ResultMessage = removeFromQueue ? "메시지를 작성 화면으로 옮겼습니다." : "메시지를 작성 화면으로 불러왔습니다.";
        SavePersistentState();
        UpdateMissionEstimate();
        NotifyChanged();
        return true;
    }

    public bool CanEditQueuedMessage(int index)
    {
        return IsValidQueuedMessageIndex(index);
    }

    public bool CanLoadQueuedMessageForEditing(int index)
    {
        return IsValidQueuedMessageIndex(index) && IsQueueActionPhaseReady();
    }

    public bool CanCancelQueuedMessage(int index)
    {
        return IsValidQueuedMessageIndex(index);
    }

    public bool CanStartQueuedMessageNow(int index)
    {
        string blockReason;
        return CanStartQueuedMessageNow(index, out blockReason);
    }

    public bool CanStartQueuedMessageNow(int index, out string blockReason)
    {
        return TryGetQueuedMessageStartNowBlockReason(index, out blockReason);
    }

    public string GetQueuedMessageActionSummary(int index)
    {
        if (!IsValidQueuedMessageIndex(index))
        {
            return string.Empty;
        }

        string startBlockReason;
        bool canStartNow = CanStartQueuedMessageNow(index, out startBlockReason);
        return (CanEditQueuedMessage(index) ? "수정 가능" : "수정 불가")
            + " · " + (CanCancelQueuedMessage(index) ? "삭제 가능" : "삭제 불가")
            + " · " + (canStartNow ? "바로 보내기 가능" : "바로 보내기 불가");
    }

    public string GetQueuedMessageStartNowSummary(int index)
    {
        string blockReason;
        if (CanStartQueuedMessageNow(index, out blockReason))
        {
            return "바로 보내기 가능";
        }
        return string.IsNullOrEmpty(blockReason) ? "바로 보내기 불가" : "바로 보내기 불가: " + blockReason;
    }

    public bool StartQueuedMessageNow(int index)
    {
        string errorMessage;
        return StartQueuedMessageNow(index, out errorMessage);
    }

    public bool StartQueuedMessageNow(int index, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (!CanStartQueuedMessageNow(index, out errorMessage))
        {
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        ApplyExplicitOriginSelection(string.IsNullOrEmpty(queued.originName) ? OrbitShareDestinations.Seoul : queued.originName);
        SelectTarget(queued.targetName);
        SetMessageText(queued.messageText);
        ApplyQueuedAttachments(queued);
        SetSendDelayMinutes(0);

        SatelliteData bestSatellite = FindPreferredOrBestSatelliteForQueuedMessage(queued, 0);
        if (bestSatellite != null && satelliteManager != null)
        {
            satelliteManager.SelectSatellite(bestSatellite.satelliteName);
        }

        if (satelliteManager == null || SelectedTarget == null || !CanServeRoute(satelliteManager.SelectedSatellite, OriginName, SelectedTarget.targetName, 0, MessageSizeKb))
        {
            errorMessage = "지금 바로 보낼 수 있는 위성을 찾지 못했습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        ReserveMission();
        if (Phase != MissionPhase.Reserved)
        {
            errorMessage = string.IsNullOrEmpty(ResultMessage) ? "바로 보내기를 시작하지 못했습니다." : ResultMessage;
            NotifyChanged();
            return false;
        }

        QueuedMessages.RemoveAt(index);
        SavePersistentState();
        StartMission();
        if (Phase == MissionPhase.Reserved)
        {
            errorMessage = string.IsNullOrEmpty(ResultMessage) ? "바로 보내기를 시작하지 못했습니다." : ResultMessage;
            NotifyChanged();
            return false;
        }
        return Phase != MissionPhase.Idle;
    }

    public string GetQueuedMessageSummary(int index)
    {
        return GetQueuedMessageStatusSummary(index);
    }

    public string GetQueuedMessageStatusSummary(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            return string.Empty;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        QueuedMessageRouteDetail detail = GetQueuedMessageRouteDetail(index);
        if (detail == null)
        {
            return BuildQueuedMessageInlineSummary(queued)
                + " · 통신 계산 전"
                + " · " + GetQueuedMessageActionSummary(index);
        }
        if (!detail.isAvailable)
        {
            return BuildQueuedMessageInlineSummary(queued)
                + " · 통신 불가"
                + " · 요금 계산 불가"
                + " · " + GetQueuedMessageActionSummary(index);
        }

        string route = detail.satelliteName + " · 예상 소요 시간 " + detail.estimatedDeliveryLabel;
        return BuildQueuedMessageInlineSummary(queued)
            + " · " + route
            + " · $" + detail.priceUsd.ToString("0.00")
            + " · " + GetQueuedMessageActionSummary(index);
    }

    public string GetQueueSummary()
    {
        if (QueuedMessages.Count == 0)
        {
            return "메시지 없음";
        }

        return "대기 " + QueuedMessages.Count + "건";
    }

    public List<int> GetQueuedMessageTimelineIndices()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < QueuedMessages.Count; i++)
        {
            indices.Add(i);
        }

        return indices;
    }

    public List<string> GetQueuedMessageTimelineSummaries()
    {
        List<string> summaries = new List<string>();
        List<int> indices = GetQueuedMessageTimelineIndices();
        for (int i = 0; i < indices.Count; i++)
        {
            summaries.Add(GetQueuedMessageSummary(indices[i]));
        }
        return summaries;
    }

    public string GetQueuedMessageTimelineDetail(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            return string.Empty;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        QueuedMessageRouteDetail detail = GetQueuedMessageRouteDetail(index);
        string route = detail != null ? detail.routeSummary : "통신 계산 전";
        string origin = string.IsNullOrEmpty(queued.originLabel) ? DestinationLabel(queued.originName) : queued.originLabel;
        string destination = string.IsNullOrEmpty(queued.destinationLabel) ? DestinationLabel(queued.targetName) : queued.destinationLabel;
        return origin + " → " + destination
            + " · " + AttachmentSummaryText(queued.attachments, queued.attachmentName, queued.attachmentSizeKb)
            + " · " + route;
    }

    public bool HasStartableQueuedMessage()
    {
        return FindNextStartableQueuedMessageIndex() >= 0;
    }

    public bool CanStartNextQueuedMessage()
    {
        return HasStartableQueuedMessage() && IsQueueActionPhaseReady();
    }

    public string GetCompletionNextActionSummary()
    {
        if (Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            return "전송 진행 중";
        }
        if (HasStartableQueuedMessage())
        {
            return "다음 메시지 전송 가능";
        }
        if (QueuedMessages.Count > 0)
        {
            return "대기 메시지가 있습니다";
        }
        return "새 메시지 작성 가능";
    }

    public List<SatelliteRecommendationRecord> GetDestinationRecommendations(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null)
        {
            return new List<SatelliteRecommendationRecord>();
        }
        return satelliteManager.GetDestinationRecommendations(OriginName, targetName, requestedDelayMinutes, payloadKb);
    }

    public List<SatelliteRecommendationRecord> GetDestinationRecommendations(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null)
        {
            return new List<SatelliteRecommendationRecord>();
        }
        return satelliteManager.GetDestinationRecommendations(originName, targetName, requestedDelayMinutes, payloadKb);
    }

    public List<SatelliteRecommendationRecord> GetCurrentDestinationRecommendations()
    {
        string targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        return GetDestinationRecommendations(targetName, RequestedSendDelayMinutes, MessageSizeKb);
    }

    public string GetDestinationRecommendationSummary(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null)
        {
            return "추천 가능한 위성이 없습니다.";
        }
        return satelliteManager.GetDestinationRecommendationSummary(OriginName, targetName, requestedDelayMinutes, payloadKb);
    }

    public string GetDestinationRecommendationShortSummary(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null)
        {
            return "추천 위성 없음";
        }
        SatelliteData routeAwareSatellite = FindBestSatelliteForRoute(OriginName, targetName, requestedDelayMinutes, payloadKb);
        if (routeAwareSatellite != null)
        {
            return BuildRouteRecommendationReason(routeAwareSatellite, OriginName, targetName, requestedDelayMinutes, payloadKb);
        }
        return satelliteManager.GetDestinationRecommendationShortSummary(targetName, requestedDelayMinutes, payloadKb);
    }

    public string GetCurrentDestinationRecommendationShortSummary()
    {
        string targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        return GetDestinationRecommendationShortSummary(targetName, RequestedSendDelayMinutes, MessageSizeKb);
    }

    public string GetSelectedSatelliteRecommendationReason()
    {
        string targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        return GetSelectedSatelliteRecommendationReason(targetName, RequestedSendDelayMinutes, MessageSizeKb);
    }

    public string GetSelectedSatelliteRecommendationReason(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null)
        {
            return "선택 위성 없음";
        }
        if (CanServeRoute(satelliteManager.SelectedSatellite, OriginName, targetName, requestedDelayMinutes, payloadKb))
        {
            return BuildRouteRecommendationReason(satelliteManager.SelectedSatellite, OriginName, targetName, requestedDelayMinutes, payloadKb);
        }
        string baseReason = satelliteManager.GetSelectedSatelliteRecommendationReason(OriginName, targetName, requestedDelayMinutes, payloadKb);
        return baseReason + " · 발신지 " + OriginLabel + " 업링크 확인 필요";
    }

    public string GetCurrentScheduleValiditySummary()
    {
        return GetScheduleValiditySummary(ScheduledDayOffset, ScheduledHour, ScheduledMinute);
    }

    public string GetScheduleValiditySummary(int dayOffset, int hour, int minute)
    {
        DateTime now = DateTime.Now;
        DateTime scheduled = now.Date.AddDays(Mathf.Clamp(dayOffset, 0, MaxScheduledDayOffset)).AddHours(Mathf.Clamp(hour, 0, 23)).AddMinutes(Mathf.Clamp(minute, 0, 59));
        int delayMinutes = Mathf.Max(0, Mathf.CeilToInt((float)(scheduled - now).TotalMinutes));
        if (delayMinutes == 0)
        {
            return "선택한 시간이 현재보다 빠르거나 같아 즉시 전송으로 처리됩니다.";
        }
        return scheduled.ToString("MM/dd HH:mm") + " 예약 전송 · 현재 기준 약 " + delayMinutes + "분 뒤";
    }

    public List<string> GetTransmissionStepSummaries()
    {
        return BuildTransmissionStepSummaries(MissionEventLog);
    }

    public string GetTransmissionStepSummary()
    {
        List<string> summaries = GetTransmissionStepSummaries();
        return summaries.Count == 0 ? "전송 단계 기록 없음" : string.Join(" / ", summaries.ToArray());
    }

    public List<string> GetSentMessageTransmissionStepSummaries(int index)
    {
        if (index < 0 || index >= SentMessages.Count || SentMessages[index] == null)
        {
            return new List<string>();
        }
        return BuildTransmissionStepSummaries(SentMessages[index].transmissionLog);
    }

    public string GetSentMessageTransmissionStepSummary(int index)
    {
        List<string> summaries = GetSentMessageTransmissionStepSummaries(index);
        return summaries.Count == 0 ? "전송 단계 기록 없음" : string.Join(" / ", summaries.ToArray());
    }

    public string GetSentMessageTrackingId(int index)
    {
        if (index < 0 || index >= SentMessages.Count)
        {
            return string.Empty;
        }
        return SentMessageTrackingId(SentMessages[index]);
    }

    public string GetSentMessageSummary(int index)
    {
        if (index < 0 || index >= SentMessages.Count || SentMessages[index] == null)
        {
            return string.Empty;
        }

        SentMessageRecord record = SentMessages[index];
        string destination = string.IsNullOrEmpty(record.destinationLabel) ? "수신 지역" : record.destinationLabel;
        string satelliteName = string.IsNullOrEmpty(record.satelliteName) ? "선택 위성" : record.satelliteName;
        return "전송 기록 " + SentMessageTrackingId(record)
            + " · " + destination
            + " · " + record.deliveredAt
            + " · " + satelliteName
            + " · " + FormatRemainingSeconds(record.elapsedSeconds)
            + " · $" + record.priceUsd.ToString("0.00");
    }

    public string GetTransmissionRecordSummary(int index)
    {
        return GetSentMessageSummary(index);
    }

    public string GetLatestTransmissionResultSummary()
    {
        if (SentMessages.Count == 0)
        {
            return Phase == MissionPhase.Complete ? CompletionSummary : "전송 기록 없음";
        }
        return GetSentMessageSummary(0);
    }

    public List<DestinationArrivalComparisonRecord> GetDestinationArrivalComparisons(int payloadKb)
    {
        List<DestinationArrivalComparisonRecord> comparisons = new List<DestinationArrivalComparisonRecord>();
        for (int i = 0; i < Targets.Count; i++)
        {
            ObservationTarget target = Targets[i];
            if (target == null)
            {
                continue;
            }

            int availableCount = CountReachableSatellitesForRoute(target.targetName, RequestedSendDelayMinutes, payloadKb);
            SatelliteData recommended = FindBestSatelliteForRoute(OriginName, target.targetName, RequestedSendDelayMinutes, payloadKb);
            SatelliteRouteEstimateRecord routeEstimate = recommended != null && satelliteManager != null
                ? satelliteManager.GetRouteEstimate(recommended, OriginName, target.targetName, RequestedSendDelayMinutes, payloadKb)
                : null;
            float passSeconds = EstimatePassSecondsToTarget(target, recommended, RequestedSendDelayMinutes, payloadKb);
            float downlinkSeconds = EstimateDownlinkSecondsToGroundStation();
            int arrivalSeconds = recommended == null
                ? 0
                : routeEstimate != null
                    ? Mathf.Max(0, routeEstimate.estimatedDeliverySeconds)
                    : Mathf.CeilToInt(RequestedSendDelayMinutes * 60f + passSeconds + downlinkSeconds + EstimateTransferSeconds(payloadKb, downlinkSeconds, recommended));

            DestinationArrivalComparisonRecord record = new DestinationArrivalComparisonRecord();
            record.targetName = target.targetName;
            record.destinationLabel = DestinationLabel(target.targetName);
            record.availableSatelliteCount = availableCount;
            record.recommendedSatelliteName = recommended != null ? recommended.satelliteName : string.Empty;
            record.estimatedArrivalSeconds = arrivalSeconds;
            record.estimatedArrivalLabel = recommended != null ? FormatRemainingSeconds(arrivalSeconds) : "계산 불가";
            record.summary = record.destinationLabel + " · 가능 " + record.availableSatelliteCount + "대 · "
                + (recommended != null ? "추천 " + record.recommendedSatelliteName + " · " + record.estimatedArrivalLabel : "추천 위성 없음");
            comparisons.Add(record);
        }
        return comparisons;
    }

    private int CountReachableSatellitesForRoute(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null || string.IsNullOrEmpty(targetName))
        {
            return 0;
        }

        List<SatelliteRouteEstimateRecord> estimates = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.Recommended, OriginName, targetName, requestedDelayMinutes, payloadKb);
        int count = 0;
        for (int i = 0; i < estimates.Count; i++)
        {
            if (estimates[i] != null && estimates[i].canServe)
            {
                count++;
            }
        }
        return count;
    }

    public List<string> GetDestinationArrivalComparisonSummaries(int payloadKb)
    {
        List<string> summaries = new List<string>();
        List<DestinationArrivalComparisonRecord> comparisons = GetDestinationArrivalComparisons(payloadKb);
        for (int i = 0; i < comparisons.Count; i++)
        {
            summaries.Add(comparisons[i].summary);
        }
        return summaries;
    }

    public string GetDestinationArrivalComparisonSummary(int payloadKb)
    {
        List<string> summaries = GetDestinationArrivalComparisonSummaries(payloadKb);
        return summaries.Count == 0 ? "비교할 목적지가 없습니다." : string.Join(" / ", summaries.ToArray());
    }

    public ModelStateSummaryRecord GetModelStateSummaryRecord()
    {
        ModelStateSummaryRecord record = new ModelStateSummaryRecord();
        record.satelliteCount = satelliteManager != null ? satelliteManager.Satellites.Count : 0;
        record.destinationCount = Targets.Count;
        record.queuedMessageCount = QueuedMessages.Count;
        record.summary = "위성 " + record.satelliteCount + "대 · 목적지 " + record.destinationCount + "곳 · 대기 " + record.queuedMessageCount + "건";
        return record;
    }

    public string GetModelStateSummary()
    {
        return GetModelStateSummaryRecord().summary;
    }

    public QueuedMessageRouteDetail GetQueuedMessageRouteDetail(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count)
        {
            return null;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        int effectiveDelayMinutes = GetEffectiveQueuedDelayMinutes(queued);
        string queuedOriginName = string.IsNullOrEmpty(queued.originName) ? OriginName : queued.originName;
        SatelliteData recommendedSatellite = FindPreferredOrBestSatelliteForQueuedMessage(queued, effectiveDelayMinutes, queuedOriginName);
        ObservationTarget target = Targets.Find(item => item.targetName == queued.targetName);

        bool available = target != null && recommendedSatellite != null;
        float estimatedPassSeconds = available ? EstimatePassSecondsToTarget(target, recommendedSatellite, effectiveDelayMinutes, queued.payloadKb) : 0f;
        float estimatedDownlinkSeconds = available ? EstimateDownlinkSecondsToGroundStation() : 0f;
        float transferSeconds = available ? EstimateTransferSeconds(queued.payloadKb, estimatedDownlinkSeconds, recommendedSatellite) : 0f;
        int estimatedDeliverySeconds = available ? Mathf.CeilToInt(effectiveDelayMinutes * 60f + estimatedPassSeconds + estimatedDownlinkSeconds + transferSeconds) : 0;
        float baseFee = 0f;
        float payloadFee = 0f;
        float urgencyFee = 0f;
        float priceUsd = available ? EstimateDetailedPriceUsd(queued.payloadKb, effectiveDelayMinutes, recommendedSatellite, out baseFee, out payloadFee, out urgencyFee) : 0f;
        int successProbability = available ? EstimateSuccessProbability(target, recommendedSatellite, estimatedPassSeconds) : 0;

        QueuedMessageRouteDetail detail = new QueuedMessageRouteDetail();
        detail.isAvailable = available;
        detail.satelliteName = available ? recommendedSatellite.satelliteName : string.Empty;
        detail.scheduledLabel = EffectiveQueuedScheduleLabel(queued);
        detail.requestedSendDelayMinutes = effectiveDelayMinutes;
        detail.estimatedDeliverySeconds = estimatedDeliverySeconds;
        detail.estimatedDeliveryLabel = available ? FormatRemainingSeconds(estimatedDeliverySeconds) : "통신 불가";
        detail.priceUsd = priceUsd;
        detail.baseFeeUsd = baseFee;
        detail.payloadFeeUsd = payloadFee;
        detail.urgencyFeeUsd = urgencyFee;
        detail.successProbabilityPercent = successProbability;
        detail.attachmentSummary = AttachmentSummaryText(queued.attachments, queued.attachmentName, queued.attachmentSizeKb);
        detail.payloadKb = queued.payloadKb;
        detail.canEdit = CanEditQueuedMessage(index);
        detail.canCancel = CanCancelQueuedMessage(index);
        detail.canStartNow = CanStartQueuedMessageNow(index);
        detail.actionSummary = GetQueuedMessageActionSummary(index);
        detail.recommendationReason = available
            ? BuildRouteRecommendationReason(recommendedSatellite, queuedOriginName, queued.targetName, effectiveDelayMinutes, queued.payloadKb)
            : "연결 가능한 위성이 없습니다.";
        detail.routeSummary = available
            ? CityLabel(queuedOriginName) + " 업링크 → LEO/ISL → " + queued.destinationLabel + " 다운링크"
                + SameRegionVerificationNote(queuedOriginName, queued.targetName)
                + " · 위성망 " + detail.satelliteName
                + " · 예상 소요 시간 " + detail.estimatedDeliveryLabel
            : CityLabel(queuedOriginName) + " → " + queued.destinationLabel
                + SameRegionVerificationNote(queuedOriginName, queued.targetName)
                + " · 통신 불가";
        detail.statusSummary = BuildQueuedMessageInlineSummary(queued)
            + " · " + (detail.isAvailable ? detail.satelliteName : "통신 불가")
            + " · " + (detail.isAvailable ? "예상 소요 시간 " + detail.estimatedDeliveryLabel + " · $" + detail.priceUsd.ToString("0.00") : "요금 계산 불가")
            + " · " + detail.actionSummary;
        return detail;
    }

    public SatelliteData FindRecommendedSatelliteForQueuedMessage(int index)
    {
        if (index < 0 || index >= QueuedMessages.Count || satelliteManager == null)
        {
            return null;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        string queuedOriginName = string.IsNullOrEmpty(queued.originName) ? OriginName : queued.originName;
        return FindPreferredOrBestSatelliteForQueuedMessage(queued, GetEffectiveQueuedDelayMinutes(queued), queuedOriginName);
    }

    private SatelliteData FindPreferredOrBestSatelliteForQueuedMessage(QueuedMessageRecord queued, int delayMinutes)
    {
        string queuedOriginName = queued != null && !string.IsNullOrEmpty(queued.originName) ? queued.originName : OriginName;
        return FindPreferredOrBestSatelliteForQueuedMessage(queued, delayMinutes, queuedOriginName);
    }

    private SatelliteData FindPreferredOrBestSatelliteForQueuedMessage(QueuedMessageRecord queued, int delayMinutes, string queuedOriginName)
    {
        if (queued == null || satelliteManager == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(queued.preferredSatelliteName))
        {
            SatelliteData preferred = satelliteManager.Satellites.Find(satellite => satellite != null && satellite.satelliteName == queued.preferredSatelliteName);
            if (preferred != null && CanServeRoute(preferred, queuedOriginName, queued.targetName, delayMinutes, queued.payloadKb))
            {
                return preferred;
            }
        }

        SatelliteData best = FindBestSatelliteForRoute(queuedOriginName, queued.targetName, delayMinutes, queued.payloadKb);
        return CanServeRoute(best, queuedOriginName, queued.targetName, delayMinutes, queued.payloadKb) ? best : null;
    }

    public bool IsQueuedMessageRouteAvailable(int index)
    {
        return FindRecommendedSatelliteForQueuedMessage(index) != null;
    }

    public bool SetQueuedMessagePreferredSatellite(int index, string satelliteName)
    {
        if (index < 0 || index >= QueuedMessages.Count || QueuedMessages[index] == null)
        {
            return false;
        }

        QueuedMessages[index].preferredSatelliteName = satelliteName ?? string.Empty;
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool SaveCurrentMessageTemplate(string templateName, out string errorMessage)
    {
        string targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        return SaveMessageTemplate(templateName, MessageText, targetName, RequestedSendDelayMinutes, CloneAttachments(attachmentFiles), out errorMessage);
    }

    public bool SaveMessageTemplate(string templateName, string messageText, string targetName, int delayMinutes, List<MessageAttachmentRecord> attachments, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(templateName))
        {
            errorMessage = "템플릿 이름을 입력하세요.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        ObservationTarget target = Targets.Find(item => item.targetName == targetName);
        if (target == null)
        {
            errorMessage = "템플릿 목적지를 선택하세요.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        List<MessageAttachmentRecord> safeAttachments = CloneAttachments(attachments);
        int textBytes = GetUtf8ByteCount(messageText);
        int payloadKb = CalculatePayloadKbFromBytes(CalculatePayloadBytes(textBytes, safeAttachments));
        if (string.IsNullOrWhiteSpace(messageText) && safeAttachments.Count == 0)
        {
            errorMessage = "템플릿에 저장할 메시지나 첨부가 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        if (payloadKb > MaxPayloadKb)
        {
            errorMessage = "크기 초과: " + PayloadKbText(payloadKb) + " / " + PayloadKbText(MaxPayloadKb);
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        MessageTemplateRecord template = new MessageTemplateRecord();
        template.templateId = "TPL-" + DateTime.Now.ToString("MMdd-HHmmss");
        template.templateName = templateName.Trim();
        template.createdAt = DateTime.Now.ToString("MM/dd HH:mm");
        template.originName = OriginName;
        template.originLabel = OriginLabel;
        template.targetName = target.targetName;
        template.destinationLabel = DestinationLabel(target.targetName);
        template.messageText = messageText ?? string.Empty;
        template.requestedSendDelayMinutes = Mathf.Max(0, delayMinutes);
        template.scheduledLabel = template.requestedSendDelayMinutes == 0
            ? "지금 보내기"
            : DateTime.Now.AddMinutes(template.requestedSendDelayMinutes).ToString("MM/dd HH:mm") + " 보내기";
        template.attachments = safeAttachments;
        ApplyAttachmentSummaryToRecord(template);
        template.messageTextBytes = textBytes;
        template.payloadKb = payloadKb;
        MessageTemplates.Insert(0, template);
        ResultMessage = "메시지 템플릿을 저장했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public bool ApplyMessageTemplate(int index, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= MessageTemplates.Count)
        {
            errorMessage = "적용할 템플릿이 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle && Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            errorMessage = "전송 중에는 템플릿을 적용할 수 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        MessageTemplateRecord template = MessageTemplates[index];
        if (Targets.Find(item => item.targetName == template.targetName) == null)
        {
            errorMessage = "템플릿의 목적지를 확인할 수 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }
        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        ApplyExplicitOriginSelection(string.IsNullOrEmpty(template.originName) ? OrbitShareDestinations.Seoul : template.originName);
        SelectTarget(template.targetName);
        SetSendDelayMinutes(template.requestedSendDelayMinutes);
        SetMessageText(template.messageText);
        SetAttachments(template.attachments);
        ResultMessage = "템플릿을 작성 화면에 적용했습니다.";
        UpdateMissionEstimate();
        NotifyChanged();
        return true;
    }

    public bool DeleteMessageTemplate(int index)
    {
        if (index < 0 || index >= MessageTemplates.Count)
        {
            return false;
        }

        MessageTemplates.RemoveAt(index);
        ResultMessage = "메시지 템플릿을 삭제했습니다.";
        SavePersistentState();
        NotifyChanged();
        return true;
    }

    public string GetMessageTemplateSummary(int index)
    {
        if (index < 0 || index >= MessageTemplates.Count)
        {
            return string.Empty;
        }

        MessageTemplateRecord template = MessageTemplates[index];
        string origin = string.IsNullOrEmpty(template.originLabel) ? "서울" : template.originLabel;
        return template.templateName + " · " + origin + " → " + template.destinationLabel + " · " + template.scheduledLabel + " · " + PayloadKbText(template.payloadKb);
    }

    public void StartRouteReplay()
    {
        if (BlockRouteReplayOutsideDemo())
        {
            return;
        }

        if (SentMessages.Count == 0 && Phase != MissionPhase.Complete)
        {
            ResultMessage = "재생할 통신 과정이 없습니다.";
            NotifyChanged();
            return;
        }

        RouteReplayProgress = 0f;
        IsReplayingRoute = true;
        IsRouteReplayPaused = false;
        ResultMessage = "통신 과정을 재생 중입니다.";
        NotifyChanged();
    }

    public void PauseRouteReplay()
    {
        if (BlockRouteReplayOutsideDemo())
        {
            return;
        }

        if (!IsReplayingRoute)
        {
            return;
        }

        IsRouteReplayPaused = true;
        ResultMessage = "통신 과정 재생을 일시정지했습니다.";
        NotifyChanged();
    }

    public void ResumeRouteReplay()
    {
        if (BlockRouteReplayOutsideDemo())
        {
            return;
        }

        if (!IsReplayingRoute)
        {
            StartRouteReplay();
            return;
        }

        IsRouteReplayPaused = false;
        ResultMessage = "통신 과정 재생을 재개했습니다.";
        NotifyChanged();
    }

    public void RestartRouteReplay()
    {
        if (BlockRouteReplayOutsideDemo())
        {
            return;
        }

        StartRouteReplay();
    }

    public void SetRouteReplaySpeed(float speed)
    {
        if (BlockRouteReplayOutsideDemo())
        {
            return;
        }

        RouteReplaySpeed = Mathf.Clamp(speed, 0.1f, DemoMaxTimeScale);
        ResultMessage = "통신 과정 재생 속도 x" + RouteReplaySpeed.ToString("0.0");
        NotifyChanged();
    }

    public void SetDemoRouteReplayEnabled(bool enabled)
    {
        SetDemoRouteReplayEnabledInternal(enabled);
        NotifyChanged();
    }

    public bool ReuseSentMessage(int index, out string errorMessage)
    {
        errorMessage = string.Empty;
        if (index < 0 || index >= SentMessages.Count)
        {
            errorMessage = "다시 보낼 기록이 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        SentMessageRecord record = SentMessages[index];
        string targetName = string.IsNullOrEmpty(record.targetName) ? TargetNameFromDestinationLabel(record.destinationLabel) : record.targetName;
        if (string.IsNullOrEmpty(targetName))
        {
            errorMessage = "기록의 목적지를 확인할 수 없습니다.";
            ResultMessage = errorMessage;
            NotifyChanged();
            return false;
        }

        if (Phase != MissionPhase.Idle && Phase != MissionPhase.Complete && Phase != MissionPhase.Failed)
        {
            bool queued = QueueMessageFromOrigin(
                record.messageText,
                string.IsNullOrEmpty(record.originName) ? OriginName : record.originName,
                targetName,
                0,
                DateTime.Now.Hour,
                DateTime.Now.Minute - DateTime.Now.Minute % 5,
                AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb),
                out errorMessage);
            if (queued)
            {
                ResultMessage = "현재 전송은 그대로 진행됩니다. 전송 기록의 메시지는 완료 후 보낼 수 있도록 메시지 목록에 추가했습니다. " + GetQueueSummary();
                NotifyChanged();
            }
            return queued;
        }

        if (Phase != MissionPhase.Idle)
        {
            ResetMission();
        }

        ApplyExplicitOriginSelection(string.IsNullOrEmpty(record.originName) ? OrbitShareDestinations.Seoul : record.originName);
        SelectTarget(targetName);
        SetSendDelayMinutes(0);
        SetMessageText(record.messageText);
        SetAttachments(AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb));
        SelectRecommendedSatelliteIfNeeded();
        ResultMessage = "전송 기록의 메시지를 새 작성 화면으로 불러왔습니다.";
        UpdateMissionEstimate();
        NotifyChanged();
        return true;
    }

    public void RemoveSentMessage(int index)
    {
        if (index < 0 || index >= SentMessages.Count)
        {
            return;
        }

        SentMessageRecord record = SentMessages[index];
        bool removedActiveTransfer = record != null
            && !SentRecordIsComplete(record)
            && !string.IsNullOrEmpty(activeSentMessageTrackingId)
            && record.trackingId == activeSentMessageTrackingId;
        SentMessages.RemoveAt(index);
        if (removedActiveTransfer)
        {
            activeSentMessageTrackingId = string.Empty;
            ResetMission();
            ResultMessage = "전송을 취소했습니다. 대기 메시지는 유지했습니다.";
            SavePersistentState();
            NotifyChanged();
            return;
        }
        RefreshTransmissionStats();
        ResultMessage = "전송 기록을 삭제했습니다.";
        SavePersistentState();
        NotifyChanged();
    }

    public void RetryFailedMission()
    {
        if (Phase != MissionPhase.Failed)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(MessageText))
        {
            ResultMessage = "재시도할 메시지가 없습니다.";
            NotifyChanged();
            return;
        }

        if (satelliteManager != null)
        {
            satelliteManager.ResetSharedSatellite();
            SelectRecommendedSatelliteIfNeeded();
        }
        AddMissionEvent("실패 재시도 요청: " + LastFailureReason);

        Phase = MissionPhase.Idle;
        CommunicationStep = SatelliteCommunicationStep.Draft;
        activeSentMessageTrackingId = string.Empty;
        if (SuggestedRetryDelayMinutes > 0)
        {
            SetSendDelayMinutes(SuggestedRetryDelayMinutes);
        }
        if (!string.IsNullOrEmpty(SuggestedRetrySatelliteName) && satelliteManager != null)
        {
            satelliteManager.SelectSatellite(SuggestedRetrySatelliteName);
        }
        ObservationProgress = 0f;
        TransferProgress = 0f;
        InterSatelliteProgress = 0f;
        RelaySelectionProgress = 0f;
        DownlinkProgress = 0f;
        FinalReceiveProgress = 0f;
        SendWaitProgress = 0f;
        scheduledWaitElapsedSeconds = 0f;
        transferElapsedSeconds = 0f;
        DestinationRelaySatelliteName = string.Empty;
        ClearInterSatelliteRoute();
        deliveryRecorded = false;
        ReserveMission();
        StartMission();
    }

    public void ReserveMission()
    {
        if (satelliteManager == null || SelectedTarget == null)
        {
            return;
        }
        if (SelectedOrigin == null && !ApplySelectedOriginSilently(OrbitShareDestinations.Seoul))
        {
            ResultMessage = "발신지를 선택하세요.";
            NotifyChanged();
            return;
        }
        if (string.IsNullOrWhiteSpace(MessageText))
        {
            ResultMessage = "메시지를 먼저 입력하세요.";
            NotifyChanged();
            return;
        }
        if (PayloadTooLarge)
        {
            ResultMessage = "크기가 " + PayloadKbText(MaxPayloadKb) + "를 넘었습니다.";
            NotifyChanged();
            return;
        }

        if (!CanServeRoute(satelliteManager.SelectedSatellite, OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb))
        {
            SatelliteData bestSatellite = FindBestSatelliteForRoute(OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb);
            if (bestSatellite != null)
            {
                satelliteManager.SelectSatellite(bestSatellite.satelliteName);
            }
        }
        if (!CanServeRoute(satelliteManager.SelectedSatellite, OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb))
        {
            ResultMessage = "발신지와 목적지를 모두 연결할 수 있는 위성을 선택하세요.";
            NotifyChanged();
            return;
        }

        if (satelliteManager.ReserveSelected(SelectedTarget.targetName, RequestedSendDelayMinutes, !suppressSatelliteChangedNotification))
        {
            Phase = MissionPhase.Reserved;
            CommunicationStep = SatelliteCommunicationStep.Reserved;
            deliveryRecorded = false;
            LastFailureReason = string.Empty;
            ObservationProgress = 0f;
            TransferProgress = 0f;
            InterSatelliteProgress = 0f;
            RelaySelectionProgress = 0f;
            DownlinkProgress = 0f;
            FinalReceiveProgress = 0f;
            DestinationRelaySatelliteName = string.Empty;
            plannedDestinationRelaySatelliteName = string.Empty;
            ClearInterSatelliteRoute();
            AddMissionEvent("예약 접수: " + ScheduledLabel);
            ResultMessage = "예약 완료: " + ScheduledLabel + " · " + OriginLabel + "에서 " + DestinationDisplayLabel + "로 보냅니다.";
            CompletionSummary = BuildCommunicationFlowSummary() + "\n예약 대기\n크기: " + PayloadKbText(MessageSizeKb);
            SavePersistentState();
            NotifyChanged();
        }
        else
        {
            ResultMessage = "사용 가능한 위성을 선택하세요.";
            NotifyChanged();
        }
    }

    public void ReserveAndStartMission()
    {
        bool previousPersistenceSuppress = suppressPersistenceSave;
        bool previousMissionNotificationSuppress = suppressMissionChangedNotification;
        bool previousSatelliteNotificationSuppress = suppressSatelliteChangedNotification;
        MissionPhase previousPhase = Phase;
        string previousResultMessage = ResultMessage;

        suppressPersistenceSave = true;
        suppressMissionChangedNotification = true;
        suppressSatelliteChangedNotification = true;
        try
        {
            if (Phase == MissionPhase.Idle)
            {
                ReserveMission();
            }
            if (Phase == MissionPhase.Reserved)
            {
                StartMission();
            }
        }
        finally
        {
            suppressPersistenceSave = previousPersistenceSuppress;
            suppressMissionChangedNotification = previousMissionNotificationSuppress;
            suppressSatelliteChangedNotification = previousSatelliteNotificationSuppress;
        }

        if (Phase != previousPhase || ResultMessage != previousResultMessage)
        {
            SavePersistentState();
            NotifyChanged();
        }
    }

    public void StartMission()
    {
        if (Phase != MissionPhase.Reserved || satelliteManager == null)
        {
            return;
        }

        Phase = MissionPhase.Running;
        CommunicationStep = SatelliteCommunicationStep.WaitingForUplink;
        SyncGroundStationToOrigin();
        deliveryRecorded = false;
        reservationAlertLogged = false;
        DestinationRelaySatelliteName = string.Empty;
        ObservationProgress = 0f;
        TransferProgress = 0f;
        InterSatelliteProgress = 0f;
        RelaySelectionProgress = 0f;
        DownlinkProgress = 0f;
        FinalReceiveProgress = 0f;
        AddMissionEvent("업링크 대기 시작: " + SenderLabel() + " → " + SelectedSatelliteName);
        scheduledWaitElapsedSeconds = 0f;
        transferElapsedSeconds = 0f;
        plannedDestinationRelaySatelliteName = string.Empty;
        ClearInterSatelliteRoute();
        SatelliteRouteEstimateRecord routeEstimate = SelectedRouteEstimate(RequestedSendDelayMinutes, MessageSizeKb);
        ConfigureTransmissionPlan(routeEstimate);
        SendWaitProgress = plannedWaitSeconds <= 0f ? 1f : 0f;
        RemainingWaitSeconds = plannedWaitSeconds;
        RemainingTransferSeconds = plannedTransferSeconds;
        satelliteManager.SetSharedState(SatelliteAvailabilityState.InMission, !suppressSatelliteChangedNotification);
        ResultMessage = plannedWaitSeconds > 0f
            ? "업링크 대기 " + FormatRemainingSeconds(plannedWaitSeconds)
            : "업링크 준비: " + SenderLabel() + " → LEO 위성.";
        CompletionSummary = BuildCommunicationFlowSummary() + "\n업링크 대기\n크기: " + PayloadKbText(MessageSizeKb);
        EnsureActiveSentMessageRecord();
        SavePersistentState();
        NotifyChanged();
    }

    public void RunFastDemo()
    {
        if (SelectedTarget == null || CurrentSatelliteTransform() == null)
        {
            return;
        }

        SetDemoRouteReplayEnabledInternal(true);
        if (Phase == MissionPhase.Idle && string.IsNullOrWhiteSpace(MessageText))
        {
            SetMessageText("뉴욕 수신 확인");
        }

        if (Phase == MissionPhase.Idle && satelliteManager != null && !CanServeRoute(satelliteManager.SelectedSatellite, OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb))
        {
            SatelliteData bestSatellite = FindBestSatelliteForRoute(OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb);
            if (bestSatellite != null)
            {
                satelliteManager.SelectSatellite(bestSatellite.satelliteName);
            }
        }

        if (Phase == MissionPhase.Idle)
        {
            ReserveMission();
        }

        demoAssist = true;
        RouteReplaySpeed = Mathf.Max(RouteReplaySpeed, DemoDefaultTimeScale);

        if (ObservationProgress >= 1f && Phase != MissionPhase.Complete && SelectedTarget != null)
        {
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = CommunicationStep == SatelliteCommunicationStep.Downlink
                ? "다운링크 중: 목적지 근처 위성에서 수신지로 전송 중."
                : "LEO 수신 후 ISL 전달을 진행 중.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Reserved)
        {
            StartMission();
            SkipScheduledWaitForDemo();
            if (groundStation != null)
            {
                AlignSatelliteTo(groundStation.transform);
            }
            ResultMessage = SenderLabel() + " 업링크 준비 중.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Running || Phase == MissionPhase.Observing)
        {
            SkipScheduledWaitForDemo();
            if (groundStation != null)
            {
                AlignSatelliteTo(groundStation.transform);
            }
            ResultMessage = SenderLabel() + "에서 LEO 위성으로 업링크 중.";
            NotifyChanged();
            return;
        }

        if ((Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting) && SelectedTarget != null)
        {
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = "ISL/다운링크 절차를 진행 중.";
            NotifyChanged();
        }
    }

    private void SkipScheduledWaitForDemo()
    {
        if (!IsDemoRouteReplayEnabled)
        {
            return;
        }

        scheduledWaitElapsedSeconds = plannedWaitSeconds;
        SendWaitProgress = 1f;
        RemainingWaitSeconds = 0f;
    }

    public void ResetMission()
    {
        Phase = MissionPhase.Idle;
        CommunicationStep = SatelliteCommunicationStep.Draft;
        activeSentMessageTrackingId = string.Empty;
        demoAssist = false;
        deliveryRecorded = false;
        ObservationProgress = 0f;
        TransferProgress = 0f;
        InterSatelliteProgress = 0f;
        RelaySelectionProgress = 0f;
        DownlinkProgress = 0f;
        FinalReceiveProgress = 0f;
        SendWaitProgress = 0f;
        scheduledWaitElapsedSeconds = 0f;
        transferElapsedSeconds = 0f;
        plannedWaitSeconds = 0f;
        plannedDownlinkReadySeconds = 0f;
        plannedIslSeconds = 3f;
        plannedRelaySelectionSeconds = 1.5f;
        plannedDownlinkSeconds = 4f;
        plannedFinalReceiveSeconds = 0.8f;
        plannedTransferSeconds = CalculateTransferSeconds();
        plannedDestinationRelaySatelliteName = string.Empty;
        ClearInterSatelliteRoute();
        RemainingWaitSeconds = 0f;
        RemainingTransferSeconds = 0f;
        SetDemoRouteReplayEnabledInternal(false);
        LastFailureReason = string.Empty;
        SuggestedRetrySatelliteName = string.Empty;
        SuggestedRetryDelayMinutes = 0;
        SuggestedRetryHint = string.Empty;
        DestinationRelaySatelliteName = string.Empty;
        reservationAlertLogged = false;
        AutoQueueCountdown = -1f;
        lastAutoCountdownSecond = -1;
        CommunicationAvailable = false;
        HasExplicitOriginSelection = false;
        HasExplicitDestinationSelection = false;
        RefreshTargetSelectionVisuals();
        MessageText = string.Empty;
        attachmentFiles.Clear();
        UpdateAttachmentSummary();
        System.DateTime scheduled = System.DateTime.Now;
        RequestedSendDelayMinutes = 0;
        ScheduledDayOffset = 0;
        ScheduledHour = scheduled.Hour;
        ScheduledMinute = scheduled.Minute - scheduled.Minute % 5;
        ScheduledLabel = "지금 보내기";
        UpdatePayloadSize();
        MissionEventLog.Clear();
        AddMissionEvent("요청 작성 시작");
        ResultMessage = "새 메시지를 작성하세요.";
        CompletionSummary = "전송이 끝나면 결과가 표시됩니다.";

        if (satelliteManager != null)
        {
            satelliteManager.ResetSharedSatellite();
        }

        if (connectionLine != null)
        {
            connectionLine.SetVisible(false);
        }

        SelectRecommendedSatelliteIfNeeded();
        UpdateMissionEstimate();
        SavePersistentState();
        NotifyChanged();
    }

    private void UpdateObservation()
    {
        if (SelectedTarget == null || groundStation == null)
        {
            return;
        }

        if (Phase == MissionPhase.Running && scheduledWaitElapsedSeconds < plannedWaitSeconds)
        {
            CommunicationStep = SatelliteCommunicationStep.WaitingForUplink;
            float delta = MissionDeltaTime();
            scheduledWaitElapsedSeconds = Mathf.Min(plannedWaitSeconds, scheduledWaitElapsedSeconds + delta);
            SendWaitProgress = plannedWaitSeconds <= 0f ? 1f : Mathf.Clamp01(scheduledWaitElapsedSeconds / plannedWaitSeconds);
            RemainingWaitSeconds = Mathf.Max(0f, plannedWaitSeconds - scheduledWaitElapsedSeconds);
            if (!reservationAlertLogged && RemainingWaitSeconds <= 5f)
            {
                reservationAlertLogged = true;
                AddMissionEvent("예약 알림: 곧 전송");
            }
            ResultMessage = reservationAlertLogged
                ? "곧 전송됩니다: " + FormatRemainingSeconds(RemainingWaitSeconds)
                : "예약 대기 " + FormatRemainingSeconds(RemainingWaitSeconds);
            NotifyChanged();
            return;
        }
        SendWaitProgress = 1f;
        RemainingWaitSeconds = 0f;

        float angle = SurfaceAngleTo(groundStation.transform);
        if (angle > groundStation.communicationAngle && !demoAssist && !IsDemoRouteReplayEnabled)
        {
            CommunicationStep = SatelliteCommunicationStep.WaitingForUplink;
            RemainingWaitSeconds = EstimateCurrentUplinkWindowSeconds();
            ResultMessage = "업링크 대기: " + SenderLabel() + " 통신 가능까지 " + FormatRemainingSeconds(RemainingWaitSeconds);
            NotifyChanged();
            return;
        }

        if (connectionLine != null)
        {
            connectionLine.SetVisible(true);
        }

        if (angle <= groundStation.communicationAngle || demoAssist)
        {
            Phase = MissionPhase.Observing;
            CommunicationStep = SatelliteCommunicationStep.Uplink;
            float delta = MissionDeltaTime();
            ObservationProgress = Mathf.Clamp01(ObservationProgress + delta / RequiredRelayLockSeconds);
            ResultMessage = "업링크 중: " + SenderLabel() + " → LEO 위성 · " + Mathf.RoundToInt(ObservationProgress * 100f) + "%";

            if (ObservationProgress >= 1f)
            {
                Phase = MissionPhase.ObservationComplete;
                CommunicationStep = SatelliteCommunicationStep.LeoReceived;
                ResultMessage = "LEO 수신 완료. 위성 간 전달(ISL)을 준비합니다.";
                CompletionSummary = BuildCommunicationFlowSummary() + "\nLEO 수신 완료\n크기: " + PayloadKbText(MessageSizeKb);
                AddMissionEvent("LEO 수신 완료: " + SelectedSatelliteName);
                if (satelliteManager != null)
                {
                    satelliteManager.UpdateSharedMission(SelectedTarget.targetName, QueuedMessageBufferPercent);
                }
                SavePersistentState();
            }

            NotifyChanged();
        }
        else if (Phase == MissionPhase.Observing)
        {
            Phase = MissionPhase.Running;
            CommunicationStep = SatelliteCommunicationStep.WaitingForUplink;
            ResultMessage = "인공위성이 " + SenderLabel() + " 발신 가능 위치로 올 때까지 기다립니다.";
            NotifyChanged();
        }
    }

    private void UpdateTransmission()
    {
        if (connectionLine != null)
        {
            connectionLine.SetVisible(false);
        }

        if (SelectedTarget == null || ObservationProgress < 1f)
        {
            NotifyChanged();
            return;
        }

        RefreshDestinationRelayCandidate();

        float delta = MissionDeltaTime();
        transferElapsedSeconds += delta;
        AdvanceTransmissionStages(delta);
        Phase = CommunicationStep == SatelliteCommunicationStep.Downlink
            || CommunicationStep == SatelliteCommunicationStep.FinalReceive
            || CommunicationStep == SatelliteCommunicationStep.WaitingForDownlink
            ? MissionPhase.Transmitting
            : MissionPhase.ObservationComplete;
        ResultMessage = CommunicationStepStatusText();

        if (TransferProgress >= 1f)
        {
            if (SimulateFailureNextSend)
            {
                Phase = MissionPhase.Failed;
                demoAssist = false;
                SimulateFailureNextSend = false;
                string failureReason = ChooseFailureReason();
                string failureStep = FailureStepLabelForReason(failureReason);
                LastFailureReason = failureStep + " 단계: " + failureReason;
                UpdateSuggestedRetryAlternative();
                CommunicationStep = SatelliteCommunicationStep.Failed;
                ResultMessage = "전송 실패: " + LastFailureReason + ". " + (string.IsNullOrEmpty(SuggestedRetryHint) ? "재시도할 수 있습니다." : SuggestedRetryHint);
                CompletionSummary = "전송 실패\n" + BuildCommunicationFlowSummary() + "\n실패 단계: " + failureStep + "\n사유: " + failureReason;
                AddMissionEvent("전송 실패(" + failureStep + "): " + failureReason);
                FailedTransmissionCount++;
                RefreshTransmissionStats();
                if (satelliteManager != null)
                {
                    satelliteManager.ResetSharedSatellite();
                }
                if (connectionLine != null)
                {
                    connectionLine.SetVisible(false);
                }
                SavePersistentState();
                NotifyChanged();
                return;
            }

            Phase = MissionPhase.Complete;
            CommunicationStep = SatelliteCommunicationStep.Complete;
            demoAssist = false;
            ResultMessage = "전송 완료. 목적지에 메시지가 도착했습니다.";
            CompletionSummary = "전송 완료\n" + BuildCommunicationFlowSummary() + "\n경과 시간: " + Mathf.RoundToInt(plannedWaitSeconds + transferElapsedSeconds) + "초";
            UpdateSecurityStatus();
            AddMissionEvent("도착 확인 완료: 목적지 도착 확인");
            RecordDelivery();
            if (SentMessages.Count > 0)
            {
                ResultMessage = "전송 완료. " + GetLatestTransmissionResultSummary();
            }
            if (satelliteManager != null)
            {
                satelliteManager.SetSharedState(SatelliteAvailabilityState.MissionComplete);
                satelliteManager.UpdateSharedMission(SelectedTarget != null ? SelectedTarget.targetName : "None", 0f);
            }
            if (connectionLine != null)
            {
                connectionLine.SetVisible(false);
            }
        }

        NotifyChanged();
    }

    private SatelliteCommunicationStep CommunicationStepFromTransferProgress(float progress)
    {
        if (progress < 0.28f)
        {
            return SatelliteCommunicationStep.InterSatelliteLink;
        }
        if (progress < 0.40f)
        {
            return SatelliteCommunicationStep.DestinationRelaySelection;
        }
        if (progress < 0.92f)
        {
            return SatelliteCommunicationStep.Downlink;
        }
        return SatelliteCommunicationStep.FinalReceive;
    }

    private void UpdateSegmentProgressFromTransfer()
    {
        InterSatelliteProgress = Mathf.Clamp01(Mathf.InverseLerp(0f, 0.28f, TransferProgress));
        RelaySelectionProgress = Mathf.Clamp01(Mathf.InverseLerp(0.28f, 0.40f, TransferProgress));
        DownlinkProgress = Mathf.Clamp01(Mathf.InverseLerp(0.40f, 0.92f, TransferProgress));
        FinalReceiveProgress = Mathf.Clamp01(Mathf.InverseLerp(0.92f, 1f, TransferProgress));
    }

    private void AddCommunicationStepEvent(SatelliteCommunicationStep step)
    {
        switch (step)
        {
            case SatelliteCommunicationStep.InterSatelliteLink:
                AddMissionEvent("ISL 전달 시작: " + BuildInterSatelliteRouteSummary());
                break;
            case SatelliteCommunicationStep.DestinationRelaySelection:
                AddMissionEvent("목적지 근처 위성 선택: " + DestinationRelaySatelliteLabel());
                break;
            case SatelliteCommunicationStep.WaitingForDownlink:
                AddMissionEvent("다운링크 대기: " + DestinationRelaySatelliteLabel() + " → " + DestinationDisplayLabel);
                break;
            case SatelliteCommunicationStep.Downlink:
                AddMissionEvent("다운링크 시작: " + DestinationRelaySatelliteLabel() + " → " + DestinationDisplayLabel);
                break;
            case SatelliteCommunicationStep.FinalReceive:
                AddMissionEvent("도착 확인: " + DestinationDisplayLabel);
                break;
        }
    }

    private string CommunicationStepStatusText()
    {
        switch (CommunicationStep)
        {
            case SatelliteCommunicationStep.InterSatelliteLink:
                return "ISL 전달 중: " + BuildInterSatelliteRouteSummary() + " · " + Mathf.RoundToInt(InterSatelliteProgress * 100f) + "%";
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return "목적지 근처 위성 선택 중: " + DestinationRelaySatelliteLabel() + " · " + Mathf.RoundToInt(RelaySelectionProgress * 100f) + "%";
            case SatelliteCommunicationStep.WaitingForDownlink:
                return "다운링크 대기: " + DestinationRelaySatelliteLabel() + " → " + DestinationDisplayLabel + " · " + FormatRemainingSeconds(RemainingTransferSeconds);
            case SatelliteCommunicationStep.Downlink:
                return "다운링크 중: " + DestinationRelaySatelliteLabel() + " → " + DestinationDisplayLabel + " · " + Mathf.RoundToInt(DownlinkProgress * 100f) + "% · 남은 " + FormatRemainingSeconds(RemainingTransferSeconds);
            case SatelliteCommunicationStep.FinalReceive:
                return "도착 확인 중: " + DestinationDisplayLabel + " · " + Mathf.RoundToInt(FinalReceiveProgress * 100f) + "%";
            default:
                return CommunicationStepLabel + " · " + FormatRemainingSeconds(RemainingTransferSeconds);
        }
    }

    private void ConfigureTransmissionPlan(SatelliteRouteEstimateRecord routeEstimate)
    {
        float requestedDelaySeconds = Mathf.Max(0, RequestedSendDelayMinutes) * 60f;
        float routeReadySeconds = routeEstimate != null ? Mathf.Max(0, routeEstimate.routeReadySeconds) : EstimateCurrentUplinkWindowSeconds();
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        SatelliteData plannedRelaySatellite = IsSameRegionRoute() ? selectedSatellite : FindBestDestinationRelaySatellite(0, MessageSizeKb);
        if (plannedRelaySatellite == null)
        {
            plannedRelaySatellite = SatelliteByName(routeEstimate != null ? routeEstimate.relaySatelliteName : string.Empty);
        }
        float plannedRelayWindowSeconds = DestinationRelayWindowSeconds(plannedRelaySatellite, 0, MessageSizeKb);
        float targetWindowSeconds = plannedRelayWindowSeconds >= 0f
            ? plannedRelayWindowSeconds
            : routeEstimate != null ? Mathf.Max(0, routeEstimate.downlinkWindowOpenSeconds) : EstimateCurrentDownlinkWindowSeconds();
        plannedWaitSeconds = Mathf.Max(requestedDelaySeconds, routeReadySeconds);
        plannedDownlinkReadySeconds = Mathf.Max(requestedDelaySeconds, targetWindowSeconds);
        plannedDestinationRelaySatelliteName = plannedRelaySatellite != null
            ? plannedRelaySatellite.satelliteName
            : routeEstimate != null && !string.IsNullOrEmpty(routeEstimate.relaySatelliteName) ? routeEstimate.relaySatelliteName : SelectedSatelliteName;
        DestinationRelaySatelliteName = plannedDestinationRelaySatelliteName;
        RebuildInterSatelliteRoute(true);

        float latency = selectedSatellite != null ? selectedSatellite.latencySeconds : 6f;
        float congestionSeconds = selectedSatellite != null ? selectedSatellite.congestionPercent * 0.045f : 0f;
        float payloadSeconds = Mathf.Max(1f, MessageSizeKb / 48f);
        float routeGapSeconds = routeEstimate != null ? Mathf.Abs(routeEstimate.originWindowSeconds - routeEstimate.targetWindowSeconds) * 0.02f : 0f;

        plannedIslSeconds = UsesInterSatelliteLink()
            ? CalculatePlannedIslSeconds(routeEstimate, congestionSeconds, routeGapSeconds)
            : 0f;
        bool relayMatchesUplink = !string.IsNullOrEmpty(plannedDestinationRelaySatelliteName) && plannedDestinationRelaySatelliteName == SelectedSatelliteName;
        plannedRelaySelectionSeconds = !UsesInterSatelliteLink() || relayMatchesUplink
            ? 0f
            : Mathf.Max(0.8f, 1.5f + congestionSeconds * 0.1f);
        plannedDownlinkSeconds = routeEstimate != null && routeEstimate.downlinkDurationSeconds > 0
            ? routeEstimate.downlinkDurationSeconds
            : Mathf.Max(1.5f, latency * 0.45f + payloadSeconds + congestionSeconds * 0.5f);
        plannedFinalReceiveSeconds = routeEstimate != null && routeEstimate.finalReceiveSeconds > 0
            ? routeEstimate.finalReceiveSeconds
            : 0.8f;
        plannedTransferSeconds = Mathf.Max(1f, plannedIslSeconds + plannedRelaySelectionSeconds + plannedDownlinkSeconds + plannedFinalReceiveSeconds);
    }

    private float CalculatePlannedIslSeconds(SatelliteRouteEstimateRecord routeEstimate, float congestionSeconds, float routeGapSeconds)
    {
        float routeSeconds = routeEstimate != null && routeEstimate.interSatelliteSeconds > 0
            ? routeEstimate.interSatelliteSeconds
            : Mathf.Max(1.5f, 2.5f + congestionSeconds * 0.4f + routeGapSeconds);
        int hopCount = Mathf.Max(1, InterSatelliteHopCount);
        float hopPenaltySeconds = Mathf.Max(0, hopCount - 1) * 1.15f;
        float routeCostSeconds = plannedIslRouteCost > 0f ? Mathf.Clamp(plannedIslRouteCost * 0.020f, 0f, 10f) : 0f;
        return Mathf.Max(1.5f, routeSeconds + hopPenaltySeconds + routeCostSeconds);
    }

    private float MissionDeltaTime()
    {
        return Time.deltaTime * (IsDemoRouteReplayEnabled ? Mathf.Max(1f, RouteReplaySpeed) : 1f);
    }

    private SatelliteRouteEstimateRecord SelectedRouteEstimate(int requestedDelayMinutes, int payloadKb)
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        if (selectedSatellite == null || satelliteManager == null || SelectedTarget == null)
        {
            return null;
        }
        return satelliteManager.GetRouteEstimate(selectedSatellite, OriginName, SelectedTarget.targetName, requestedDelayMinutes, payloadKb);
    }

    private void AdvanceTransmissionStages(float delta)
    {
        if (CommunicationStep == SatelliteCommunicationStep.LeoReceived || CommunicationStep == SatelliteCommunicationStep.Reserved || CommunicationStep == SatelliteCommunicationStep.WaitingForUplink || CommunicationStep == SatelliteCommunicationStep.Uplink)
        {
            if (UsesInterSatelliteLink())
            {
                SetCommunicationStep(SatelliteCommunicationStep.InterSatelliteLink);
            }
            else
            {
                InterSatelliteProgress = 1f;
                RelaySelectionProgress = 1f;
                RefreshDestinationRelayCandidate();
                float waitSeconds = RemainingDownlinkWindowSeconds();
                SetCommunicationStep(waitSeconds > 0f ? SatelliteCommunicationStep.WaitingForDownlink : SatelliteCommunicationStep.Downlink);
                UpdateTransmissionProgressSummary();
                return;
            }
        }

        if (CommunicationStep == SatelliteCommunicationStep.InterSatelliteLink)
        {
            RebuildInterSatelliteRoute(false);
            InterSatelliteProgress = Mathf.Clamp01(InterSatelliteProgress + delta / Mathf.Max(0.1f, plannedIslSeconds));
            if (InterSatelliteProgress >= 1f)
            {
                SetCommunicationStep(SatelliteCommunicationStep.DestinationRelaySelection);
                UpdateTransmissionProgressSummary();
                return;
            }
        }

        if (CommunicationStep == SatelliteCommunicationStep.DestinationRelaySelection)
        {
            RefreshDestinationRelayCandidate();
            RelaySelectionProgress = Mathf.Clamp01(RelaySelectionProgress + delta / Mathf.Max(0.1f, plannedRelaySelectionSeconds));
            if (RelaySelectionProgress >= 1f)
            {
                float waitSeconds = RemainingDownlinkWindowSeconds();
                SetCommunicationStep(waitSeconds > 0f ? SatelliteCommunicationStep.WaitingForDownlink : SatelliteCommunicationStep.Downlink);
                UpdateTransmissionProgressSummary();
                return;
            }
        }

        if (CommunicationStep == SatelliteCommunicationStep.WaitingForDownlink)
        {
            RefreshDestinationRelayCandidate();
            float waitSeconds = RemainingDownlinkWindowSeconds();
            if (waitSeconds <= 0f)
            {
                SetCommunicationStep(SatelliteCommunicationStep.Downlink);
                UpdateTransmissionProgressSummary();
                return;
            }
        }

        if (CommunicationStep == SatelliteCommunicationStep.Downlink)
        {
            if (demoAssist)
            {
                AlignDestinationRelayToTarget();
            }
            else if (!IsDemoRouteReplayEnabled && !IsDownlinkWindowOpen())
            {
                SetCommunicationStep(SatelliteCommunicationStep.WaitingForDownlink);
                UpdateTransmissionProgressSummary();
                return;
            }

            DownlinkProgress = Mathf.Clamp01(DownlinkProgress + delta / Mathf.Max(0.1f, plannedDownlinkSeconds));
            if (DownlinkProgress >= 1f)
            {
                SetCommunicationStep(SatelliteCommunicationStep.FinalReceive);
                UpdateTransmissionProgressSummary();
                return;
            }
        }

        if (CommunicationStep == SatelliteCommunicationStep.FinalReceive)
        {
            FinalReceiveProgress = Mathf.Clamp01(FinalReceiveProgress + delta / Mathf.Max(0.1f, plannedFinalReceiveSeconds));
        }

        UpdateTransmissionProgressSummary();
    }

    private void SetCommunicationStep(SatelliteCommunicationStep step)
    {
        if (CommunicationStep == step)
        {
            return;
        }
        CommunicationStep = step;
        AddCommunicationStepEvent(step);
        SavePersistentState();
    }

    private void UpdateTransmissionProgressSummary()
    {
        float completedSeconds = InterSatelliteProgress * plannedIslSeconds
            + RelaySelectionProgress * plannedRelaySelectionSeconds
            + DownlinkProgress * plannedDownlinkSeconds
            + FinalReceiveProgress * plannedFinalReceiveSeconds;
        plannedTransferSeconds = Mathf.Max(1f, plannedIslSeconds + plannedRelaySelectionSeconds + plannedDownlinkSeconds + plannedFinalReceiveSeconds);
        TransferProgress = Mathf.Clamp01(completedSeconds / plannedTransferSeconds);

        float remainingWorkSeconds = (1f - InterSatelliteProgress) * plannedIslSeconds
            + (1f - RelaySelectionProgress) * plannedRelaySelectionSeconds
            + (1f - DownlinkProgress) * plannedDownlinkSeconds
            + (1f - FinalReceiveProgress) * plannedFinalReceiveSeconds;
        float downlinkWaitSeconds = (CommunicationStep == SatelliteCommunicationStep.InterSatelliteLink
            || CommunicationStep == SatelliteCommunicationStep.DestinationRelaySelection
            || CommunicationStep == SatelliteCommunicationStep.WaitingForDownlink)
            ? RemainingDownlinkWindowSeconds()
            : 0f;
        RemainingTransferSeconds = Mathf.Max(0f, remainingWorkSeconds + downlinkWaitSeconds);
    }

    private float MissionElapsedSecondsForWindow()
    {
        return scheduledWaitElapsedSeconds + ObservationProgress * RequiredRelayLockSeconds + transferElapsedSeconds;
    }

    private float RemainingDownlinkWindowSeconds()
    {
        RefreshDestinationRelayCandidate();
        float scheduledWindowSeconds = Mathf.Max(0f, plannedDownlinkReadySeconds - MissionElapsedSecondsForWindow());
        if (IsDemoRouteReplayEnabled)
        {
            return 0f;
        }
        if (IsDownlinkWindowOpen())
        {
            return 0f;
        }

        float liveWindowSeconds = EstimateCurrentDownlinkWindowSeconds();
        if (scheduledWindowSeconds > 0f)
        {
            return Mathf.Min(scheduledWindowSeconds, liveWindowSeconds);
        }
        return liveWindowSeconds;
    }

    private float EstimateCurrentUplinkWindowSeconds()
    {
        SatelliteRouteEstimateRecord estimate = SelectedRouteEstimate(RequestedSendDelayMinutes, MessageSizeKb);
        if (estimate != null)
        {
            return Mathf.Max(0, estimate.originWindowSeconds);
        }
        return EstimateDownlinkSecondsToGroundStation();
    }

    private float EstimateCurrentDownlinkWindowSeconds()
    {
        if (SelectedTarget == null)
        {
            return 0f;
        }

        SatelliteData relaySatellite = SatelliteByName(string.IsNullOrEmpty(DestinationRelaySatelliteName) ? SelectedSatelliteName : DestinationRelaySatelliteName);
        float relayWindowSeconds = DestinationRelayWindowSeconds(relaySatellite, 0, MessageSizeKb);
        if (relayWindowSeconds >= 0f)
        {
            return relayWindowSeconds;
        }
        return EstimatePassSecondsToTarget(SelectedTarget);
    }

    private bool IsDownlinkWindowOpen()
    {
        if (SelectedTarget == null)
        {
            return false;
        }
        Transform relayTransform = DestinationRelaySatelliteTransform();
        if (relayTransform == null)
        {
            return false;
        }
        SatelliteData relaySatellite = SatelliteByName(string.IsNullOrEmpty(DestinationRelaySatelliteName) ? SelectedSatelliteName : DestinationRelaySatelliteName);
        float contactAngle = DownlinkContactAngleDegrees(relaySatellite, SelectedTarget);
        return SurfaceAngleFromTransformTo(relayTransform, SelectedTarget.transform) <= contactAngle;
    }

    private void UpdateCommunicationAvailability()
    {
        if (groundStation == null)
        {
            CommunicationAvailable = false;
            return;
        }

        CommunicationAvailable = SurfaceAngleTo(groundStation.transform) <= groundStation.communicationAngle;
    }

    private void UpdateMissionEstimate()
    {
        if (SelectedTarget == null || CurrentSatelliteTransform() == null)
        {
            EstimatedPassSeconds = 0f;
            EstimatedDownlinkSeconds = 0f;
            MissionQualityScore = 0;
            ServiceReliabilityScore = 0;
            EstimatedPriceCredits = 0;
            EstimatedPriceUsd = 0f;
            PriceBaseUsd = 0f;
            PricePayloadUsd = 0f;
            PriceUrgencyUsd = 0f;
            SuccessProbabilityPercent = 0;
            EstimatedDeliverySeconds = 0;
            return;
        }

        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        float targetAngle = SurfaceAngleTo(SelectedTarget.transform);
        SatelliteRouteEstimateRecord routeEstimate = SelectedRouteEstimate(RequestedSendDelayMinutes, MessageSizeKb);
        EstimatedPassSeconds = routeEstimate != null ? Mathf.Max(0, routeEstimate.originWindowSeconds) : EstimateCurrentUplinkWindowSeconds();
        EstimatedDownlinkSeconds = routeEstimate != null ? Mathf.Max(0, routeEstimate.targetWindowSeconds) : EstimateCurrentDownlinkWindowSeconds();

        EstimatedDeliverySeconds = routeEstimate != null
            ? Mathf.Max(0, routeEstimate.estimatedDeliverySeconds)
            : Mathf.CeilToInt(RequestedSendDelayMinutes * 60f + EstimatedPassSeconds + EstimatedDownlinkSeconds + CalculateTransferSeconds());

        float angleScore = Mathf.Clamp01(1f - targetAngle / 140f);
        float storageScore = 1f - Mathf.Clamp01(MessageSizeKb / 1024f);
        int congestionPenalty = selectedSatellite != null ? selectedSatellite.CongestionReliabilityPenalty : 0;
        MissionQualityScore = Mathf.Clamp(Mathf.RoundToInt(62f + angleScore * 26f + storageScore * 12f - SelectedTarget.weatherRiskPercent * 0.25f - congestionPenalty * 0.4f), 0, 100);
        ServiceReliabilityScore = Mathf.Clamp(Mathf.RoundToInt(98f - SelectedTarget.weatherRiskPercent * 0.6f - Mathf.Max(0f, EstimatedPassSeconds - 10f) * 0.03f - congestionPenalty), 1, 100);
        if (selectedSatellite != null)
        {
            EstimatedPriceCredits = selectedSatellite.EstimatePriceCredits(MessageSizeKb);
            EstimatedPriceUsd = EstimateDetailedPriceUsd(MessageSizeKb, RequestedSendDelayMinutes, selectedSatellite, out float baseFee, out float payloadFee, out float urgencyFee);
            PriceBaseUsd = baseFee;
            PricePayloadUsd = payloadFee;
            PriceUrgencyUsd = urgencyFee;
            SuccessProbabilityPercent = Mathf.Clamp(
                Mathf.RoundToInt((ServiceReliabilityScore + selectedSatellite.reliabilityPercent) * 0.5f - SelectedTarget.weatherRiskPercent * 0.2f - selectedSatellite.CongestionReliabilityPenalty),
                0,
                100);
        }
        UpdateSecurityStatus();
    }

    public float EstimateDetailedPriceUsd(int payloadKb, int delayMinutes, SatelliteData satellite, out float baseFee, out float payloadFee, out float urgencyFee)
    {
        baseFee = 0.50f;
        payloadFee = satellite != null ? satellite.EstimatePriceUsd(payloadKb) : Mathf.Max(0.25f, Mathf.CeilToInt(Mathf.Max(1, payloadKb) / 32f) * 0.25f);
        urgencyFee = delayMinutes == 0 ? 0.75f : delayMinutes <= 5 ? 0.50f : delayMinutes <= 15 ? 0.25f : 0f;
        return baseFee + payloadFee + urgencyFee;
    }

    public int EstimatePayloadTransferDurationSeconds(int payloadKb, SatelliteData satellite)
    {
        float latency = satellite != null ? satellite.latencySeconds : 6f;
        float congestionSeconds = satellite != null ? satellite.congestionPercent * 0.045f : 0f;
        float payloadSeconds = Mathf.Max(1f, Mathf.Max(1, payloadKb) / 48f);
        float uplinkSeconds = Mathf.Max(1f, payloadSeconds * 0.35f);
        float islSeconds = Mathf.Max(1.5f, 2.5f + congestionSeconds * 0.4f);
        float relaySelectionSeconds = Mathf.Max(0.8f, 1.5f + congestionSeconds * 0.1f);
        float downlinkSeconds = Mathf.Max(1.5f, latency * 0.45f + payloadSeconds + congestionSeconds * 0.5f);
        float finalReceiveSeconds = 0.8f;
        return Mathf.CeilToInt(Mathf.Max(1f, uplinkSeconds + islSeconds + relaySelectionSeconds + downlinkSeconds + finalReceiveSeconds));
    }

    public string PriceBreakdownText()
    {
        return "$" + EstimatedPriceUsd.ToString("0.00")
            + " (기본 $" + PriceBaseUsd.ToString("0.00")
            + " + 내용(32,768 Bytes 단위) $" + PricePayloadUsd.ToString("0.00")
            + " + 시간 $" + PriceUrgencyUsd.ToString("0.00") + ")";
    }

    public string GetStatisticsSummary()
    {
        return StatisticsSummary;
    }

    public string GetAverageCostText()
    {
        return "$" + AverageCostUsd.ToString("0.00");
    }

    public string GetAverageDelayText()
    {
        return FormatRemainingSeconds(AverageDelaySeconds);
    }

    public string GetAverageSuccessRateText()
    {
        return AverageSuccessRatePercent.ToString("0") + "%";
    }

    private void EnsureActiveSentMessageRecord()
    {
        if (!IsActiveMissionPhase(Phase) || deliveryRecorded)
        {
            return;
        }
        if (string.IsNullOrEmpty(activeSentMessageTrackingId))
        {
            activeSentMessageTrackingId = BuildTransmissionTrackingId(DateTime.Now);
        }

        int index;
        SentMessageRecord record = FindSentMessageByTrackingId(activeSentMessageTrackingId, out index);
        if (record == null)
        {
            record = new SentMessageRecord();
            record.trackingId = activeSentMessageTrackingId;
            SentMessages.Insert(0, record);
        }
        PopulateSentMessageRecord(record, false, DateTime.Now);
        TrimSentMessages();
    }

    private void RefreshActiveSentMessageRecord()
    {
        if (string.IsNullOrEmpty(activeSentMessageTrackingId))
        {
            return;
        }

        int index;
        SentMessageRecord record = FindSentMessageByTrackingId(activeSentMessageTrackingId, out index);
        if (record == null)
        {
            return;
        }
        PopulateSentMessageRecord(record, false, DateTime.Now);
    }

    private SentMessageRecord FindSentMessageByTrackingId(string trackingId, out int index)
    {
        index = -1;
        if (string.IsNullOrEmpty(trackingId))
        {
            return null;
        }

        for (int i = 0; i < SentMessages.Count; i++)
        {
            SentMessageRecord record = SentMessages[i];
            if (record != null && record.trackingId == trackingId)
            {
                index = i;
                return record;
            }
        }
        return null;
    }

    private void PopulateSentMessageRecord(SentMessageRecord record, bool complete, DateTime timestamp)
    {
        if (record == null)
        {
            return;
        }

        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        if (string.IsNullOrEmpty(record.trackingId))
        {
            record.trackingId = complete || string.IsNullOrEmpty(activeSentMessageTrackingId)
                ? BuildTransmissionTrackingId(timestamp)
                : activeSentMessageTrackingId;
        }
        record.deliveredAt = complete ? timestamp.ToString("MM/dd HH:mm") : string.Empty;
        record.scheduledLabel = ScheduledLabel;
        record.originName = OriginName;
        record.originLabel = OriginLabel;
        record.targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        record.destinationLabel = SelectedTarget != null ? DestinationLabel(SelectedTarget.targetName) : "수신 지역";
        record.satelliteName = selectedSatellite != null ? selectedSatellite.satelliteName : "선택 위성";
        record.messageText = MessageText ?? string.Empty;
        record.attachmentName = AttachmentName ?? string.Empty;
        record.attachmentSizeKb = AttachmentSizeKb;
        record.attachments = CloneAttachments(attachmentFiles);
        record.messageTextBytes = MessageTextBytes;
        record.payloadBytes = MessagePayloadBytes;
        record.payloadKb = MessageSizeKb;
        record.priceUsd = EstimatedPriceUsd;
        record.baseFeeUsd = PriceBaseUsd;
        record.payloadFeeUsd = PricePayloadUsd;
        record.urgencyFeeUsd = PriceUrgencyUsd;
        record.elapsedSeconds = complete
            ? Mathf.RoundToInt(Mathf.Max(0f, scheduledWaitElapsedSeconds) + Mathf.Max(0f, transferElapsedSeconds))
            : Mathf.RoundToInt(Mathf.Max(0f, scheduledWaitElapsedSeconds) + Mathf.Max(0f, transferElapsedSeconds));
        record.successProbabilityPercent = SuccessProbabilityPercent;
        record.destinationRelaySatelliteName = DestinationRelaySatelliteName ?? string.Empty;
        record.transmissionLog = string.Join("\n", MissionEventLog.ToArray());
        record.securitySummary = SecurityStatusLabel;
        record.checksumLabel = SecurityChecksumLabel;
        record.isComplete = complete;
        record.isActiveTransferRecord = !complete;
        record.statusLabel = complete ? "전달 완료" : ActiveSentRecordStatusLabel();
    }

    private string ActiveSentRecordStatusLabel()
    {
        switch (CommunicationStep)
        {
            case SatelliteCommunicationStep.Reserved:
            case SatelliteCommunicationStep.WaitingForUplink:
                return "업링크 대기";
            case SatelliteCommunicationStep.Uplink:
                return "업링크 중";
            case SatelliteCommunicationStep.LeoReceived:
            case SatelliteCommunicationStep.InterSatelliteLink:
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return "ISL 전달 중";
            case SatelliteCommunicationStep.Downlink:
            case SatelliteCommunicationStep.FinalReceive:
                return "다운링크 중";
            case SatelliteCommunicationStep.Failed:
                return "전송 실패";
            default:
                return "전달 중";
        }
    }

    private bool SentRecordIsComplete(SentMessageRecord record)
    {
        return record != null && (record.isComplete || (!string.IsNullOrEmpty(record.deliveredAt) && record.statusLabel != "전송 중"));
    }

    private int CompletedSentMessageCount()
    {
        int count = 0;
        for (int i = 0; i < SentMessages.Count; i++)
        {
            if (SentRecordIsComplete(SentMessages[i]))
            {
                count++;
            }
        }
        return count;
    }

    private void TrimSentMessages()
    {
        while (SentMessages.Count > 8)
        {
            SentMessages.RemoveAt(SentMessages.Count - 1);
        }
    }

    private void RecordDelivery()
    {
        if (deliveryRecorded)
        {
            return;
        }

        deliveryRecorded = true;
        DateTime deliveredTime = DateTime.Now;
        if (string.IsNullOrEmpty(activeSentMessageTrackingId))
        {
            activeSentMessageTrackingId = BuildTransmissionTrackingId(deliveredTime);
        }

        int index;
        SentMessageRecord record = FindSentMessageByTrackingId(activeSentMessageTrackingId, out index);
        if (record == null)
        {
            record = new SentMessageRecord();
            record.trackingId = activeSentMessageTrackingId;
            SentMessages.Insert(0, record);
        }
        else if (index > 0)
        {
            SentMessages.RemoveAt(index);
            SentMessages.Insert(0, record);
        }

        PopulateSentMessageRecord(record, true, deliveredTime);
        activeSentMessageTrackingId = string.Empty;
        TrimSentMessages();
        successfulTransmissionTotal = Mathf.Max(successfulTransmissionTotal + 1, CompletedSentMessageCount());
        RefreshTransmissionStats();
        SavePersistentState();
    }

    private string BuildTransmissionTrackingId(DateTime deliveredTime)
    {
        return "TRK-" + deliveredTime.ToString("MMdd-HHmmss");
    }

    private string SentMessageTrackingId(SentMessageRecord record)
    {
        if (record == null)
        {
            return string.Empty;
        }
        if (!string.IsNullOrEmpty(record.trackingId))
        {
            return record.trackingId;
        }
        if (!string.IsNullOrEmpty(record.deliveredAt))
        {
            return "TRK-" + record.deliveredAt.Replace("/", string.Empty).Replace(":", string.Empty).Replace(" ", string.Empty);
        }
        return "TRK-UNKNOWN";
    }

    private void RefreshTransmissionStats()
    {
        int completedCount = CompletedSentMessageCount();
        SuccessfulTransmissionCount = Mathf.Max(successfulTransmissionTotal, completedCount);
        FailedTransmissionCount = Mathf.Max(0, FailedTransmissionCount);
        TotalTransmissionCount = SuccessfulTransmissionCount + FailedTransmissionCount;

        float totalCost = 0f;
        float totalDelay = 0f;
        float totalProbability = 0f;
        Dictionary<string, int> satelliteUseCounts = new Dictionary<string, int>();
        for (int i = 0; i < SentMessages.Count; i++)
        {
            SentMessageRecord record = SentMessages[i];
            if (!SentRecordIsComplete(record))
            {
                continue;
            }

            totalCost += record.priceUsd;
            totalDelay += Mathf.Max(0, record.elapsedSeconds);
            totalProbability += Mathf.Clamp(record.successProbabilityPercent, 0, 100);
            string satelliteName = string.IsNullOrEmpty(record.satelliteName) ? "선택 위성" : record.satelliteName;
            if (!satelliteUseCounts.ContainsKey(satelliteName))
            {
                satelliteUseCounts[satelliteName] = 0;
            }
            satelliteUseCounts[satelliteName]++;
        }

        AverageCostUsd = completedCount == 0 ? 0f : totalCost / completedCount;
        AverageDelaySeconds = completedCount == 0 ? 0f : totalDelay / completedCount;
        AverageSuccessProbabilityPercent = completedCount == 0 ? 0f : totalProbability / completedCount;
        AverageSuccessRatePercent = TotalTransmissionCount == 0 ? 0f : SuccessfulTransmissionCount * 100f / TotalTransmissionCount;
        MostUsedSatelliteName = "없음";
        int bestCount = 0;
        foreach (KeyValuePair<string, int> pair in satelliteUseCounts)
        {
            if (pair.Value > bestCount)
            {
                bestCount = pair.Value;
                MostUsedSatelliteName = pair.Key;
            }
        }

        StatisticsSummary = TotalTransmissionCount == 0
            ? "전송 기록 없음"
            : "총 " + TotalTransmissionCount + "건 · 성공 " + SuccessfulTransmissionCount + "건 · 평균 비용 $" + AverageCostUsd.ToString("0.00")
                + " · 평균 지연 " + FormatRemainingSeconds(AverageDelaySeconds)
                + " · 최다 위성 " + MostUsedSatelliteName
                + " · 성공률 " + AverageSuccessRatePercent.ToString("0") + "%";
    }

    private void UpdateRouteReplay()
    {
        if (!IsReplayingRoute)
        {
            return;
        }
        if (!IsDemoRouteReplayEnabled)
        {
            ClearRouteReplayState();
            return;
        }
        if (IsRouteReplayPaused)
        {
            return;
        }

        RouteReplayProgress = Mathf.Clamp01(RouteReplayProgress + Time.deltaTime * RouteReplaySpeed / 4f);
        if (RouteReplayProgress >= 1f)
        {
            IsReplayingRoute = false;
            IsRouteReplayPaused = false;
            ResultMessage = "통신 과정 재생 완료.";
        }
        NotifyChanged();
    }

    private bool BlockRouteReplayOutsideDemo()
    {
        if (IsDemoRouteReplayEnabled)
        {
            return false;
        }

        ClearRouteReplayState();
        ResultMessage = "통신 과정 재생/배속은 시연 기능에서만 사용할 수 있습니다.";
        NotifyChanged();
        return true;
    }

    private void SetDemoRouteReplayEnabledInternal(bool enabled)
    {
        IsDemoRouteReplayEnabled = enabled;
        if (enabled && RouteReplaySpeed < DemoDefaultTimeScale)
        {
            RouteReplaySpeed = DemoDefaultTimeScale;
        }
        if (!enabled)
        {
            ClearRouteReplayState();
            RouteReplaySpeed = 1f;
        }
    }

    private void ClearRouteReplayState()
    {
        IsReplayingRoute = false;
        IsRouteReplayPaused = false;
        RouteReplayProgress = 0f;
    }

    private int FindNextStartableQueuedMessageIndex()
    {
        for (int i = 0; i < QueuedMessages.Count; i++)
        {
            if (IsQueuedMessageStartable(i))
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsQueuedMessageStartable(int index)
    {
        if (!IsValidQueuedMessageIndex(index))
        {
            return false;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        if (Targets.Find(item => item.targetName == queued.targetName) == null)
        {
            return false;
        }
        if (!string.IsNullOrEmpty(queued.originName) && Targets.Find(item => item.targetName == queued.originName) == null)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(queued.messageText) || queued.payloadKb > MaxPayloadKb)
        {
            return false;
        }

        string queuedOriginName = string.IsNullOrEmpty(queued.originName) ? OriginName : queued.originName;
        int effectiveDelayMinutes = GetEffectiveQueuedDelayMinutes(queued);
        return FindPreferredOrBestSatelliteForQueuedMessage(queued, effectiveDelayMinutes, queuedOriginName) != null;
    }

    private void UpdateAutoQueue()
    {
        if (!AutoStartQueuedMessages || FindNextStartableQueuedMessageIndex() < 0 || Phase != MissionPhase.Complete)
        {
            AutoQueueCountdown = -1f;
            lastAutoCountdownSecond = -1;
            return;
        }

        if (AutoQueueCountdown < 0f)
        {
            AutoQueueCountdown = 3f;
        }

        AutoQueueCountdown = Mathf.Max(0f, AutoQueueCountdown - Time.deltaTime);
        int seconds = Mathf.CeilToInt(AutoQueueCountdown);
        if (seconds != lastAutoCountdownSecond)
        {
            lastAutoCountdownSecond = seconds;
            ResultMessage = "다음 메시지를 " + seconds + "초 뒤 자동 전송합니다.";
            NotifyChanged();
        }

        if (AutoQueueCountdown <= 0f)
        {
            AutoQueueCountdown = -1f;
            lastAutoCountdownSecond = -1;
            StartNextQueuedMessage();
        }
    }

    private void UpdatePayloadSize()
    {
        MessageTextBytes = GetUtf8ByteCount(MessageText);
        MessagePayloadBytes = CalculatePayloadBytes(MessageTextBytes, attachmentFiles);
        MessageSizeKb = CalculatePayloadKbFromBytes(MessagePayloadBytes);
        PayloadTooLarge = MessageSizeKb > MaxPayloadKb;
    }

    private int GetUtf8ByteCount(string text)
    {
        return string.IsNullOrEmpty(text) ? 0 : Encoding.UTF8.GetByteCount(text);
    }

    private string FormatBytes(long bytes)
    {
        return Math.Max(0L, bytes).ToString("N0") + " Bytes";
    }

    private string PayloadKbText(int sizeKb)
    {
        return FormatBytes(KilobytesToBytes(sizeKb));
    }

    private int CalculatePayloadKb(int textBytes, int totalAttachmentSizeKb)
    {
        long totalBytes = (long)Mathf.Max(0, textBytes) + KilobytesToBytes(totalAttachmentSizeKb);
        return CalculatePayloadKbFromBytes(totalBytes);
    }

    private int CalculatePayloadKbFromBytes(long totalBytes)
    {
        if (totalBytes <= 0)
        {
            return 0;
        }

        long payloadKb = (totalBytes + 1023L) / 1024L;
        return payloadKb > int.MaxValue ? int.MaxValue : (int)payloadKb;
    }

    private int CalculatePayloadBytes(int textBytes, List<MessageAttachmentRecord> attachments)
    {
        long totalBytes = (long)Mathf.Max(0, textBytes) + TotalAttachmentBytes(attachments);
        return totalBytes > int.MaxValue ? int.MaxValue : (int)totalBytes;
    }

    private int KilobytesToBytes(int sizeKb)
    {
        long bytes = (long)Mathf.Max(0, sizeKb) * 1024L;
        return bytes > int.MaxValue ? int.MaxValue : (int)bytes;
    }

    private int TotalAttachmentSize(List<MessageAttachmentRecord> attachments)
    {
        long total = 0L;
        if (attachments == null)
        {
            return 0;
        }

        for (int i = 0; i < attachments.Count; i++)
        {
            if (attachments[i] != null)
            {
                total += Mathf.Max(0, attachments[i].sizeKb);
                if (total > int.MaxValue)
                {
                    return int.MaxValue;
                }
            }
        }
        return (int)total;
    }

    private int TotalAttachmentBytes(List<MessageAttachmentRecord> attachments)
    {
        long total = 0L;
        if (attachments == null)
        {
            return 0;
        }

        for (int i = 0; i < attachments.Count; i++)
        {
            if (attachments[i] != null)
            {
                total += AttachmentSizeBytes(attachments[i]);
                if (total > int.MaxValue)
                {
                    return int.MaxValue;
                }
            }
        }
        return (int)total;
    }

    private int AttachmentSizeBytes(MessageAttachmentRecord attachment)
    {
        if (attachment == null)
        {
            return 0;
        }
        if (attachment.sizeBytes > 0)
        {
            return attachment.sizeBytes;
        }
        return KilobytesToBytes(attachment.sizeKb);
    }

    private List<MessageAttachmentRecord> CloneAttachments(List<MessageAttachmentRecord> attachments)
    {
        List<MessageAttachmentRecord> cloned = new List<MessageAttachmentRecord>();
        if (attachments == null)
        {
            return cloned;
        }

        for (int i = 0; i < attachments.Count; i++)
        {
            MessageAttachmentRecord attachment = attachments[i];
            string safeName = NormalizeAttachmentFileName(attachment != null ? attachment.fileName : string.Empty);
            int safeBytes = AttachmentSizeBytes(attachment);
            if (attachment == null || string.IsNullOrEmpty(safeName) || (attachment.sizeKb <= 0 && safeBytes <= 0))
            {
                continue;
            }
            cloned.Add(new MessageAttachmentRecord(safeName, attachment.sizeKb, safeBytes));
        }
        return cloned;
    }

    private List<MessageAttachmentRecord> AttachmentsOrLegacy(List<MessageAttachmentRecord> attachments, string legacyAttachmentName, int legacyAttachmentSizeKb)
    {
        List<MessageAttachmentRecord> cloned = CloneAttachments(attachments);
        if (cloned.Count == 0 && legacyAttachmentSizeKb > 0)
        {
            string safeName = string.IsNullOrEmpty(legacyAttachmentName) ? "첨부 파일" : legacyAttachmentName;
            cloned.Add(new MessageAttachmentRecord(safeName, legacyAttachmentSizeKb));
        }
        return cloned;
    }

    private void SetAttachmentsInternal(List<MessageAttachmentRecord> attachments)
    {
        attachmentFiles.Clear();
        List<MessageAttachmentRecord> cloned = CloneAttachments(attachments);
        for (int i = 0; i < cloned.Count; i++)
        {
            attachmentFiles.Add(cloned[i]);
        }
        UpdateAttachmentSummary();
        UpdatePayloadSize();
    }

    private string BuildAttachmentDisplayName(List<MessageAttachmentRecord> attachments)
    {
        List<MessageAttachmentRecord> safeAttachments = CloneAttachments(attachments);
        if (safeAttachments.Count == 0)
        {
            return string.Empty;
        }

        string firstName = CompactAttachmentFileName(safeAttachments[0].fileName);
        return safeAttachments.Count == 1 ? firstName : firstName + " 외 " + (safeAttachments.Count - 1) + "개";
    }

    private string BuildAttachmentDisplaySummary(List<MessageAttachmentRecord> attachments)
    {
        List<MessageAttachmentRecord> safeAttachments = CloneAttachments(attachments);
        int totalSizeKb = TotalAttachmentSize(safeAttachments);
        if (safeAttachments.Count == 0)
        {
            return "첨부 없음";
        }

        string displayName = BuildAttachmentDisplayName(safeAttachments);
        if (safeAttachments.Count == 1)
        {
            return displayName + " · " + PayloadKbText(totalSizeKb);
        }
        return displayName + " · 총 " + safeAttachments.Count + "개 · " + PayloadKbText(totalSizeKb);
    }

    private string NormalizeAttachmentFileName(string fileName)
    {
        return string.IsNullOrWhiteSpace(fileName) ? string.Empty : fileName.Trim();
    }

    private string CompactAttachmentFileName(string fileName)
    {
        string safeName = NormalizeAttachmentFileName(fileName);
        if (string.IsNullOrEmpty(safeName))
        {
            return "첨부 파일";
        }
        if (safeName.Length <= AttachmentNameSummaryMaxChars)
        {
            return safeName;
        }

        string extension = string.Empty;
        int dotIndex = safeName.LastIndexOf('.');
        if (dotIndex > 0 && dotIndex < safeName.Length - 1 && safeName.Length - dotIndex <= 8)
        {
            extension = safeName.Substring(dotIndex);
        }

        int prefixLength = Mathf.Max(8, AttachmentNameSummaryMaxChars - extension.Length - 1);
        prefixLength = Mathf.Min(prefixLength, safeName.Length);
        if (prefixLength > 0 && char.IsHighSurrogate(safeName[prefixLength - 1]))
        {
            prefixLength--;
        }
        if (prefixLength <= 0)
        {
            return "…" + extension;
        }
        return safeName.Substring(0, prefixLength) + "…" + extension;
    }

    private void UpdateAttachmentSummary()
    {
        AttachmentCount = attachmentFiles.Count;
        TotalAttachmentSizeKb = TotalAttachmentSize(attachmentFiles);
        AttachmentSizeKb = TotalAttachmentSizeKb;
        if (AttachmentCount == 0)
        {
            AttachmentName = string.Empty;
            AttachmentSummary = "첨부 없음";
            return;
        }

        AttachmentName = BuildAttachmentDisplayName(attachmentFiles);
        AttachmentSummary = BuildAttachmentDisplaySummary(attachmentFiles);
    }

    private void ApplyQueuedAttachments(QueuedMessageRecord queued)
    {
        if (queued == null)
        {
            SetAttachment(string.Empty, 0);
            return;
        }
        SetAttachments(AttachmentsOrLegacy(queued.attachments, queued.attachmentName, queued.attachmentSizeKb));
    }

    private void ApplyQueuedSchedule(QueuedMessageRecord queued)
    {
        int effectiveDelayMinutes = GetEffectiveQueuedDelayMinutes(queued);
        if (effectiveDelayMinutes <= 0)
        {
            SetSendDelayMinutes(0);
            return;
        }

        DateTime scheduled;
        if (TryGetQueuedScheduledDateTime(queued, out scheduled) && scheduled > DateTime.Now)
        {
            DateTime now = DateTime.Now;
            int dayOffset = Mathf.Clamp((scheduled.Date - now.Date).Days, 0, MaxScheduledDayOffset);
            SetScheduledDateTime(dayOffset, scheduled.Hour, scheduled.Minute);
            return;
        }

        SetSendDelayMinutes(effectiveDelayMinutes);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string targetName, List<MessageAttachmentRecord> attachments, int requestedSendDelayMinutes, out QueuedMessageRecord record, out string errorMessage)
    {
        return TryBuildQueuedMessageRecord(messageText, OriginName, targetName, attachments, requestedSendDelayMinutes, out record, out errorMessage);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string originName, string targetName, List<MessageAttachmentRecord> attachments, int requestedSendDelayMinutes, out QueuedMessageRecord record, out string errorMessage)
    {
        int safeDelayMinutes;
        int scheduledDayOffset;
        int scheduledHour;
        int scheduledMinute;
        string scheduledLabel;
        BuildScheduleFromDelay(requestedSendDelayMinutes, out safeDelayMinutes, out scheduledDayOffset, out scheduledHour, out scheduledMinute, out scheduledLabel);
        return TryBuildQueuedMessageRecord(messageText, originName, targetName, attachments, safeDelayMinutes, scheduledDayOffset, scheduledHour, scheduledMinute, scheduledLabel, out record, out errorMessage);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string targetName, List<MessageAttachmentRecord> attachments, int scheduledDayOffset, int scheduledHour, int scheduledMinute, out QueuedMessageRecord record, out string errorMessage)
    {
        return TryBuildQueuedMessageRecord(messageText, OriginName, targetName, attachments, scheduledDayOffset, scheduledHour, scheduledMinute, out record, out errorMessage);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string originName, string targetName, List<MessageAttachmentRecord> attachments, int scheduledDayOffset, int scheduledHour, int scheduledMinute, out QueuedMessageRecord record, out string errorMessage)
    {
        int safeDelayMinutes;
        int safeDayOffset;
        int safeHour;
        int safeMinute;
        string scheduledLabel;
        BuildScheduleFromDateTime(scheduledDayOffset, scheduledHour, scheduledMinute, out safeDelayMinutes, out safeDayOffset, out safeHour, out safeMinute, out scheduledLabel);
        return TryBuildQueuedMessageRecord(messageText, originName, targetName, attachments, safeDelayMinutes, safeDayOffset, safeHour, safeMinute, scheduledLabel, out record, out errorMessage);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string targetName, List<MessageAttachmentRecord> attachments, int requestedSendDelayMinutes, int scheduledDayOffset, int scheduledHour, int scheduledMinute, string scheduledLabel, out QueuedMessageRecord record, out string errorMessage)
    {
        return TryBuildQueuedMessageRecord(messageText, OriginName, targetName, attachments, requestedSendDelayMinutes, scheduledDayOffset, scheduledHour, scheduledMinute, scheduledLabel, out record, out errorMessage);
    }

    private bool TryBuildQueuedMessageRecord(string messageText, string originName, string targetName, List<MessageAttachmentRecord> attachments, int requestedSendDelayMinutes, int scheduledDayOffset, int scheduledHour, int scheduledMinute, string scheduledLabel, out QueuedMessageRecord record, out string errorMessage)
    {
        record = null;
        errorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(messageText))
        {
            errorMessage = "메시지를 입력하세요.";
            return false;
        }

        ObservationTarget target = Targets.Find(item => item.targetName == targetName);
        if (target == null)
        {
            errorMessage = "목적지를 선택하세요.";
            return false;
        }
        ObservationTarget origin = Targets.Find(item => item.targetName == originName);
        if (origin == null)
        {
            errorMessage = "발신지를 선택하세요.";
            return false;
        }

        List<MessageAttachmentRecord> safeAttachments = CloneAttachments(attachments);
        int textBytes = GetUtf8ByteCount(messageText);
        int payloadKb = CalculatePayloadKbFromBytes(CalculatePayloadBytes(textBytes, safeAttachments));
        if (payloadKb > MaxPayloadKb)
        {
            errorMessage = "크기 초과: " + PayloadKbText(payloadKb) + " / " + PayloadKbText(MaxPayloadKb);
            return false;
        }

        SatelliteData bestSatellite = FindBestSatelliteForRoute(origin.targetName, targetName, requestedSendDelayMinutes, payloadKb);
        bool routeAvailable = CanServeRoute(bestSatellite, origin.targetName, targetName, requestedSendDelayMinutes, payloadKb);
        float baseFee = 0f;
        float payloadFee = 0f;
        float urgencyFee = 0f;
        float totalFee = routeAvailable ? EstimateDetailedPriceUsd(payloadKb, requestedSendDelayMinutes, bestSatellite, out baseFee, out payloadFee, out urgencyFee) : 0f;
        record = new QueuedMessageRecord();
        record.queuedAt = ToQueuedAtIsoString(DateTime.Now);
        record.originName = origin.targetName;
        record.originLabel = DestinationLabel(origin.targetName);
        record.targetName = targetName;
        record.destinationLabel = DestinationLabel(targetName);
        record.messageText = messageText;
        record.requestedSendDelayMinutes = Mathf.Max(0, requestedSendDelayMinutes);
        record.scheduledDayOffset = Mathf.Clamp(scheduledDayOffset, 0, MaxScheduledDayOffset);
        record.scheduledHour = Mathf.Clamp(scheduledHour, 0, 23);
        record.scheduledMinute = Mathf.Clamp(scheduledMinute, 0, 59);
        record.scheduledLabel = string.IsNullOrEmpty(scheduledLabel) ? "지금 보내기" : scheduledLabel;
        record.attachments = safeAttachments;
        ApplyAttachmentSummaryToRecord(record);
        record.messageTextBytes = textBytes;
        record.payloadKb = payloadKb;
        record.priceUsd = totalFee;
        record.baseFeeUsd = baseFee;
        record.payloadFeeUsd = payloadFee;
        record.urgencyFeeUsd = urgencyFee;
        return true;
    }

    private string BuildQueuedMessageAddedStatus(QueuedMessageRecord record)
    {
        string queueSummary = GetQueueSummary();
        if (IsActiveMissionPhase(Phase))
        {
            return "현재 전송은 그대로 진행됩니다. 새 메시지는 메시지 목록에 추가했고 완료 후 시작할 수 있습니다. "
                + queueSummary + " · " + BuildQueuedMessageInlineSummary(record);
        }

        return "새 메시지를 메시지 목록에 추가했습니다. " + queueSummary + " · " + BuildQueuedMessageInlineSummary(record);
    }

    private string BuildQueuedMessageInlineSummary(QueuedMessageRecord queued)
    {
        if (queued == null)
        {
            return "메시지";
        }

        string origin = string.IsNullOrEmpty(queued.originLabel) ? OriginLabel : queued.originLabel;
        string destination = string.IsNullOrEmpty(queued.destinationLabel) ? "수신 지역" : queued.destinationLabel;
        return origin + " → " + destination
            + SameRegionVerificationNote(queued.originName, queued.targetName)
            + " · " + PayloadKbText(queued.payloadKb);
    }

    private List<string> BuildTransmissionStepSummaries(List<string> eventLog)
    {
        List<string> summaries = new List<string>();
        if (eventLog == null)
        {
            return summaries;
        }

        for (int i = 0; i < eventLog.Count; i++)
        {
            AddTransmissionStepSummary(summaries, eventLog[i]);
        }
        return summaries;
    }

    private List<string> BuildTransmissionStepSummaries(string transmissionLog)
    {
        List<string> summaries = new List<string>();
        if (string.IsNullOrEmpty(transmissionLog))
        {
            return summaries;
        }

        string[] lines = transmissionLog.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            AddTransmissionStepSummary(summaries, lines[i]);
        }
        return summaries;
    }

    private void AddTransmissionStepSummary(List<string> summaries, string rawEvent)
    {
        if (summaries == null || string.IsNullOrEmpty(rawEvent))
        {
            return;
        }

        string message = rawEvent.Length > 9 ? rawEvent.Substring(9) : rawEvent;
        string summary;
        if (message.Contains("예약 접수"))
        {
            summary = "예약을 접수했습니다.";
        }
        else if (message.Contains("업링크 대기") || message.Contains("발신 시작") || message.Contains("전송 시작"))
        {
            summary = SenderLabel() + "에서 LEO 위성으로 업링크를 시작했습니다.";
        }
        else if (message.Contains("예약 알림"))
        {
            summary = "예약 시간이 가까워졌습니다.";
        }
        else if (message.Contains("LEO 수신 완료") || message.Contains("발신 완료") || message.Contains("서울→위성 전송 완료"))
        {
            summary = "LEO 수신 완료: 업링크 메시지가 위성에 도착했습니다.";
        }
        else if (message.Contains("ISL 전달 시작"))
        {
            summary = "ISL 전달: 위성 간 링크로 메시지를 넘깁니다.";
        }
        else if (message.Contains("목적지 근처 위성 선택"))
        {
            summary = "목적지 근처 위성을 선택했습니다.";
        }
        else if (message.Contains("다운링크 시작"))
        {
            summary = "다운링크: 목적지 근처 위성에서 수신지로 보냅니다.";
        }
        else if (message.Contains("수신 시작") || message.Contains("수신 전송 시작"))
        {
            summary = "다운링크 전송을 시작했습니다.";
        }
        else if (message.Contains("최종 수신") || message.Contains("수신 완료") || message.Contains("도착 확인"))
        {
            summary = "도착 확인 완료: 목적지 도착을 확인했습니다.";
        }
        else if (message.Contains("전송 실패"))
        {
            summary = message;
        }
        else if (message.Contains("실패 재시도"))
        {
            summary = "실패한 메시지를 다시 시도했습니다.";
        }
        else
        {
            summary = message;
        }

        if (!summaries.Contains(summary))
        {
            summaries.Add(summary);
        }
    }

    private void BuildAttachmentSummary(List<MessageAttachmentRecord> attachments, out string attachmentName, out int attachmentSizeKb)
    {
        List<MessageAttachmentRecord> safeAttachments = CloneAttachments(attachments);
        attachmentSizeKb = TotalAttachmentSize(safeAttachments);
        attachmentName = BuildAttachmentDisplayName(safeAttachments);
    }

    private void ApplyAttachmentSummaryToRecord(QueuedMessageRecord record)
    {
        BuildAttachmentSummary(record.attachments, out record.attachmentName, out record.attachmentSizeKb);
    }

    private void ApplyAttachmentSummaryToRecord(MessageTemplateRecord record)
    {
        BuildAttachmentSummary(record.attachments, out record.attachmentName, out record.attachmentSizeKb);
    }

    private void NormalizeSentMessageRecord(SentMessageRecord record)
    {
        record.attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
        BuildAttachmentSummary(record.attachments, out record.attachmentName, out record.attachmentSizeKb);
        record.messageTextBytes = GetUtf8ByteCount(record.messageText);
        record.payloadBytes = CalculatePayloadBytes(record.messageTextBytes, record.attachments);
        record.payloadKb = CalculatePayloadKbFromBytes(record.payloadBytes);
        if (string.IsNullOrEmpty(record.originName) || Targets.Find(item => item.targetName == record.originName) == null)
        {
            record.originName = OrbitShareDestinations.Seoul;
        }
        if (string.IsNullOrEmpty(record.originLabel))
        {
            record.originLabel = DestinationLabel(record.originName);
        }
        if (string.IsNullOrEmpty(record.destinationRelaySatelliteName))
        {
            record.destinationRelaySatelliteName = record.satelliteName;
        }
        if (!record.isActiveTransferRecord && !string.IsNullOrEmpty(record.deliveredAt))
        {
            record.isComplete = true;
        }
        if (string.IsNullOrEmpty(record.statusLabel))
        {
            record.statusLabel = record.isComplete || !string.IsNullOrEmpty(record.deliveredAt) ? "전달 완료" : "전달 중";
        }
        record.trackingId = SentMessageTrackingId(record);
    }

    private void NormalizeQueuedMessageRecord(QueuedMessageRecord record)
    {
        record.attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
        ApplyAttachmentSummaryToRecord(record);
        if (string.IsNullOrEmpty(record.originName) || Targets.Find(item => item.targetName == record.originName) == null)
        {
            record.originName = OrbitShareDestinations.Seoul;
        }
        if (string.IsNullOrEmpty(record.originLabel))
        {
            record.originLabel = DestinationLabel(record.originName);
        }
        record.messageTextBytes = GetUtf8ByteCount(record.messageText);
        record.payloadKb = CalculatePayloadKbFromBytes(CalculatePayloadBytes(record.messageTextBytes, record.attachments));
        record.requestedSendDelayMinutes = Mathf.Max(0, record.requestedSendDelayMinutes);
        record.preferredSatelliteName = record.preferredSatelliteName ?? string.Empty;
        record.isHeld = false;
        record.queuedAt = NormalizeQueuedAtText(record.queuedAt);
        if (string.IsNullOrEmpty(record.scheduledLabel))
        {
            int delayMinutes;
            int dayOffset;
            int hour;
            int minute;
            string label;
            BuildScheduleFromDelay(record.requestedSendDelayMinutes, out delayMinutes, out dayOffset, out hour, out minute, out label);
            record.requestedSendDelayMinutes = delayMinutes;
            record.scheduledDayOffset = dayOffset;
            record.scheduledHour = hour;
            record.scheduledMinute = minute;
            record.scheduledLabel = label;
        }
        else
        {
            record.scheduledDayOffset = Mathf.Clamp(record.scheduledDayOffset, 0, MaxScheduledDayOffset);
            record.scheduledHour = Mathf.Clamp(record.scheduledHour, 0, 23);
            record.scheduledMinute = Mathf.Clamp(record.scheduledMinute, 0, 59);
        }
        int effectiveDelayMinutes = GetEffectiveQueuedDelayMinutes(record);
        SatelliteData bestSatellite = FindBestSatelliteForRoute(record.originName, record.targetName, effectiveDelayMinutes, record.payloadKb);
        if (CanServeRoute(bestSatellite, record.originName, record.targetName, effectiveDelayMinutes, record.payloadKb))
        {
            record.priceUsd = EstimateDetailedPriceUsd(record.payloadKb, effectiveDelayMinutes, bestSatellite, out record.baseFeeUsd, out record.payloadFeeUsd, out record.urgencyFeeUsd);
        }
        else
        {
            record.priceUsd = 0f;
            record.baseFeeUsd = 0f;
            record.payloadFeeUsd = 0f;
            record.urgencyFeeUsd = 0f;
        }
    }

    private void NormalizeMessageTemplateRecord(MessageTemplateRecord record)
    {
        record.attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
        ApplyAttachmentSummaryToRecord(record);
        if (string.IsNullOrEmpty(record.originName) || Targets.Find(item => item.targetName == record.originName) == null)
        {
            record.originName = OrbitShareDestinations.Seoul;
        }
        if (string.IsNullOrEmpty(record.originLabel))
        {
            record.originLabel = DestinationLabel(record.originName);
        }
        record.messageTextBytes = GetUtf8ByteCount(record.messageText);
        record.payloadKb = CalculatePayloadKbFromBytes(CalculatePayloadBytes(record.messageTextBytes, record.attachments));
    }

    private void BuildScheduleFromDelay(int requestedSendDelayMinutes, out int safeDelayMinutes, out int scheduledDayOffset, out int scheduledHour, out int scheduledMinute, out string scheduledLabel)
    {
        DateTime now = DateTime.Now;
        safeDelayMinutes = Mathf.Max(0, requestedSendDelayMinutes);
        DateTime scheduled = now.AddMinutes(safeDelayMinutes);
        scheduledDayOffset = Mathf.Clamp((scheduled.Date - now.Date).Days, 0, MaxScheduledDayOffset);
        scheduledHour = scheduled.Hour;
        scheduledMinute = scheduled.Minute - scheduled.Minute % 5;
        scheduledLabel = safeDelayMinutes == 0 ? "지금 보내기" : scheduled.ToString("MM/dd HH:mm") + " 보내기";
    }

    private void BuildScheduleFromDateTime(int dayOffset, int hour, int minute, out int safeDelayMinutes, out int scheduledDayOffset, out int scheduledHour, out int scheduledMinute, out string scheduledLabel)
    {
        DateTime now = DateTime.Now;
        DateTime scheduled = now.Date.AddDays(Mathf.Clamp(dayOffset, 0, MaxScheduledDayOffset)).AddHours(Mathf.Clamp(hour, 0, 23)).AddMinutes(Mathf.Clamp(minute, 0, 59));
        safeDelayMinutes = Mathf.Max(0, Mathf.CeilToInt((float)(scheduled - now).TotalMinutes));
        if (safeDelayMinutes == 0)
        {
            scheduled = now;
        }
        scheduledDayOffset = Mathf.Clamp((scheduled.Date - now.Date).Days, 0, MaxScheduledDayOffset);
        scheduledHour = scheduled.Hour;
        scheduledMinute = scheduled.Minute - scheduled.Minute % 5;
        scheduledLabel = safeDelayMinutes == 0 ? "지금 보내기" : scheduled.ToString("MM/dd HH:mm") + " 보내기";
    }

    private string SafeQueuedScheduleLabel(QueuedMessageRecord queued)
    {
        if (queued == null)
        {
            return "지금 보내기";
        }
        if (!string.IsNullOrEmpty(queued.scheduledLabel))
        {
            return queued.scheduledLabel;
        }
        return queued.requestedSendDelayMinutes == 0 ? "지금 보내기" : "+" + queued.requestedSendDelayMinutes + "분";
    }

    private string EffectiveQueuedScheduleLabel(QueuedMessageRecord queued)
    {
        return GetEffectiveQueuedDelayMinutes(queued) == 0 ? "지금 보내기" : SafeQueuedScheduleLabel(queued);
    }

    private int GetEffectiveQueuedDelayMinutes(QueuedMessageRecord queued)
    {
        if (queued == null)
        {
            return 0;
        }

        DateTime scheduled;
        if (TryGetQueuedScheduledDateTime(queued, out scheduled))
        {
            return Mathf.Max(0, Mathf.CeilToInt((float)(scheduled - DateTime.Now).TotalMinutes));
        }
        return Mathf.Max(0, queued.requestedSendDelayMinutes);
    }

    private bool TryGetQueuedScheduledDateTime(QueuedMessageRecord queued, out DateTime scheduled)
    {
        scheduled = DateTime.Now;
        if (queued == null)
        {
            return false;
        }
        if (queued.requestedSendDelayMinutes <= 0)
        {
            return true;
        }

        DateTime queuedAt;
        if (!TryParseQueuedAt(queued.queuedAt, out queuedAt))
        {
            scheduled = DateTime.Now.AddMinutes(queued.requestedSendDelayMinutes);
            return true;
        }

        scheduled = queuedAt.AddMinutes(queued.requestedSendDelayMinutes);
        return true;
    }

    private bool TryParseQueuedAt(string queuedAtText, out DateTime queuedAt)
    {
        queuedAt = DateTime.Now;
        if (string.IsNullOrEmpty(queuedAtText))
        {
            return false;
        }

        DateTime parsed;
        string yearPrefixed = DateTime.Now.Year + "/" + queuedAtText;
        if (!DateTime.TryParse(yearPrefixed, out parsed) && !DateTime.TryParse(queuedAtText, out parsed))
        {
            return false;
        }
        if (parsed > DateTime.Now.AddDays(3))
        {
            parsed = parsed.AddYears(-1);
        }

        queuedAt = parsed;
        return true;
    }

    private string NormalizeQueuedAtText(string queuedAtText)
    {
        DateTime queuedAt;
        return TryParseQueuedAt(queuedAtText, out queuedAt)
            ? ToQueuedAtIsoString(queuedAt)
            : ToQueuedAtIsoString(DateTime.Now);
    }

    private string ToQueuedAtIsoString(DateTime queuedAt)
    {
        return queuedAt.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    private float EstimatePassSecondsToTarget(ObservationTarget target)
    {
        if (target == null || CurrentSatelliteTransform() == null || earth == null)
        {
            return 0f;
        }

        float effectiveDegreesPerSecond = CurrentSatelliteDegreesPerSecond();
        float targetAngle = SurfaceAngleTo(target.transform);
        return Mathf.Max(0f, targetAngle - DownlinkContactAngleDegrees(CurrentUplinkSatellite(), target)) / effectiveDegreesPerSecond;
    }

    private float EstimatePassSecondsToTarget(ObservationTarget target, SatelliteData satellite, int requestedDelayMinutes, int payloadKb)
    {
        if (target == null || satellite == null || satelliteManager == null)
        {
            return EstimatePassSecondsToTarget(target);
        }

        SatelliteRouteEstimateRecord estimate = satelliteManager.GetRouteEstimate(satellite, target.targetName, requestedDelayMinutes, payloadKb);
        return estimate != null ? estimate.estimatedWindowSeconds : EstimatePassSecondsToTarget(target);
    }

    private float EstimateDownlinkSecondsToGroundStation()
    {
        if (groundStation == null || CurrentSatelliteTransform() == null || earth == null)
        {
            return 0f;
        }

        float effectiveDegreesPerSecond = CurrentSatelliteDegreesPerSecond();
        float stationGap = Mathf.Max(0f, SurfaceAngleTo(groundStation.transform) - groundStation.communicationAngle);
        return stationGap / effectiveDegreesPerSecond;
    }

    private float EstimateTransferSeconds(int payloadKb, float estimatedDownlinkSeconds, SatelliteData satellite)
    {
        float latency = satellite != null ? satellite.latencySeconds : 6f;
        float congestionSeconds = satellite != null ? satellite.congestionPercent * 0.045f : 0f;
        float payloadSeconds = Mathf.Max(1f, Mathf.Max(0, payloadKb) / 48f);
        float islSeconds = Mathf.Max(1.5f, 2.5f + congestionSeconds * 0.4f);
        float relaySelectionSeconds = Mathf.Max(0.8f, 1.5f + congestionSeconds * 0.1f);
        float downlinkSeconds = Mathf.Max(1.5f, latency * 0.45f + payloadSeconds + congestionSeconds * 0.5f + Mathf.Max(0f, estimatedDownlinkSeconds));
        float finalReceiveSeconds = 0.8f;
        return Mathf.Max(1f, islSeconds + relaySelectionSeconds + downlinkSeconds + finalReceiveSeconds);
    }

    private int EstimateSuccessProbability(ObservationTarget target, SatelliteData satellite, float estimatedPassSeconds)
    {
        if (target == null || satellite == null)
        {
            return 0;
        }

        int serviceReliability = Mathf.Clamp(Mathf.RoundToInt(98f - target.weatherRiskPercent * 0.6f - Mathf.Max(0f, estimatedPassSeconds - 10f) * 0.03f - satellite.CongestionReliabilityPenalty), 0, 100);
        return Mathf.Clamp(Mathf.RoundToInt((serviceReliability + satellite.reliabilityPercent) * 0.5f - target.weatherRiskPercent * 0.2f - satellite.CongestionReliabilityPenalty), 0, 100);
    }

    private string AttachmentSummaryText(List<MessageAttachmentRecord> attachments, string legacyAttachmentName, int legacyAttachmentSizeKb)
    {
        List<MessageAttachmentRecord> safeAttachments = AttachmentsOrLegacy(attachments, legacyAttachmentName, legacyAttachmentSizeKb);
        return BuildAttachmentDisplaySummary(safeAttachments);
    }

    private string AttachmentSeed(List<MessageAttachmentRecord> attachments)
    {
        if (attachments == null || attachments.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < attachments.Count; i++)
        {
            MessageAttachmentRecord attachment = attachments[i];
            if (attachment == null)
            {
                continue;
            }
            builder.Append(attachment.fileName);
            builder.Append(':');
            builder.Append(attachment.sizeKb);
            builder.Append('|');
        }
        return builder.ToString();
    }

    private float CalculateTransferSeconds()
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        float latency = selectedSatellite != null ? selectedSatellite.latencySeconds : 6f;
        float congestionSeconds = selectedSatellite != null ? selectedSatellite.congestionPercent * 0.045f : 0f;
        float payloadSeconds = Mathf.Max(1f, MessageSizeKb / 48f);
        float islSeconds = IsSameRegionRoute() ? 0f : Mathf.Max(1.5f, 2.5f + congestionSeconds * 0.4f);
        float relaySelectionSeconds = IsSameRegionRoute() ? 0f : Mathf.Max(0.8f, 1.5f + congestionSeconds * 0.1f);
        float downlinkSeconds = Mathf.Max(1.5f, latency * 0.45f + payloadSeconds + congestionSeconds * 0.5f);
        float finalReceiveSeconds = 0.8f;
        return Mathf.Max(1f, islSeconds + relaySelectionSeconds + downlinkSeconds + finalReceiveSeconds);
    }

    private void SelectRecommendedSatelliteIfNeeded()
    {
        if (satelliteManager == null || SelectedTarget == null || Phase != MissionPhase.Idle)
        {
            return;
        }

        SatelliteData selected = satelliteManager.SelectedSatellite;
        if (selected != null && CanServeRoute(selected, OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb))
        {
            return;
        }

        SatelliteData best = FindBestSatelliteForRoute(OriginName, SelectedTarget.targetName, RequestedSendDelayMinutes, MessageSizeKb);
        if (best != null)
        {
            satelliteManager.SelectSatellite(best.satelliteName);
        }
    }

    private SatelliteData FindBestSatelliteForRoute(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteData> candidates = GetRouteAwareSatelliteCandidates(originName, targetName, requestedDelayMinutes, payloadKb, true);
        if (candidates.Count > 0)
        {
            return candidates[0];
        }

        candidates = GetRouteAwareSatelliteCandidates(originName, targetName, requestedDelayMinutes, payloadKb, false);
        if (candidates.Count > 0)
        {
            return candidates[0];
        }

        return satelliteManager != null ? satelliteManager.FindBestSatellite(targetName, requestedDelayMinutes, payloadKb) : null;
    }

    private List<SatelliteData> GetRouteAwareSatelliteCandidates(string originName, string targetName, int requestedDelayMinutes, int payloadKb, bool requireOriginServe)
    {
        List<SatelliteData> routeCandidates = new List<SatelliteData>();
        if (satelliteManager == null || string.IsNullOrEmpty(targetName))
        {
            return routeCandidates;
        }

        string safeOriginName = SafeOriginName(originName);
        List<SatelliteData> targetCandidates = satelliteManager.GetFilteredSatellites(SatelliteListFilter.Recommended, targetName, requestedDelayMinutes);
        for (int i = 0; i < targetCandidates.Count; i++)
        {
            SatelliteData candidate = targetCandidates[i];
            if (candidate == null)
            {
                continue;
            }

            SatelliteRouteEstimateRecord targetEstimate = satelliteManager.GetRouteEstimate(candidate, targetName, requestedDelayMinutes, payloadKb);
            if (targetEstimate == null || !targetEstimate.canServe)
            {
                continue;
            }

            SatelliteRouteEstimateRecord originEstimate = satelliteManager.GetRouteEstimate(candidate, safeOriginName, requestedDelayMinutes, payloadKb);
            if (requireOriginServe && (originEstimate == null || !originEstimate.canServe))
            {
                continue;
            }

            routeCandidates.Add(candidate);
        }

        routeCandidates.Sort(delegate (SatelliteData left, SatelliteData right)
        {
            float leftScore = RouteAwareSatelliteScore(left, safeOriginName, targetName, requestedDelayMinutes, payloadKb);
            float rightScore = RouteAwareSatelliteScore(right, safeOriginName, targetName, requestedDelayMinutes, payloadKb);
            int scoreCompare = leftScore.CompareTo(rightScore);
            if (scoreCompare != 0)
            {
                return scoreCompare;
            }
            return string.Compare(left.satelliteName, right.satelliteName, StringComparison.Ordinal);
        });
        return routeCandidates;
    }

    private bool CanServeRoute(SatelliteData satellite, string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satellite == null || satelliteManager == null || string.IsNullOrEmpty(targetName))
        {
            return false;
        }

        SatelliteRouteEstimateRecord targetEstimate = satelliteManager.GetRouteEstimate(satellite, targetName, requestedDelayMinutes, payloadKb);
        if (targetEstimate == null || !targetEstimate.canServe)
        {
            return false;
        }

        SatelliteRouteEstimateRecord originEstimate = satelliteManager.GetRouteEstimate(satellite, SafeOriginName(originName), requestedDelayMinutes, payloadKb);
        return originEstimate != null && originEstimate.canServe;
    }

    private float RouteAwareSatelliteScore(SatelliteData satellite, string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satellite == null || satelliteManager == null)
        {
            return float.MaxValue;
        }

        SatelliteRouteEstimateRecord targetEstimate = satelliteManager.GetRouteEstimate(satellite, targetName, requestedDelayMinutes, payloadKb);
        if (targetEstimate == null)
        {
            return float.MaxValue;
        }

        SatelliteRouteEstimateRecord originEstimate = satelliteManager.GetRouteEstimate(satellite, SafeOriginName(originName), requestedDelayMinutes, payloadKb);
        float originPenalty = originEstimate != null && originEstimate.canServe ? 0f : 5000f;
        float originScore = originEstimate != null ? originEstimate.score : 2500f;
        float originWindowSeconds = originEstimate != null ? originEstimate.estimatedWindowSeconds : 9999f;
        float targetWindowSeconds = targetEstimate.estimatedWindowSeconds;
        return targetEstimate.score
            + originScore * RouteOriginScoreWeight
            + Mathf.Abs(originWindowSeconds - targetWindowSeconds) * 0.25f
            + originPenalty;
    }

    private string BuildRouteRecommendationReason(SatelliteData satellite, string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (satellite == null || satelliteManager == null)
        {
            return "추천 위성 없음";
        }

        string safeOriginName = SafeOriginName(originName);
        SatelliteRouteEstimateRecord originEstimate = satelliteManager.GetRouteEstimate(satellite, safeOriginName, requestedDelayMinutes, payloadKb);
        SatelliteRouteEstimateRecord targetEstimate = satelliteManager.GetRouteEstimate(satellite, targetName, requestedDelayMinutes, payloadKb);
        string originStatus = originEstimate != null && originEstimate.canServe
            ? CityLabel(safeOriginName) + " 업링크 " + RouteWindowLabel(originEstimate)
            : CityLabel(safeOriginName) + " 업링크 통신 불가";
        string targetStatus = targetEstimate != null && targetEstimate.canServe
            ? DestinationLabel(targetName) + " 다운링크 " + RouteWindowLabel(targetEstimate)
            : DestinationLabel(targetName) + " 다운링크 통신 불가";
        string sameRegion = safeOriginName == targetName ? "동일 지역 검증 · " : string.Empty;
        return sameRegion + "발신/수신 양쪽 기준 · " + originStatus + " · " + targetStatus;
    }

    private string RouteWindowLabel(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || !estimate.canServe || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable)
        {
            return "통신 불가";
        }
        if (estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining || estimate.estimatedWindowSeconds <= 0)
        {
            int availableSeconds = estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedAvailableForSeconds;
            return FormatRemainingSeconds(availableSeconds) + " 남음";
        }
        int waitSeconds = estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedWindowSeconds;
        return FormatRemainingSeconds(waitSeconds) + " 후 가능";
    }

    private string SafeOriginName(string originName)
    {
        return string.IsNullOrEmpty(originName) ? OrbitShareDestinations.Seoul : originName;
    }

    private bool ApplySelectedTargetSilently(string targetName)
    {
        ObservationTarget selected = Targets.Find(target => target.targetName == targetName);
        if (selected == null)
        {
            return false;
        }

        SelectedTarget = selected;
        RefreshTargetSelectionVisuals();
        return true;
    }

    private bool ApplySelectedOriginSilently(string originName)
    {
        ObservationTarget selected = Targets.Find(target => target.targetName == originName);
        if (selected == null)
        {
            selected = Targets.Find(target => target.targetName == OrbitShareDestinations.Seoul);
        }
        if (selected == null && Targets.Count > 0)
        {
            selected = Targets[0];
        }
        if (selected == null)
        {
            return false;
        }

        SelectedOrigin = selected;
        SyncGroundStationToOrigin();
        return true;
    }

    private bool ApplyExplicitOriginSelection(string originName)
    {
        if (!ApplySelectedOriginSilently(originName))
        {
            return false;
        }

        HasExplicitOriginSelection = true;
        RefreshTargetSelectionVisuals();
        return true;
    }

    private void RefreshTargetSelectionVisuals()
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            ObservationTarget target = Targets[i];
            bool showAsSelected = (HasExplicitOriginSelection && target == SelectedOrigin)
                || (HasExplicitDestinationSelection && target == SelectedTarget);
            target.SetSelected(showAsSelected);
        }
    }

    private void SyncGroundStationToOrigin()
    {
        if (SelectedOrigin == null || groundStation == null)
        {
            return;
        }

        Transform originTransform = SelectedOrigin.transform;
        Vector3 originNormal = originTransform.position.sqrMagnitude > 0.001f ? originTransform.position.normalized : Vector3.up;
        groundStation.transform.position = originTransform.position + originNormal * 0.045f;
        groundStation.transform.up = originNormal;
        groundStation.stationName = OriginLabel + " Gateway";
        if (connectionLine != null)
        {
            connectionLine.groundStation = groundStation.transform;
        }
    }

    private bool IsActiveMissionPhase(MissionPhase phase)
    {
        return phase == MissionPhase.Reserved
            || phase == MissionPhase.Running
            || phase == MissionPhase.Observing
            || phase == MissionPhase.ObservationComplete
            || phase == MissionPhase.Transmitting;
    }

    private bool IsQueueActionPhaseReady()
    {
        return Phase == MissionPhase.Idle || Phase == MissionPhase.Complete || Phase == MissionPhase.Failed;
    }

    private bool IsValidQueuedMessageIndex(int index)
    {
        return index >= 0 && index < QueuedMessages.Count && QueuedMessages[index] != null;
    }

    private bool TryGetQueuedMessageStartNowBlockReason(int index, out string blockReason)
    {
        blockReason = string.Empty;
        if (!IsValidQueuedMessageIndex(index))
        {
            blockReason = "메시지가 없습니다.";
            return false;
        }
        if (!IsQueueActionPhaseReady())
        {
            blockReason = "현재 전송이 끝난 뒤 시작할 수 있습니다.";
            return false;
        }

        QueuedMessageRecord queued = QueuedMessages[index];
        if (Targets.Find(item => item.targetName == queued.targetName) == null)
        {
            blockReason = "목적지를 확인할 수 없습니다.";
            return false;
        }
        if (!string.IsNullOrEmpty(queued.originName) && Targets.Find(item => item.targetName == queued.originName) == null)
        {
            blockReason = "발신지를 확인할 수 없습니다.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(queued.messageText))
        {
            blockReason = "메시지 내용이 없습니다.";
            return false;
        }
        if (queued.payloadKb > MaxPayloadKb)
        {
            blockReason = "크기 초과: " + PayloadKbText(queued.payloadKb) + " / " + PayloadKbText(MaxPayloadKb);
            return false;
        }
        string queuedOriginName = string.IsNullOrEmpty(queued.originName) ? OriginName : queued.originName;
        if (GetRouteAwareSatelliteCandidates(queuedOriginName, queued.targetName, 0, queued.payloadKb, true).Count == 0)
        {
            blockReason = "발신지와 목적지를 모두 연결할 수 있는 위성이 없습니다.";
            return false;
        }

        return true;
    }

    private ActiveMissionPersistenceState CaptureActiveMissionState()
    {
        ActiveMissionPersistenceState snapshot = new ActiveMissionPersistenceState();
        snapshot.hasActiveMission = IsActiveMissionPhase(Phase);
        if (!snapshot.hasActiveMission)
        {
            return snapshot;
        }

        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        snapshot.phase = (int)Phase;
        snapshot.resultMessage = ResultMessage ?? string.Empty;
        snapshot.completionSummary = CompletionSummary ?? string.Empty;
        snapshot.originName = OriginName;
        snapshot.targetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        snapshot.satelliteName = selectedSatellite != null ? selectedSatellite.satelliteName : string.Empty;
        snapshot.messageText = MessageText ?? string.Empty;
        snapshot.requestedSendDelayMinutes = RequestedSendDelayMinutes;
        snapshot.scheduledDayOffset = ScheduledDayOffset;
        snapshot.scheduledHour = ScheduledHour;
        snapshot.scheduledMinute = ScheduledMinute;
        snapshot.scheduledLabel = ScheduledLabel ?? string.Empty;
        snapshot.attachmentName = AttachmentName ?? string.Empty;
        snapshot.attachmentSizeKb = AttachmentSizeKb;
        snapshot.attachments = CloneAttachments(attachmentFiles);
        snapshot.messageTextBytes = MessageTextBytes;
        snapshot.messageSizeKb = MessageSizeKb;
        snapshot.payloadTooLarge = PayloadTooLarge;
        snapshot.communicationStep = (int)CommunicationStep;
        snapshot.observationProgress = ObservationProgress;
        snapshot.transferProgress = TransferProgress;
        snapshot.interSatelliteProgress = InterSatelliteProgress;
        snapshot.relaySelectionProgress = RelaySelectionProgress;
        snapshot.downlinkProgress = DownlinkProgress;
        snapshot.finalReceiveProgress = FinalReceiveProgress;
        snapshot.sendWaitProgress = SendWaitProgress;
        snapshot.scheduledWaitElapsedSeconds = scheduledWaitElapsedSeconds;
        snapshot.plannedWaitSeconds = plannedWaitSeconds;
        snapshot.transferElapsedSeconds = transferElapsedSeconds;
        snapshot.plannedTransferSeconds = plannedTransferSeconds;
        snapshot.plannedDownlinkReadySeconds = plannedDownlinkReadySeconds;
        snapshot.plannedIslSeconds = plannedIslSeconds;
        snapshot.plannedRelaySelectionSeconds = plannedRelaySelectionSeconds;
        snapshot.plannedDownlinkSeconds = plannedDownlinkSeconds;
        snapshot.plannedFinalReceiveSeconds = plannedFinalReceiveSeconds;
        snapshot.remainingWaitSeconds = RemainingWaitSeconds;
        snapshot.remainingTransferSeconds = RemainingTransferSeconds;
        snapshot.estimatedDeliverySeconds = EstimatedDeliverySeconds;
        snapshot.estimatedPriceUsd = EstimatedPriceUsd;
        snapshot.priceBaseUsd = PriceBaseUsd;
        snapshot.pricePayloadUsd = PricePayloadUsd;
        snapshot.priceUrgencyUsd = PriceUrgencyUsd;
        snapshot.successProbabilityPercent = SuccessProbabilityPercent;
        snapshot.destinationRelaySatelliteName = DestinationRelaySatelliteName ?? string.Empty;
        snapshot.deliveryRecorded = deliveryRecorded;
        snapshot.activeSentMessageTrackingId = activeSentMessageTrackingId ?? string.Empty;
        snapshot.reservationAlertLogged = reservationAlertLogged;
        snapshot.missionEventLog = new List<string>(MissionEventLog);
        return snapshot;
    }

    private void RestoreActiveMissionState(ActiveMissionPersistenceState snapshot)
    {
        if (snapshot == null || !snapshot.hasActiveMission)
        {
            return;
        }

        MissionPhase restoredPhase = (MissionPhase)Mathf.Clamp(snapshot.phase, 0, Enum.GetValues(typeof(MissionPhase)).Length - 1);
        if (!IsActiveMissionPhase(restoredPhase) || !ApplySelectedOriginSilently(string.IsNullOrEmpty(snapshot.originName) ? OrbitShareDestinations.Seoul : snapshot.originName) || !ApplySelectedTargetSilently(snapshot.targetName))
        {
            return;
        }

        HasExplicitOriginSelection = true;
        HasExplicitDestinationSelection = true;
        RefreshTargetSelectionVisuals();

        if (satelliteManager != null && !string.IsNullOrEmpty(snapshot.satelliteName))
        {
            satelliteManager.SelectSatellite(snapshot.satelliteName);
        }

        Phase = MissionPhase.Idle;
        MessageText = snapshot.messageText ?? string.Empty;
        SetAttachmentsInternal(AttachmentsOrLegacy(snapshot.attachments, snapshot.attachmentName, snapshot.attachmentSizeKb));
        RequestedSendDelayMinutes = Mathf.Max(0, snapshot.requestedSendDelayMinutes);
        ScheduledDayOffset = Mathf.Clamp(snapshot.scheduledDayOffset, 0, MaxScheduledDayOffset);
        ScheduledHour = Mathf.Clamp(snapshot.scheduledHour, 0, 23);
        ScheduledMinute = Mathf.Clamp(snapshot.scheduledMinute, 0, 59);
        ScheduledLabel = string.IsNullOrEmpty(snapshot.scheduledLabel)
            ? (RequestedSendDelayMinutes == 0 ? "지금 보내기" : "+" + RequestedSendDelayMinutes + "분")
            : snapshot.scheduledLabel;
        UpdatePayloadSize();
        if (snapshot.messageSizeKb > MessageSizeKb)
        {
            MessageSizeKb = snapshot.messageSizeKb;
            MessagePayloadBytes = KilobytesToBytes(MessageSizeKb);
        }
        PayloadTooLarge = snapshot.payloadTooLarge || MessageSizeKb > MaxPayloadKb;
        UpdateMissionEstimate();

        Phase = restoredPhase;
        CommunicationStep = (SatelliteCommunicationStep)Mathf.Clamp(snapshot.communicationStep, 0, Enum.GetValues(typeof(SatelliteCommunicationStep)).Length - 1);
        if (CommunicationStep == SatelliteCommunicationStep.Draft && restoredPhase != MissionPhase.Idle)
        {
            CommunicationStep = restoredPhase == MissionPhase.Reserved ? SatelliteCommunicationStep.Reserved
                : restoredPhase == MissionPhase.Running ? SatelliteCommunicationStep.WaitingForUplink
                : restoredPhase == MissionPhase.Observing ? SatelliteCommunicationStep.Uplink
                : restoredPhase == MissionPhase.ObservationComplete ? SatelliteCommunicationStep.LeoReceived
                : restoredPhase == MissionPhase.Transmitting ? SatelliteCommunicationStep.Downlink
                : SatelliteCommunicationStep.Draft;
        }
        ResultMessage = string.IsNullOrEmpty(snapshot.resultMessage) ? "저장된 전송 상태를 복원했습니다." : snapshot.resultMessage;
        CompletionSummary = snapshot.completionSummary ?? string.Empty;
        ObservationProgress = Mathf.Clamp01(snapshot.observationProgress);
        TransferProgress = Mathf.Clamp01(snapshot.transferProgress);
        InterSatelliteProgress = Mathf.Clamp01(snapshot.interSatelliteProgress);
        RelaySelectionProgress = Mathf.Clamp01(snapshot.relaySelectionProgress);
        DownlinkProgress = Mathf.Clamp01(snapshot.downlinkProgress);
        FinalReceiveProgress = Mathf.Clamp01(snapshot.finalReceiveProgress);
        if (TransferProgress > 0f && (InterSatelliteProgress + RelaySelectionProgress + DownlinkProgress + FinalReceiveProgress) <= 0f)
        {
            UpdateSegmentProgressFromTransfer();
        }
        SendWaitProgress = Mathf.Clamp01(snapshot.sendWaitProgress);
        scheduledWaitElapsedSeconds = Mathf.Max(0f, snapshot.scheduledWaitElapsedSeconds);
        plannedWaitSeconds = Mathf.Max(0f, snapshot.plannedWaitSeconds);
        transferElapsedSeconds = Mathf.Max(0f, snapshot.transferElapsedSeconds);
        plannedTransferSeconds = Mathf.Max(1f, snapshot.plannedTransferSeconds);
        plannedDownlinkReadySeconds = Mathf.Max(0f, snapshot.plannedDownlinkReadySeconds);
        plannedIslSeconds = Mathf.Max(0.1f, snapshot.plannedIslSeconds);
        plannedRelaySelectionSeconds = Mathf.Max(0.1f, snapshot.plannedRelaySelectionSeconds);
        plannedDownlinkSeconds = Mathf.Max(0.1f, snapshot.plannedDownlinkSeconds);
        plannedFinalReceiveSeconds = Mathf.Max(0.1f, snapshot.plannedFinalReceiveSeconds);
        RemainingWaitSeconds = Mathf.Max(0f, snapshot.remainingWaitSeconds);
        RemainingTransferSeconds = Mathf.Max(0f, snapshot.remainingTransferSeconds);
        EstimatedDeliverySeconds = snapshot.estimatedDeliverySeconds;
        EstimatedPriceUsd = snapshot.estimatedPriceUsd;
        PriceBaseUsd = snapshot.priceBaseUsd;
        PricePayloadUsd = snapshot.pricePayloadUsd;
        PriceUrgencyUsd = snapshot.priceUrgencyUsd;
        SuccessProbabilityPercent = snapshot.successProbabilityPercent;
        DestinationRelaySatelliteName = snapshot.destinationRelaySatelliteName ?? string.Empty;
        deliveryRecorded = snapshot.deliveryRecorded;
        activeSentMessageTrackingId = snapshot.activeSentMessageTrackingId ?? string.Empty;
        reservationAlertLogged = snapshot.reservationAlertLogged;
        MissionEventLog.Clear();
        if (snapshot.missionEventLog != null)
        {
            for (int i = 0; i < snapshot.missionEventLog.Count; i++)
            {
                if (!string.IsNullOrEmpty(snapshot.missionEventLog[i]))
                {
                    MissionEventLog.Add(snapshot.missionEventLog[i]);
                }
            }
        }
        if (MissionEventLog.Count == 0)
        {
            AddMissionEvent("저장 상태 복원");
        }

        if (satelliteManager != null && SelectedTarget != null)
        {
            satelliteManager.ReserveSelected(SelectedTarget.targetName, RequestedSendDelayMinutes);
            if (Phase != MissionPhase.Reserved)
            {
                satelliteManager.SetSharedState(SatelliteAvailabilityState.InMission);
            }
            if (Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting)
            {
                satelliteManager.UpdateSharedMission(SelectedTarget.targetName, QueuedMessageBufferPercent);
            }
        }
    }

    private void LoadPersistentState()
    {
        if (!PlayerPrefs.HasKey(PersistenceKey))
        {
            return;
        }

        string json = PlayerPrefs.GetString(PersistenceKey, string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        MissionPersistenceState state;
        try
        {
            state = JsonUtility.FromJson<MissionPersistenceState>(json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("OrbitShare 저장 데이터를 불러오지 못했습니다: " + exception.Message);
            return;
        }

        if (state == null)
        {
            return;
        }

        bool previousSuppressValue = suppressPersistenceSave;
        suppressPersistenceSave = true;
        try
        {
            SentMessages.Clear();
            if (state.sentMessages != null)
            {
                for (int i = 0; i < state.sentMessages.Count && SentMessages.Count < 8; i++)
                {
                    if (state.sentMessages[i] != null)
                    {
                        NormalizeSentMessageRecord(state.sentMessages[i]);
                        SentMessages.Add(state.sentMessages[i]);
                    }
                }
            }

            QueuedMessages.Clear();
            if (state.queuedMessages != null)
            {
                for (int i = 0; i < state.queuedMessages.Count; i++)
                {
                    if (state.queuedMessages[i] != null)
                    {
                        NormalizeQueuedMessageRecord(state.queuedMessages[i]);
                        QueuedMessages.Add(state.queuedMessages[i]);
                    }
                }
            }

            MessageTemplates.Clear();
            if (state.messageTemplates != null)
            {
                for (int i = 0; i < state.messageTemplates.Count; i++)
                {
                    if (state.messageTemplates[i] != null)
                    {
                        NormalizeMessageTemplateRecord(state.messageTemplates[i]);
                        MessageTemplates.Add(state.messageTemplates[i]);
                    }
                }
            }

            CopyUniqueStrings(state.favoriteTargetNames, FavoriteTargetNames, 0);
            CopyUniqueStrings(state.favoriteSatelliteNames, FavoriteSatelliteNames, 0);
            CopyUniqueStrings(state.recentTargetNames, RecentTargetNames, 4);

            AutoStartQueuedMessages = state.autoStartQueuedMessages;
            AutoQueueCountdown = -1f;
            lastAutoCountdownSecond = -1;
            successfulTransmissionTotal = Mathf.Max(CompletedSentMessageCount(), state.successfulTransmissionCount);
            FailedTransmissionCount = Mathf.Max(0, state.failedTransmissionCount);
            RefreshTransmissionStats();

            HasDraft = state.hasDraft;
            draftOriginName = state.draftOriginName ?? string.Empty;
            draftTargetName = state.draftTargetName ?? string.Empty;
            draftDelayMinutes = Mathf.Max(0, state.draftDelayMinutes);
            draftMessageText = state.draftMessageText ?? string.Empty;
            draftMessageSizeKb = Mathf.Max(0, state.draftMessageSizeKb);
            draftAttachmentName = state.draftAttachmentName ?? string.Empty;
            draftAttachmentSizeKb = Mathf.Max(0, state.draftAttachmentSizeKb);
            draftAttachments = AttachmentsOrLegacy(state.draftAttachments, draftAttachmentName, draftAttachmentSizeKb);
            DraftSummary = state.draftSummary ?? string.Empty;
            if (HasDraft)
            {
                UpdateDraftSummary();
            }
            if (!HasDraft)
            {
                draftTargetName = string.Empty;
                draftOriginName = string.Empty;
                draftDelayMinutes = 0;
                draftMessageText = string.Empty;
                draftMessageSizeKb = 0;
                draftAttachmentName = string.Empty;
                draftAttachmentSizeKb = 0;
                draftAttachments.Clear();
                DraftSummary = string.Empty;
            }

            RestoreActiveMissionState(state.activeMission);
            if (IsActiveMissionPhase(Phase))
            {
                EnsureActiveSentMessageRecord();
            }
        }
        finally
        {
            suppressPersistenceSave = previousSuppressValue;
        }
    }

    private void SavePersistentState()
    {
        if (suppressPersistenceSave)
        {
            return;
        }

        RefreshActiveSentMessageRecord();

        MissionPersistenceState state = new MissionPersistenceState();
        state.sentMessages = new List<SentMessageRecord>(SentMessages);
        state.queuedMessages = new List<QueuedMessageRecord>(QueuedMessages);
        state.messageTemplates = new List<MessageTemplateRecord>(MessageTemplates);
        state.favoriteTargetNames = new List<string>(FavoriteTargetNames);
        state.favoriteSatelliteNames = new List<string>(FavoriteSatelliteNames);
        state.recentTargetNames = new List<string>(RecentTargetNames);
        state.autoStartQueuedMessages = AutoStartQueuedMessages;
        state.successfulTransmissionCount = SuccessfulTransmissionCount;
        state.failedTransmissionCount = FailedTransmissionCount;
        state.activeMission = CaptureActiveMissionState();
        state.hasDraft = HasDraft;
        state.draftSummary = DraftSummary ?? string.Empty;
        state.draftOriginName = draftOriginName ?? string.Empty;
        state.draftTargetName = draftTargetName ?? string.Empty;
        state.draftDelayMinutes = draftDelayMinutes;
        state.draftMessageText = draftMessageText ?? string.Empty;
        state.draftMessageSizeKb = draftMessageSizeKb;
        state.draftAttachmentName = draftAttachmentName ?? string.Empty;
        state.draftAttachmentSizeKb = draftAttachmentSizeKb;
        state.draftAttachments = CloneAttachments(draftAttachments);

        try
        {
            PlayerPrefs.SetString(PersistenceKey, JsonUtility.ToJson(state));
            PlayerPrefs.Save();
        }
        catch (Exception exception)
        {
            Debug.LogWarning("OrbitShare 저장 데이터를 기록하지 못했습니다: " + exception.Message);
        }
    }

    private void CopyUniqueStrings(List<string> source, List<string> destination, int maxCount)
    {
        destination.Clear();
        if (source == null)
        {
            return;
        }

        for (int i = 0; i < source.Count; i++)
        {
            string item = source[i];
            if (string.IsNullOrEmpty(item) || destination.Contains(item))
            {
                continue;
            }

            destination.Add(item);
            if (maxCount > 0 && destination.Count >= maxCount)
            {
                return;
            }
        }
    }

    private void UpdateDraftSummary()
    {
        string safeDraftMessageText = draftMessageText ?? string.Empty;
        int draftAttachmentTotalKb = TotalAttachmentSize(draftAttachments);
        DraftSummary = (string.IsNullOrEmpty(draftOriginName) ? "발신지 미정" : CityLabel(draftOriginName))
            + " → "
            + (string.IsNullOrEmpty(draftTargetName) ? "목적지 미정" : DestinationLabel(draftTargetName))
            + " · " + (string.IsNullOrWhiteSpace(safeDraftMessageText) ? "내용 없음" : "문자 " + FormatBytes(GetUtf8ByteCount(safeDraftMessageText)))
            + (draftMessageSizeKb > 0 ? " · 크기 " + PayloadKbText(draftMessageSizeKb) : string.Empty)
            + (draftAttachmentTotalKb > 0 ? " · 첨부 " + PayloadKbText(draftAttachmentTotalKb) : string.Empty);
    }

    private void SaveDraftSnapshot()
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }
        if (SelectedOrigin == null && SelectedTarget == null && string.IsNullOrWhiteSpace(MessageText) && AttachmentSizeKb <= 0)
        {
            return;
        }

        draftOriginName = OriginName;
        draftTargetName = SelectedTarget != null ? SelectedTarget.targetName : string.Empty;
        draftDelayMinutes = RequestedSendDelayMinutes;
        draftMessageText = MessageText ?? string.Empty;
        draftMessageSizeKb = MessageSizeKb;
        draftAttachmentName = AttachmentName ?? string.Empty;
        draftAttachmentSizeKb = AttachmentSizeKb;
        draftAttachments = CloneAttachments(attachmentFiles);
        HasDraft = true;
        UpdateDraftSummary();
        SavePersistentState();
    }

    private void AddRecentTarget(string targetName)
    {
        if (string.IsNullOrEmpty(targetName))
        {
            return;
        }
        RecentTargetNames.Remove(targetName);
        RecentTargetNames.Insert(0, targetName);
        while (RecentTargetNames.Count > 4)
        {
            RecentTargetNames.RemoveAt(RecentTargetNames.Count - 1);
        }
        SavePersistentState();
    }

    private void UpdateSecurityStatus()
    {
        int checksum = 17;
        string seed = (MessageText ?? string.Empty) + "|" + AttachmentSeed(attachmentFiles) + "|" + (SelectedTarget != null ? SelectedTarget.targetName : string.Empty);
        unchecked
        {
            for (int i = 0; i < seed.Length; i++)
            {
                checksum = checksum * 31 + seed[i];
            }
        }
        checksum = Mathf.Abs(checksum % 10000);
        SecurityChecksumLabel = "CHK-" + checksum.ToString("0000");
        SecurityStatusLabel = "보안 확인 · 코드 " + SecurityChecksumLabel + " · 도착 확인 대기";
        if (Phase == MissionPhase.Complete)
        {
            SecurityStatusLabel = "보안 확인 · 코드 " + SecurityChecksumLabel + " · 도착 확인 완료";
        }
    }

    private void UpdateSuggestedRetryAlternative()
    {
        SuggestedRetrySatelliteName = string.Empty;
        SuggestedRetryDelayMinutes = 0;
        SuggestedRetryHint = string.Empty;
        if (satelliteManager == null || SelectedTarget == null)
        {
            return;
        }

        SatelliteData current = CurrentUplinkSatellite();
        int[] delays = { RequestedSendDelayMinutes, 5, 15, 30 };
        for (int i = 0; i < delays.Length; i++)
        {
            int delay = Mathf.Max(0, delays[i]);
            List<SatelliteData> candidates = GetRouteAwareSatelliteCandidates(OriginName, SelectedTarget.targetName, delay, MessageSizeKb, true);
            if (candidates.Count == 0)
            {
                candidates = GetRouteAwareSatelliteCandidates(OriginName, SelectedTarget.targetName, delay, MessageSizeKb, false);
            }
            for (int j = 0; j < candidates.Count; j++)
            {
                SatelliteData candidate = candidates[j];
                if (current != null && candidate.satelliteName == current.satelliteName)
                {
                    continue;
                }
                SuggestedRetrySatelliteName = candidate.satelliteName;
                SuggestedRetryDelayMinutes = delay;
                SuggestedRetryHint = "추천안: " + candidate.satelliteName + " / " + (delay == 0 ? "지금" : "+" + delay + "분") + " 다시 시도";
                return;
            }
        }
    }

    private string ChooseFailureReason()
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        if (selectedSatellite != null && selectedSatellite.congestionPercent >= 70)
        {
            return "위성 혼잡";
        }
        if (SelectedTarget != null && SelectedTarget.weatherRiskPercent >= 20f)
        {
            return "날씨 영향";
        }
        if (EstimatedDownlinkSeconds >= 8f)
        {
            return "목적지 연결 끊김";
        }
        if (MessageSizeKb >= MaxPayloadKb * 0.85f)
        {
            return "저장 공간 부족";
        }
        return "연결 끊김";
    }

    private string FailureStepLabelForReason(string failureReason)
    {
        if (failureReason == "위성 혼잡")
        {
            return CommunicationStepLabelFor(SatelliteCommunicationStep.InterSatelliteLink);
        }
        if (failureReason == "날씨 영향" || failureReason == "목적지 연결 끊김")
        {
            return CommunicationStepLabelFor(SatelliteCommunicationStep.Downlink);
        }
        if (failureReason == "저장 공간 부족")
        {
            return CommunicationStepLabelFor(SatelliteCommunicationStep.LeoReceived);
        }
        if (CommunicationStep == SatelliteCommunicationStep.Complete || CommunicationStep == SatelliteCommunicationStep.Failed)
        {
            return CommunicationStepLabelFor(CommunicationStepFromTransferProgress(TransferProgress));
        }
        return CommunicationStepLabel;
    }

    private void AddMissionEvent(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        MissionEventLog.Add(DateTime.Now.ToString("HH:mm:ss") + " " + message);
        while (MissionEventLog.Count > 14)
        {
            MissionEventLog.RemoveAt(0);
        }
    }

    private string FormatRemainingSeconds(float seconds)
    {
        int rounded = Mathf.Max(0, Mathf.CeilToInt(seconds));
        if (rounded >= 60)
        {
            return (rounded / 60) + "분 " + (rounded % 60) + "초";
        }
        return rounded + "초";
    }

    private string TargetNameFromDestinationLabel(string label)
    {
        return OrbitShareDestinations.TargetNameFromLabel(label);
    }

    private string DestinationLabel(string targetName)
    {
        return OrbitShareDestinations.Label(targetName);
    }

    private string CityLabel(string targetName)
    {
        return OrbitShareDestinations.Label(targetName);
    }

    private string SenderLabel()
    {
        return SenderLabel(OriginName);
    }

    private string SenderLabel(string originName)
    {
        return "사용자(" + CityLabel(originName) + ")";
    }

    private string RouteSummary(string targetName)
    {
        return RouteSummary(OriginName, targetName);
    }

    private string RouteSummary(string originName, string targetName)
    {
        bool sameRegion = !string.IsNullOrEmpty(originName) && !string.IsNullOrEmpty(targetName) && originName == targetName;
        return sameRegion
            ? SenderLabel(originName) + " 업링크 → LEO 수신 → 직접 다운링크 → " + DestinationLabel(targetName) + " 도착 확인" + SameRegionVerificationNote(originName, targetName)
            : SenderLabel(originName) + " 업링크 → LEO 수신 → ISL → 목적지 근처 위성 선택 → 다운링크 → " + DestinationLabel(targetName) + " 도착 확인";
    }

    private string RouteSummary()
    {
        return RouteSummary(SelectedTarget != null ? SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName);
    }

    private string BuildCommunicationFlowSummary()
    {
        string uplinkSatellite = string.IsNullOrEmpty(SelectedSatelliteName) ? "LEO 위성" : SelectedSatelliteName;
        string relaySatellite = DestinationRelaySatelliteLabel();
        string destination = SelectedTarget != null ? DestinationLabel(SelectedTarget.targetName) : "수신 지역";
        if (!UsesInterSatelliteLink())
        {
            return SenderLabel()
                + " 업링크 → " + uplinkSatellite
                + " LEO 수신 → 직접 다운링크 → " + destination + " 도착 확인"
                + SameRegionVerificationNote(OriginName, SelectedTarget != null ? SelectedTarget.targetName : string.Empty);
        }
        return SenderLabel()
            + " 업링크 → " + uplinkSatellite
            + " LEO 수신 → ISL(" + BuildInterSatelliteRouteSummary() + ") → 목적지 근처 위성 선택(" + relaySatellite + ")"
            + " → 다운링크 → " + destination + " 도착 확인"
            + SameRegionVerificationNote(OriginName, SelectedTarget != null ? SelectedTarget.targetName : string.Empty);
    }

    private string SameRegionVerificationNote(string originName, string targetName)
    {
        return !string.IsNullOrEmpty(originName) && !string.IsNullOrEmpty(targetName) && originName == targetName
            ? " · 동일 지역 검증"
            : string.Empty;
    }

    private string CommunicationStepLabelFor(SatelliteCommunicationStep step)
    {
        switch (step)
        {
            case SatelliteCommunicationStep.Reserved:
                return "예약";
            case SatelliteCommunicationStep.WaitingForUplink:
                return "업링크 대기";
            case SatelliteCommunicationStep.Uplink:
                return "업링크";
            case SatelliteCommunicationStep.LeoReceived:
                return "LEO 수신";
            case SatelliteCommunicationStep.InterSatelliteLink:
                return "ISL 전달";
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return "목적지 근처 위성 선택";
            case SatelliteCommunicationStep.Downlink:
                return "다운링크";
            case SatelliteCommunicationStep.WaitingForDownlink:
                return "다운링크 대기";
            case SatelliteCommunicationStep.FinalReceive:
                return "도착 확인";
            case SatelliteCommunicationStep.Complete:
                return "전송 완료";
            case SatelliteCommunicationStep.Failed:
                return "전송 실패";
            default:
                return "작성";
        }
    }

    private void AlignSatelliteTo(Transform target)
    {
        if (target == null || earth == null)
        {
            return;
        }

        Transform activeSatellite = CurrentSatelliteTransform();
        if (activeSatellite == null)
        {
            return;
        }

        SatelliteOrbit activeOrbit = activeSatellite.GetComponent<SatelliteOrbit>();
        if (activeOrbit != null)
        {
            activeOrbit.SetAngle(BestOrbitAngleForTarget(activeOrbit, target));
            UpdateCommunicationAvailability();
            return;
        }

        EnterpriseSatelliteAnimator animator = activeSatellite.GetComponent<EnterpriseSatelliteAnimator>();
        if (animator != null)
        {
            animator.angle = BestAnimatorAngleForTarget(animator, target);
            ApplyAnimatorPosition(animator);
            UpdateCommunicationAvailability();
            return;
        }

        if (satelliteOrbit != null)
        {
            satelliteOrbit.SetAngle(BestOrbitAngleForTarget(satelliteOrbit, target));
            UpdateCommunicationAvailability();
        }
    }

    private void AlignDestinationRelayToTarget()
    {
        if (SelectedTarget == null)
        {
            return;
        }

        Transform relay = DestinationRelaySatelliteTransform();
        if (relay == null)
        {
            AlignSatelliteTo(SelectedTarget.transform);
            return;
        }

        AlignSatelliteTransformTo(relay, SelectedTarget.transform);
    }

    private void AlignSatelliteTransformTo(Transform satelliteTransform, Transform target)
    {
        if (satelliteTransform == null || target == null || earth == null)
        {
            return;
        }

        SatelliteOrbit orbit = satelliteTransform.GetComponent<SatelliteOrbit>();
        if (orbit != null)
        {
            orbit.SetAngle(BestOrbitAngleForTarget(orbit, target));
            return;
        }

        EnterpriseSatelliteAnimator animator = satelliteTransform.GetComponent<EnterpriseSatelliteAnimator>();
        if (animator != null)
        {
            animator.angle = BestAnimatorAngleForTarget(animator, target);
            ApplyAnimatorPosition(animator);
        }
    }

    private float BestOrbitAngleForTarget(SatelliteOrbit orbit, Transform target)
    {
        float bestAngle = 0f;
        float bestSurfaceAngle = float.MaxValue;
        Vector3 center = earth.position;
        Vector3 targetDirection = (target.position - center).normalized;

        for (int i = 0; i < 360; i++)
        {
            Vector3 satelliteDirection = (orbit.GetOrbitPosition(i) - center).normalized;
            float surfaceAngle = Vector3.Angle(satelliteDirection, targetDirection);
            if (surfaceAngle < bestSurfaceAngle)
            {
                bestSurfaceAngle = surfaceAngle;
                bestAngle = i;
            }
        }

        return bestAngle;
    }

    private float BestAnimatorAngleForTarget(EnterpriseSatelliteAnimator animator, Transform target)
    {
        float bestAngle = 0f;
        float bestSurfaceAngle = float.MaxValue;
        Vector3 center = earth.position;
        Vector3 targetDirection = (target.position - center).normalized;

        for (int i = 0; i < 360; i++)
        {
            Vector3 satellitePosition = AnimatorPosition(animator, i);
            Vector3 satelliteDirection = (satellitePosition - center).normalized;
            float surfaceAngle = Vector3.Angle(satelliteDirection, targetDirection);
            if (surfaceAngle < bestSurfaceAngle)
            {
                bestSurfaceAngle = surfaceAngle;
                bestAngle = i;
            }
        }

        return bestAngle;
    }

    private Vector3 AnimatorPosition(EnterpriseSatelliteAnimator animator, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        Vector3 local = new Vector3(Mathf.Cos(radians) * animator.radius, 0f, Mathf.Sin(radians) * animator.radius);
        return (earth != null ? earth.position : Vector3.zero) + Quaternion.Euler(animator.inclination, 0f, 0f) * local;
    }

    private void ApplyAnimatorPosition(EnterpriseSatelliteAnimator animator)
    {
        animator.transform.position = AnimatorPosition(animator, animator.angle);
        animator.transform.LookAt(earth != null ? earth.position : Vector3.zero);
    }

    private Transform CurrentSatelliteTransform()
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        Transform selectedTransform = selectedSatellite != null && satelliteManager != null
            ? satelliteManager.GetSatelliteTransform(selectedSatellite.satelliteName)
            : null;
        if (selectedTransform != null)
        {
            return selectedTransform;
        }
        return satelliteOrbit != null ? satelliteOrbit.transform : null;
    }

    private SatelliteData CurrentUplinkSatellite()
    {
        if (satelliteManager == null)
        {
            return null;
        }
        if (!string.IsNullOrEmpty(satelliteManager.MissionSatelliteName) && Phase != MissionPhase.Idle && Phase != MissionPhase.Failed)
        {
            return satelliteManager.GetMissionSatellite();
        }
        return satelliteManager.SelectedSatellite;
    }

    private Transform DestinationRelaySatelliteTransform()
    {
        if (!string.IsNullOrEmpty(DestinationRelaySatelliteName) && satelliteManager != null)
        {
            Transform relay = satelliteManager.GetSatelliteTransform(DestinationRelaySatelliteName);
            if (relay != null)
            {
                return relay;
            }
        }
        return CurrentSatelliteTransform();
    }

    private string ChooseDestinationRelaySatelliteName()
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        string selectedName = selectedSatellite != null ? selectedSatellite.satelliteName : string.Empty;
        if (IsSameRegionRoute())
        {
            return selectedName;
        }
        if (satelliteManager == null || SelectedTarget == null)
        {
            return string.IsNullOrEmpty(plannedDestinationRelaySatelliteName) ? selectedName : plannedDestinationRelaySatelliteName;
        }

        SatelliteData bestRelay = FindBestDestinationRelaySatellite(0, MessageSizeKb);
        if (bestRelay != null)
        {
            return bestRelay.satelliteName;
        }

        if (!string.IsNullOrEmpty(plannedDestinationRelaySatelliteName))
        {
            return plannedDestinationRelaySatelliteName;
        }

        List<SatelliteData> candidates = satelliteManager.GetFilteredSatellites(SatelliteListFilter.Recommended, SelectedTarget.targetName, 0);
        SatelliteData best = null;
        float bestAngle = float.MaxValue;
        for (int i = 0; i < candidates.Count; i++)
        {
            if (candidates[i] == null)
            {
                continue;
            }

            float angle = SurfaceAngleFromSatelliteToTarget(candidates[i]);
            if (angle < bestAngle)
            {
                best = candidates[i];
                bestAngle = angle;
            }
        }
        if (best != null)
        {
            return best.satelliteName;
        }
        return string.IsNullOrEmpty(selectedName) && candidates.Count > 0 ? candidates[0].satelliteName : selectedName;
    }

    private void RefreshDestinationRelayCandidate()
    {
        if (CommunicationStep == SatelliteCommunicationStep.Downlink
            || CommunicationStep == SatelliteCommunicationStep.FinalReceive
            || CommunicationStep == SatelliteCommunicationStep.Complete
            || CommunicationStep == SatelliteCommunicationStep.Failed)
        {
            return;
        }

        string relayName = ChooseDestinationRelaySatelliteName();
        bool routeRefreshDue = Application.isPlaying && Time.time >= nextInterSatelliteRouteRefreshTime;
        if (!string.IsNullOrEmpty(relayName) && relayName != DestinationRelaySatelliteName)
        {
            DestinationRelaySatelliteName = relayName;
            routeRefreshDue = true;
        }
        if (routeRefreshDue)
        {
            RebuildInterSatelliteRoute(false);
        }
    }

    private SatelliteData FindBestDestinationRelaySatellite(int requestedDelayMinutes, int payloadKb)
    {
        if (satelliteManager == null || SelectedTarget == null)
        {
            return null;
        }

        List<SatelliteData> candidates = satelliteManager.GetFilteredSatellites(SatelliteListFilter.Recommended, SelectedTarget.targetName, requestedDelayMinutes);
        SatelliteData sourceSatellite = CurrentUplinkSatellite();
        SatelliteData best = null;
        float bestScore = float.MaxValue;
        for (int pass = 0; pass < 2; pass++)
        {
            bool requireIslRoute = pass == 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                SatelliteData candidate = candidates[i];
                if (candidate == null)
                {
                    continue;
                }
                if (requireIslRoute
                    && !IsSameRegionRoute()
                    && sourceSatellite != null
                    && candidate.satelliteName == sourceSatellite.satelliteName)
                {
                    continue;
                }

                SatelliteRouteEstimateRecord estimate = satelliteManager.GetRouteEstimate(candidate, SelectedTarget.targetName, requestedDelayMinutes, payloadKb);
                if (estimate == null || !estimate.canServe)
                {
                    continue;
                }

                float islCost;
                int hopCount;
                if (!CanBuildInterSatelliteRouteTo(candidate, out islCost, out hopCount))
                {
                    if (requireIslRoute)
                    {
                        continue;
                    }
                    islCost = 5000f;
                    hopCount = 99;
                }

                float windowSeconds = Mathf.Max(0f, estimate.estimatedWindowSeconds);
                float availableSeconds = Mathf.Max(0f, estimate.estimatedAvailableForSeconds);
                float score = windowSeconds * 1000f
                    + SurfaceAngleFromSatelliteToTarget(candidate) * 3f
                    + Mathf.Max(0f, candidate.congestionPercent) * 0.25f
                    + islCost * 0.35f
                    + Mathf.Max(0, hopCount - 1) * 28f
                    - Mathf.Min(availableSeconds, 600f) * 0.05f;
                if (score < bestScore)
                {
                    best = candidate;
                    bestScore = score;
                }
            }

            if (best != null)
            {
                return best;
            }
        }

        return null;
    }

    public void CopyInterSatelliteRouteTransforms(List<Transform> results)
    {
        if (results == null)
        {
            return;
        }

        results.Clear();
        if (!UsesInterSatelliteLink())
        {
            return;
        }

        EnsureInterSatelliteRouteReady();
        for (int i = 0; i < interSatelliteRouteNames.Count; i++)
        {
            Transform satelliteTransform = satelliteManager != null ? satelliteManager.GetSatelliteTransform(interSatelliteRouteNames[i]) : null;
            if (satelliteTransform != null)
            {
                results.Add(satelliteTransform);
            }
        }
    }

    private void EnsureInterSatelliteRouteReady()
    {
        if (UsesInterSatelliteLink() && interSatelliteRouteNames.Count < 2)
        {
            RebuildInterSatelliteRoute(true);
        }
    }

    private void ClearInterSatelliteRoute()
    {
        interSatelliteRouteNames.Clear();
        plannedIslRouteCost = 0f;
        nextInterSatelliteRouteRefreshTime = 0f;
    }

    private bool IsSameRegionRoute()
    {
        return SelectedTarget != null
            && !string.IsNullOrEmpty(OriginName)
            && OriginName == SelectedTarget.targetName;
    }

    private bool UsesInterSatelliteLink()
    {
        SatelliteData source = CurrentUplinkSatellite();
        SatelliteData relay = SatelliteByName(string.IsNullOrEmpty(DestinationRelaySatelliteName) ? plannedDestinationRelaySatelliteName : DestinationRelaySatelliteName);
        return RequiresInterSatelliteLink(source, relay);
    }

    private bool RequiresInterSatelliteLink(SatelliteData source, SatelliteData relay)
    {
        return !IsSameRegionRoute()
            && source != null
            && relay != null
            && source.satelliteName != relay.satelliteName;
    }

    private bool CanBuildInterSatelliteRouteTo(SatelliteData relay, out float routeCost, out int hopCount)
    {
        routeCost = 0f;
        hopCount = 0;
        SatelliteData source = CurrentUplinkSatellite();
        if (!RequiresInterSatelliteLink(source, relay))
        {
            return source != null && relay != null;
        }

        List<string> path;
        if (!TryBuildInterSatelliteRoute(source, relay, out path, out routeCost))
        {
            return false;
        }

        hopCount = Mathf.Max(0, path.Count - 1);
        return true;
    }

    private void RebuildInterSatelliteRoute(bool force)
    {
        if (!force && Application.isPlaying && Time.time < nextInterSatelliteRouteRefreshTime)
        {
            return;
        }

        interSatelliteRouteNames.Clear();
        plannedIslRouteCost = 0f;
        nextInterSatelliteRouteRefreshTime = Application.isPlaying ? Time.time + InterSatelliteRouteRefreshSeconds : 0f;

        SatelliteData source = CurrentUplinkSatellite();
        SatelliteData relay = SatelliteByName(string.IsNullOrEmpty(DestinationRelaySatelliteName) ? plannedDestinationRelaySatelliteName : DestinationRelaySatelliteName);
        if (source == null)
        {
            return;
        }

        interSatelliteRouteNames.Add(source.satelliteName);
        if (!RequiresInterSatelliteLink(source, relay))
        {
            return;
        }

        List<string> path;
        if (TryBuildInterSatelliteRoute(source, relay, out path, out plannedIslRouteCost) && path.Count > 0)
        {
            interSatelliteRouteNames.Clear();
            interSatelliteRouteNames.AddRange(path);
            return;
        }

        if (relay != null)
        {
            interSatelliteRouteNames.Add(relay.satelliteName);
            plannedIslRouteCost = 5000f;
        }
    }

    private bool TryBuildInterSatelliteRoute(SatelliteData source, SatelliteData relay, out List<string> path, out float routeCost)
    {
        path = new List<string>();
        routeCost = 0f;
        if (source == null || relay == null || satelliteManager == null)
        {
            return false;
        }

        if (source.satelliteName == relay.satelliteName)
        {
            path.Add(source.satelliteName);
            return true;
        }

        List<SatelliteData> nodes = BuildInterSatelliteGraphNodes(source, relay);
        int sourceIndex = nodes.IndexOf(source);
        int relayIndex = nodes.IndexOf(relay);
        if (sourceIndex < 0 || relayIndex < 0)
        {
            return false;
        }

        int count = nodes.Count;
        float[] distances = new float[count];
        int[] previous = new int[count];
        bool[] visited = new bool[count];
        for (int i = 0; i < count; i++)
        {
            distances[i] = float.PositiveInfinity;
            previous[i] = -1;
        }
        distances[sourceIndex] = 0f;

        for (int step = 0; step < count; step++)
        {
            int current = -1;
            float currentDistance = float.PositiveInfinity;
            for (int i = 0; i < count; i++)
            {
                if (!visited[i] && distances[i] < currentDistance)
                {
                    current = i;
                    currentDistance = distances[i];
                }
            }

            if (current < 0)
            {
                break;
            }
            if (current == relayIndex)
            {
                break;
            }

            visited[current] = true;
            for (int next = 0; next < count; next++)
            {
                if (visited[next] || next == current)
                {
                    continue;
                }

                float linkCost;
                if (!TryInterSatelliteLinkCost(nodes[current], nodes[next], out linkCost))
                {
                    continue;
                }

                float candidateDistance = distances[current] + linkCost;
                if (candidateDistance < distances[next])
                {
                    distances[next] = candidateDistance;
                    previous[next] = current;
                }
            }
        }

        if (float.IsInfinity(distances[relayIndex]))
        {
            return false;
        }

        List<string> reversed = new List<string>();
        int cursor = relayIndex;
        while (cursor >= 0)
        {
            reversed.Add(nodes[cursor].satelliteName);
            cursor = previous[cursor];
        }
        for (int i = reversed.Count - 1; i >= 0; i--)
        {
            path.Add(reversed[i]);
        }
        routeCost = distances[relayIndex];
        return path.Count >= 2;
    }

    private List<SatelliteData> BuildInterSatelliteGraphNodes(SatelliteData source, SatelliteData relay)
    {
        List<SatelliteData> nodes = new List<SatelliteData>();
        AddInterSatelliteGraphNode(nodes, source);
        AddInterSatelliteGraphNode(nodes, relay);
        if (satelliteManager == null)
        {
            return nodes;
        }

        for (int i = 0; i < satelliteManager.Satellites.Count; i++)
        {
            SatelliteData satellite = satelliteManager.Satellites[i];
            if (satellite == null || satellite.ownerType != SatelliteOwnerType.Shared || satellite.batteryPercent < 35f)
            {
                continue;
            }
            if (satellite.availabilityState == SatelliteAvailabilityState.MissionComplete)
            {
                continue;
            }
            if (satelliteManager.GetSatelliteTransform(satellite.satelliteName) == null)
            {
                continue;
            }

            AddInterSatelliteGraphNode(nodes, satellite);
        }
        return nodes;
    }

    private void AddInterSatelliteGraphNode(List<SatelliteData> nodes, SatelliteData satellite)
    {
        if (nodes == null || satellite == null)
        {
            return;
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] != null && nodes[i].satelliteName == satellite.satelliteName)
            {
                return;
            }
        }
        nodes.Add(satellite);
    }

    private bool TryInterSatelliteLinkCost(SatelliteData from, SatelliteData to, out float linkCost)
    {
        linkCost = 0f;
        if (from == null || to == null || satelliteManager == null)
        {
            return false;
        }

        Transform fromTransform = satelliteManager.GetSatelliteTransform(from.satelliteName);
        Transform toTransform = satelliteManager.GetSatelliteTransform(to.satelliteName);
        if (fromTransform == null || toTransform == null)
        {
            return false;
        }

        float separationDegrees = SurfaceAngleBetweenSatelliteTransforms(fromTransform, toTransform);
        float rangeDegrees = InterSatelliteLinkRangeDegrees(from, to);
        if (separationDegrees > rangeDegrees)
        {
            return false;
        }

        float congestionPenalty = (Mathf.Max(0f, from.congestionPercent) + Mathf.Max(0f, to.congestionPercent)) * 0.035f;
        float latencyPenalty = (Mathf.Max(1, from.latencySeconds) + Mathf.Max(1, to.latencySeconds)) * 0.18f;
        float reliabilityBonus = (Mathf.Max(0, from.reliabilityPercent) + Mathf.Max(0, to.reliabilityPercent)) * 0.012f;
        linkCost = Mathf.Max(0.1f, separationDegrees + congestionPenalty + latencyPenalty - reliabilityBonus);
        return true;
    }

    private float InterSatelliteLinkRangeDegrees(SatelliteData from, SatelliteData to)
    {
        return Mathf.Clamp(IndividualInterSatelliteRangeDegrees(from) + IndividualInterSatelliteRangeDegrees(to), 8f, 72f);
    }

    private float IndividualInterSatelliteRangeDegrees(SatelliteData satellite)
    {
        if (satellite == null)
        {
            return 18f;
        }

        float altitudeKm = Mathf.Max(120f, satellite.altitudeKm);
        float horizonDegrees = Mathf.Acos(Mathf.Clamp(EarthRadiusKm / (EarthRadiusKm + altitudeKm), -1f, 1f)) * Mathf.Rad2Deg;
        float linkBudget = Mathf.Clamp((satellite.reliabilityPercent - 85f) * 0.08f - satellite.congestionPercent * 0.025f, -3f, 4f);
        return Mathf.Clamp(horizonDegrees + linkBudget, 6f, 38f);
    }

    private float SurfaceAngleBetweenSatelliteTransforms(Transform from, Transform to)
    {
        if (from == null || to == null)
        {
            return 180f;
        }

        Vector3 center = earth != null ? earth.position : Vector3.zero;
        Vector3 fromDirection = (from.position - center).normalized;
        Vector3 toDirection = (to.position - center).normalized;
        return Vector3.Angle(fromDirection, toDirection);
    }

    private string BuildInterSatelliteRouteSummary()
    {
        EnsureInterSatelliteRouteReady();
        if (!UsesInterSatelliteLink())
        {
            return "ISL 생략";
        }
        if (interSatelliteRouteNames.Count < 2)
        {
            return SelectedSatelliteName + " → " + DestinationRelaySatelliteLabel();
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < interSatelliteRouteNames.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(" → ");
            }
            builder.Append(interSatelliteRouteNames[i]);
        }
        builder.Append(" (");
        builder.Append(Mathf.Max(1, interSatelliteRouteNames.Count - 1));
        builder.Append("홉)");
        return builder.ToString();
    }

    private float DestinationRelayWindowSeconds(SatelliteData relaySatellite, int requestedDelayMinutes, int payloadKb)
    {
        if (relaySatellite == null || satelliteManager == null || SelectedTarget == null)
        {
            return -1f;
        }

        SatelliteRouteEstimateRecord estimate = satelliteManager.GetRouteEstimate(relaySatellite, SelectedTarget.targetName, requestedDelayMinutes, payloadKb);
        if (estimate == null || !estimate.canServe)
        {
            return -1f;
        }
        return Mathf.Max(0, estimate.estimatedWindowSeconds);
    }

    private float DownlinkContactAngleDegrees(SatelliteData relaySatellite, ObservationTarget target)
    {
        if (relaySatellite != null)
        {
            return SatelliteRouteEstimator.ContactAngleDegrees(relaySatellite, target != null ? target.targetName : string.Empty);
        }
        return target != null ? target.observationAngle : 18f;
    }

    private float SurfaceAngleFromSatelliteToTarget(SatelliteData satellite)
    {
        if (satellite == null || satelliteManager == null || SelectedTarget == null)
        {
            return float.MaxValue;
        }

        Transform satelliteTransform = satelliteManager.GetSatelliteTransform(satellite.satelliteName);
        if (satelliteTransform == null || earth == null)
        {
            return satellite.nextWindowMinutes;
        }

        Vector3 center = earth.position;
        Vector3 satelliteDirection = (satelliteTransform.position - center).normalized;
        Vector3 targetDirection = (SelectedTarget.transform.position - center).normalized;
        return Vector3.Angle(satelliteDirection, targetDirection);
    }

    private string DestinationRelaySatelliteLabel()
    {
        if (string.IsNullOrEmpty(DestinationRelaySatelliteName))
        {
            return "목적지 근처 위성 자동 선택";
        }

        string uplinkSatelliteName = SelectedSatelliteName;
        if (!string.IsNullOrEmpty(uplinkSatelliteName) && DestinationRelaySatelliteName == uplinkSatelliteName)
        {
            return DestinationRelaySatelliteName + " 단일 위성 중계";
        }
        if (satelliteManager != null && satelliteManager.GetSatelliteTransform(DestinationRelaySatelliteName) == null)
        {
            return DestinationRelaySatelliteName + " 후보(단일 경로 표시)";
        }
        return DestinationRelaySatelliteName;
    }

    private float CurrentSatelliteDegreesPerSecond()
    {
        SatelliteData selectedSatellite = CurrentUplinkSatellite();
        if (selectedSatellite != null && satelliteManager != null)
        {
            return satelliteManager.DegreesPerSecond(selectedSatellite);
        }
        return satelliteOrbit != null ? Mathf.Max(1f, satelliteOrbit.orbitSpeed) : 6f;
    }

    private float SurfaceAngleTo(Transform point)
    {
        if (point == null)
        {
            return 180f;
        }
        Transform activeSatellite = CurrentSatelliteTransform();
        if (activeSatellite == null)
        {
            return 180f;
        }
        Vector3 center = earth != null ? earth.position : Vector3.zero;
        Vector3 satelliteDirection = (activeSatellite.position - center).normalized;
        Vector3 pointDirection = (point.position - center).normalized;
        return Vector3.Angle(satelliteDirection, pointDirection);
    }

    private float SurfaceAngleFromTransformTo(Transform satelliteTransform, Transform point)
    {
        if (satelliteTransform == null || point == null)
        {
            return 180f;
        }
        Vector3 center = earth != null ? earth.position : Vector3.zero;
        Vector3 satelliteDirection = (satelliteTransform.position - center).normalized;
        Vector3 pointDirection = (point.position - center).normalized;
        return Vector3.Angle(satelliteDirection, pointDirection);
    }

    private SatelliteData SatelliteByName(string satelliteName)
    {
        if (satelliteManager == null || string.IsNullOrEmpty(satelliteName))
        {
            return null;
        }
        return satelliteManager.Satellites.Find(satellite => satellite != null && satellite.satelliteName == satelliteName);
    }

    private void NotifyChanged()
    {
        if (IsActiveMissionPhase(Phase))
        {
            RefreshActiveSentMessageRecord();
        }
        if (suppressMissionChangedNotification)
        {
            return;
        }
        if (ShouldThrottleMissionChangedNotification())
        {
            return;
        }
        if (OnMissionChanged != null)
        {
            OnMissionChanged.Invoke();
        }
        MarkMissionChangedNotificationSent();
    }

    private bool ShouldThrottleMissionChangedNotification()
    {
        if (!Application.isPlaying || !IsActiveMissionPhase(Phase))
        {
            return false;
        }

        bool structuralChange = Phase != lastNotifiedPhase
            || CommunicationStep != lastNotifiedStep
            || ResultMessage != lastNotifiedResultMessage;
        if (structuralChange)
        {
            return false;
        }

        return Time.unscaledTime < nextActiveMissionNotifyTime;
    }

    private void MarkMissionChangedNotificationSent()
    {
        lastNotifiedPhase = Phase;
        lastNotifiedStep = CommunicationStep;
        lastNotifiedResultMessage = ResultMessage ?? string.Empty;
        nextActiveMissionNotifyTime = Application.isPlaying && IsActiveMissionPhase(Phase)
            ? Time.unscaledTime + ActiveMissionNotifyIntervalSeconds
            : 0f;
    }
}
