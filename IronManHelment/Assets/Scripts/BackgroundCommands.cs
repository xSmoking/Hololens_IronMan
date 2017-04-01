using UnityEngine;

public class BackgroundCommands : MonoBehaviour
{
    private Renderer rs;
    private bool target = false;
    private AudioSource powerUp;
    private AudioSource powerUpLoop;
    private float timePlaying = 0;

    void Start()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        powerUp = audios[0];
        powerUpLoop = audios[1];
        rs = GetComponent<Renderer>();
        rs.enabled = false;
    }

    void Update()
    {
        if (target)
        {
            timePlaying += Time.deltaTime;
            if (timePlaying >= 5.0f)
            {
                rs.enabled = true;
                target = false;
                timePlaying = 0.0f;
                powerUpLoop.Play();
            }
        }
    }

    void OnActivate()
    {
        powerUp.Play();
        target = true;
    }

    void OnDeActivate()
    {
        rs.enabled = false;
        powerUpLoop.Stop();
    }

    void OnReset()
    {
        rs.enabled = false;
        powerUpLoop.Stop();
    }
}
