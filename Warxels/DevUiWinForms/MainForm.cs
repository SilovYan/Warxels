using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLogic;
using GameLogic.Helper;

namespace DevUiWinForms
{
    public enum DrawMode
    {
        Units, Terrain
    }
    public partial class MainForm : Form
    {
        private DrawMode _drawMode = DrawMode.Units;

        
        private bool squareSelection = false;

        private static bool Paused = true;
        private const int DefaultDelay = 100;
        private static int Delay = DefaultDelay;

        private static IWorld World;
        private WorldsGenerator WorldGen;
        private readonly RenderGDI _render = new RenderGDI();
        private bool _renderSquare;
        private Point _squareBegin;
        private Point _squareEnd;

        private bool RenderSquare
        {
            get { return _renderSquare; }
            set
            {
                _renderSquare = value;
                if (_renderSquare == false)
                    _render.DisableSquare();
            }
        }

        private Point SquareBegin
        {
            get { return _squareBegin; }
            set { _squareBegin = value;
                SetRenderSquare();
            }

        }

        private Point SquareEnd
        {
            get { return _squareEnd; }
            set
            {
                _squareEnd = value;
                SetRenderSquare();
            }
        }

        private void SetRenderSquare()
        {
            _render.SetSquare(((float)_squareBegin.X / pictureBox1.Width), ((float)_squareBegin.Y / pictureBox1.Height),
                    ((float)_squareEnd.X / pictureBox1.Width), ((float)_squareEnd.Y / pictureBox1.Height));
        }

        public MainForm()
        {
            InitializeComponent();

            WorldGen = WorldsGenerator.GetDefault(64, 64);
            textBoxWorldX.Text = "64";
            textBoxWorldY.Text = "64";

            SetWorld(WorldGen.GetWorld());

            
            _render.UpdateBackgroundImage(World);
            UpdateDrawMode();
            pictureBox1.Image = _render.GetImage();
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
                
                var world = World;

                _render.Render(World);

                pictureBox1.BeginInvoke(new Action(() =>
                {
                    pictureBox1.Invalidate();
                }));

                Task.Delay(Delay).Wait();
            }
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            Task.Factory.StartNew(Render);
        }

        private Point ControlCoordsToWorldCoords(int x, int y)
        {
            return new Point(x * World.Width / pictureBox1.Width, y * World.Length / pictureBox1.Height);
        }

        

        private void UnitDrawMouseUp(MouseEventArgs e)
        {
            if (!squareSelection)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var coords = ControlCoordsToWorldCoords(e.X, e.Y);
                    AddUnit(_radioTeamA.Checked ? Team.Red : Team.Blue, coords.X, coords.Y);
                }
            }
            else
            {
                UnitType t = UnitType.SwordsMan;
                if (radioButtonUnitSwords.Checked)
                    t = UnitType.SwordsMan;
                else if (radioButtonUnitHorse.Checked)
                    t = UnitType.HorseMan;
                if (radioButtonUnitArcher.Checked)
                    t = UnitType.Archer;

                
                var coords1 = ControlCoordsToWorldCoords(SquareBegin.X, SquareBegin.Y);
                var coords2 = ControlCoordsToWorldCoords(SquareEnd.X, SquareEnd.Y);
                int amount = int.Parse(textBoxSquareAmount.Text);
                WorldGen.AddUnitSquare(_radioTeamA.Checked ? Team.Red : Team.Blue, coords1.Y, coords1.X, coords2.X - coords1.X, coords2.Y - coords1.Y,
                    t, amount);
            }
        }

        private void TerrainDrawMouseUp(MouseEventArgs e)
        {
            if (!squareSelection)
            {
                var coords1 = ControlCoordsToWorldCoords(e.X, e.Y);
                var size = int.Parse(textBoxTerrainBrushSize.Text);
                WorldGen.SetTerrain(coords1.Y, coords1.X, coords1.Y+size, coords1.X+size, (TerrainType)comboBoxTerrain.SelectedIndex);
            }
            else
            {
                var coords1 = ControlCoordsToWorldCoords(SquareBegin.X, SquareBegin.Y);
                var coords2 = ControlCoordsToWorldCoords(SquareEnd.X, SquareEnd.Y);

                WorldGen.SetTerrain(coords1.Y, coords1.X, coords2.Y, coords2.X, (TerrainType)comboBoxTerrain.SelectedIndex);
                _render.UpdateBackgroundImage(World);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            switch (_drawMode)
            {
                case DrawMode.Units: UnitDrawMouseUp(e); break;
                case DrawMode.Terrain: TerrainDrawMouseUp(e); break;
            }

            if (squareSelection)
                RenderSquare = false;
        }

        private void AddUnit(Team team, int worldX, int worldY)
        {
            if (World.Army.GetUnit(worldY, worldX) == null)
            {
                if (radioButtonUnitSwords.Checked)
                    WorldGen.CreateUnit(UnitType.SwordsMan, team, worldY, worldX);
                if (radioButtonUnitHorse.Checked)
                    WorldGen.CreateUnit(UnitType.HorseMan, team, worldY, worldX);
                if (radioButtonUnitArcher.Checked)
                    WorldGen.CreateUnit(UnitType.Archer, team, worldY, worldX);
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
            SetPaused(!Paused);
        }

        private void SetPaused(bool paused)
        {
            Paused = paused;

            button1.Text = Paused ? "Start" : "Pause";
        }

        private void UnitDrawMouseMove(MouseEventArgs e)
        {
            if (!squareSelection)
            {
                var coords = ControlCoordsToWorldCoords(e.X, e.Y);
                if (e.Button == MouseButtons.Left)
                    AddUnit(_radioTeamA.Checked ? Team.Red : Team.Blue, coords.X, coords.Y);
                else
                    if (e.Button == MouseButtons.Right)
                    AddUnit(_radioTeamA.Checked ? Team.Blue : Team.Red, coords.X, coords.Y);
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                    SquareEnd = new Point(e.X, e.Y);
            }
        }

        private void TerrainDrawMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (!squareSelection)
            {
                var coords1 = ControlCoordsToWorldCoords(e.X, e.Y);
                var size = int.Parse(textBoxTerrainBrushSize.Text);
                WorldGen.SetTerrain(coords1.Y, coords1.X, coords1.Y + size, coords1.X + size, (TerrainType)comboBoxTerrain.SelectedIndex);
                _render.UpdateBackgroundImage(World);
            }
            else
            {
                SquareEnd = new Point(e.X, e.Y);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            switch (_drawMode)
            {
                case DrawMode.Units: UnitDrawMouseMove(e);break;
                case DrawMode.Terrain: TerrainDrawMouseMove(e);break;
            }
        }

        private void buttonGenerateWorld_Click(object sender, EventArgs e)
        {
            int x = Int32.Parse(textBoxWorldX.Text);
            int y = Int32.Parse(textBoxWorldY.Text);
            
            WorldGen = WorldsGenerator.GetDefault(y, x);
            
            var world = WorldGen.GetWorld();
            
            SetWorld(world);
            _render.UpdateBackgroundImage(world);

            SetPaused(true);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (squareSelection)
            {
                SquareBegin = SquareEnd = new Point(e.X, e.Y);
                RenderSquare = true;
            }
        }

        private void radioButtonX8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonX8.Checked)
                Delay = DefaultDelay / 8;
        }

        private void tabControlUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDrawMode();
        }

        private void UpdateDrawMode()
        {
            switch (tabControlUnits.SelectedIndex)
            {
                case 0: _drawMode = DrawMode.Units; break;
                case 1: _drawMode = DrawMode.Terrain; break;
            }

            CheckRenderSquare();
        }

        private void radioButtonTerrainBrush_CheckedChanged(object sender, EventArgs e)
        {
            CheckRenderSquare();
        }

        private void radioButtonTerrainSquare_CheckedChanged(object sender, EventArgs e)
        {
            CheckRenderSquare();
        }

        private void CheckRenderSquare()
        {
            squareSelection = radioButtonTerrainSquare.Checked && _drawMode == DrawMode.Terrain
                || radioButtonSquare.Checked && _drawMode == DrawMode.Units;
        }

        private void radioButtonSquare_CheckedChanged(object sender, EventArgs e)
        {
            CheckRenderSquare();
        }

        private void buttonUnitsSave_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "*.units|*.units|*.*|*.*";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                World.SaveUnits(dialog.FileName);
            }
        }

        private void buttonTerrainLoad_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "*.terr|*.terr|*.*|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WorldGen = SaveLoadHelper.LoadTerrainFromFile(dialog.FileName);
                SetWorld(WorldGen.GetWorld());
            }
        }
        
        
        private void buttonTerrainSave_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "*.terr|*.terr|*.*|*.*";
            dialog.AddExtension = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                World.SaveTerrain(dialog.FileName);
            }
        }

        private void buttonUnitsLoad_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "*.units|*.units|*.*|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
                WorldGen.LoadUnitsFromFile(dialog.FileName);
        }

        private void plotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                @"Первые лучи рассветного солнца нежно скользнули по верхушкам монументальных гор в долине и поползли вниз по отвесным скалам и стволам деревьев, снимая с них сумеречную вуаль и открывая их миру. Лучи солнца освещали эти древние земли, секунда за секундой открывая больше деталей. Вот бурлящие воды быстрой горной реки, пересекающей долину. Вот оживленная заводь, прибрежная топь и виднеется камыш. Вот иссеченные латы, заляпанные кровью мечи и угрюмые взгляды, пронзающие рассветную дымку.
И они узрели. Рассветное небо посерело от урагана стрел, обрушившегося на первые ряды пехотинцев. Лишь утерев кровь павших товарищей с лиц, воины смогли увидеть монолитную стену врагов, надвигающихся на них с противоположной стороны реки. Лица их были перекошены, а синие флаги остервенело трепетали на ветру.
— Вперед! - боевой клич вознесся над полем брани и предопределил судьбу чужеземцев. Кони рванулись в бой под алыми, как этот роковой рассвет, флагами великого королевства, лучники натянули тетиву и пехотинцы загремели, смертоносной стеной двигаясь навстречу вечности.");
        }

        private void buttonAddPreset_Click(object sender, EventArgs e)
        {
            WorldGen.AddPresetUnits();
        }
    }
}
