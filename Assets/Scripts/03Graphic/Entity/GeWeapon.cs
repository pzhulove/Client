// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// 
// public class GeWeapon : GeAttach
// {
//     public GeWeapon(string name) : base (name)
//     {
// 
//     }
// 
//     protected class PhaseMatSurfDesc
//     {
//         public PhaseMatSurfDesc(Material[] origMat, Renderer mr)
//         {
//             m_MeshRenderer = mr;
//             m_OriginMatList = origMat;
//         }
// 
//         public Renderer m_MeshRenderer = null;
//         public Material[] m_OriginMatList = null;
//     }
//     protected List<PhaseMatSurfDesc> m_PhaseMatSurfDescList = new List<PhaseMatSurfDesc>();
// 
//     public void ChangePhase(string phaseEffect,int phaseIdx)
//     {
//         return;
//         
//         Logger.LogWarningFormat("Change phase effect [{0}] stage index [{1}]", phaseEffect, phaseIdx);
// 
//         if(!string.IsNullOrEmpty(phaseEffect) && phaseIdx > 0)
//         {
//             Material phaseMat = GePhaseMaterial.instance.CreatePhaseMaterial(phaseEffect, phaseIdx-1);
//             if (null != phaseMat)
//             {
//                 MeshRenderer[] amr = root.GetComponentsInChildren<MeshRenderer>();
//                 for (int j = 0; j < amr.Length; ++j)
//                 {
//                     if (null == amr[j]) continue;
// 
// 					if (amr [j].gameObject.CompareTag ("EffectModel"))
// 						continue;
// 
//                     Material[] am = amr[j].materials;
//                     Material[] amat = new Material[am.Length];
// 
//                     PhaseMatSurfDesc newPhaseMatSurf = new PhaseMatSurfDesc(am, amr[j]);
//                     for (int k = 0; k < am.Length; ++k)
//                     {
//                         if (null == am[k]) continue;
// 
//                         Material newMat = new Material(phaseMat);
//                         if (null == newMat) continue;
// 
//                         if (am[k].HasProperty("_MainTex"))
//                             newMat.SetTexture("_MainTex", am[k].GetTexture("_MainTex"));
// 
//                         if (am[k].HasProperty("_BumpMap"))
//                             newMat.SetTexture("_BumpMap", am[k].GetTexture("_BumpMap"));
// 
//                         if (am[k].HasProperty("_Ramp"))
//                             newMat.SetTexture("_Ramp", am[k].GetTexture("_Ramp"));
// 
//                         amat[k] = newMat;
//                     }
// 
//                     amr[j].materials = amat;
//                     m_PhaseMatSurfDescList.Add(newPhaseMatSurf);
//                 }
//             }
//         }
//         else
//         {
//             _ClearPhase();
//         }
//     }
// 
//     protected void _ClearPhase()
//     {
//         m_PhaseMatSurfDescList.RemoveAll(
//             e =>
//             {
//                 Material[] am = e.m_MeshRenderer.materials;
//                 e.m_MeshRenderer.materials = e.m_OriginMatList;
// 
//                 for (int k = 0; k < am.Length; ++k)
//                 {
//                     if (null == am[k]) continue;
// 
//                     Object.Destroy(am[k]);
//                     am[k] = null;
//                 }
//                 return true;
//             });
//     }
// }
