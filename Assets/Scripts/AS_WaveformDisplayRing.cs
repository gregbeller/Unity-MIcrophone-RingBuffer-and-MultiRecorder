using UnityEngine;
using AS;

public class AS_WaveformDisplayRing : MonoBehaviour
{

    public AS_RingBuffer audioBuffer;
    [SerializeField] public AudioClip ringClip;
    [SerializeField] public float ringClipDuration = 3.0f;
    [SerializeField] public int resolution = 4096;
	private MeshFilter meshFilter;
	void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
        audioBuffer = this.transform.parent.GetComponent<AS_RingBuffer>();
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ringClip = audioBuffer.GetAudioSegment(ringClipDuration);
        WaveFormDisplay(ringClip);
    }

    void WaveFormDisplay(AudioClip clip){
        var samples = new float[clip.channels * clip.samples];
		clip.GetData(samples, 0);

		var vertices = new Vector3[resolution];
		for (var i = 0; i < resolution; i++)
		{
			var level = samples[samples.Length * i / resolution];
			vertices[i] = new Vector3(3.4f / resolution * i - 1.7f, level / 2, 0);
		}
		meshFilter.mesh.vertices = vertices;

		var indices = new int[resolution];
		for (var i = 0; i < resolution; i++)
		{
			indices[i] = i;
		}
		meshFilter.mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
    }
}



/* 
// Récupérer un segment de 2 secondes sous forme d'AudioClip

AudioSource.PlayClipAtPoint(segment, Vector3.zero); // Joue le segment audio

// Démarrer un enregistrement et le stopper après 3 secondes
recorder.StartRecording();
Invoke("StopRecording", 3.0f);
 */