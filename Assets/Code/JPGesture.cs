/*
Modified from unity demo Static Hand Gestures
by John Bruneau
*/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class JBGesture : MonoBehaviour
{
    [SerializeField]
    XRHandTrackingEvents m_HandTrackingEvents;

    [SerializeField]
    ScriptableObject m_HandShapeOrPose;

    [SerializeField]
    Transform m_TargetTransform;

    [SerializeField]
    UnityEvent m_GesturePerformed;

    [SerializeField]
    UnityEvent m_GestureEnded;

    [SerializeField]
    float m_MinimumHoldTime = 0.2f;

    [SerializeField]
    float m_GestureDetectionInterval = 0.1f;

    XRHandShape m_HandShape;
    XRHandPose m_HandPose;
    bool m_WasDetected;
    bool m_PerformedTriggered;
    float m_TimeOfLastConditionCheck;
    float m_HoldStartTime;

    public XRHandTrackingEvents handTrackingEvents
    {
        get => m_HandTrackingEvents;
        set => m_HandTrackingEvents = value;
    }

    public ScriptableObject handShapeOrPose
    {
        get => m_HandShapeOrPose;
        set => m_HandShapeOrPose = value;
    }

    public Transform targetTransform
    {
        get => m_TargetTransform;
        set => m_TargetTransform = value;
    }

    public UnityEvent gesturePerformed
    {
        get => m_GesturePerformed;
        set => m_GesturePerformed = value;
    }

    public UnityEvent gestureEnded
    {
        get => m_GestureEnded;
        set => m_GestureEnded = value;
    }

    public float minimumHoldTime
    {
        get => m_MinimumHoldTime;
        set => m_MinimumHoldTime = value;
    }

    public float gestureDetectionInterval
    {
        get => m_GestureDetectionInterval;
        set => m_GestureDetectionInterval = value;
    }

    void OnEnable()
    {
        m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
        m_HandShape = m_HandShapeOrPose as XRHandShape;
        m_HandPose = m_HandShapeOrPose as XRHandPose;
        if (m_HandPose != null && m_HandPose.relativeOrientation != null)
            m_HandPose.relativeOrientation.targetTransform = m_TargetTransform;
    }

    void OnDisable() => m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
            return;

        var detected =
            m_HandTrackingEvents.handIsTracked &&
            m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
            m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

        if (!m_WasDetected && detected)
        {
            m_HoldStartTime = Time.timeSinceLevelLoad;
        }
        else if (m_WasDetected && !detected)
        {
            m_PerformedTriggered = false;
            m_GestureEnded?.Invoke();
        }

        m_WasDetected = detected;

        if (!m_PerformedTriggered && detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
            if (holdTimer > m_MinimumHoldTime)
            {
                m_GesturePerformed?.Invoke();
                m_PerformedTriggered = true;
            }
        }

        m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
    }
}