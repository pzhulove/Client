using UnityEngine;
using GameClient;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ComNewbieGuideBattleDrugDrag : ComNewbieGuideBase
{
    List<Component> m_pointerClickHandleList = null;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.绑定界面的cs文件的名称
        // 1.绑定控件路径
        // 2.引导文字
        // 3.显示位置
        // 4.文字模板类型
        // 5.坐标微调
        // 6.保存点
        // 7.暂停单局
        // 8.需要高亮显示的节点及其子节点

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 2)
            {
                mFrameType = args[0] as string;
                mComRoot = args[1] as string;
            }

            if (args.Length >= 3)
            {
                mTextTips = args[2] as string;
            }

            if (args.Length >= 4)
            {
                mAnchor = (eNewbieGuideAnchor)args[3];
            }

            if (args.Length >= 5)
            {
                mTextTipType = (TextTipType)args[4];
            }

            if (args.Length >= 6)
            {
                mLocalPos = (Vector3)args[5];
            }

            if (args.Length >= 7)
            {
                if ((eNewbieGuideAgrsName)args[6] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 8)
            {
                if ((eNewbieGuideAgrsName)args[7] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }

            if (args.Length >= 9)
            {
                if (args[8] as string != "")
                {
                    mHighLightPointPath = args[8] as string;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        IGameBind ibind = null;

        if (mFrameType == "ClientSystemBattle")
        {
            ibind = ClientSystemManager.instance.CurrentSystem as IGameBind;
        }
        else
        {
            var frameManager = ClientSystemManager.instance as IClientFrameManager;

            if (frameManager.IsFrameOpen(mFrameType))
            {
                ibind = frameManager.GetFrame(mFrameType) as IGameBind;
            }
        }

        if (ibind == null)
        {
            Logger.LogWarningFormat("新手引导---点击[Button/Toggle]类型控件，找不到{0}界面,处于wait状态", mFrameType);
            return GuideState.Wait;
        }

        if (mHighLightPointPath != "")
        {
            RectTransform rect = ibind.GetComponent<RectTransform>(mHighLightPointPath);
            if (rect == null)
            {
                Logger.LogWarningFormat("新手引导---高亮显示,找不到路径{0},处于Exception状态", mHighLightPointPath);
                return GuideState.Exception;
            }

            AddCanvasCom(rect.gameObject, true);
        }

        var rectTrans = ibind.GetComponent<RectTransform>(mComRoot);
        if (rectTrans == null)
        {
            Logger.LogErrorFormat(" path {0} is not a UI GameObject", mComRoot);
            return GuideState.Exception;
        }

        var coms = GamePool.ListPool<Component>.Get();

        if (coms == null)
        {
            Logger.LogError("ListPool get coms is null");
            return GuideState.Exception;
        }

        rectTrans.gameObject.GetComponents(coms);
        for (int i = 0; i < coms.Count; ++i)
        {
            var current = coms[i];
            if (current is IPointerClickHandler)
            {
                if (m_pointerClickHandleList == null)
                {
                    m_pointerClickHandleList = GamePool.ListPool<Component>.Get();
                }

                m_pointerClickHandleList.Add(current);
            }
        }

        if (m_pointerClickHandleList == null)
        {
            Logger.LogErrorFormat(" path {0}  Do not have a IPointerClickHandler Component", mComRoot);
            return GuideState.Exception;
        }

        GamePool.ListPool<Component>.Release(coms);
        var gClickObj = AddBattleDrugDrag(rectTrans.gameObject, _OnClickDrugDrag);

        if (mTextTips.Length > 0)
        {
            AddTextTips(gClickObj, mAnchor, mTextTips, mTextTipType, mLocalPos);
        }

        return GuideState.Normal;
    }

    private void _OnClickDrugDrag()
    {
        var pointerData = new PointerEventData(EventSystem.current);

        for (int i = 0; i < m_pointerClickHandleList.Count; ++i)
        {
            var cur = m_pointerClickHandleList[i] as IPointerClickHandler;

            if (cur != null)
            {
                var pointerDown = cur as IPointerDownHandler;
                if (pointerDown != null)
                    pointerDown.OnPointerDown(pointerData);
                cur.OnPointerClick(pointerData);
            }
        }

        GamePool.ListPool<Component>.Release(m_pointerClickHandleList);
    }

    protected override void _update()
    {

    }
    #endregion
}
