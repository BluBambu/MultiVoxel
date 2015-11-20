using System;
using System.Collections;
using UnityEngine;

public class VoxelAnimation : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public void AddVoxelAnimation(Voxel voxel, Action finishCallback)
    {
        StartCoroutine(AddVoxelAnimationCor(voxel, finishCallback));
    }

    public void RemoveVoxelAnimation(Vector3Int pos)
    {
        StartCoroutine(RemoveVoxelAnimationCor(pos));
    }

    private IEnumerator AddVoxelAnimationCor(Voxel voxel, Action finishCallback)
    {
        Transform tempVoxel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        tempVoxel.position = (Vector3) voxel.Pos;
        tempVoxel.localScale = Vector3.zero;
        tempVoxel.GetComponent<MeshRenderer>().material.color = voxel.Color;
        while (tempVoxel.localScale.x < .95f)
        {
            tempVoxel.localScale = Vector3.Lerp(tempVoxel.localScale, Vector3.one, .1f);
            yield return null;
        }
        Destroy(tempVoxel.gameObject);
        if (finishCallback != null)
        {
            finishCallback();
        }
    }

    private IEnumerator RemoveVoxelAnimationCor(Vector3Int pos)
    {
        yield return null;
    }
}
