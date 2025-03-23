using UnityEngine;
using UnityEngine.UI;
using AS;

public class AS_WaveformDisplayRecord : MonoBehaviour
{
    public AS_Recorder recorder;
    [SerializeField] public static bool recording = false;
    private Toggle m_Toggle;
    [SerializeField] public int resolution = 4096;
    private bool isRecording = false;

	private MeshFilter meshFilter;
	void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh = new Mesh();
        recorder = this.transform.parent.GetComponent<AS_Recorder>();
        recording = false;
        isRecording = false;
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponentInChildren<Toggle>();
        //Add listener for when the state of the Toggle changes, and output the state
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });
    }

    //Output the new state of the Toggle into Text when the user uses the Toggle
    void ToggleValueChanged(Toggle change)
    {
        if(m_Toggle.isOn){
            recorder.StartRecording();
            //Debug.Log("Start Recording: " + Time.time);
            isRecording = true;
        }
        if(!m_Toggle.isOn){
            recorder.StopRecording();
            //Debug.Log("Stop Recording: " + Time.time);
            isRecording = false;
        }
    }

    // Update is called once per frame
    void Update(){

        if (isRecording){
            WaveFormDisplay(recorder.recordedClip);
        }
    }

    void WaveFormDisplay(AudioClip clip){
        var samples = new float[clip.channels * clip.samples];
		clip.GetData(samples, 0);

		var vertices = new Vector3[resolution];
		for (var i = 0; i < resolution; i++)
		{
            var sampleIndex = Mathf.Clamp(samples.Length * i / resolution, 0, samples.Length - 1);
            var level = samples[sampleIndex];
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