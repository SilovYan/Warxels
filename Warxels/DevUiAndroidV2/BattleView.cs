using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using GameLogic;

namespace DevUiAndroidV2
{
    class BattleView : View
    {
        public bool EndGame => _lastMoveAgo > 50;
        public int Delay { get; } = 10;
        public IWorld World { get; }
        private const int Size = MyView.Size;
        private int _lastMoveAgo;
        private int _step;
        private readonly Paint _marshPaint = new Paint { Color = new Color(0, 0, 255, 64) };
        private readonly Paint _projectilePen = new Paint { Color = Color.Brown };

        public int CountBlueDead { get; private set; }
        public int CountRedDead { get; private set; }


        private readonly Paint[] _teamAPens = {
            new Paint{Color = Color.Red, Alpha = 50},
            new Paint{Color = Color.Red, Alpha = 100},
            new Paint{Color = Color.Red, Alpha = 150},
            new Paint{Color = Color.Red, Alpha = 200},
            new Paint{Color = Color.Red, Alpha = 255}
        };

        private readonly Paint[] _teamBPens = {
            new Paint{Color = Color.Blue, Alpha = 50},
            new Paint{Color = Color.Blue, Alpha = 100},
            new Paint{Color = Color.Blue, Alpha = 150},
            new Paint{Color = Color.Blue, Alpha = 200},
            new Paint{Color = Color.Blue, Alpha = 255}
        };

        private readonly Paint[] _teamASolidPens = {
            new Paint{Color = Color.Red, Alpha = 50},
            new Paint{Color = Color.Red, Alpha = 100},
            new Paint{Color = Color.Red, Alpha = 150},
            new Paint{Color = Color.Red, Alpha = 200},
            new Paint{Color = Color.Red, Alpha = 255}
        };

        private readonly Paint[] _teamBSolidPens = {
            new Paint{Color = Color.Blue, Alpha = 50},
            new Paint{Color = Color.Blue, Alpha = 100},
            new Paint{Color = Color.Blue, Alpha = 150},
            new Paint{Color = Color.Blue, Alpha = 200},
            new Paint{Color = Color.Blue, Alpha = 255}
        };

        public BattleView(Context context, IWorld world) : base(context)
        {
            World = world;
            foreach (var p in _teamASolidPens)
            {
                p.SetStyle(Paint.Style.Fill);
            }
            foreach (var p in _teamBSolidPens)
            {
                p.SetStyle(Paint.Style.Fill);
            }

            foreach (var p in _teamAPens)
            {
                p.SetStyle(Paint.Style.Stroke);
            }
            foreach (var p in _teamBPens)
            {
                p.SetStyle(Paint.Style.Stroke);
            }
        }

        private void DrawTerrain(Canvas canvas)
        {
            for (int i = 0; i < World.Length; i++)
            {
                for (int j = 0; j < World.Width; j++)
                {
                    switch (World.GetTerrainType(i,j))
                    {
                        case TerrainType.Marsh:
                            canvas.DrawRect(i * _step, j * _step, (i + 1) * _step, (j + 1) * _step, _marshPaint);
                            break;
                    }
                }
            }
        }
        private void DrawUnits(Canvas canvas)
        {
            foreach (var unit in World.Army.GetUnits())
            {
                var healthPercentageIndex = unit.GetHealthPercentage() / 25;
                switch (unit.UnitType)
                {
                    case UnitType.SwordsMan:
                        canvas.DrawOval(unit.X * _step, unit.Y * _step, (unit.X + 1) * _step, (unit.Y + 1) * _step, unit.Team == Team.Red ? _teamAPens[healthPercentageIndex] : _teamBPens[healthPercentageIndex]); break;
                    case UnitType.HorseMan:
                        canvas.DrawRect(unit.X * _step, unit.Y * _step, (unit.X + 1) * _step, (unit.Y + 1) * _step, unit.Team == Team.Red ? _teamAPens[healthPercentageIndex] : _teamBPens[healthPercentageIndex]); break;
                    case UnitType.Archer:
                        canvas.DrawOval(unit.X * _step, unit.Y * _step, (unit.X + 1) * _step, (unit.Y + 1) * _step, unit.Team == Team.Red ? _teamASolidPens[healthPercentageIndex] : _teamBSolidPens[healthPercentageIndex]); break;
                }
            }
        }

        private void DrawProjectiles(Canvas canvas)
        {
            foreach (var projectile in World.GetProjectiles())
            {
                canvas.DrawOval(projectile.X * _step, projectile.Y * _step, projectile.X * _step + _step / 2.0f,
                    projectile.Y * _step + _step / 2.0f, _projectilePen);
            }
        }

        public override void Draw(Canvas canvas)
        {
            _step = Right / Size;

            if (!EndGame)
            {

                var result = World.DoTick();
                if (result.WasMoves)
                    _lastMoveAgo = 0;
                else
                    _lastMoveAgo++;
                CountBlueDead += result.DeadUnits.Count(c => c.Team == Team.Blue);
                CountRedDead += result.DeadUnits.Count(c => c.Team == Team.Red);
            }
            canvas.DrawARGB(255, 255, 255, 255);

            DrawTerrain(canvas);
            DrawUnits(canvas);
            DrawProjectiles(canvas);
        }
    }
}