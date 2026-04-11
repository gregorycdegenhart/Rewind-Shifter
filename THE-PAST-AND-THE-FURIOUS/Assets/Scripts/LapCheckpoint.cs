using UnityEngine;

public class LapCheckpoint : MonoBehaviour
{
    public int checkpointIndex = 0;
    public bool completesLap = false; // only matters in Laps mode

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && RaceManager.Instance != null)
        {
            RaceManager.Instance.HitCheckpoint(checkpointIndex, completesLap);

            CarAudio carAudio = other.GetComponent<CarAudio>();
            if (carAudio != null) carAudio.PlayCheckpointSound();
        }
    }
}