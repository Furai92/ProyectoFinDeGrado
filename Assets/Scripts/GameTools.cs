﻿using UnityEngine;

public class GameTools
{
    private const float MIGHT_TO_DAMAGE = 0.01f;
    private const float NEGATIVE_MIGHT_TO_DAMAGE_DIV = 0.01f;
    private const float DEXTERITY_TO_DAMAGE = 0.01f;
    private const float NEGATIVE_DEXTERITY_TO_DAMAGE_DIV = 0.01f;
    private const float INTELLECT_TO_BUILDUP = 0.01f;
    private const float NEGATIVE_INTELLECT_TO_BUILDUP_DIV = 0.01f;
    private const float ENDURANCE_TO_DR_DIV = 0.01f;
    private const float NEGATIVE_ENDURANCE_TO_DAMAGE = 0.01f;

    private const float DIF_TO_DMG_MAX = 3f;
    private const float DIF_TO_DMG_SCALING = 0.1f;
    private const float DIF_TO_HP_MAX = 100f;
    private const float DIF_TO_HP_SCALING = 0.5f;


    #region Scaling
    public static float DifficultyToHealthMultiplier(float d) 
    {
        return Mathf.Min(1 + d * DIF_TO_HP_SCALING, DIF_TO_HP_MAX);
    }
    public static float DifficultyToDamageMultiplier(float d) 
    {
        return Mathf.Min(1 + d * DIF_TO_DMG_SCALING, DIF_TO_DMG_MAX);
    }
    #endregion
    #region Stats
    public static float MightToDamageMultiplier(float m)
    {
        return m >= 0 ? 1 + m * MIGHT_TO_DAMAGE : 1 / (1 + NEGATIVE_MIGHT_TO_DAMAGE_DIV * -m);
    }
    public static float DexterityToDamageMultiplier(float d)
    {
        return d >= 0 ? 1 + d * DEXTERITY_TO_DAMAGE : 1 / (1 + NEGATIVE_DEXTERITY_TO_DAMAGE_DIV * -d);
    }
    public static float IntellectToBuildupMultiplier(float i)
    {
        return i >= 0 ? 1 + i * INTELLECT_TO_BUILDUP : 1 / (1 + NEGATIVE_INTELLECT_TO_BUILDUP_DIV * -i);
    }
    public static float EnduranceToDamageMultiplier(float e) 
    {
        return e >= 0 ? 1 / (1 + ENDURANCE_TO_DR_DIV * e) : 1 + -e * NEGATIVE_ENDURANCE_TO_DAMAGE;
    }
    #endregion
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
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
    }
    /// <summary> Returns the angle at wich pos1 would be looking at pos2. </summary>
    public static float AngleBetween(Vector3 pos1, Vector3 pos2)
    {
        Vector3 posDelta = pos2 - pos1;
        return Mathf.Atan2(posDelta.x, posDelta.z) * Mathf.Rad2Deg;
    }
    public static float VectorToAngle(Vector3 v) 
    {
        if (v.x == 0) { return v.z > 0 ? 90 : 270; } // Prevents NaN
        if (v.z == 0) { return v.x > 0 ? 0 : 180; }  // Prevents NaN
        return Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg + 180;
    }
    /// <summary> Transforms a Vector3 normal into an euler degree. </summary>
    public static float NormalToEuler(Vector3 normal)
    {
        return VectorToAngle(normal);
    }
    /// <summary> Transforms an euler angle into a 2D vector. </summary>
    public static Vector3 AngleToVector(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
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
