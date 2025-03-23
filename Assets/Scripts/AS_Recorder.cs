using UnityEngine;
using System.IO;
using AS;

public class AS_Recorder : MonoBehaviour
{
    public AS_RingBuffer audioBuffer;
    public AudioSource audioSource;
    [SerializeField] public AudioClip recordedClip;
    [SerializeField] public int maxDuration = 10;

    public string fileName = "RecordedAudio.wav";
    private int startSample = 0;
    private float elapsedTime = 0f;
    public bool isRecording = false;
	
    void Awake()
	{
        audioBuffer = GameObject.Find("AS_RingBuffer").GetComponent<AS_RingBuffer>();
        audioSource = GetComponent<AudioSource>();
        isRecording = false;
	}
    public void StartRecording()
    {
        startSample = audioBuffer.GetCurrentSampleIndex();
        isRecording = true;

        // Créer un AudioClip dynamique de 10 secondes max (ajustable)
        recordedClip = AudioClip.Create("LiveRecording", audioBuffer.sampleRate * maxDuration, 1, audioBuffer.sampleRate, false);
        audioSource.clip = recordedClip;
    }

    public void StopRecording()
    {
        isRecording = false;
        // SaveAudio;
    }

    private void Update()
    {
        if (isRecording)
        {
            UpdateRecordedClip();
            if (elapsedTime >= maxDuration){
                StopRecording();
            }
        }
    }

    private void UpdateRecordedClip()
    {
        recordedClip = audioBuffer.GetAccumulatedAudio(startSample);
        audioSource.clip = recordedClip;
        elapsedTime = (audioBuffer.GetCurrentSampleIndex() - startSample) / audioBuffer.sampleRate;
        //Debug.Log(elapsedTime);
    }

    public void StartPlaying(){
        audioSource.Play(); // Lecture en live
    }

    public void StopPlaying(){
        audioSource.Stop(); // Lecture en live
    }

    private void SaveAudio()
    {
        AudioClip recordedClip = audioBuffer.GetAccumulatedAudio(startSample);
        if (recordedClip == null)
        {
            Debug.LogWarning("No audio to record!");
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, fileName);
        SaveWav(path, recordedClip);
        Debug.Log($"Audio recorded under {path}");
    }

    private void SaveWav(string filePath, AudioClip clip)
    {
        int sampleCount = clip.samples;
        int frequency = clip.frequency;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        using (FileStream file = new FileStream(filePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(file))
        {
            // Header WAV
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + sampleCount * 2);  // File size
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // Subchunk1 size
            writer.Write((ushort)1); // Audio format (PCM)
            writer.Write((ushort)1); // Number of channels
            writer.Write(frequency); // Sample rate
            writer.Write(frequency * 2); // Byte rate
            writer.Write((ushort)2); // Block align
            writer.Write((ushort)16); // Bits per sample
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(sampleCount * 2); // Data size

            // Write audio data
            foreach (var sample in samples)
            {
                short intSample = (short)(sample * 32767);
                writer.Write(intSample);
            }
        }
    }
}
