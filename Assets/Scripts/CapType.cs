using UnityEngine;


public enum CapColor
{
    Blue,
    Red,
    Green,
    Purple    
}

public static class CapType
{
    public static Color GetColor(CapColor capColor)
    {
        switch (capColor)
        {
            case CapColor.Blue:
                return Color.blue;
            case CapColor.Red:
                return Color.red;
            case CapColor.Green:
                return Color.green;
            case CapColor.Purple:
                return new Color(0.5f, 0f, 0.5f);
            default:
                return Color.white; 
        }
    }
}