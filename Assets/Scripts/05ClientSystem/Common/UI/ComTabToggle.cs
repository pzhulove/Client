using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GameClient
{
    public interface IRedPointToggle
    {
        void SetRedPointActive(bool value);
    }

    public interface IOutLineToggle
    {
        void SetSelectOutLineColor(Color color);
    }

    public interface IComTabToggle : IDisposable
    {
        void Init();
        void BindEvent();
    }

    public interface ISetNameToggle
    {
        void SetName(string normal, string selected);
    }


    [RequireComponent(typeof(Toggle))]
    public class ComTabToggle : MonoBehaviour, IComTabToggle, IRedPointToggle, IOutLineToggle, ISetNameToggle
    {

        [SerializeField] private GameObject mRedPoint;
        [SerializeField] private NicerOutline mTextOutline;
        [SerializeField] private Text mTextNormal;
        [SerializeField] private Text mTextSelected;
        private Color mOutlineNormalColor;
        private Color mOutlineSelectColor;
        private Toggle mToggle;

        public void SetName(string normal, string selected)
        {
            mTextNormal.SafeSetText(normal);
            if (string.IsNullOrEmpty(selected))
            {
                selected = normal;
            }
            mTextSelected.SafeSetText(selected);
        }

        public void SetRedPointActive(bool value)
        {
            this.mRedPoint.CustomActive(value);
        }

        public void SetSelectOutLineColor(Color color)
        {
            this.mOutlineSelectColor = color;
            if (this.mToggle.isOn)
            {
                this.mTextOutline.effectColor = color;
            }
        }

        public void Dispose()
        {
            this.mToggle.SafeRemoveOnValueChangedListener(OnValueChanged);
        }

        public void Init()
        {
            BindEvent();
            if (this.mTextOutline != null)
            {
                this.mOutlineNormalColor = this.mTextOutline.effectColor;
                this.mOutlineSelectColor = this.mOutlineNormalColor;
            }
        }

        public void BindEvent()
        {
            this.mToggle.SafeRemoveOnValueChangedListener(OnValueChanged);
            this.mToggle.SafeAddOnValueChangedListener(OnValueChanged);
        }

        void OnValueChanged(bool value)
        {
            if (this.mTextOutline != null)
            {
                this.mTextOutline.effectColor = value ? this.mOutlineSelectColor : this.mOutlineNormalColor;
            }

            if (mTextSelected != null)
            {
                mTextNormal.CustomActive(!value);
                mTextSelected.CustomActive(value);
            }
        }

        void Awake()
        {
            this.mToggle = GetComponent<Toggle>();
        }
    }
}