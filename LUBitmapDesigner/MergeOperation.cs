namespace LUBitmapDesigner
{
    public enum MergeOperation
    {
        Normal,
        APlusB,
        AMinusB,
        BMinusA,
        Difference,
        APlusBBy2,
        Multiply,
        Screen,
        DarkenOnly,
        LightenOnly,
        DivideAB,
        DivideABStraight, 
        DivideBA,
        DivideBAStraight,
        AlphaMask_BGForAlphaGr0,  
        AlphaMask_AlphaFromMask,  
        AlphaMask_Invers,   
        AlphaMask_AlphaFromMaskWhenLower
    }
}