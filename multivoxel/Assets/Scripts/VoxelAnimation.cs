using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelAnimation : MonoBehaviour
{
    private class AnimTuple
    {
        public Transform Trans;
        public Coroutine Cor;
    }

    private Dictionary<Vector3Int, AnimTuple> _animDic;

    private void Awake()
    {
        _animDic = new Dictionary<Vector3Int, AnimTuple>();
    }

    public void AddVoxelAnimation(bool hasPreviousBlock, Voxel voxel, Action finishCallback, bool firstTime = true)
    {
        if (_animDic.ContainsKey(voxel.Pos) && firstTime) // Animation going on at this point
        {
            RemoveVoxelAnimation(hasPreviousBlock, voxel, () =>
            {
                AddVoxelAnimation(false, voxel, () =>
                {
                    finishCallback();
                }, false);
            });
        }
        else if (_animDic.ContainsKey(voxel.Pos))
        {
            AnimTuple anim = _animDic[voxel.Pos];
            anim.Trans.GetComponent<MeshRenderer>().material.color = voxel.Color;
            anim.Cor = anim.Trans.GetComponent<MonoBehaviour>().StartCoroutine(AddVoxelAnimationCor(voxel.Pos, anim.Trans, () =>
            {
                finishCallback();
            }));
        }
        else // No animation going on at this point
        {
            if (hasPreviousBlock)
            {
                RemoveVoxelAnimation(hasPreviousBlock, voxel, () =>
                {
                    AddVoxelAnimation(false, voxel, () =>
                    {
                        finishCallback();
                    }, false);
                });
            }
            else
            {
                AnimTuple anim = new AnimTuple();
                _animDic[voxel.Pos] = anim;
                anim.Trans = CreateAnimTransform(voxel);
                anim.Cor = anim.Trans.GetComponent<MonoBehaviour>().StartCoroutine(AddVoxelAnimationCor(voxel.Pos, anim.Trans, () =>
                {
                    finishCallback();
                }));
            }
        }
    }

    public void RemoveVoxelAnimation(bool hasPreviousBlock, Voxel voxel, Action finishCallback)
    {
        if (_animDic.ContainsKey(voxel.Pos))
        {
            AnimTuple anim = _animDic[voxel.Pos];
            anim.Trans.GetComponent<MeshRenderer>().material.color = voxel.Color;
            anim.Trans.GetComponent<MonoBehaviour>().StopAllCoroutines();
            anim.Cor = anim.Trans.GetComponent<MonoBehaviour>().StartCoroutine(RemoveVoxelAnimationCor(voxel.Pos, anim.Trans, () =>
            {
                finishCallback();
            }));
        }
        else
        {
            if (hasPreviousBlock)
            {
                AnimTuple anim = new AnimTuple();
                _animDic[voxel.Pos] = anim;
                anim.Trans = CreateAnimTransform(voxel);
                anim.Trans.localScale = Vector3.one;
                anim.Cor = anim.Trans.GetComponent<MonoBehaviour>().StartCoroutine(RemoveVoxelAnimationCor(voxel.Pos, anim.Trans, () =>
                {
                    finishCallback();
                }));

            }
        }
    }

    private IEnumerator AddVoxelAnimationCor(Vector3Int pos, Transform animTrans, Action finishCallback)
    {
        while (animTrans.localScale.x < .98f)
        {
            animTrans.GetComponent<MeshRenderer>().enabled = true;
            animTrans.localScale = Vector3.Lerp(animTrans.localScale, Vector3.one, .1f);
            yield return null;
        }
        animTrans.GetComponent<MeshRenderer>().enabled = false;
        if (finishCallback != null)
        {
            finishCallback();
        }
    }

    private IEnumerator RemoveVoxelAnimationCor(Vector3Int pos, Transform animTrans, Action finishCallback)
    {
        while (animTrans.localScale.x > .02f)
        {
            animTrans.GetComponent<MeshRenderer>().enabled = true;
            animTrans.localScale = Vector3.Lerp(animTrans.localScale, Vector3.zero, .1f);
            yield return null;
        }
        animTrans.GetComponent<MeshRenderer>().enabled = false;
        if (finishCallback != null)
        {
            finishCallback();
        }
    }

    private Transform CreateAnimTransform(Voxel voxel)
    {
        Transform tempVoxel = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        tempVoxel.position = (Vector3)voxel.Pos;
        tempVoxel.localScale = Vector3.zero;
        tempVoxel.GetComponent<MeshRenderer>().material.color = voxel.Color;
        tempVoxel.gameObject.AddComponent<MonoBehaviour>();

        return tempVoxel;
    }
}
