using System;
using UnityEngine;
using System.Collections.Generic;

public class ActorMaterialEffect : MonoBehaviour
{
    List<Material> materialCache = new List<Material>();
    public float scale;
    public Color color;    
    void Start()
    {
        SkinnedMeshRenderer[] renders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        for(int i = 0; i < renders.Length; ++i)
        {
            var current = renders[i];
            
            for(int j = 0; j < current.materials.Length; ++j)
            {
                var mat = current.materials[j];
                
                materialCache.Add(mat);
            }
        }
    }
    
    void OnWillRenderObject()
    {
        Vector4 pos = gameObject.transform.position;
        for(int i = 0; i < materialCache.Count; ++i)
        {
            var current = materialCache[i];
            
            current.SetColor("_SingleColor",color);
            current.SetFloat("_Scale",scale);
            current.SetVector("_ActorPos",pos);
            current.renderQueue = 3000;
        }
    }
}