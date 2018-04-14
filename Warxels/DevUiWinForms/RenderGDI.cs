using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameLogic;

namespace DevUiWinForms
{
    internal sealed class RenderGDI
    {
        private static readonly Brush MarshBrush = new SolidBrush(Color.FromArgb(64, 0, 0, 255));
        private static readonly Pen Pen = new Pen(Brushes.AliceBlue);
        private static readonly Pen ProjectilePen = new Pen(Brushes.Brown);

        private static readonly Pen SquarePen = new Pen(Color.Black);
        private readonly Graphics _gfx;
        private readonly Graphics _gfxBack;

        private const int ImageSizeX = 1024;
        private const int ImageSizeY = 1024;

        private static readonly Image image = new Bitmap(ImageSizeX, ImageSizeY);
        private static readonly Image backgroundImage = new Bitmap(ImageSizeX, ImageSizeY);

        private bool _renderSquare;
        private Point _squareBegin;
        private Point _squareEnd;
        

        public RenderGDI()
        {
            _gfx = Graphics.FromImage(image);
            _gfxBack = Graphics.FromImage(backgroundImage);
        }

        public Image GetImage()
        {
            return image;
        }

        public void DisableSquare()
        {
            _renderSquare = false;
        }

        public void SetSquare(float x1, float y1, float x2, float y2)
        {
            _renderSquare = true;
            _squareBegin = new Point((int)(x1 * ImageSizeX), (int)(y1 * ImageSizeY));
            _squareEnd = new Point((int)(x2 * ImageSizeX), (int)(y2 * ImageSizeY));
        }

        public Image Render(IWorld world)
        {
            _gfx.Clear(Color.White);
            _gfx.DrawImage(backgroundImage, 0, 0);
            DrawUnits(_gfx, world);
            DrawProjectiles(_gfx, world);

            if (_renderSquare)
            {
                _gfx.DrawRectangle(SquarePen, _squareBegin.X,_squareBegin.Y, _squareEnd.X - _squareBegin.X, _squareEnd.Y - _squareBegin.Y);
            }

            return image;
        }
        

        public void UpdateBackgroundImage(IWorld world)
        {
            _gfxBack.Clear(Color.White);
            DrawGrid(_gfxBack, world);
            DrawTerrain(_gfxBack, world);
        }

        private void DrawTerrain(Graphics gfx, IWorld world)
        {
            var dX = (float)ImageSizeX / world.Width;
            var dY = (float)ImageSizeY / world.Length;

            for (int i = 0; i < world.Width; i++)
                for (int j = 0; j < world.Length; j++)
                    switch (world.GetTerrainType(j, i))
                    {
                        case TerrainType.Marsh: gfx.FillRectangle(MarshBrush, i * dX, j * dY, dX, dY); break;
                        default: break;
                    }
        }

        private void DrawProjectiles(Graphics gfx, IWorld world)
        {
            float dX = (float)ImageSizeX / world.Width;
            float dY = (float)ImageSizeY / world.Length;

            foreach (var projectile in world.GetProjectiles())
            {
                gfx.DrawEllipse(ProjectilePen, projectile.X * dX, projectile.Y * dY, dX / 2, dY / 2);
            }
        }

        private Pen[] TeamAPens = new Pen[] {
            new Pen(Color.FromArgb(50, Color.Red)),
            new Pen(Color.FromArgb(100, Color.Red)),
            new Pen(Color.FromArgb(150, Color.Red)),
            new Pen(Color.FromArgb(200, Color.Red)),
            new Pen(Color.FromArgb(255, Color.Red))
        };

        private Pen[] TeamBPens = new Pen[] {
            new Pen(Color.FromArgb(50, Color.Blue)),
            new Pen(Color.FromArgb(100, Color.Blue)),
            new Pen(Color.FromArgb(150, Color.Blue)),
            new Pen(Color.FromArgb(200, Color.Blue)),
            new Pen(Color.FromArgb(255, Color.Blue))
        };

        private SolidBrush[] TeamASolidPens = new SolidBrush[] {
            new SolidBrush(Color.FromArgb(50, Color.Red)),
            new SolidBrush(Color.FromArgb(100, Color.Red)),
            new SolidBrush(Color.FromArgb(150, Color.Red)),
            new SolidBrush(Color.FromArgb(200, Color.Red)),
            new SolidBrush(Color.FromArgb(255, Color.Red))
        };

        private SolidBrush[] TeamBSolidPens = new SolidBrush[] {
            new SolidBrush(Color.FromArgb(50, Color.Blue)),
            new SolidBrush(Color.FromArgb(100, Color.Blue)),
            new SolidBrush(Color.FromArgb(150, Color.Blue)),
            new SolidBrush(Color.FromArgb(200, Color.Blue)),
            new SolidBrush(Color.FromArgb(255, Color.Blue))
        };

        private void DrawUnits(Graphics gfx, IWorld world)
        {
            float dX = (float)ImageSizeX / world.Width;
            float dY = (float)ImageSizeY / world.Length;

            foreach (var unit in world.Army.GetUnits())
            {
                var healthPercentageIndex = unit.GetHealthPercentage() / 25;
                switch (unit.UnitType)
                {
                    case UnitType.SwordsMan:
                        gfx.DrawEllipse(unit.Team == Team.Red ? TeamAPens[healthPercentageIndex] : TeamBPens[healthPercentageIndex], unit.X * dX, unit.Y * dY, dX, dY); break;
                    case UnitType.HorseMan:
                        gfx.DrawRectangle(unit.Team == Team.Red ? TeamAPens[healthPercentageIndex] : TeamBPens[healthPercentageIndex], unit.X * dX, unit.Y * dY, dX, dY); break;
                    case UnitType.Archer:
                        gfx.FillEllipse(unit.Team == Team.Red ? TeamASolidPens[healthPercentageIndex] : TeamBSolidPens[healthPercentageIndex], unit.X * dX, unit.Y * dY, dX, dY); break;
                }

            }
        }

        private void DrawGrid(Graphics gfx, IWorld world)
        {
            float stepX = (float)ImageSizeX / world.Width;
            float stepY = (float)ImageSizeY / world.Length;

            for (float x = 0; x < ImageSizeX; x += stepX)
            {
                gfx.DrawLine(Pen, x, 0, x, ImageSizeY);
            }

            for (float y = 0; y < ImageSizeY; y += stepY)
            {
                gfx.DrawLine(Pen, 0, y, ImageSizeX, y);
            }
        }
    }
}
