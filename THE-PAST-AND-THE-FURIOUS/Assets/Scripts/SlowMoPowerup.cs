using System.Collections;
using UnityEngine;

public class SlowMoPowerup : MonoBehaviour
{
    public float slowMultiplier = 0.3f;
    public float duration = 3f;

    private bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Opponent"))
            return;

        if (active) return;
        active = true;

        GameObject picker = other.gameObject;

        StartCoroutine(ApplySlowMo(picker));

        gameObject.SetActive(false);
    }

    IEnumerator ApplySlowMo(GameObject picker)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] opponents = GameObject.FindGameObjectsWithTag("Opponent");

        // Slow down all other players
        foreach (GameObject obj in players)
        {
            if (obj == picker) continue;

            CarController car = obj.GetComponent<CarController>();
            if (car != null)
                car.SetSpeedMultiplier(slowMultiplier);
        }

        // Slow down AI opponents
        foreach (GameObject obj in opponents)
        {
            if (obj == picker) continue;

            // TODO: Add AICarController.SetSpeedMultiplier() when AI is implemented
            CarController car = obj.GetComponent<CarController>();
            if (car != null)
                car.SetSpeedMultiplier(slowMultiplier);
        }

        yield return new WaitForSeconds(duration);

        // Reset everyone back to normal
        foreach (GameObject obj in players)
        {
            if (obj == picker) continue;

            CarController car = obj.GetComponent<CarController>();
            if (car != null)
                car.SetSpeedMultiplier(1f);
        }

        foreach (GameObject obj in opponents)
        {
            if (obj == picker) continue;

            CarController car = obj.GetComponent<CarController>();
            if (car != null)
                car.SetSpeedMultiplier(1f);
        }
    }
}