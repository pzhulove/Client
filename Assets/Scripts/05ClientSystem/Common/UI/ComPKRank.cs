﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using DG.Tweening;


namespace GameClient
{
    public class ComPKRank : MonoBehaviour
    {
        public RectTransform rectStarRoot;
        public GameObject objStarTemplate;
        public Text labRank;
        public Image imgRankIcon;
        public GameObject objRankMaxRoot;
        public Text labRankMax;
        public Slider sliderExp;
        public Text labExp;

        public GameObject[] objEffects = new GameObject[0];

        public bool test = false;
        public int startID = SeasonDataManager.GetInstance().GetMinRankID();
        public int startStar = 0;
        public int startExp = 0;
        public int endID = 14505;
        public int endStar = 0;
        public int endExp = 0;
        public float starEffectTime = 0.45f;
        public float textChangeDelay = 0.25f;
        public float upAnimDelay = 0.5f;
        public float downAnimDelay = 0.5f;
        public float expSpeed = 1;

        static float ms_fStarAngleInterval = 45.0f;
        static string ms_strGetStarEffectPath = "Effects/Scene_effects/EffectUI/EffUI_ui_xing";
        static string ms_strGetStarEffect = "GetEffect";
        static string ms_strLoseStarEffectPath = "Effects/Scene_effects/EffectUI/EffUI_ui_xingjiang";
        static string ms_strLoseStarEffect = "LoseEffect";

//         static string[] ms_arrSubRankUpEffectPaths = new string[]
//         {
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_qingtong",
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_baiyin",
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_haungjin",
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_bojin",
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_zuanshi",
//             "Effects/Scene_effects/EffectUI/EffUI_xiaoduanwei_zhizun",
//         };
// 
//         static string[] ms_arrSubRankUpEffects = new string[]
//         {
//             "SubRankUpEffect_qingtong",
//             "SubRankUpEffect_baiyin",
//             "SubRankUpEffect_haungjin",
//             "SubRankUpEffect_bojin",
//             "SubRankUpEffect_zuanshi",
//             "SubRankUpEffect_zhizun",
//         };
// 
//         static string ms_strSubRankDownEffectPath = "Effects/Scene_effects/EffectUI/EffUI_shibai_xiao ";
// 
//         static string ms_strSubRankDownEffect = "SubRankDownEffect";
// 
//         static string[] ms_arrMainRankUpEffectPaths = new string[]
//         {
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_qingtong",
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_baiyin",
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_huangjin",
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_bojin",
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_zuanshi",
//             "Effects/Scene_effects/EffectUI/EffUI_shengduan_zhizun",
//         };
// 
//         static string[] ms_arrMainRankUpEffects = new string[]
//         {
//             "MainRankUpEffect_qingtong",
//             "MainRankUpEffect_baiyin",
//             "MainRankUpEffect_haungjin",
//             "MainRankUpEffect_bojin",
//             "MainRankUpEffect_zuanshi",
//             "MainRankUpEffect_zhizun",
//         };
// 
//         static string[] ms_arrMainRankDownEffectPaths = new string[]
//         {
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_qingtong",
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_baiyin",
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_huangjin",
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_bojin",
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_zuanshi",
//             "Effects/Scene_effects/EffectUI/EffUI_shibai_zhizun",
//         };
// 
//         static string[] ms_arrMainRankDownEffects = new string[]
//         {
//             "MainRankDownEffect_qingtong",
//             "MainRankDownEffect_baiyin",
//             "MainRankDownEffect_haungjin",
//             "MainRankDownEffect_bojin",
//             "MainRankDownEffect_zuanshi",
//             "MainRankDownEffect_zhizun",
//         };

        static string ms_strAnimRankUpTag = "shengduan";
        static string ms_strAnimRankDownTag = "jiangduan";
        static string ms_strAnimNormalTag = "tongyong";

        UnityEngine.Coroutine m_iterRankIncrease = null;
        UnityEngine.Coroutine m_iterRankDecrease = null;
        UnityEngine.Coroutine m_iterRankNormal = null;
        List<DOTweenAnimation> m_arrRankUpAnims = new List<DOTweenAnimation>();
        List<DOTweenAnimation> m_arrRankDownAnims = new List<DOTweenAnimation>();
        List<DOTweenAnimation> m_arrRankNormalAnims = new List<DOTweenAnimation>();

        static public CommonPKRank CreateCommonPKRank(GameObject a_objParent)
        {
            if (a_objParent == null)
            {
                Logger.LogError("ComPKRank Create function param parent is null!");
                return null;
            }

            GameObject obj = AssetLoader.GetInstance().LoadResAsGameObject("UIFlatten/Prefabs/Pk/CommonPKRank");
            if (obj != null)
            {
                CommonPKRank comRank = obj.GetComponent<CommonPKRank>();
                if (comRank != null)
                {
                    comRank.gameObject.transform.SetParent(a_objParent.transform, false);
                    return comRank;
                }
            }
            return null;
        }

        static public ComPKRank Create(GameObject a_objParent)
        {
            if (a_objParent == null)
            {
                Logger.LogError("ComPKRank Create function param parent is null!");
                return null;
            }

            GameObject obj = AssetLoader.GetInstance().LoadResAsGameObject("UIFlatten/Prefabs/Pk/PKRank");
            if (obj != null)
            {
                ComPKRank comRank = obj.GetComponent<ComPKRank>();
                if (comRank != null)
                {
                    comRank.gameObject.transform.SetParent(a_objParent.transform, false);
                    return comRank;
                }
            }
            return null;
        }

        void Awake()
        {
            m_arrRankUpAnims.Clear();
            m_arrRankDownAnims.Clear();
            m_arrRankNormalAnims.Clear();
            Component[] coms = gameObject.GetComponentsInChildren(typeof(DOTweenAnimation));
            for (int i = 0; i < coms.Length; ++i)
            {
                DOTweenAnimation anim = coms[i] as DOTweenAnimation;
                if (anim != null)
                {
                    if (anim.id == ms_strAnimRankUpTag)
                    {
                        m_arrRankUpAnims.Add(anim);
                        anim.isActive = false;
                    }
                    else if (anim.id == ms_strAnimRankDownTag)
                    {
                        m_arrRankDownAnims.Add(anim);
                        anim.isActive = false;
                    }
                    else// if (anim.id == ms_strAnimNormalTag)
                    {
                        m_arrRankNormalAnims.Add(anim);
                        anim.isActive = false;
                    }
                }
            }
        }

        void OnValidate()
        {
            if (test)
            {
                StartRankChange(startID, startStar, startExp, endID, endStar, endExp);
                test = false;
            }
        }

        public void Initialize(int a_nRankID, int a_nExp)
        {
            _SetupRank(a_nRankID, a_nExp);
        }

        public void Clear()
        {
            if (IsRankChanging())
            {
                StopRankChange();
            }

            if (m_iterRankNormal != null)
            {
                StopCoroutine(m_iterRankNormal);
                m_iterRankNormal = null;
            }
        }

        public void StartRankChange(int a_nStartRankID, int a_nStartStar, int a_nStartExp, int a_nEndRankID, int a_nEndStar, int a_nEndExp)
        {
            StopRankChange();
            if (
                TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nStartRankID) == null ||
                TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nEndRankID) == null
                )
            {
                return;
            }

            if (a_nStartRankID > a_nEndRankID)
            {
                m_iterRankDecrease = StartCoroutine(_DecreaseRank(a_nStartRankID, a_nStartExp, a_nEndRankID, a_nEndExp));
            }
            else if (a_nStartRankID < a_nEndRankID)
            {
                m_iterRankIncrease = StartCoroutine(_IncreaseRank(a_nStartRankID, a_nStartExp, a_nEndRankID, a_nEndExp));
            }
            else
            {
                if (a_nStartExp > a_nEndExp)
                {
                    m_iterRankDecrease = StartCoroutine(_DecreaseRank(a_nStartRankID, a_nStartExp, a_nEndRankID, a_nEndExp));
                }
                else if (a_nStartExp < a_nEndExp)
                {
                    m_iterRankIncrease = StartCoroutine(_IncreaseRank(a_nStartRankID, a_nStartExp, a_nEndRankID, a_nEndExp));
                }
                else
                {
                    _SetupRank(a_nEndRankID, a_nEndExp);
                }
            }
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RankUpEnded);
        }

        public void StopRankChange()
        {
            if (m_iterRankIncrease != null)
            {
                StopCoroutine(m_iterRankIncrease);
                m_iterRankIncrease = null;
            }

            if (m_iterRankDecrease != null)
            {
                StopCoroutine(m_iterRankDecrease);
                m_iterRankDecrease = null;
            }
        }

        public bool IsRankChanging()
        {
            return m_iterRankIncrease != null || m_iterRankDecrease != null;
        }

        public bool IsRankIncreasing()
        {
            return m_iterRankIncrease != null;
        }

        public bool IsRankDecreasing()
        {
            return m_iterRankDecrease != null;
        }

        IEnumerator _IncreaseRank(int a_nStartRankID, int a_nStartExp, int a_nEndRankID, int a_nEndExp)
        {
            _SetupRank(a_nStartRankID, a_nStartExp);
            yield return Yielders.EndOfFrame;

            int nNewExp = a_nStartExp;

            SeasonLevelTable oldRank = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nStartRankID);
            while (oldRank != null)
            {
                SeasonLevelTable newRank = null;
                if (oldRank.ID >= a_nEndRankID)
                {
                    newRank = oldRank;
                }
                else
                {
                    newRank = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(oldRank.AfterId);
                }
                if (newRank != null)
                {
                    if (oldRank.MainLevel < newRank.MainLevel)
                    {
                        SeasonLevelTable newTableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(newRank.ID);

                        string strEffectCtrl = newRank.ID >= SeasonDataManager.GetInstance().GetMaxRankID() ? "EffectUpMaxLevel" : "EffectUpMainLevel";
                        ComEffectController controller = Utility.GetComponetInChild<ComEffectController>(gameObject, strEffectCtrl);
                        if (controller != null)
                        {
                            controller.Clear();

                            controller.RegisterEvent(EEffectEvent.SeasonLevel_StartChangeLevel, () =>
                            {
                                _ClearAllStars();
                            });

                            controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelIcon, () =>
                            {
                                _SetupRankIcon(newTableData);
                            });

                            controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelName, () =>
                            {
                                _SetupRankName(newTableData);
                            });

                            controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelStar, () =>
                            {
                                _SetupRankStars(newTableData, true);
                            });

                            controller.RegisterEvent(EEffectEvent.SeasonLevel_FinishChangeLevel, () =>
                            {
                                _SetupRankStars(newTableData, false);
                            });

                            controller.Play();

                            yield return Yielders.GetWaitForSeconds(controller.Duration());
                        }
                    }
                    else if (oldRank.MainLevel == newRank.MainLevel)
                    {
                        if (oldRank.SmallLevel > newRank.SmallLevel)
                        {
                            SeasonLevelTable newTableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(newRank.ID);

                            ComEffectController controller = Utility.GetComponetInChild<ComEffectController>(gameObject, "EffectUpSubLevel");
                            if (controller != null)
                            {
                                controller.Clear();

                                controller.RegisterEvent(EEffectEvent.SeasonLevel_StartChangeLevel, () =>
                                {
                                    _ClearAllStars();
                                });

                                controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelIcon, () =>
                                {
                                    _SetupRankIcon(newTableData);
                                });

                                controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelName, () =>
                                {
                                    _SetupRankName(newTableData);
                                });

                                controller.RegisterEvent(EEffectEvent.SeasonLevel_ChangeLevelStar, () =>
                                {
                                    _SetupRankStars(newTableData, true);
                                });

                                controller.RegisterEvent(EEffectEvent.SeasonLevel_FinishChangeLevel, () =>
                                {
                                    _SetupRankStars(newTableData, false);
                                });

                                controller.Play();

                                yield return Yielders.GetWaitForSeconds(controller.Duration());
                            }
                        }
                        else if (newRank.SmallLevel == oldRank.SmallLevel)
                        {
                            if (oldRank.Star < newRank.Star)
                            {
                                nNewExp = oldRank.MaxExp;
                                yield return _ShowIncreaseExp(nNewExp, oldRank.MaxExp);
                                
                                yield return _ShowIncreaseStar(newRank.Star - 1);
                                nNewExp = 0;
                                yield return _ShowIncreaseExp(nNewExp, newRank.MaxExp);
                                
                            }
                            else if (newRank.Star == oldRank.Star)
                            {
                                nNewExp = a_nEndExp;
                                yield return _ShowIncreaseExp(a_nEndExp, newRank.MaxExp);
                            }
                            else
                            {
                                Logger.LogErrorFormat("【段位表】错误 ID:{0}的星星 > 下一个段位ID:{1}的星星！", oldRank.ID, oldRank.AfterId);
                                break;
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("【段位表】错误 ID:{0}的小段位 > 下一个段位ID:{1}的小段位！", oldRank.ID, oldRank.AfterId);
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("【段位表】错误  ID:{0}的大段位 > 下一个段位ID:{1}的大段位！", oldRank.ID, oldRank.AfterId);
                        break;
                    }


                    if (newRank.ID >= a_nEndRankID && nNewExp >= a_nEndExp)
                    {
                        break;
                    }

                    oldRank = newRank;
                }
                else
                {
                    if (oldRank.AfterId != 0)
                    {
                        Logger.LogErrorFormat("【段位表】错误 ID:{0}的下一个段位ID:{1}不存在！", oldRank.ID, oldRank.AfterId);
                        break;
                    }
                }
            }

            m_iterRankIncrease = null;
        }

        IEnumerator _ShowIncreaseExp(int a_nExp, int a_nMaxExp)
        {
            float fProgress = a_nExp / (float)a_nMaxExp;
            if (fProgress < 0.0f)
            {
                fProgress = 0.0f;
            }
            else if (fProgress > 1.0f)
            {
                fProgress = 1.0f;
            }

            while (sliderExp.value < fProgress)
            {
                float fValue = sliderExp.value + expSpeed * Time.deltaTime;
                if (fValue >= 1.0f)
                {
                    sliderExp.value = 1.0f;
                    labExp.text = string.Format("{0}/{1}", a_nMaxExp, a_nMaxExp);
                    break;
                }
                else
                {
                    sliderExp.value = fValue;
                    labExp.text = string.Format("{0}/{1}", (int)(fValue * a_nMaxExp), a_nMaxExp);
                }
                yield return Yielders.EndOfFrame;
            }
            sliderExp.value = fProgress;
            labExp.text = string.Format("{0}/{1}", a_nExp, a_nMaxExp);

            if (sliderExp.value >= 1.0f)
            {

                yield return Yielders.GetWaitForSeconds(0.1f);
            }
        }

        IEnumerator _ShowIncreaseStar(int a_nIdx)
        {
            if (a_nIdx >= 0 && a_nIdx < rectStarRoot.childCount)
            {
                GameObject objRoot = rectStarRoot.GetChild(a_nIdx).gameObject;
                GameObject objEffect = Utility.FindGameObject(objRoot, ms_strGetStarEffect, false);
                if (objEffect == null)
                {
                    objEffect = AssetLoader.GetInstance().LoadResAsGameObject(ms_strGetStarEffectPath);
                    objEffect.transform.SetParent(objRoot.transform, false);
                    objEffect.name = ms_strGetStarEffect;
                }
                objEffect.SetActive(true);

                yield return Yielders.GetWaitForSeconds(starEffectTime);

                AudioManager.instance.PlaySound(17);

                Utility.FindGameObject(objRoot, "Light").SetActive(true);
            }

            yield return Yielders.GetWaitForSeconds(0.1f);
        }

        IEnumerator _DecreaseRank(int a_nStartRankID, int a_nStartExp, int a_nEndRankID, int a_nEndExp)
        {
            _SetupRank(a_nStartRankID, a_nStartExp);
            yield return Yielders.EndOfFrame;

            int nNewExp = a_nStartExp;

            SeasonLevelTable oldRank = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nStartRankID);
            while (oldRank != null)
            {
                SeasonLevelTable newRank = null;
                if (oldRank.ID <= a_nEndRankID)
                {
                    newRank = oldRank;
                }
                else
                {
                    newRank = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(oldRank.PreId);
                }
                if (newRank != null)
                {
                    if (oldRank.MainLevel > newRank.MainLevel)
                    {
                        SeasonLevelTable newTableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(newRank.ID);

                        nNewExp = 0;
                        yield return _ShowDecreaseExp(nNewExp, oldRank.MaxExp);

                        yield return _ShowDecreaseStar(oldRank.Star - 1);
                        nNewExp = newRank.MaxExp;
                        yield return _ShowDecreaseExp(nNewExp, newRank.MaxExp);
                        

                        _PlayParticles(Utility.FindGameObject(gameObject, "EffUI_shibai_da"));

                        yield return Yielders.GetWaitForSeconds(downAnimDelay);

                        yield return Yielders.GetWaitForSeconds(_PlayAnims(m_arrRankDownAnims));
                        _SetupRankIcon(newTableData);
                        _SetupRankStars(newTableData, false);
                        _RewindAnims(m_arrRankDownAnims);

                        yield return Yielders.GetWaitForSeconds(_PlayAnims(m_arrRankUpAnims));
                        //Utility.FindGameObject(gameObject, "RankDesc/EffUI_shengduan_zi").SetActive(true);

                        //yield return Yielders.GetWaitForSeconds(textChangeDelay);
                        _SetupRankName(newTableData);

                        yield return Yielders.GetWaitForSeconds(0.5f);
                        _StopEffect();
                    }
                    else if (newRank.MainLevel == oldRank.MainLevel)
                    {
                        if (newRank.SmallLevel > oldRank.SmallLevel)
                        {
                            SeasonLevelTable newTableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(newRank.ID);

                            nNewExp = 0;
                            yield return _ShowDecreaseExp(nNewExp, oldRank.MaxExp);

                            yield return _ShowDecreaseStar(oldRank.Star - 1);
                            nNewExp = newRank.MaxExp;
                            yield return _ShowDecreaseExp(nNewExp, newRank.MaxExp);

                            _PlayParticles(Utility.FindGameObject(gameObject, "EffUI_shibai_xiao"));

                            yield return Yielders.GetWaitForSeconds(downAnimDelay);

                            yield return Yielders.GetWaitForSeconds(_PlayAnims(m_arrRankDownAnims));
                            _SetupRankIcon(newTableData);
                            _SetupRankStars(newTableData, false);
                            _RewindAnims(m_arrRankDownAnims);

                            yield return Yielders.GetWaitForSeconds(_PlayAnims(m_arrRankUpAnims));
                            //Utility.FindGameObject(gameObject, "RankDesc/EffUI_shengduan_zi").SetActive(true);

                            //yield return Yielders.GetWaitForSeconds(textChangeDelay);
                            _SetupRankName(newTableData);

                            yield return Yielders.GetWaitForSeconds(0.5f);
                            _StopEffect();
                        }
                        else if (newRank.SmallLevel == oldRank.SmallLevel)
                        {
                            if (oldRank.Star > newRank.Star)
                            {
                                nNewExp = 0;
                                yield return _ShowDecreaseExp(nNewExp, oldRank.MaxExp);

                                yield return _ShowDecreaseStar(oldRank.Star - 1);
                                nNewExp = newRank.MaxExp;
                                yield return _ShowDecreaseExp(nNewExp, newRank.MaxExp);

                                yield return Yielders.GetWaitForSeconds(0.3f);
                            }
                            else if (newRank.Star == oldRank.Star)
                            {
                                nNewExp = a_nEndExp;
                                yield return _ShowDecreaseExp(nNewExp, oldRank.MaxExp);
                            }
                            else
                            {
                                Logger.LogErrorFormat("【段位表】错误 ID:{0}的星星 < 上一个段位ID:{1}的星星！", oldRank.ID, oldRank.PreId);
                                break;
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("【段位表】错误 ID:{0}的小段位 < 上一个段位ID:{1}的小段位！", oldRank.ID, oldRank.PreId);
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogErrorFormat("【段位表】错误 ID:{0}的大段位 < 上一个段位ID:{1}的大段位！", oldRank.ID, oldRank.PreId);
                        break;
                    }


                    if (newRank.ID <= a_nEndRankID && nNewExp <= a_nEndExp)
                    {
                        break;
                    }

                    oldRank = newRank;
                }
                else
                {
                    if (oldRank.PreId != 0)
                    {
                        Logger.LogErrorFormat("【段位表】错误 ID:{0}的上一个段位ID:{1}不存在！", oldRank.ID, oldRank.PreId);
                        break;
                    }
                }
            }

            m_iterRankDecrease = null;
        }

        IEnumerator _ShowDecreaseExp(int a_nExp, int a_nMaxExp)
        {
            float fProgress = a_nExp / (float)a_nMaxExp;
            if (fProgress < 0.0f)
            {
                fProgress = 0.0f;
            }
            else if (fProgress > 1.0f)
            {
                fProgress = 1.0f;
            }

            while (sliderExp.value > fProgress)
            {
                float fValue = sliderExp.value - expSpeed * Time.deltaTime;
                if (fValue <= 0.0f)
                {
                    sliderExp.value = 0.0f;
                    labExp.text = string.Format("{0}/{1}", 0, a_nMaxExp);
                    break;
                }
                else
                {
                    sliderExp.value = fValue;
                    labExp.text = string.Format("{0}/{1}", (int)(fValue * a_nMaxExp), a_nMaxExp);
                }
                yield return Yielders.EndOfFrame;
            }
            sliderExp.value = fProgress;
            labExp.text = string.Format("{0}/{1}", a_nExp, a_nMaxExp);

            if (sliderExp.value <= 0.0f)
            {

                yield return Yielders.GetWaitForSeconds(0.1f);
            }
        }

        IEnumerator _ShowDecreaseStar(int a_nIdx)
        {
            if (a_nIdx >= 0 && a_nIdx < rectStarRoot.childCount)
            {
                GameObject objRoot = rectStarRoot.GetChild(a_nIdx).gameObject;
                GameObject objEffect = Utility.FindGameObject(objRoot, ms_strLoseStarEffect, false);
                if (objEffect == null)
                {
                    objEffect = AssetLoader.GetInstance().LoadResAsGameObject(ms_strLoseStarEffectPath);
                    objEffect.transform.SetParent(objRoot.transform, false);
                    objEffect.name = ms_strLoseStarEffect;
                }
                objEffect.SetActive(true);

                yield return Yielders.GetWaitForSeconds(starEffectTime);

                AudioManager.instance.PlaySound(18);

                Utility.FindGameObject(objRoot, "Light").SetActive(false);
            }
        }

        void _SetupRank(int a_nRankID, int a_nExp, bool bClearStar = false)
        {
            SeasonLevelTable tableData = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(a_nRankID);
            if (tableData != null)
            {
                _SetupRankIcon(tableData);
                _SetupRankName(tableData);
                _SetupRankStars(tableData, bClearStar);
                _SetupRankExp(tableData, a_nExp);

                if (SeasonDataManager.GetInstance().GetPromotionInfo(a_nRankID).eState == EPromotionState.Promoting)
                {
                    ComEffectController controller = Utility.GetComponetInChild<ComEffectController>(gameObject, "EffectPromotion");
                    if (controller != null)
                    {
                        controller.Clear();
                        controller.Play();
                    }
                }
            }
            else
            {
                Logger.LogErrorFormat("段位表不存在段位ID：{0}", a_nRankID);
            }
        }

        void _ClearAllStars()
        {
            objRankMaxRoot.SetActive(false);
            rectStarRoot.gameObject.SetActive(false);
        }

        void _SetupRankStars(SeasonLevelTable a_tableData, bool a_bOnlySlots)
        {
            if (a_tableData == null)
            {
                objRankMaxRoot.SetActive(false);
                rectStarRoot.gameObject.SetActive(false);
                return;
            }


            if (a_tableData.AfterId == 0)
            {
                objRankMaxRoot.SetActive(true);
                rectStarRoot.gameObject.SetActive(false);
                labRankMax.text = string.Format("x{0}", SeasonDataManager.GetInstance().seasonStar);
            }
            else
            {
                objRankMaxRoot.SetActive(false);
                rectStarRoot.gameObject.SetActive(true);

                for (int i = 0; i < rectStarRoot.childCount; ++i)
                {
                    rectStarRoot.GetChild(i).gameObject.SetActive(false);
                }
                //                 int nInterval = a_tableData.MaxStar - 1;
                //                 if (nInterval < 0)
                //                 {
                //                     nInterval = 0;
                //                 }
                //                 float fStartAngle = ms_fStarAngleInterval * nInterval * 0.5f + 90.0f;
                //                 float fRadius = rectStarRoot.sizeDelta.x * 0.5f;
                Assert.IsTrue(a_tableData.MaxStar <= rectStarRoot.childCount);
                for (int i = 0; i < a_tableData.MaxStar; ++i)
                {
//                     float fRadinas = (fStartAngle - i * ms_fStarAngleInterval) * Mathf.PI / 180.0f;
//                     Vector3 vecPos = new Vector3(Mathf.Cos(fRadinas) * fRadius, Mathf.Sin(fRadinas) * fRadius);

                    GameObject objStar;
//                     if (i < rectStarRoot.childCount)
//                     {
                        objStar = rectStarRoot.GetChild(i).gameObject;
//                     }
//                     else
//                     {
//                         objStar = GameObject.Instantiate(objStarTemplate);
//                         objStar.transform.SetParent(rectStarRoot, false);
//                     }

                    objStar.SetActive(true);
                    //objStar.GetComponent<RectTransform>().localPosition = vecPos;
                    if (a_bOnlySlots)
                    {
                        Utility.FindGameObject(objStar, "Light").SetActive(false);
                    }
                    else
                    {
                        Utility.FindGameObject(objStar, "Light").SetActive((i + 1) <= a_tableData.Star);
                    }

                    GameObject objGetStarEffect = Utility.FindGameObject(objStar, ms_strGetStarEffect, false);
                    if (objGetStarEffect != null)
                    {
                        objGetStarEffect.SetActive(false);
                    }
                    GameObject objLoseStarEffect = Utility.FindGameObject(objStar, ms_strLoseStarEffect, false);
                    if (objLoseStarEffect != null)
                    {
                        objLoseStarEffect.SetActive(false);
                    }
                }
            }
        }

        void _SetupRankExp(SeasonLevelTable a_tableData, int a_nExp)
        {
            if (a_tableData == null)
            {
                sliderExp.value = 0.0f;
                labExp.text = string.Empty;
                return;
            }

            float fValue = a_nExp / (float)a_tableData.MaxExp;
            if (fValue > 1.0f)
            {
                fValue = 1.0f;
            }
            if (fValue < 0.0f)
            {
                fValue = 0.0f;
            }
            sliderExp.value = fValue;
            labExp.text = string.Format("{0}/{1}", a_nExp, a_tableData.MaxExp);
        }

        void _SetupRankIcon(SeasonLevelTable a_tableData)
        {
            if (a_tableData == null)
            {
                imgRankIcon.gameObject.SetActive(false);
            }
            else
            {
                imgRankIcon.gameObject.SetActive(true);
                // imgRankIcon.sprite = AssetLoader.GetInstance().LoadRes(a_tableData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref imgRankIcon, a_tableData.Icon);
            }
        }

        void _SetupRankName(SeasonLevelTable a_tableData)
        {
            if (a_tableData == null)
            {
                labRank.text = string.Empty;
            }
            else
            {
                labRank.text = SeasonDataManager.GetInstance().GetRankName(a_tableData.ID);
            }
        }

        void _PlayParticles(GameObject a_objRoot)
        {
            a_objRoot.SetActive(true);
            GeUIEffectParticle[] pars = a_objRoot.GetComponentsInChildren<GeUIEffectParticle>();
            for (int i = 0; i < pars.Length; ++i)
            {
                pars[i].StartEmit();
            }
        }

        void _StopEffect()
        {
            for (int i = 0; i < objEffects.Length; ++i)
            {
                if (objEffects[i]!=null)
                {
                    objEffects[i].SetActive(false);
                }
            }
        }

        float _PlayAnims(List<DOTweenAnimation> a_arrAnims)
        {
            float fTime = 0.0f;
            for (int i = 0; i < a_arrAnims.Count; ++i)
            {
                a_arrAnims[i].isActive = true;
                if(a_arrAnims[i].tween == null )
                {
                    a_arrAnims[i].CreateTween();
                }
                a_arrAnims[i].tween.Restart();

                float fTempTime = a_arrAnims[i].delay + a_arrAnims[i].duration;
                if (fTime < fTempTime)
                {
                    fTime = fTempTime;
                }
            }

            return fTime;
        }


        void _RewindAnims(List<DOTweenAnimation> a_arrAnims)
        {
            for (int i = 0; i < a_arrAnims.Count; ++i)
            {
                a_arrAnims[i].DORewind();
                a_arrAnims[i].isActive = false;
            }
        }
    }
}