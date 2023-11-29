using BuildingVolumes.Streaming;
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

    bool GroundFlag = false;

    void Start()
    {
        LeftObjectPivot.gameObject.SetActive(false);
        MiddleObjectPiovt.gameObject.SetActive(false);
        RightObjectPivot.gameObject.SetActive(false);
    }

    private void Update()
    {
        
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

            //gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            gameObject.SetActive(true);
            meshSequenceStreamerPlayer.Stop();
            meshSequenceStreamerPlayer.Play();
        }

        if(gameObject.TryGetComponent<GeometrySequencePlayer>(out GeometrySequencePlayer geometrySequencePlayer))
        {
            gameObject.SetActive(true);
            
            StartCoroutine(AwaitForPlayGeometrySequence(geometrySequencePlayer, pivotPos));
        }
    }

    IEnumerator AwaitForPlayGeometrySequence(GeometrySequencePlayer geometrySequencePlayer, Vector3 pivotPos)
    {
        while(!geometrySequencePlayer.IsPlaying())
        {
            geometrySequencePlayer.PlayFromStart();

            yield return null;
        }

        while(true)
        {
            if(PosGroundObject(geometrySequencePlayer.gameObject, pivotPos))
            {
                break;
            }
        }
    }



    bool PosGroundObject(GameObject gobj, Vector3 pivotPos)
    {
        MeshFilter msh = FindMeshFilterInChildren(gobj);

        if(msh == null) return false;

        Bounds bounds = msh.mesh.bounds;

        if (bounds == null) return false;

        if (bounds.min.y - bounds.max.y == 0) return false;

        gobj.transform.position = new Vector3
            (
                pivotPos.x,
                pivotPos.y - bounds.min.y,
                pivotPos.z
            );
        
        //gobj.transform.rotation = Quaternion.Euler(0, 180, 0);


        return true;
    }


    // A function that recursively looks for a MeshFilter component in the children of a GameObject and returns the first one it finds.
    private MeshFilter FindMeshFilterInChildren(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            return meshFilter;
        }

        foreach (Transform child in gameObject.transform)
        {
            meshFilter = FindMeshFilterInChildren(child.gameObject);

            if (meshFilter != null)
            {
                return meshFilter;
            }
        }

        return null;
    }

    private void AwaitPreload()
    {

    }
}
