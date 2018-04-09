namespace GameLogic
{
    using GameLogic.Strategies;

    internal sealed class SwordsMan : UnitBase
    {
        public SwordsMan(Team team, StrategySet strategies, int y, int x)
            : base(team, strategies, y, x)
        {
        }

        public override UnitType UnitType => UnitType.SwordsMan;

        public override int DamageValue => 25;

        public override int MoveCost => 10;

        public override float GetTerrainPenalty(TerrainType terrainType)
        {
            if (terrainType == TerrainType.Marsh)
                return 1.7f;

            return base.GetTerrainPenalty(terrainType);
        }
    }
}
