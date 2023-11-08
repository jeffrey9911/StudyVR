using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    public Transform LeftObjectPivot;
    public Transform MiddleObjectPiovt;
    public Transform RightObjectPivot;

    private List<GameObject> SpawnedObjects = new List<GameObject>();

    void Start()
    {
        LeftObjectPivot.gameObject.SetActive(false);
        MiddleObjectPiovt.gameObject.SetActive(false);
        RightObjectPivot.gameObject.SetActive(false);
    }

    public void SpawnObject()
    {
        string objectLink1 = RuntimeManager.Instance.DATA_MANAGER.LoadedRecord.fields.AssetLink1;
        string objectLink2 = RuntimeManager.Instance.DATA_MANAGER.LoadedRecord.fields.AssetLink2;

        switch (RuntimeManager.Instance.DATA_MANAGER.CurrentStudyType)
        {
            case StudyType.Noset:
                // CALL MANAGER
                break;
                
            case StudyType.Evualuation:
                SpawnedObjects.Add(Instantiate(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink1), MiddleObjectPiovt.position, Quaternion.identity));
                break;

            case StudyType.Comparison:
                SpawnedObjects.Add(Instantiate(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink1), LeftObjectPivot.position, Quaternion.identity));
                SpawnedObjects.Add(Instantiate(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink2), RightObjectPivot.position, Quaternion.identity));
                break;

            default:
                break;
        }
    }

    private void AwaitPreload()
    {

    }
}
