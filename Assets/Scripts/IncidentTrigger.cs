using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncidentTrigger : MonoBehaviour
{
    [EventList("Avalanche")]
    public string incidentName;
    public void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.TriggerEvent(incidentName);
        Destroy(this.gameObject);
    }
}
