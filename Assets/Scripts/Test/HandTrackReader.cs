using Leap;
using Leap.Unity;
using UnityEngine;

namespace com.hive.projectr
{
    public class HandTrackReader : MonoBehaviour
    {
        public LeapProvider leapProvider;

        private void OnEnable()
        {
            leapProvider.OnUpdateFrame += OnUpdateFrame;
        }
        private void OnDisable()
        {
            leapProvider.OnUpdateFrame -= OnUpdateFrame;
        }

        void OnUpdateFrame(Frame frame)
        {
            if (frame.Hands != null && frame.Hands.Count > 0)
            {
                foreach (var hand in frame.Hands)
                {
                    var pos = hand.WristPosition;
                    var side = hand.IsLeft ? "Left" : "Right";
                    Debug.LogError($"Hand - Id: {hand.Id} | {side} | Pos: {pos}");
                }
            }
        }
    }
}