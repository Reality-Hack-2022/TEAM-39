using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTKExtensions.Gesture;
using UnityEngine;
using UnityEngine.Events;
using VRKeys;

namespace DyiPinchGrab
{
    public class DyiHandManipulation : MonoBehaviour
    {
        /*
        [SerializeField]
        private TrackedHandJoint trackedHandJoint = TrackedHandJoint.IndexMiddleJoint;

        [SerializeField]
        private float grabDistance = 0.1f;

        [SerializeField]
        private Handedness trackedHand = Handedness.Both;
        */
        private Handedness leftHand = Handedness.Left;
        private Handedness rightHand = Handedness.Right;
        public Keyboard keyboard;

        int lastLeftFinger = -1;
        int leftFingerFrameCount = 0;

        int lastRightFinger = -1;
        int rightFingerFrameCount = 0;

        /*
        [SerializeField]
        private bool trackPinch = true;

        [SerializeField]
        private bool trackGrab = true;

        private IMixedRealityHandJointService handJointService;

        private IMixedRealityHandJointService HandJointService =>
            handJointService ??
            (handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>());

        private MixedRealityPose? previousLeftHandPose;

        private MixedRealityPose? previousRightHandPose;
        */
        [SerializeField]
        public UnityEvent leftIndex;
        private void Update()
        {
            if (leftFingerFrameCount == 15) Debug.Log("Left hand finger " + lastLeftFinger + " gesture");

            double[] fingerCurls = new double[8];
            double threshold = 0.3;
            double maxLeftFingerCurl = -10.0;
            double maxRightFingerCurl = -10.0;

            int maxLeftFinger = -1;
            int maxRightFinger = -1;


            fingerCurls[0] = HandPoseUtils.IndexFingerCurl(leftHand);
            fingerCurls[1] = HandPoseUtils.MiddleFingerCurl(leftHand);
            fingerCurls[2] = HandPoseUtils.RingFingerCurl(leftHand);
            fingerCurls[3] = HandPoseUtils.PinkyFingerCurl(leftHand);

            fingerCurls[4] = HandPoseUtils.IndexFingerCurl(rightHand);
            fingerCurls[5] = HandPoseUtils.MiddleFingerCurl(rightHand);
            fingerCurls[6] = HandPoseUtils.RingFingerCurl(rightHand);
            fingerCurls[7] = HandPoseUtils.PinkyFingerCurl(rightHand);


            int currentFrame = Time.frameCount;

            for (int i = 0; i < 4; i++)
            {
                if (fingerCurls[i] >= threshold && fingerCurls[i] > maxLeftFingerCurl)
                {
                    // left finger pinch detected
                    maxLeftFinger = i;
                    maxLeftFingerCurl = fingerCurls[i];

 
                }
            }
            for (int i = 4; i < 8; i++)
            {
                if (fingerCurls[i] >= threshold && fingerCurls[i] > maxRightFingerCurl)
                {
                    // right finger pinch detected
                    maxRightFinger = i;
                    maxRightFingerCurl = fingerCurls[i];


                }
            }

            // update frame count on each hand if same pinch as last frame, otherwise update and reset frame counts
            if (lastLeftFinger == maxLeftFinger)
            {
                leftFingerFrameCount++;
            }
            else
            {
                lastLeftFinger = maxLeftFinger;
                leftFingerFrameCount = 0;
                Debug.Log("left finger change to " + lastLeftFinger);
            }

            if (lastRightFinger == maxRightFinger)
            {
                rightFingerFrameCount++;
            }
            else
            {
                lastRightFinger = maxRightFinger;
                rightFingerFrameCount = 0;
                Debug.Log("right finger change to " + lastRightFinger);
            }



            // if left hand pinching and 15 frames have passed since pinching, then indicate
            if (maxLeftFinger >= 0)
            {
                if (leftFingerFrameCount >= 15)
                {
                    // moves up column
                    keyboard.IndicatePress(maxLeftFinger);
                }
            } else
            {
                if (lastLeftFinger == -1)  {
                    leftFingerFrameCount++;

                    if (leftFingerFrameCount >= 15)
                    {
                        Debug.Log("no curl");
                        keyboard.IndicateRelease();
                    }
                } else
                {
                    lastLeftFinger = -1;
                    leftFingerFrameCount++;
                }
            }

            if (maxRightFinger >= 0)
            {
                if (rightFingerFrameCount >= 15)
                {
                    // if column indicated, indicate row
                    keyboard.IndicateRowPress(maxRightFinger);
                }
            }
            else
            {
                if (lastRightFinger == -1)
                {
                    rightFingerFrameCount++;

                    if (rightFingerFrameCount >= 15)
                    {
                        Debug.Log("no curl");
                        keyboard.IndicateRowRelease();
                    }
                }
                else
                {
                    lastRightFinger = -1;
                    rightFingerFrameCount++;
                }
            }



            /*
            var leftHandPose = GetHandPose(Handedness.Left, previousLeftHandPose != null);
            var rightHandPose = GetHandPose(Handedness.Right, previousRightHandPose != null);
            {
                var jointTransform = HandJointService.RequestJointTransform(trackedHandJoint, trackedHand);
                if (rightHandPose != null && previousRightHandPose != null)
                {
                    if (leftHandPose != null && previousLeftHandPose != null)
                    {
                        // fight! pick the closest one
                        var isRightCloser = Vector3.Distance(rightHandPose.Value.Position, jointTransform.position) <
                                            Vector3.Distance(leftHandPose.Value.Position, jointTransform.position);

                        ProcessPoseChange(
                            isRightCloser ? previousRightHandPose : previousLeftHandPose,
                            isRightCloser ? rightHandPose : leftHandPose);
                    }
                    else
                    {
                        ProcessPoseChange(previousRightHandPose, rightHandPose);
                    }
                }
                else if (leftHandPose != null && previousLeftHandPose != null)
                {
                    ProcessPoseChange(previousLeftHandPose, leftHandPose);
                }
            }
            previousLeftHandPose = leftHandPose;
            previousRightHandPose = rightHandPose;
            */
        }
        /*
        private MixedRealityPose? GetHandPose(Handedness hand, bool hasBeenGrabbed)
        {
            if ((trackedHand & hand) == hand)
            {
                if (HandJointService.IsHandTracked(hand) &&
                    ((GestureUtils.IsPinching(hand) && trackPinch) ||
                     (GestureUtils.IsGrabbing(hand) && trackGrab)))
                {
                    var jointTransform = HandJointService.RequestJointTransform(trackedHandJoint, hand);
                    var palmTransForm = HandJointService.RequestJointTransform(TrackedHandJoint.Palm, hand);
                    Debug.Log("Index " + HandPoseUtils.RingFingerCurl(leftHand));

                    if (hasBeenGrabbed ||
                       Vector3.Distance(gameObject.transform.position, jointTransform.position) <= grabDistance)
                    {

                        return new MixedRealityPose(jointTransform.position, palmTransForm.rotation);
                    }
                }
            }

            return null;
        }

        private void ProcessPoseChange(MixedRealityPose? previousPose, MixedRealityPose? currentPose)
        {
            var delta = currentPose.Value.Position - previousPose.Value.Position;
            var deltaRotation = Quaternion.FromToRotation(previousPose.Value.Forward, currentPose.Value.Forward);
            gameObject.transform.position += delta;
            gameObject.transform.rotation *= deltaRotation;
        }

        */
    }
}