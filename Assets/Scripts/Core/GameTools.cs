using UnityEngine;

public class GameTools
{
    #region Debug
    public static void ClearLogConsole()
    {
        #if UNITY_EDITOR
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
        System.Type type = assembly.GetType("UnityEditor.LogEntries");
        System.Reflection.MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        #endif
    }
    #endregion
    #region Positioning And Rotation
    /// <summary> Advances a vector into the specified direction. </summary>
    public static Vector3 DisplaceVector(Vector3 start, float angle, float distance)
    {
        return start += OffsetFromAngle(angle, distance);
    }
    /// <summary> Returns a vector representing the offset from a center point at the specified angle. </summary>
    public static Vector3 OffsetFromAngle(float angle, float distance)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
    }
    /// <summary> Returns the angle at wich pos1 would be looking at pos2. </summary>
    public static float AngleBetween(Vector3 pos1, Vector3 pos2)
    {
        Vector3 posDelta = pos2 - pos1;
        return Mathf.Atan2(posDelta.y, posDelta.x) * Mathf.Rad2Deg;
    }
    /// <summary> Transforms a Vector3 normal into an euler degree. </summary>
    public static float NormalToEuler(Vector3 normal)
    {
        if (normal.x == 0) { return normal.y > 0 ? 90 : 270; }
        if (normal.y == 0) { return normal.x > 0 ? 0 : 180; }
        return Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 180;
    }
    /// <summary> Transforms a Vector2 normal into an euler degree. </summary>
    public static float NormalToEuler(Vector2 normal)
    {
        if (normal.x == 0) { return normal.y > 0 ? 90 : 270; }
        if (normal.y == 0) { return normal.x > 0 ? 0 : 180; }
        return Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg + 180;
    }
    /// <summary> Transforms an euler angle into a 2D vector. </summary>
    public static Vector3 AngleToVector(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
    /// <summary> Reflects the provided angle over the provided normal (in euler). </summary>
    public static float AngleReflection(float angle, float surfaceNormal)
    {
        // Any angle can be reflected over a 90º normal (standard plain surface) multiplicating by -1.
        float normalDifference = 90 - surfaceNormal; // Difference in angles between the provided normal and the standard plain surface.
        float result = angle + normalDifference; // Convert to basic surface
        result *= -1; // Reflect
        result -= normalDifference; // Revert to input surface
        return result;
    }
    /// <summary> Reflects the provided angle over the provided normal. </summary>
    public static float AngleReflection(float angle, Vector2 surfaceNormal)
    {
        return AngleReflection(angle, NormalToEuler(surfaceNormal));
    }
    /// <summary> Returns the height of an arc at the T position (between 1 and 0). </summary>
    public static float HeightArc(float t, float heightMult)
    {
        return Mathf.Sin(t * 180 * Mathf.Deg2Rad) * heightMult;
    }
    #endregion 
}
