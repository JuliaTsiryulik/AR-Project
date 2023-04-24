using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using TMPro;


[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoMultipleManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI imageTrackedText;

    [SerializeField]
    private TextMeshProUGUI[] arObjectsToPlace;

    public GameObject PanelInfo;

    private ARTrackedImageManager m_TrackedImageManager;

    private Dictionary<string, TextMeshProUGUI> arObjects = new Dictionary<string, TextMeshProUGUI>();

    private void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        foreach (var arObject in arObjectsToPlace)
        {
            //GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            TextMeshProUGUI newARObject = arObject;
            arObjects.Add(arObject.name, newARObject);
            newARObject.enabled = false;
        }

        PanelInfo.SetActive(false);
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private static bool IsInList(string name, List<ARTrackedImage> imgs)
    {
        foreach (var img in imgs)
        {
            if (img.referenceImage.name == name &&
                img.trackingState == TrackingState.Tracking)
            {
                return true;
            }
        }

        return false;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var arObject in arObjects)
        {
            if (IsInList(arObject.Key, eventArgs.added) ||
                IsInList(arObject.Key, eventArgs.updated))
            {
                continue;
            }

            imageTrackedText.text = "Nothing tracked";

            arObject.Value.enabled = false;

            PanelInfo.SetActive(false);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateARImage(trackedImage);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            imageTrackedText.text = trackedImage.referenceImage.name;

            PanelInfo.SetActive(true);
            arObjects[trackedImage.referenceImage.name].enabled = true;

            AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);
        }
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        if (arObjectsToPlace != null)
        {
            TextMeshProUGUI goARObject = arObjects[name];
            //goARObject.transform.position = newPosition;
        }
    }
}
