using UnityEngine;
using UnityEditor;

public class EventListExample : MonoBehaviour
{
    [EventList]
    public string selectedEvent;
}

// Custom property drawer for the EventList attribute
[CustomPropertyDrawer(typeof(EventListAttribute))]
public class EventListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Ensure that the property is a string
        if (property.propertyType == SerializedPropertyType.String)
        {
            // Get the list of events from the attribute
            EventListAttribute eventListAttribute = attribute as EventListAttribute;
            string[] eventNames = eventListAttribute.eventNames;

            // Find the index of the currently selected event
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(eventNames, property.stringValue));

            // Draw the dropdown list
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, eventNames);

            // Set the selected event based on the index
            property.stringValue = eventNames[selectedIndex];
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use EventList with string fields only.");
        }
    }
}

// Custom attribute for the EventList
public class EventListAttribute : PropertyAttribute
{
    public readonly string[] eventNames;

    public EventListAttribute(params string[] events)
    {
        this.eventNames = events;
    }
}