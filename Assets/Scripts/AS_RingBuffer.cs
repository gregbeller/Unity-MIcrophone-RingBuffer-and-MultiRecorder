using System.Collections;
using UnityEngine;
using System;

namespace AS{
    public class AS_RingBuffer : MonoBehaviour
    {
        [SerializeField] public AudioClip microphoneClip;
        public int sampleRate = 48000;
        public int bufferMicMaxDuration = 120; // Durée du buffer circulaire en secondes
        private int bufferSize;
        private string micDevice;

        private void Start()
        {
            sampleRate = AudioSettings.outputSampleRate;
            bufferSize = sampleRate * bufferMicMaxDuration;
            StartMicrophone();
        }

        private void StartMicrophone()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone detected !");
                return;
            }

            micDevice = Microphone.devices[0];
            Debug.Log("Micrphone used: " + micDevice);
            microphoneClip = Microphone.Start(micDevice, true, bufferMicMaxDuration, sampleRate);
        }

        public AudioClip GetAudioSegment(float duration)
        {
            int length = Mathf.Clamp((int)(duration * sampleRate), 0, bufferSize);
            float[] segment = new float[length];

            int micPosition = Microphone.GetPosition(micDevice);
            if (micPosition < 0)
            {
                Debug.LogWarning("Microphone not available");
                return null;
            }

            int readIndex = (micPosition - length + bufferSize) % bufferSize;
            microphoneClip.GetData(segment, readIndex);

            return CreateAudioClip(segment, "SegmentClip");
        }

        public AudioClip GetAccumulatedAudio(int startSample)
        {
            int micPosition = Microphone.GetPosition(micDevice);
            if (micPosition < 0 || startSample >= bufferSize)
            {
                Debug.LogWarning("Unable to access audio buffer");
                return null;
            }

            int length = (micPosition - startSample + bufferSize) % bufferSize;
            float[] accumulatedAudio = new float[length];
            if (length > 0){
                microphoneClip.GetData(accumulatedAudio, startSample);  
            }

            return CreateAudioClip(accumulatedAudio, "AccumulatedClip");
        }

        public int GetCurrentSampleIndex()
        {
            return Microphone.GetPosition(micDevice);
        }

        private AudioClip CreateAudioClip(float[] samples, string clipName)
        {
            if (samples.Length==0){
                samples = new float[10];
                }
            AudioClip clip = AudioClip.Create(clipName, samples.Length, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        void OnDestroy()
        {
            Microphone.End(micDevice);
        }
    }
}