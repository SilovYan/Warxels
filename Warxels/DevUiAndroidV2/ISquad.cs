using GameLogic;

namespace DevUiAndroidV2
{
    internal interface ISquad
    {
        int MinX { get; }
        int MaxX { get; }
        int MinY { get; }
        int MaxY { get; }
        int Size { get; }
        UnitType Type { get; }
        Team Team { get; }
        bool CheckAndSetPos(GenerateArmy army, int x, int y);
    }
}