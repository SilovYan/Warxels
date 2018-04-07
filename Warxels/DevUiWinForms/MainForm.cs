﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLogic;

namespace DevUiWinForms
{

    public partial class MainForm : Form
    {
        private static bool Paused = true;
        private const int DefaultDelay = 100;
        private static int Delay = DefaultDelay;
        private static readonly Pen Pen = new Pen(Brushes.AliceBlue);
        private static readonly Pen TeamAPen = new Pen(Brushes.Red);
        private static readonly Pen TeamBPen = new Pen(Brushes.Blue);
        private static readonly Pen ProjectilePen = new Pen(Brushes.Brown);

        private static readonly SolidBrush TeamASolidPen = new SolidBrush(Color.Red);
        private static readonly SolidBrush TeamBSolidPen = new SolidBrush(Color.Blue);

        private static IWorld World;
        private WorldsGenerator WorldGen;
        private readonly Graphics _gfx;

        private const int ImageSizeX = 1024;
        private const int ImageSizeY = 1024;

        private static readonly Image image = new Bitmap(ImageSizeX, ImageSizeY);

        public MainForm()
        {
            InitializeComponent();

            WorldGen = WorldsGenerator.GetDefault(64, 64);
            textBoxWorldX.Text = "64";
            textBoxWorldY.Text = "64";

            SetWorld(WorldGen.GetWorld());

            _gfx = Graphics.FromImage(image);
        }

        public void SetWorld(IWorld world)
        {
            World = world;
        }


        private void Render()
        {
            while (true)
            {
                if (!Paused)
                    World.DoTick();

                Task.Delay(Delay).Wait();
                var world = World;

                {
                    _gfx.Clear(Color.White);

                    DrawGrid(_gfx, world);
                    DrawUnits(_gfx, world);
                    DrawProjectiles(_gfx, world);
                }

                pictureBox1.BeginInvoke(new Action(() =>
                {
                    pictureBox1.Image = image;
                }));

                Task.Delay(Delay).Wait();
            }
        }

        private void DrawProjectiles(Graphics gfx, IWorld world)
        {
            int dX = ImageSizeX / world.Width;
            int dY = ImageSizeY / world.Length;

            foreach (var projectile in world.GetProjectiles())
            {
                gfx.DrawEllipse(ProjectilePen, projectile.X * dX, projectile.Y * dY, dX , dY);
            }
        }

        private void DrawUnits(Graphics gfx, IWorld world)
        {
            int dX = ImageSizeX / world.Width;
            int dY = ImageSizeY / world.Length;

            foreach (var unit in world.Army.GetUnits())
            {
                switch (unit.UnitType)
                {
                    case UnitType.SwordsMan:
                        gfx.DrawEllipse(unit.Team == Team.Red ? TeamAPen : TeamBPen, unit.X * dX, unit.Y * dY, dX, dY); break;
                    case UnitType.HorseMan:
                        gfx.DrawRectangle(unit.Team == Team.Red ? TeamAPen : TeamBPen, unit.X * dX, unit.Y * dY, dX, dY); break;
                    case UnitType.Archer:
                        gfx.FillEllipse(unit.Team == Team.Red ? TeamASolidPen : TeamBSolidPen, unit.X * dX, unit.Y * dY, dX, dY); break;
                }

            }
        }

        private void DrawGrid(Graphics gfx, IWorld world)
        {
            int stepX = ImageSizeX / world.Width;
            int stepY = ImageSizeY / world.Length;

            for (int x = 0; x < ImageSizeX; x += stepX)
            {
                gfx.DrawLine(Pen, x, 0, x, ImageSizeY);
            }

            for (int y = 0; y < ImageSizeY; y += stepY)
            {
                gfx.DrawLine(Pen, 0, y, ImageSizeX, y);
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            Task.Factory.StartNew(Render);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                AddUnit(_radioTeamA.Checked ? Team.Red : Team.Blue, e.X * World.Width / pictureBox1.Width, e.Y * World.Length / pictureBox1.Height);
        }

        private void AddUnit(Team team, int worldX, int worldY)
        {
            if (World.Army.GetUnit(worldY, worldX) == null)
            {
                if (radioButtonUnitSwords.Checked)
                    WorldGen.CreateSwordsman(team, worldY, worldX);
                else if (radioButtonUnitHorse.Checked)
                    WorldGen.CreateHorseman(team, worldY, worldX);
                else if (radioButtonUnitArcher.Checked)
                    WorldGen.CreateArcher(team, worldY, worldX);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGameSpeedNormal.Checked)
                Delay = DefaultDelay;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGameSpeedX2.Checked)
                Delay = DefaultDelay / 2;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonGameSpeedX4.Checked)
                Delay = DefaultDelay / 4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Paused = !Paused;

            button1.Text = Paused ? "Start" : "Pause";
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                AddUnit(_radioTeamA.Checked ? Team.Red : Team.Blue, e.X * World.Width / pictureBox1.Width, e.Y * World.Length / pictureBox1.Height);
            else
                if (e.Button == MouseButtons.Right)
                AddUnit(_radioTeamA.Checked ? Team.Blue : Team.Red, e.X * World.Width / pictureBox1.Width, e.Y * World.Length / pictureBox1.Height);
        }

        private void buttonGenerateWorld_Click(object sender, EventArgs e)
        {
            int x = Int32.Parse(textBoxWorldX.Text);
            int y = Int32.Parse(textBoxWorldY.Text);


            WorldGen = WorldsGenerator.GetDefault(y, x);
            var world = WorldGen.GetWorld();
            SetWorld(world);

        }
    }
}
