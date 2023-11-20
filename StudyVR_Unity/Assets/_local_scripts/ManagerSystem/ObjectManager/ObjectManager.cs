using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

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
                SpawnedObjects.Add(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink1));
                PlayAnimation(SpawnedObjects[0], MiddleObjectPiovt.position);
                break;

            case StudyType.Comparison:
                SpawnedObjects.Add(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink1));
                PlayAnimation(SpawnedObjects[0], LeftObjectPivot.position);

                SpawnedObjects.Add(RuntimeManager.Instance.DATA_MANAGER.GetPreloadedObject(objectLink2));
                PlayAnimation(SpawnedObjects[1], RightObjectPivot.position);
                break;

            default:
                break;
        }
    }

    private void PlayAnimation(GameObject gameObject, Vector3 pivotPos)
    {
        if (gameObject.TryGetComponent<MeshSequenceStreamerPlayer>(out MeshSequenceStreamerPlayer meshSequenceStreamerPlayer))
        {
            Bounds bounds = gameObject.GetComponent<MeshRenderer>().bounds;
            gameObject.transform.position = new Vector3
            (
                pivotPos.x,
                pivotPos.y - bounds.min.y,
                pivotPos.z
            );

            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            meshSequenceStreamerPlayer.Stop();
            meshSequenceStreamerPlayer.Play();
        }
    }

    private void AwaitPreload()
    {

    }
}
