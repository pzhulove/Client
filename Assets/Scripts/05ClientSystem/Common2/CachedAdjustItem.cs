using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class CachedAdjustItem : CachedNormalObject<AdjustChangedAttr>
    {
        Text attrName;
        Slider slider;
        Text progress;
        Text deltaValue;
        GameObject goArrowUp;
        GameObject goArrowDown;

        ComEffect comEffect;

        Text extraAttr;
        UIGray graExtra;
        GameObject goExtraFront;

        public override void Initialize()
        {
            attrName = Utility.FindComponent<Text>(goLocal, "Name");
            slider = Utility.FindComponent<Slider>(goLocal, "BaseAttrProgress/Front");
            progress = Utility.FindComponent<Text>(goLocal, "BaseAttrProgress/Progress");
            deltaValue = Utility.FindComponent<Text>(goLocal, "UpAttr");
            goArrowUp = Utility.FindChild(goLocal,"Arrows/Arrows_up");
            goArrowDown = Utility.FindChild(goLocal,"Arrows/Arrows_Down");
            extraAttr = Utility.FindComponent<Text>(goLocal, "ExtraAttrProgress/Progress");
            graExtra = Utility.FindComponent<UIGray>(goLocal, "ExtraAttrProgress");
            goExtraFront = Utility.FindChild(goLocal, "ExtraAttrProgress/Front");
            comEffect = goLocal.GetComponent<ComEffect>();
        }

        public override void UnInitialize()
        {

        }

        void _PlayEffect()
        {
            StopEffect();
            if(Value.bEffect)
            {
                if (Value.IsUp && Value.IsFull)
                {
                    comEffect.Play("Perfect");
                }
                else
                {
                    comEffect.Play("Normal");
                }
            }
        }

        public void StopEffect()
        {
            comEffect.Stop("Normal");
            comEffect.Stop("Perfect");
        }

        public override void OnRecycle()
        {
            StopEffect();
            Disable();
        }

        public override void OnUpdate()
        {
            PropAttribute attr = Utility.GetEnumAttribute<EEquipProp, PropAttribute>(Value.eEEquipProp);
            if(attr != null)
            {
                attrName.text = attr.desc;
            }

            float fValue = Value.fAmount;
            slider.value = fValue;

            progress.text = Value.Process;

            if (!Value.IsChanged)
            {
                deltaValue.text = "0.0";
                deltaValue.enabled = false;
                deltaValue.color = Color.white;

                goArrowDown.CustomActive(false);
                goArrowUp.CustomActive(false);
            }
            else
            {
                deltaValue.text = string.Format("{0:F1}",Value.DeltaValue);
                deltaValue.enabled = true;

                if (Value.IsUp)
                {
                    deltaValue.color = Color.green;
                    goArrowDown.CustomActive(false);
                    goArrowUp.CustomActive(true);
                }
                else
                {
                    deltaValue.color = Color.red;
                    goArrowDown.CustomActive(true);
                    goArrowUp.CustomActive(false);
                }
            }

            extraAttr.text = Value.ExtraAttr;
            graExtra.enabled = false && !Value.IsFull;
            goExtraFront.CustomActive(Value.IsFull);

            _PlayEffect();
        }
    }
}