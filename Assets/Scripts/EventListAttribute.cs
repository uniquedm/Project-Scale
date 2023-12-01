using UnityEngine;
using UnityEditor;

// Custom attribute for the EventList
public class EventListAttribute : PropertyAttribute
{
    public readonly string[] eventNames;

    public EventListAttribute(params string[] events)
    {
        this.eventNames = events;
    }
}