using System;
using System.Collections;
using UnityEngine;

public class RayCaster {

    public Transform StartTransform;
    public Transform EndTransform;
    public Vector3 Direction;
    public float RayLength;
    public int LineCastLayerMask = 0;
    public LayerMask RayCastLayerMask = 0;

    public event Action<Collider> OnRayEnter;
    public event Action<Collider> OnRayStay;
    public event Action<Collider> OnRayExit;

    Collider previous;
    RaycastHit hit = new RaycastHit();

    public bool CastRay() {
        Ray ray = new Ray(StartTransform.position, Direction);
        Physics.Raycast(ray, out hit, RayLength, RayCastLayerMask);
        Debug.DrawRay(StartTransform.position, Direction * hit.distance, Color.red);
        ProcessCollision(hit.collider);
        return hit.collider != null ? true : false;
    }

    public bool CastLine() {
        Physics.Linecast(StartTransform.position, EndTransform.position, out hit, LineCastLayerMask);
        ProcessCollision(hit.collider);
        return hit.collider != null ? true : false;
    }

    private void ProcessCollision(Collider current) {
        // No collision this frame.
        if (current == null) {
            // But there was an object hit last frame.
            if (previous != null) {
                DoEvent(OnRayExit, previous);
            }
        }

        // The object is the same as last frame.
        else if (previous == current) {
            DoEvent(OnRayStay, current);
        }

        // The object is different than last frame.
        else if (previous != null) {
            DoEvent(OnRayExit, previous);
            DoEvent(OnRayEnter, current);
        }

        // There was no object hit last frame.
        else {
            DoEvent(OnRayEnter, current);
        }

        // Remember this object for comparing with next frame.
        previous = current;
    }


    private void DoEvent(Action<Collider> action, Collider collider) {
        if (action != null) {
            action(collider);
        }
    }

    public static int GetLayerMask(string layerName, int existingMask=0) {
        int layer = LayerMask.NameToLayer(layerName);
        return existingMask | (1 << layer);
    }

}