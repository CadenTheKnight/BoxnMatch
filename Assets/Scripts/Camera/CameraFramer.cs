using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraFramer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera actionCam;
    [SerializeField] private Transform cameraTracker;

    private List<GameObject> trackedObjects;

    private float leftmostPos;
    private float rightmostPos;
    private float upmostPos;
    private float downmostPos;

    [Header("Tracker Settings")]
    public float zoomFactor = 0.6f;
    public float maxZoom = 7f;
    public float minZoom = 1f;
    public float reframeInterval = 0.2f;
    public Vector2 battleCenter = Vector2.zero;
    public float zoomBasicDamping = 0.5f;
    public bool updateZoom = true;

    private void Start()
    {
        PlayerRotator[] pcs = FindObjectsOfType<PlayerRotator>();
        trackedObjects = new();
        for(int i = 0; i < pcs.Length; i++)
        {
            trackedObjects.Add(pcs[i].gameObject);
        }
        //Debug.Log("camera is tracking " + trackedObjects.Count + " objects");

        if (cameraTracker == null) cameraTracker = transform;

        StartCoroutine(ReframeRoutine());
    }

    private IEnumerator ReframeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(reframeInterval);
            //wait a frame just in case reframe interval is 0
            yield return null; 


            SetCameraBounds();
            
            //set cameraTrack position to track action
            Vector3 newCameraPos =
                new Vector3(battleCenter.x + ((rightmostPos + leftmostPos) / 2f),
                battleCenter.y + ((upmostPos + downmostPos) / 2f), -30f);

            cameraTracker.transform.DOMove(newCameraPos, reframeInterval);

            if (updateZoom)
            {
                //set camera orthographic zoom to track action
                //calculate a zoom based on the greater between the horizontal and vertical bounds
                float oldZoom = actionCam.m_Lens.OrthographicSize;
                float newZoom = maxZoom;
                float properZoom = rightmostPos - leftmostPos;
                if (upmostPos - leftmostPos > properZoom)
                    properZoom = (upmostPos - leftmostPos) * zoomFactor;

                //if new zoom is calculated to be outside bounds, correct it
                if (properZoom < newZoom) newZoom = properZoom;
                if (newZoom < minZoom) newZoom = minZoom;

                //limit change for smooth zoom changes
                newZoom = ((newZoom * zoomBasicDamping) +
                    (oldZoom * (1 - zoomBasicDamping)));

                DOTween.To(() => (float)actionCam.m_Lens.OrthographicSize,
                    x => actionCam.m_Lens.OrthographicSize = x,
                    newZoom, reframeInterval);
            }
        }
    }

    private void SetCameraBounds()
    {
        leftmostPos = Mathf.Infinity;
        rightmostPos = -Mathf.Infinity;
        upmostPos = -Mathf.Infinity;
        downmostPos = Mathf.Infinity;

        for (int i = 0; i < trackedObjects.Count; i++)
        {
            if (trackedObjects[i] == null)
            {
                trackedObjects.RemoveAt(i);
                continue;
            }

            if (trackedObjects[i].transform.position.x < leftmostPos)
                leftmostPos = trackedObjects[i].transform.position.x;

            if (trackedObjects[i].transform.position.x > rightmostPos)
                rightmostPos = trackedObjects[i].transform.position.x;

            if (trackedObjects[i].transform.position.y < downmostPos)
                downmostPos = trackedObjects[i].transform.position.y;

            if (trackedObjects[i].transform.position.y > upmostPos)
                upmostPos = trackedObjects[i].transform.position.y;
        }
    }
}
