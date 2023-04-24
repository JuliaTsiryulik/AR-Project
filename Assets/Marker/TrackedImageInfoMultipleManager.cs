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
    private GameObject[] arObjectsToPlace;

    //[SerializeField]
    //private Vector3 scaleFactor = new Vector3(0.1f, 0.1f, 0.1f);

    private ARTrackedImageManager m_TrackedImageManager;

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        foreach (GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);
            newARObject.SetActive(false);
        }

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

            arObject.Value.SetActive(false);
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

            arObjects[trackedImage.referenceImage.name].SetActive(true);

            AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);
        }

        //imageTrackedText.text = trackedImage.referenceImage.name;
        /*
        if (trackedImage.trackingState != TrackingState.None)
        {
           
        }*/




        /*foreach (GameObject go in arObjects.Values)
        {
            if (trackedImage.trackingState != TrackingState.None)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }*/

        /*
        foreach (GameObject go in arObjects.Values)
        {
            //Debug.Log($"Go in arObjects.Values: {go.name}");
            if (go.name != trackedImage.referenceImage.name)
            {
                go.SetActive(false);
            }
        }
        */
        //Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        if (arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name];
            //goARObject.SetActive(true);
            goARObject.transform.position = newPosition;
            /*goARObject.transform.localScale = scaleFactor;*/

            /*
            foreach (GameObject go in arObjects.Values)
            {
                Debug.Log($"Go in arObjects.Values: {go.name}");
                if (go.name != name)
                {
                    go.SetActive(false);
                }
            }
            */
        }
    }
}
