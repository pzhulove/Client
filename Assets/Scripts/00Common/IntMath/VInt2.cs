using System;
using System.Runtime.InteropServices;
using behaviac;
using UnityEngine;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct VInt2
{
    public int x;
    public int y;
    public static VInt2 zero;
    private static readonly int[] Rotations;
    public VInt2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

 	public float fx
	{
		get {
			return x * IntMath.fInvIntDen;
		}
	}

	public float fy
	{
		get {
			return y * IntMath.fInvIntDen;
		}
	}
 
    public VInt2(float x,float y)
    {
		this.x = IntMath.Float2IntWithFixed(x);//(int) Math.Round((double) (x * IntMath.fIntDen));
		this.y = IntMath.Float2IntWithFixed(y);//(int) Math.Round((double) (y * IntMath.fIntDen));
    }

    public VInt2(Vector2 position)
    {
		/*
        this.x = (int) Math.Round((double) (position.x * 1000f));
        this.y = (int) Math.Round((double) (position.y * 1000f));
        this.z = (int) Math.Round((double) (position.z * 1000f));
        */

		this.x = IntMath.Float2IntWithFixed(position.x); //(int) Math.Round((double) (position.x * IntMath.fIntDen));
		this.y = IntMath.Float2IntWithFixed(position.y); //(int) Math.Round((double) (position.y * IntMath.fIntDen));
    }


    static VInt2()
    {
        zero = new VInt2();
        Rotations = new int[] { 1, 0, 0, 1, 0, 1, -1, 0, -1, 0, 0, -1, 0, -1, 1, 0 };
    }

    public int sqrMagnitude
    {
        get
        {
            return ((this.x * this.x) + (this.y * this.y));
        }
    }
    public long sqrMagnitudeLong
    {
        get
        {
            long x = this.x;
            long y = this.y;
            return ((x * x) + (y * y));
        }
    }
    public int magnitude
    {
        get
        {
            long x = this.x;
            long y = this.y;
            return IntMath.Sqrt((x * x) + (y * y));
        }
    }
    public static int Dot(ref VInt2 a, ref VInt2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static VFactor AngleInt(VInt2 lhs, VInt2 rhs)
    {
        long den = lhs.magnitude * rhs.magnitude;
        return IntMath.acos((long) DotLong(ref lhs, ref rhs), den);
    }

    public static long DotLong(ref VInt2 a, ref VInt2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static long DotLong(VInt2 a, VInt2 b)
    {
        return ((a.x * b.x) + (a.y * b.y));
    }

    public static long DetLong(ref VInt2 a, ref VInt2 b)
    {
        return ((a.x * b.y) - (a.y * b.x));
    }

    public static long DetLong(VInt2 a, VInt2 b)
    {
        return ((a.x * b.y) - (a.y * b.x));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VInt2 num = (VInt2) o;
        return ((this.x == num.x) && (this.y == num.y));
    }

    public override int GetHashCode()
    {
        return ((this.x * 0xc005) + (this.y * 0x1800d));
    }

    public static VInt2 Rotate(VInt2 v, int r)
    {
        r = r % 4;
        return new VInt2((v.x * Rotations[r * 4]) + (v.y * Rotations[(r * 4) + 1]), (v.x * Rotations[(r * 4) + 2]) + (v.y * Rotations[(r * 4) + 3]));
    }

    public static VInt2 Min(VInt2 a, VInt2 b)
    {
        return new VInt2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
    }

    public static VInt2 Max(VInt2 a, VInt2 b)
    {
        return new VInt2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    }

    public static VInt2 FromInt3XZ(VInt3 o)
    {
        return new VInt2(o.x, o.z);
    }

    public static VInt3 ToInt3XZ(VInt2 o)
    {
        return new VInt3(o.x, 0, o.y);
    }

    public override string ToString()
    {
        object[] objArray1 = new object[] { "(", this.x, ", ", this.y, ")" };
        return string.Concat(objArray1);
    }

    public void Min(ref VInt2 r)
    {
        this.x = Mathf.Min(this.x, r.x);
        this.y = Mathf.Min(this.y, r.y);
    }

    public void Max(ref VInt2 r)
    {
        this.x = Mathf.Max(this.x, r.x);
        this.y = Mathf.Max(this.y, r.y);
    }

    public void Normalize()
    {
        long num = this.x * 100;
        long num2 = this.y * 100;
        long a = (num * num) + (num2 * num2);
        if (a != 0)
        {
            long b = IntMath.Sqrt(a);
            this.x = (int) IntMath.Divide((long) (num * 0x3e8L), b);
            this.y = (int) IntMath.Divide((long) (num2 * 0x3e8L), b);
        }
    }

    public VInt2 NormalizeTo(int newMagn)
    {
        long num = this.x * 100;
        long num2 = this.y * 100;
        long a = ((num * num) + (num2 * num2));
        if (a != 0)
        {
            long b = IntMath.Sqrt(a);
            long num6 = newMagn;
            this.x = (int) IntMath.Divide((long) (num * num6), b);
            this.y = (int) IntMath.Divide((long) (num2 * num6), b);
        }
        return this;
    }


    public VInt2 normalized
    {
        get
        {
            VInt2 num = new VInt2(this.x, this.y);
            num.Normalize();
            return num;
        }
    }
    public static VInt2 ClampMagnitude(VInt2 v, int maxLength)
    {
        long sqrMagnitudeLong = v.sqrMagnitudeLong;
        long num2 = maxLength;
        if (sqrMagnitudeLong > (num2 * num2))
        {
            long b = IntMath.Sqrt(sqrMagnitudeLong);
            int x = (int) IntMath.Divide((long) (v.x * maxLength), b);
            return new VInt2(x, (int) IntMath.Divide((long) (v.x * maxLength), b));
        }
        return v;
    }

    public static explicit operator Vector2(VInt2 ob)
    {
        return new Vector2(
                ob.x * IntMath.fInvIntDen,
                ob.y * IntMath.fInvIntDen);
    }

    public static explicit operator VInt2(Vector2 ob)
    {
        return new VInt2(
                IntMath.Float2IntWithFixed(ob.x),
                IntMath.Float2IntWithFixed(ob.y));
    }

    public static VInt2 operator +(VInt2 a, VInt2 b)
    {
        return new VInt2(a.x + b.x, a.y + b.y);
    }

    public static VInt2 operator -(VInt2 a, VInt2 b)
    {
        return new VInt2(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(VInt2 a, VInt2 b)
    {
        return ((a.x == b.x) && (a.y == b.y));
    }

    public static bool operator !=(VInt2 a, VInt2 b)
    {
        return ((a.x != b.x) || (a.y != b.y));
    }

    public static VInt2 operator -(VInt2 lhs)
    {
        lhs.x = -lhs.x;
        lhs.y = -lhs.y;
        return lhs;
    }

    public static VInt2 operator *(VInt2 lhs, int rhs)
    {
        lhs.x *= rhs;
        lhs.y *= rhs;
        return lhs;
    }


    //new Vector3(pos.x,pos.z,pos.y);
    public Vector3 vector2
    {
        get
        {
            return new Vector2(this.x * IntMath.fInvIntDen, this.y * IntMath.fInvIntDen);
        }
    }
    
    public Point Point
    {
        get
        {
            var ret =  new Point();
            ret.x = x / 10;
            ret.y = y / 10;
            return ret;
        }
    }
}

