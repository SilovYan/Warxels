using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using GameLogic;

namespace DevUiAndroidV2
{
    internal sealed class MyView : View
    {
        public const int Size = 64;
        private int _step;
        public GenerateArmy Army { get; }
        private readonly Paint _paint;
        private readonly Paint _focusPaint;
        private readonly Paint _borderPaint;
        private Tuple<Rect, ISquad> _focus;
        private readonly List<Tuple<Rect, ISquad>> _lists = new List<Tuple<Rect, ISquad>>();
        public MyView(Context context) : base(context)
        {
            SetPadding(0, 0, 0, 0);
            _paint = new Paint
            {
                Color = Color.Red
            };
            _paint.SetStyle(Paint.Style.Fill);
            _borderPaint = new Paint(_paint) {Color = Color.Cyan};
            _focusPaint = new Paint(_paint) {Color = Color.Gray};
            Army = new GenerateArmy();
        }
        
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        public override void Draw(Canvas canvas)
        {
            canvas.DrawLine(0, Bottom / 2, Right, Bottom / 2, _borderPaint);
            foreach(var t in _lists)
            {
                canvas.DrawRect(t.Item1, Equals(t, _focus) ? _focusPaint : _paint);
            }
        }

        public void TapTap(float x, float y, int rows, int ranks, UnitType type)
        {
            _step = Width / Size;
            var location = new Point((int)x, (int)y);
            if (_focus == null)
            {
                _focus = _lists.FirstOrDefault(val => location.X < val.Item2.MaxX * _step && location.X > val.Item2.MinX * _step
                                               && location.Y < val.Item2.MaxY * _step && location.Y > val.Item2.MinY * _step);
                if (_focus == null && rows != 0 && ranks!=0)
                {

                    var rectsquad = new RectSquad(rows, ranks, type, Team.Red, location.X/_step, location.Y/_step);
                    var isAdded = Army.AddSquad(rectsquad);
                    if (isAdded)
                    {
                        var a = new Rect(rectsquad.MinX * _step, rectsquad.MinY * _step,
                            rectsquad.MaxX * _step, rectsquad.MaxY * _step);

                        _lists.Add(new Tuple<Rect, ISquad>(a, rectsquad));
                    }
                }
            }
            else
            {
                if(_focus.Item2.CheckAndSetPos(Army, location.X / _step, location.Y / _step))
                {
                    var rectsquad = _focus.Item2;
                    _focus.Item1.Set(new Rect(rectsquad.MinX * _step, rectsquad.MinY * _step,
                        rectsquad.MaxX * _step, rectsquad.MaxY * _step));
                }
                _focus = null;
            }
            Invalidate();
        }

        public void SetLoadedWorld(IWorld world)
        {
            _step = Width / Size;
            foreach (var unit in world.Army.GetUnits())
            {
                _lists.Add(new Tuple<Rect, ISquad>(
                    new Rect(unit.X * _step, (unit.Y - Size / 2) * _step, (unit.X + 1) * _step,
                        (unit.Y + 1 - Size / 2) * _step), null));
            }
            Invalidate();
        }
    }
}