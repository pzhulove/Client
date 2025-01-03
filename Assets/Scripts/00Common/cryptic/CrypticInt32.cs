﻿//namespace Assets.Scripts.GameLogic
//{
    //using Assets.Scripts.Framework;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CrypticInt32
    {
        private int _Decrypt;
        private int _Cryptic;
        public CrypticInt32(int nValue)
        {
            this._Decrypt = FrameRandom2.GetSeed();
			
			FrameRandom2.ChangeSeed();

            this._Cryptic = nValue ^ this._Decrypt;
        }

        //暂时去掉，强制类型转换不通过
        //         public unsafe int ToInt()
        //         {
        //             return *(((int*)this));
        //         }

        public int ToInt()
        {
            return (int)this;
        }

        public uint ToUint()
        {
            return (uint) this;
        }

        public override string ToString()
        {
            return this.ToInt().ToString();
        }

        public string ToUintString()
        {
            return this.ToUint().ToString();
        }

        public static implicit operator CrypticInt32(int nValue)
        {
            return new CrypticInt32(nValue);
        }

        public static implicit operator int(CrypticInt32 inData)
        {
            return (inData._Cryptic ^ inData._Decrypt);
        }

        public static explicit operator uint(CrypticInt32 inData)
        {
            return (uint) (inData._Cryptic ^ inData._Decrypt);
        }

        public static explicit operator ushort(CrypticInt32 inData)
        {
            return (ushort) (inData._Cryptic ^ inData._Decrypt);
        }
		
		public static explicit operator float(CrypticInt32 inData)
		{
			return (float) (inData._Cryptic ^ inData._Decrypt);
		}
    }
//}

