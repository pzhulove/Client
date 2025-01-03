using UnityEngine;
using System.Collections;

public class DBoxConfig
{
    public static  bool b2D = true;
    public static readonly VFactor angle =  VFactor.pi * 20 / 180;                //12.0f / 180.0f * Mathf.PI;
    public static readonly VFactor fSinA = IntMath.sin(angle.nom,angle.den);
    public static readonly VFactor fCosA = IntMath.cos(angle.nom,angle.den);
}

public struct DGrid
{
    public int x;
    public int y;

     public DGrid(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

}

// public struct VInt2
// {
// 
//     public float x;
//     public float y;
// 
//     public VInt2(float iX = 0, float iY = 0)
//     {
//         x = iX;
//         y = iY;
//     }
// 
//     public void set(float iX, float iY)
//     {
//         x = iX;
//         y = iY;
//     }
// 
//     public VInt2 normalize()
//     {
//         return new VInt2(x / (Mathf.Abs(x) + Mathf.Abs(y)), y / (Mathf.Abs(x) + Mathf.Abs(y)));
//     }        
// 
//     public float distance(VInt2 other)
//     {
//         return Mathf.Sqrt( (x - other.x) * (x - other.x) + (y - other.y) * (y - other.y) );
//     }
// }

[System.Serializable]
public struct Vec3
{
    public float x;
    public float y;
    public float z;

    public static Vec3 zero {
        get
        {
            return new Vec3(0, 0, 0);
        }
    }

    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 vector3()
    {
        return new Vector3(x, z, y);
    }

    public Vec3(Vector3 v)
    {
        this.x = v.x;
        this.y = v.z;
        this.z = v.y;
    }

    public static bool operator ==(Vec3 v1, Vec3 v2)
    {
        return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
    }

    public static bool operator !=(Vec3 v1, Vec3 v2)
    {
        return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
    }

    public static Vec3 operator +(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vec3 operator -(Vec3 v1, Vec3 v2)
    {
        return new Vec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static Vec3 operator *(Vec3 v1, int v)
    {
        return new Vec3(v1.x * v, v1.y * v, v1.z * v);
    }

    public static Vec3 operator *(Vec3 v1, float v)
    {
        return new Vec3(v1.x * v, v1.y * v, v1.z * v);
    }

    public static float Distance(Vec3 v1, Vec3 v2)
    {
        Vec3 dis = v1 - v2;
        return Mathf.Sqrt(dis.x * dis.x + dis.y * dis.y);
    }
}

public struct DBox
{
    public VInt2 _min;
    public VInt2 _max;

    public void Print()
    {
        Logger.Log(ToString());
    }

    public static int MAX(int f1, int f2)
    {
        return (f1 > f2 ? f1 : f2);
    }

    public static int MIN(int f1, int f2)
    {
        return (f1 < f2 ? f1 : f2);
    }

    public bool IsValide()
    {
        return _min.x != 0 || _min.y != 0 || _max.x != 0 || _max.y != 0;
    }

    public VInt2 getCenter()
    {
        VInt2 center = new VInt2();
        center.x = (_min.x + _max.x) / 2;
        center.y = (_min.y + _max.y) / 2;
        return center;
    }

    public bool intersects(DBox aabb)
    {
        return !
            (_min.x > aabb._max.x ||
            _max.x < aabb._min.x ||
            _min.y > aabb._max.y ||
            _max.y < aabb._min.y
            );
    }

    public void getIntersects(DBox aabb, ref DBox rkOut)
    {
        rkOut._min.x = MAX(_min.x, aabb._min.x);
        rkOut._min.y = MAX(_min.y, aabb._min.y);
        rkOut._max.x = MIN(_max.x, aabb._max.x);
        rkOut._max.y = MIN(_max.y, aabb._max.y);
    }

    public bool containPoint(ref VInt2 point)
    {
        if (point.x < _min.x) return false;
        if (point.y < _min.y) return false;
        if (point.x > _max.x) return false;
        if (point.y > _max.y) return false;
        return true;
    }

    public void merge(DBox box)
    {
        // Calculate the new minimum point.
        _min.x = MIN(_min.x, box._min.x);
        _min.y = MIN(_min.y, box._min.y);

        // Calculate the new maximum point.
        _max.x = MAX(_max.x, box._max.x);
        _max.y = MAX(_max.y, box._max.y);
    }

    public void set(VInt2 min, VInt2 max)
    {
        _min = min;
        _max = max;
    }

    public void offset(DBox rkBox, VInt x, VInt y, VFactor scale, bool bNegX)
    {
        if (bNegX)
        {
            _min.x = -rkBox._max.x * scale + x.i;
            _max.x = -rkBox._min.x * scale + x.i;
            _min.y = rkBox._min.y * scale + y.i;
            _max.y = rkBox._max.y * scale + y.i;
        }
        else
        {
            _min.x = rkBox._min.x * scale + x.i;
            _max.x = rkBox._max.x * scale + x.i;
            _min.y = rkBox._min.y * scale + y.i;
            _max.y = rkBox._max.y * scale + y.i;
        }
    }

    public void reset()
    {
        _min = new VInt2(99999, 99999);
        _max = new VInt2(-99999, -99999);
    }

    public bool isEmpty()
    {
        return _min.x > _max.x || _min.y > _max.y;
    }

    public void updateMinMax(VInt2 point, int num)
    {

    }
    
    public override string ToString()
    {
        return string.Format("({0},{1}) ({2},{3})", _min.x, _min.y, _max.x, _max.y);
    }
}


public struct DBox3
{
    public  VInt3 _min;
    public  VInt3 _max;

    public DBox3(DBox box,float zDim)
    {
        _min = new VInt3 (box._min.x, box._min.y, -zDim);
        _max = new VInt3 (box._max.x, box._max.y, zDim);
    }

    public void Print()
    {
        //UnityEngine.Debug.Log(string.Format("({0},{1},{2}) ({3},{4},{5})", _min.x, _min.y, _min.z,_max.x, _max.y, _max.z));
    }

    public static int MAX(int f1, int f2)
    {
        return (f1 > f2 ? f1 : f2);
    }

    public static int MIN(int f1, int f2)
    {
        return (f1 < f2 ? f1 : f2);
    }

    public bool IsValide()
    {
        return _min.x != 0 || _min.y != 0 || _max.x != 0 || _max.y != 0 || _min.z != 0 || _max.z != 0;
    }

    public  VInt3 getCenter()
    {
        VInt3 center = new VInt3();
        center.x = (_min.x + _max.x) / 2;
        center.y = (_min.y + _max.y) / 2;
        center.z = (_min.z + _max.z) / 2;
        return center;
    }

    public bool intersects(DBox3 aabb)
    {
        return !
            (_min.x > aabb._max.x ||
            _max.x < aabb._min.x ||
            _min.y > aabb._max.y ||
            _max.y < aabb._min.y ||
            _min.z > aabb._max.z ||
            _max.z < aabb._min.z
            );
    }

    public void getIntersects(DBox3 aabb, ref DBox3 rkOut)
    {
        rkOut._min.x = MAX(_min.x, aabb._min.x);
        rkOut._min.y = MAX(_min.y, aabb._min.y);
        rkOut._min.z = MAX(_min.z, aabb._min.z);
        rkOut._max.x = MIN(_max.x, aabb._max.x);
        rkOut._max.y = MIN(_max.y, aabb._max.y);
        rkOut._max.z = MIN(_max.z, aabb._max.z);
    }

    public bool containPoint(ref VInt3 point)
    {
        if (point.x < _min.x) return false;
        if (point.y < _min.y) return false;
        if (point.z < _min.z) return false;
        if (point.x > _max.x) return false;
        if (point.y > _max.y) return false;
        if (point.z > _max.z) return false;
        return true;
    }

    public void merge(DBox3 box)
    {
        // Calculate the new minimum point.
        _min.x = MIN(_min.x, box._min.x);
        _min.y = MIN(_min.y, box._min.y);
        _min.z = MIN(_min.z, box._min.z);

        // Calculate the new maximum point.
        _max.x = MAX(_max.x, box._max.x);
        _max.y = MAX(_max.y, box._max.y);
        _max.z = MAX(_max.z, box._max.z);
    }

    public void set( VInt3 min,  VInt3 max)
    {
        _min = min;
        _max = max;
    }

    public void offset(DBox3 rkBox, VInt x, VInt y, VInt z,VFactor scale, bool bNegX)
    {
        if (bNegX)
        {
            _min.x = -rkBox._max.x * scale + x.i;
            _max.x = -rkBox._min.x * scale + x.i;
            _min.y = rkBox._min.y * scale + y.i;
            _max.y = rkBox._max.y * scale + y.i;
            _min.z = rkBox._min.z * scale + z.i;
            _max.z = rkBox._max.z * scale + z.i;
        }
        else
        {
            _min.x = rkBox._min.x * scale + x.i;
            _max.x = rkBox._max.x * scale + x.i;
            _min.y = rkBox._min.y * scale + y.i;
            _max.y = rkBox._max.y * scale + y.i;
            _min.z = rkBox._min.z * scale + z.i;
            _max.z = rkBox._max.z * scale + z.i;
        }
    }

    public void reset()
    {
        _min = new VInt3(99999, 99999, 99999);
        _max = new VInt3(-99999, -99999, -99999);
    }

    public bool isEmpty()
    {
        return _min.x > _max.x || _min.y > _max.y || _min.z > _max.z;
    }

    public void updateMinMax(VInt2 point, int num)
    {

    }
}