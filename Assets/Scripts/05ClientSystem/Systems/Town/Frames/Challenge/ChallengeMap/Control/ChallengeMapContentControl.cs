using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using DungeonModelTable = ProtoTable.DungeonModelTable;

namespace GameClient
{
    public class ChallengeMapContentControl : MonoBehaviour
    {
        private DungeonModelTable.eType _modelType;
        private ChallengeMapParamDataModel _paramDataModel;
        private int _baseDungeonId;
        private int _detailDungeonId;

        private ChallengeMapViewControl _deepView;
        private ChallengeMapViewControl _ancientView;
        private ChallengeMapViewControl _weekHellView;
        private ChallengeMapViewControl _viodCrackView;
        private ChallengeTeamDuplicationView _teamDuplicationView;
        private ChallengeMapViewControl yscaView;

        private ChallengeMapViewControl zhengzhanantuenView;

        private OnContentEffectAction _onContentEffectAction;

       
        [Space(15)]
        [HeaderAttribute("AuctionNewContent")]
        [SerializeField] private GameObject deepRoot;
        [SerializeField] private GameObject ancientRoot;
        [SerializeField] private GameObject weekHellRoot;
        [SerializeField] private GameObject viodCrackRoot;
        [SerializeField] private GameObject teamDuplicationRoot;
        [SerializeField] private GameObject yscaRoot;

        [SerializeField] private GameObject zhanzhengantuenRoot;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
        }

        private void UnBindEvents()
        {
        }

        private void ClearData()
        {
            _baseDungeonId = 0;
            _detailDungeonId = 0;
            _modelType = DungeonModelTable.eType.Type_None;
            _paramDataModel = null;
        }

        public void InitMapContentData(OnContentEffectAction onContentEffectAction)
        {
            _onContentEffectAction = onContentEffectAction;
        }

        //需要传递默认的参数
        public void InitMapContentControl(DungeonModelTable.eType modelType,
            ChallengeMapParamDataModel mapParamDataModel = null,
            OnContentEffectAction onContentEffectAction = null)
        {

            _modelType = modelType;
            _paramDataModel = mapParamDataModel;

            InitMapContent();
        }
        
        private void InitMapContent()
        {
            ResetContentRoot();

            switch (_modelType)
            {
                case DungeonModelTable.eType.DeepModel:
                    OnDeepClicked();
                    break;
                case DungeonModelTable.eType.AncientModel:
                    OnAncientClicked();
                    break;
                case DungeonModelTable.eType.WeekHellModel:
                    OnWeekHellClicked();
                    break;
                case DungeonModelTable.eType.VoidCrackModel:
                    OnVoidCrackClicked();
                    break;
                case DungeonModelTable.eType.TeamDuplicationModel:
                    OnTeamDuplicationClicked();
                    break;
                case DungeonModelTable.eType.YunShangChangAnModel:
                    OnYSCAClicked();
                    break;
                case DungeonModelTable.eType.ZhengzhanAntuenModel:
                    OnZhengzhanAntuenClicked();
                    break;
                default:
                    break;
            }
        }

        private void OnZhengzhanAntuenClicked()
        {
            if (zhanzhengantuenRoot != null && zhanzhengantuenRoot.activeSelf == false)
                zhanzhengantuenRoot.CustomActive(true);

            if (zhengzhanantuenView == null)
            {
                //第一次加载
                zhengzhanantuenView = LoadContentBaseView(zhanzhengantuenRoot);

                if(zhengzhanantuenView != null)
                    zhengzhanantuenView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
            }
            else
            {
                if(zhengzhanantuenView != null)
                    zhengzhanantuenView.OnEnableView();
            }
        }

        private void OnYSCAClicked()
        {
            if (yscaRoot != null && yscaRoot.activeSelf == false)
                yscaRoot.CustomActive(true);

            if (yscaView == null)
            {
                //第一次加载
                yscaView = LoadContentBaseView(yscaRoot);

                if(yscaView != null)
                    yscaView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
            }
            else
            {
                if(yscaView != null)
                    yscaView.OnEnableView();
            }
        }

        private ChallengeMapViewControl LoadContentBaseView(GameObject contentRoot)
        {
            if (contentRoot == null)
                return null;

            ChallengeMapViewControl viewControl = null;

            var uiPrefabWrapper = contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(contentRoot.transform, false);

                    viewControl = uiPrefab.GetComponent<ChallengeMapViewControl>();
                }
            }

            return viewControl;
        }

        private void OnDeepClicked()
        {
            if (deepRoot != null && deepRoot.activeSelf == false)
                deepRoot.CustomActive(true);

            if (_deepView == null)
            {
                //第一次加载
                _deepView = LoadContentBaseView(deepRoot);
                if (_deepView != null)
                {
                    _deepView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
                }
            }
            else
            {
                if(_deepView != null)
                    _deepView.OnEnableView();
            }
        }

        private void OnAncientClicked()
        {
            if (ancientRoot != null && ancientRoot.activeSelf == false)
                ancientRoot.CustomActive(true);

            if (_ancientView == null)
            {
                //第一次加载
                _ancientView = LoadContentBaseView(ancientRoot);
                if (_ancientView != null)
                {
                    _ancientView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
                }
            }
            else
            {
                if (_ancientView != null)
                    _ancientView.OnEnableView();
            }
        }

        private void OnWeekHellClicked()
        {
            if (weekHellRoot != null && weekHellRoot.activeSelf == false)
                weekHellRoot.CustomActive(true);

            if (_weekHellView == null)
            {
                //第一次加载
                _weekHellView = LoadContentBaseView(weekHellRoot);
                if (_weekHellView != null)
                {
                    _weekHellView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
                }
            }
            else
            {
                if (_weekHellView != null)
                    _weekHellView.OnEnableView();
            }
        }

        private void OnVoidCrackClicked()
        {
            if (viodCrackRoot != null && viodCrackRoot.activeSelf == false)
            {
                viodCrackRoot.CustomActive(true);
            }

            if (_viodCrackView == null)
            {
                _viodCrackView = LoadContentBaseView(viodCrackRoot);
                if (_viodCrackView != null)
                {
                    _viodCrackView.InitMapModelControl(_modelType, _paramDataModel, _onContentEffectAction);
                }
            }
            else
            {
                if (_viodCrackView != null)
                    _viodCrackView.OnEnableView();
            }
        }

        private void OnTeamDuplicationClicked()
        {
            if (teamDuplicationRoot != null && teamDuplicationRoot.activeSelf == false)
                teamDuplicationRoot.CustomActive(true);

            if (_teamDuplicationView == null)
            {
                var challengeTeamDuplicationViewPrefab = CommonUtility.LoadGameObject(teamDuplicationRoot);
                if (challengeTeamDuplicationViewPrefab != null)
                {
                    _teamDuplicationView =
                        challengeTeamDuplicationViewPrefab.GetComponent<ChallengeTeamDuplicationView>();

                    if(_teamDuplicationView != null)
                        _teamDuplicationView.InitView();
                }
            }
            else
            {
                _teamDuplicationView.OnEnableView();
            }
        }

        private void ResetContentRoot()
        {
            if (deepRoot != null)
                deepRoot.CustomActive(false);

            if (ancientRoot != null)
                ancientRoot.CustomActive(false);

            if (weekHellRoot != null)
                weekHellRoot.CustomActive(false);

            if (viodCrackRoot != null)
                viodCrackRoot.CustomActive(false);

            if (teamDuplicationRoot != null)
                teamDuplicationRoot.CustomActive(false);
            
            if (yscaRoot != null)
                yscaRoot.CustomActive(false);

            if (zhanzhengantuenRoot != null)
                zhanzhengantuenRoot.CustomActive(false);
        }
        
    }
}