using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
//!!几率，使用1000做基数
public struct VRate
{
    public int i;
    public static readonly VRate zero;
	public static readonly VRate one;
	public static readonly VRate half;

    private const long kValidBit = 1000;
    private const long kFloatBit = 100;


 	public float f
	{
		get {
			return i * 0.001f;
		}
	}

	
	static VRate()
	{
		zero = new VRate(0);
		one = new VRate(1.0f);
		half = new VRate(0.5f);
	}

    public VRate(int i)
    {
        this.i = i;
    }

    public VRate(float f)
    {
        this.i = IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit);
        //this.i = (int) Math.Round((double) (f * 1000f));
    }

    public override bool Equals(object o)
    {
        if (o == null)
        {
            return false;
        }
        VRate num = (VRate) o;
        return (this.i == num.i);
    }

    public override int GetHashCode()
    {
        return this.i.GetHashCode();
    }

    public static VRate Min(VRate a, VRate b)
    {
        return new VRate(Math.Min(a.i, b.i));
    }

    public static VRate Max(VRate a, VRate b)
    {
        return new VRate(Math.Max(a.i, b.i));
    }


    public static VRate Conver2VInt(long n,long d)
    {
        VRate value = new VRate();
        value.i = (int)(n * 1000 / d);
        return value;
    }

    public override string ToString()
    {
        return this.scalar.ToString();
    }

    public float scalar
    {
        get
        {
            return (this.i *0.001f);
        }
    }

    public VFactor factor
    {
        get{
            return new VFactor(i,1000);
        }
    }

    public static VFactor Factor(int v)
    {
        return new VFactor(v,1000);
    }

    public static int Conver(float f)
    {
        return IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit);
        //return (int) Math.Round((double) (f * 1000f));
    }


    public static int Conver(float n,int d)
    {
        return IntMath.Float2IntWithFixed((n * (float)d), 1, kFloatBit);
        //return (int) Math.Round((double) (n * (float)d));
    }

    public static int ConverFloor(float n,int d)
    {
        return (int) Math.Floor((double) (n * (float)d));
    }

    public static explicit operator VRate(float f)
    {
        return new VRate(IntMath.Float2IntWithFixed(f, kValidBit, kFloatBit));
        //return new VRate((int) Math.Round((double) (f * 1000f)));
    }

    public static implicit operator VRate(int i)
    {
        return new VRate(i);
    }

    public static explicit operator float(VRate ob)
    {
        return (ob.i * 1000f);
    }

    public static explicit operator long(VRate ob)
    {
        return (long) ob.i;
    }

    public static VRate operator +(VRate a, VRate b)
    {
        return new VRate(a.i + b.i);
    }

    public static VRate operator -(VRate a, VRate b)
    {
        return new VRate(a.i - b.i);
    }

    public static bool operator ==(VRate a, VRate b)
    {
        return (a.i == b.i);
    }

    public static bool operator !=(VRate a, VRate b)
    {
        return (a.i != b.i);
    }

    public static bool operator >=(VRate a, VRate b)
    {
        return (a.i >= b.i);
    }

    public static bool operator <=(VRate a, VRate b)
    {
        return (a.i <= b.i);
    }

    public static bool operator >(VRate a, VRate b)
    {
        return (a.i > b.i);
    }

    public static bool operator <(VRate a, VRate b)
    {
        return (a.i < b.i);
    }

     public static bool operator ==(VRate a, int b)
    {
        return (a.i == b);
    }

    public static bool operator !=(VRate a, int b)
    {
        return (a.i != b);
    }

    public static bool operator >=(VRate a, int b)
    {
        return (a.i >= b);
    }

    public static bool operator <=(VRate a, int b)
    {
        return (a.i <= b);
    }

    public static bool operator >(VRate a, int b)
    {
        return (a.i > b);
    }

    public static bool operator <(VRate a, int b)
    {
        return (a.i < b);
    }

    public static VRate operator -(VRate a)
    {
        a.i = -a.i;
        return a;
    }
}

