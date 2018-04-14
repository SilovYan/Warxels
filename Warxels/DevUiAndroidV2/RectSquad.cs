using System.Collections.Generic;
using GameLogic;

namespace DevUiAndroidV2
{
    internal class RectSquad : ISquad
    {
        private readonly int _x;
        private readonly int _y;

        public RectSquad(int x, int y, UnitType type, Team team, int xMin, int yMin)
        {
            _x = x;
            _y = y;
            MinX = xMin;
            MinY = yMin;
            Type = type;
            Team = team;
        }
        
        public int MinX { get; private set; }

        public int MaxX => MinX + _x;
        public int MinY { get; private set; }

        public int MaxY => MinY + _y;

        public int Size => _x * _y;

        public UnitType Type { get; }
        public Team Team { get; }

        public bool CheckAndSetPos(GenerateArmy army, int x, int y)
        {
            var tempX = MinX;
            var tempY = MinY;
            MinX = x;
            MinY = y;
            var canAdded = army.CheckSquad(this);
            MinX = canAdded ? MinX : tempX;
            MinY = canAdded ? MinY : tempY;
            return canAdded;
        }
        public IEnumerable<IUnit> GetUnits()
        {
            return null;
        }
    }
}