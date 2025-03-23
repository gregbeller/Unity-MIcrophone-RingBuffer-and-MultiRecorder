using UnityEngine;
using UnityEngine.UI;
using AS;

public class AS_RecorderPlayer : MonoBehaviour
{

    public AS_Recorder recorder;
    [SerializeField] public AudioSource audioSourceRecorder;
    [SerializeField] public static bool recording = false;
    private Toggle m_Toggle;

	void Awake()
	{
        audioSourceRecorder = GetComponent<AudioSource>();
        recorder = this.transform.parent.GetComponent<AS_Recorder>();
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
            recorder.StartPlaying();
        }
        if(!m_Toggle.isOn){
            recorder.StopPlaying();
        }
    }
}