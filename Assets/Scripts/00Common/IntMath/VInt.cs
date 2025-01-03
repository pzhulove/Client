using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct VInt
{
    public int i;
    public static readonly VInt zero;
	public static readonly VInt one;
	public static readonly VInt half;
    public static readonly VInt onehalf;
    public static readonly VInt quarter;
    public static readonly VInt zeroDotOne;

    static VInt()
	{
		zero = new VInt(0);
		one = new VInt(1.0f);
		half = new VInt(0.5f);
        onehalf = new VInt(1.5f);
        zeroDotOne = new VInt(0.1f);
        quarter = new VInt(0.25f);
    }

    public VInt(int i)
    {
        this.i = i;
    }

    public VInt(float f)
    {
		this.i = IntMath.Float2IntWithFixed(f);//(int) Math.Round((double) (f * IntMath.fIntDen));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VInt num = (VInt) o;
        return (this.i == num.i);
    }

    public override int GetHashCode()
    {
        return this.i.GetHashCode();
    }

    public static VInt Min(VInt a, VInt b)
    {
        return new VInt(Math.Min(a.i, b.i));
    }

    public static VInt Max(VInt a, VInt b)
    {
        return new VInt(Math.Max(a.i, b.i));
    }


    public override string ToString()
    {
        return this.i.ToString();
    }

    public float scalar
    {
        get
        {
            return (this.i * IntMath.fInvIntDen);
        }
    }

    public VFactor factor
    {
        get{
            return new VFactor(i,IntMath.kIntDen);
        }
    }

    public static int Float2VIntValue(float f)
    {
        return IntMath.Float2IntWithFixed(f);
        //return (int) Math.Round((double) (f * IntMath.fIntDen));
    }

    public static VInt NewVInt(int n,int d)
    {
        return IntMath.Float2IntWithFixed((double)(n / (double)d));
        //return (int) Math.Round((double) (n / (float)d * IntMath.kIntDen));
    }
 
    public static VInt NewVInt(long n,long d)
    {
        VInt value = new VInt();
        value.i = (int)(n * IntMath.kIntDen / d);
        return value;
    }


    public static explicit operator VInt(float f)
    {
        return new VInt(IntMath.Float2IntWithFixed(f));
        //return new VInt((int) Math.Round((double) (f * IntMath.fIntDen)));
    }

    public static implicit operator VInt(int i)
    {
        return new VInt(i);
    }

    public static explicit operator float(VInt ob)
    {
        return (ob.i * IntMath.fInvIntDen);
    }

    public static explicit operator long(VInt ob)
    {
        return (long) ob.i;
    }

    public static VInt operator +(VInt a, VInt b)
    {
        return new VInt(a.i + b.i);
    }

    public static VInt operator -(VInt a, VInt b)
    {
        return new VInt(a.i - b.i);
    }

    public static bool operator ==(VInt a, VInt b)
    {
        return (a.i == b.i);
    }

    public static bool operator !=(VInt a, VInt b)
    {
        return (a.i != b.i);
    }

    public static bool operator >=(VInt a, VInt b)
    {
        return (a.i >= b.i);
    }

    public static bool operator <=(VInt a, VInt b)
    {
        return (a.i <= b.i);
    }

    public static bool operator >(VInt a, VInt b)
    {
        return (a.i > b.i);
    }

    public static bool operator <(VInt a, VInt b)
    {
        return (a.i < b.i);
    }

     public static bool operator ==(VInt a, int b)
    {
        return (a.i == b);
    }

    public static bool operator !=(VInt a, int b)
    {
        return (a.i != b);
    }

    public static bool operator >=(VInt a, int b)
    {
        return (a.i >= b);
    }

    public static bool operator <=(VInt a, int b)
    {
        return (a.i <= b);
    }

    public static bool operator >(VInt a, int b)
    {
        return (a.i > b);
    }

    public static bool operator <(VInt a, int b)
    {
        return (a.i < b);
    }

    public static VInt operator -(VInt a)
    {
        a.i = -a.i;
        return a;
    }

    public static VInt Clamp(VInt a, VInt min, VInt max)
    {
        if (a < min)
        {
            return min;
        }
        if (a > max)
        {
            return max;
        }
        return a;
    }
}

