using System;
using System.IO;
using System.Text;

namespace GameLogic.Helper
{
    public static class SaveLoadHelper
    {
        public static void SaveTerrain(this IWorld world, string fname, bool android = false)
        {
            if (android)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fname = Path.Combine(path, fname);
            }
            var builder = new StringBuilder();
            for (int i = 0; i < world.Length; i++)
            {
                for (int j = 0; j < world.Width; j++)
                {
                    var terrainType = world.GetTerrainType(i, j);
                    builder.Append(GetTerrainView(terrainType));
                }

                builder.Append(Environment.NewLine);
            }
            File.WriteAllText(fname, builder.ToString(), Encoding.Unicode);
        }

        public static void SaveUnits(this IWorld world, string fname, bool android = false)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < world.Length; i++)
            {
                for (int j = 0; j < world.Width; j++)
                {
                    builder.Append(GetUnitView(world.Army.GetUnit(i,j)));
                }

                builder.Append(Environment.NewLine);
            }
            if (android) {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fname = Path.Combine(path, fname);               
            }

            File.WriteAllText(fname, builder.ToString(), Encoding.Unicode);
        }


        private static char GetUnitView(IUnit unit)
        {
            if (unit == null)
                return ' ';
            switch (unit.UnitType)
            {
                case UnitType.Archer: return unit.Team == Team.Blue ? 'a' : 'A';
                case UnitType.SwordsMan: return unit.Team == Team.Blue ? 's' : 'S';
                case UnitType.HorseMan: return unit.Team == Team.Blue ? 'h' : 'H';
                default: throw new ArgumentOutOfRangeException(nameof(unit.UnitType));
            }
        }

        private static char GetTerrainView(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Ground: return ' ';
                case TerrainType.Marsh: return 'm';
                default: throw new ArgumentOutOfRangeException(nameof(terrain));
            }
        }

        public static WorldsGenerator LoadTerrainFromFile(string fname, bool android = false)
        {
            if (android)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fname = Path.Combine(path, fname);
            }

            var lines = File.ReadAllLines(fname, Encoding.Unicode);
            
            var width = lines[0].Length;
            var height = lines.Length;

            var worldGen = WorldsGenerator.GetDefault(height, width);
            var world = worldGen.GetWorld();

            int y = 0;

            foreach (var line in lines)
            {
                int x = 0;
                if (x >= world.Length)
                    break;

                foreach (var c in line)
                {
                    if (x >= world.Width)
                        break;

                    if (c == 'm')
                        worldGen.SetTerrain(y, x, TerrainType.Marsh);
                    x++;
                }
                y++;
            }

            return worldGen;
        }


        public static void LoadUnitsFromFile(this WorldsGenerator worldGen, string fname, bool android = false)
        {
            if (android)
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fname = Path.Combine(path, fname);
            }

            var lines = File.ReadAllLines(fname,Encoding.Unicode);
            if (lines.Length < 0)
                return;

            var world = worldGen.GetWorld();
            world.Army.Clear();

            int y = 0;

            foreach (var line in lines)
            {
                int x = 0;
                if (y >= world.Length)
                    break;

                foreach (var c in line)
                {
                    if (x >= world.Width)
                        break;
                    
                    switch (c)
                    {
                        case 'a': worldGen.CreateUnit(UnitType.Archer, Team.Blue, y, x); break;
                        case 's': worldGen.CreateUnit(UnitType.SwordsMan, Team.Blue, y, x); break;
                        case 'h': worldGen.CreateUnit(UnitType.HorseMan, Team.Blue, y, x); break;
                        case 'A': worldGen.CreateUnit(UnitType.Archer, Team.Red, y, x); break;
                        case 'S': worldGen.CreateUnit(UnitType.SwordsMan, Team.Red, y, x); break;
                        case 'H': worldGen.CreateUnit(UnitType.HorseMan, Team.Red, y, x); break;
                        case ' ': break;
                        default: throw new InvalidOperationException("Unknown unit type " + c);
                    }
                    x++;
                }
                y++;
            }
        }
    }
}
