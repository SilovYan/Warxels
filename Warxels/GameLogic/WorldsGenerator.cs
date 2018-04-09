﻿using System;

namespace GameLogic
{
    using GameLogic.Strategies;

    public class WorldsGenerator
    {
        private readonly World _world;

        private readonly StrategySet _meleeStrategiesUp;
        private readonly StrategySet _meleeStrategiesDown;

        private readonly StrategySet _strategiesHorseUp;
        private readonly StrategySet _strategiesHorseDown;

        private readonly StrategySet _archerStrategiesUp;
        private readonly StrategySet _archerStrategiesDown;

        private WorldsGenerator(World world, int width)
        {
            var meleeLookupRadius = width / 10;
            var horsemanLookupRadius = width / 6;
            var archerLookupRadius = width / 2;

            var meleeFightStrategy = new MeleeFightStrategy(world);
            var fireStrategy = new FireStrategy(world);

            IStrategy up = new DefaultMoveStrategy(1, 0, world);
            IStrategy down = new DefaultMoveStrategy(-1, 0, world);

            _meleeStrategiesUp = new StrategySet(meleeFightStrategy, new MoveToEnemyStrategy(world, meleeLookupRadius), up);
            _meleeStrategiesDown = new StrategySet(meleeFightStrategy, new MoveToEnemyStrategy(world, meleeLookupRadius), down);

            IStrategy horseUp = new HorseMoveStrategy(1, 0, world);
            IStrategy horseDown = new HorseMoveStrategy(-1, 0, world);

            _strategiesHorseUp = new StrategySet(meleeFightStrategy, new MoveToEnemyStrategy(world, horsemanLookupRadius), horseUp);
            _strategiesHorseDown = new StrategySet(meleeFightStrategy, new MoveToEnemyStrategy(world, horsemanLookupRadius), horseDown);

            _archerStrategiesUp = new StrategySet(fireStrategy, meleeFightStrategy, new MoveToEnemyStrategy(world, archerLookupRadius), up);
            _archerStrategiesDown = new StrategySet(fireStrategy, meleeFightStrategy, new MoveToEnemyStrategy(world, archerLookupRadius), down);

            _world = world;
        }

        public IWorld GetWorld()
        {
            return _world;
        }

        public static WorldsGenerator GetDefault(int length, int width)
        {
            var world = Game.GenerateWorld(length, width);

            return new WorldsGenerator(world, width);
        }


        private IUnit CreateSwordsman(Team team, int y, int x)
        {
            var strats = team == Team.Blue ? _meleeStrategiesUp : _meleeStrategiesDown;

            var unit = new SwordsMan(team, strats, y, x);

            _world.AddUnit(unit);
            return unit;
        }

        private IUnit CreateHorseman(Team team, int y, int x)
        {
            var strats = team == Team.Blue ? _strategiesHorseUp : _strategiesHorseDown;

            var unit = new HorseMan(team, strats, y, x);

            _world.AddUnit(unit);
            return unit;
        }

        public void AddPresetUnits()
        {
            float dx = _world.Width / 512.0f;
            float dy = _world.Length / 512.0f;

            AddUnitSquare(Team.Blue, 2, 50 * dx, 60 * dx, 40 * dy, UnitType.HorseMan, 150);
            AddUnitSquare(Team.Blue, 2, 380 * dx, 60 * dx, 40 * dy, UnitType.HorseMan, 150);

            AddUnitSquare(Team.Blue, 5, 130 * dx, 100 * dx, 40 * dy, UnitType.Archer, 200);
            AddUnitSquare(Team.Blue, 5, 270 * dx, 100 * dx, 40 * dy, UnitType.Archer, 200);

            AddUnitSquare(Team.Blue, 90 * dy, 100 * dx, 100 * dx, 50 * dy, UnitType.SwordsMan, 400);
            AddUnitSquare(Team.Blue, 90 * dy, 300 * dx, 100 * dx, 50 * dy, UnitType.SwordsMan, 400);

            AddUnitSquare(Team.Blue, 50 * dy, 220 * dx, 60 * dx, 150 * dy, UnitType.SwordsMan, 200);
        }

        public static WorldsGenerator CreatePreset(int sizeY, int sizeX)
        {
            var world = Game.GenerateWorld(sizeY, sizeX);
            var gen = new WorldsGenerator(world, sizeY);
            gen.AddPresetUnits();
            return gen;

        }

        public void AddUnitSquare(Team team, int y, int x, int width, int height, UnitType type, int amount)
        {
            var density = (float)Math.Sqrt(width * height / amount);

            if (density == 0)
                density = 1;
            int k = 0;
            for (float i = 0; i < height; i += density)
                for (float j = 0; j < width; j += density)
                {
                    CreateUnit(type, team, (int)(y + i), (int)(x + j));
                    k++;

                    if (k == amount)
                        return;
                }
        }

        public void AddUnitSquare(Team team, float y, float x, float width, float height, UnitType type, int amount)
        {
            AddUnitSquare(team, (int)y, (int)x, (int)width, (int)height, type, amount);

        }

        public IUnit CreateUnit(UnitType type, Team team, int y, int x)
        {
            switch (type)
            {
                case UnitType.HorseMan:
                    return CreateHorseman(team, y, x);
                case UnitType.SwordsMan:
                    return CreateSwordsman(team, y, x);
                case UnitType.Archer:
                    return CreateArcher(team, y, x);
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        private IUnit CreateArcher(Team team, int y, int x)
        {
            var strats = team == Team.Blue ? _archerStrategiesUp : _archerStrategiesDown;

            var unit = new Archer(team, strats, y, x);

            _world.AddUnit(unit);
            return unit;
        }

        public void ClearUnits()
        {
            _world.Army.Clear();
        }

        public void SetTerrain(int y, int x, int y1, int x1, TerrainType terrainType)
        {
            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            for (int i = x; i < x1 && i < _world.Width; i++)
                for (int j = y; j < y1 && j < _world.Length; j++)
                    _world.Terrain[i, j] = terrainType;
        }

        public void SetTerrain(int y, int x, TerrainType terrainType)
        {
            _world.Terrain[x, y] = terrainType;
        }

    }
}
