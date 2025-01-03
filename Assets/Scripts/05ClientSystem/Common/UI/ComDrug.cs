using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Diagnostics;
using GameClient;
using ProtoTable;

public class ComDrug : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IComparable<ComDrug>, IPointerClickHandler
{

#region IComparable implementation
    public int CompareTo(ComDrug other)
    {
        if (null == mTable)
        {
            return 1;
        }

        if (null == other.mTable)
        {
            return 0;
        }

        return mTable.DefualtUsePriority - other.mTable.DefualtUsePriority;
    }
#endregion

    public Button mButton;
    public Text mCount;
    public Image mBar;
	public Text mCountDown;
    public Image mIcon;
    public Battle.DungeonItem.eType mType;
    public int mItemId;
    public bool locked;

    public float durationThreshold = 0.5f;
    public UnityEvent onLongPress = new UnityEvent();
    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    private void Update()
    {
        if (isPointerDown && !longPressTriggered)
        {
            if (Time.time - timePressStarted > durationThreshold)
            {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }
    }
    public ProtoTable.ItemTable.eSubType itemSubType
    {
        get; private set;
    }

    private enum eState
    {
        onReady,
        onCooldown,
    }

    private eState mState = eState.onReady;

    private float mCD = 0.0f;
    private float mCurrent = 0f;
    public int mLeftCount = 0;
    private ItemConfigTable mTable = null;

    public void SetCount(int cnt)
    {
        mLeftCount = cnt;
        if (mCount == null) return;
        mCount.text = cnt.ToString();
    }

    public void SetItemID(int id)
    {
        mItemId = id;
        ItemData data = ItemDataManager.GetInstance().GetItemByTableID(mItemId);
        if (null == data)
        {
            itemSubType = ProtoTable.ItemTable.eSubType.ST_NONE;
        }
        else
        {
            itemSubType = (ProtoTable.ItemTable.eSubType)data.SubType;
        }

        Logger.LogProcessFormat("[PotionSet] 设置药 {0}, {1}", id, itemSubType);

        if (null != mIcon)
        {
            if (null == data)
            {
                mIcon.color = Color.clear;
            }
            else
            {
                mIcon.color = Color.white;
                // mIcon.sprite = AssetLoader.instance.LoadRes(data.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, data.Icon);
            }
        }
        mTable = ChapterBattlePotionSetUtiilty.GetItemConfigTalbeByID(mItemId);
    }

    public bool StartCD(float cd)
    {
        if (mState == eState.onReady)
        {
            mCD = cd;
            mCurrent = mCD;
            mState = eState.onCooldown;
            if (mButton != null)
                mButton.interactable = false;
            if (mBar != null)
                mBar.enabled = true;

            return true;
        }

        return false;
    }

    public bool IsCD()
	{
		return mState == eState.onCooldown;
	}

	public void PlayEffect()
	{
		AudioManager.instance.PlaySound(7);

        if (null == mTable)
        {
            Logger.LogProcessFormat("[PotionSet] 表格数据为空");
            return ;
        }

        for (int i = 0; i < mTable.UseItemEffect.Count; ++i)
        { 
            _playerEffect(mTable.UseItemEffect[i]);
        }
	}

    private void _playerEffect(string path)
    {
		BattlePlayer battlePlayer = BattleMain.instance.GetLocalPlayer();
        if (null == battlePlayer)
        {
            Logger.LogProcessFormat("[PotionSet] 玩家数据为空");
            return ;
        }

        BeActor actor = battlePlayer.playerActor;
        if (null == actor)
        {
            Logger.LogProcessFormat("[PotionSet] 玩家对象为空");
            return ;
        }

        GeEffectEx effect = actor.CurrentBeScene.currentGeScene.CreateEffect(path, 0f, actor.GetGePosition(PositionType.BODY));

        if (null == effect)
        {
            Logger.LogProcessFormat("[PotionSet] 无法创建特效 {0}", path);
            return ;
        }

        Logger.LogProcessFormat("[PotionSet] 创建成果 {0}", path);
    }

    public void RealUpdate()
    {
        if (mState == eState.onCooldown)
        {
            mCurrent -= Time.deltaTime;

			mBar.fillAmount = (1-Mathf.Clamp01(1 - mCurrent / mCD));

			if (mCountDown != null)
			{
				mCountDown.CustomActive(true);
				float rate = mCurrent *1000 / 1000f;
				if (rate > 1.0f)
				{
					mCountDown.text = string.Format("{0:0}", rate);
				}
				else 
				{
					mCountDown.text = string.Format("{0:F1}", rate);
				}
			}

            if (mCurrent <= 0)
            {
				mCountDown.CustomActive(false);
                mState = eState.onReady;
                mButton.interactable = true;
                mBar.enabled = false;
            }
        }
    }


    public UnityEvent onClick     = new UnityEvent();
    public float deltaSplit       = 20.0f;
    public UnityEvent onDragRight = new UnityEvent();

    public enum eModeState
    {
        onNone,
        onClickDown,
        onDrag,
    }

    public enum eMode
    {
        Drag,
        Click,
    }

    private eModeState mModeState = eModeState.onNone;
    private eMode mMode = eMode.Click;

    public void SetMode(eMode mode)
    {
        mMode = mode;
    }
        
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObject.name=="Item0")
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }
        //Logger.LogError("onPointDown");
        mModeState = eModeState.onClickDown;
        if (mMode == eMode.Drag)
        {
            OnPointerUp(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (gameObject.name == "Item0")
        {
            isPointerDown = false;
        }
        if (mModeState != eModeState.onNone)
        {
            if (mModeState == eModeState.onClickDown)
            {
                //Logger.LogError("onClickDown");
                //onClick.Invoke();
            }
            else if (mModeState == eModeState.onDrag)
            {
                //Logger.LogError("onDrag");
                onDragRight.Invoke();
            }

            mModeState = eModeState.onNone;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gameObject.name == "Item0")
        {
            Vector2 divDir = Vector2.left;
            Vector2 move = eventData.position - eventData.pressPosition;

            float v = Vector2.Dot(divDir, move);
            if (v > 0 && move.magnitude > deltaSplit)
            {
                ComDrugTipsBar.instance.SetDrugcolumnState();
                OnPointerUp(eventData);
            }
        }
            if (mModeState == eModeState.onClickDown)
        {
            Vector2 divDir = Vector2.right;
            Vector2 move = eventData.position - eventData.pressPosition;

            float v = Vector2.Dot(divDir, move);
            if (v > 0 && move.magnitude > deltaSplit)
            {
                mModeState = eModeState.onDrag;
                OnPointerUp(eventData);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "Item0")
        {
            if (!longPressTriggered)
            {
                onClick.Invoke();
            }
        }
           
    }

    void OnDestroy()
    {
        mButton = null;
        mCount = null;
        mBar = null;
        mCountDown = null;
        mIcon = null;
    }
}
