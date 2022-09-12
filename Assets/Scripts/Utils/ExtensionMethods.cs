using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static T PickRandomElementInList<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T PickRandomElementInArray<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    
    public static int IndexOf<T>(this T[] array, T element)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(element)) return i;
        }

        Debug.LogError("Element " + element + " not found in array " + array);
        return -1;
    }

    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] dest = new T[source.Length - 1];
        if (index > 0)
            System.Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            System.Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }

    public static int GetRandomNumberAt(this System.Random random, int seed, int step)
    {
        return 0;
    }

    public static void DrawDot(Vector3 position, float size, Color color, float duration)
    {
        Debug.DrawLine(position - Vector3.forward * size/2, position + Vector3.forward*size/2, color, duration);
        Debug.DrawLine(position - Vector3.up * size / 2, position + Vector3.up * size / 2, color, duration);
        Debug.DrawLine(position - Vector3.right * size / 2, position + Vector3.right * size / 2, color, duration);
    }

    public static Vector3 VectorProjectionOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        if (planeNormal.magnitude == 0)
        {
            Debug.LogError("Plane normal magnitude can't be equal to 0");
            return Vector3.zero;
        }
        return vector - Vector3.Dot(vector, planeNormal) / (planeNormal.magnitude * planeNormal.magnitude) * planeNormal;
    }

    public static Vector3 IntersectionVectorPlane(Vector3 vectorOrigin, Vector3 vectorDirection, Vector3 planeNormal, Vector3 planePoint)
    {
        if(planeNormal.x * vectorDirection.x + planeNormal.y * vectorDirection.y + planeNormal.z * vectorDirection.z == 0)
        {
            Debug.LogError("No point found, vector is parallel to plane");
            return Vector3.zero;
        }
        float t = (planeNormal.x * (-vectorOrigin.x + planePoint.x) + planeNormal.y * (-vectorOrigin.y + planePoint.y) + planeNormal.z * (-vectorOrigin.z + planePoint.z)) / (planeNormal.x * vectorDirection.x + planeNormal.y * vectorDirection.y + planeNormal.z * vectorDirection.z);
        return new Vector3(vectorOrigin.x + vectorDirection.x * t, vectorOrigin.y + vectorDirection.y * t, vectorOrigin.z + vectorDirection.z * t);
    }

    public static int Value(this bool b)
    {
        return b ? 1 : 0;
    }

    public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
    
}
