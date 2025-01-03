﻿namespace Assets.Scripts.Common
{
    using System;

    public class PooledClassObject
    {
        public bool bChkReset = true;
        public IObjPoolCtrl holder;
        public uint usingSeq;

        public virtual void OnRelease()
        {
        }

        public virtual void OnUse()
        {
        }

        public void Release()
        {
            this.OnRelease();
            this.holder.Release(this);
        }
    }
}

