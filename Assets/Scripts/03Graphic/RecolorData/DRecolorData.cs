using UnityEngine;

public class DRecolorData :ScriptableObject
{
    public Matrix4x4[] m_MatrixArray = new Matrix4x4[8];

    public void Save(Matrix4x4[] matrixArray)
    {
        for (int i = 0, icnt = matrixArray.Length; i < icnt; ++i)
            m_MatrixArray[i] = matrixArray[i];
    }
}
