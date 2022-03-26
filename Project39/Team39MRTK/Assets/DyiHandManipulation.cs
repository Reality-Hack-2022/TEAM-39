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
        MixedRealityPose pose;

        private Handedness leftHand = Handedness.Left;
        private Handedness rightHand = Handedness.Right;
        public Keyboard keyboard;

        int lastLeftFinger = -1;
        int leftFingerFrameCount = 0;

        int lastRightFinger = -1;
        int rightFingerFrameCount = 0;

        float firstLeftFingerPos = 0.0f;

        bool leftGesture = false;
        bool rightGesture = false;


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
                if (leftFingerFrameCount == 15)
                {
                    // moves up column
                    keyboard.IndicatePress(maxLeftFinger, false, false);

                    if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out pose))
                    {
                        firstLeftFingerPos = pose.Position.x;
                    }
                } else if (leftFingerFrameCount > 15)
                {
                    if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out pose))
                    {
                        if ((pose.Position.x - firstLeftFingerPos) > 0.1 && !rightGesture) {
                            keyboard.IndicateRelease();
                            keyboard.IndicatePress(maxLeftFinger, false, true);
                            rightGesture = true;
                        } else if ((pose.Position.x - firstLeftFingerPos) < -0.1 && !leftGesture) {
                            keyboard.IndicateRelease();
                            keyboard.IndicatePress(maxLeftFinger, true, false);
                            leftGesture = true;
                        }
                    }
                }
            } else
            {
                if (lastLeftFinger == -1)  {
                    leftFingerFrameCount++;

                    if (leftFingerFrameCount >= 15)
                    {
                        Debug.Log("no curl");
                        keyboard.IndicateRelease();
                        leftGesture = false;
                        rightGesture = false;
                        firstLeftFingerPos = 0.0f;
                    }
                } else
                {
                    lastLeftFinger = -1;
                    leftFingerFrameCount++;
                }
            }

            if (maxRightFinger >= 0)
            {
                if (rightFingerFrameCount == 15)
                {
                    // if column indicated, indicate row
                    keyboard.IndicateRowPress(maxRightFinger);
                    Debug.Log("indicate finger" + maxRightFinger);
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

        }
    }
}