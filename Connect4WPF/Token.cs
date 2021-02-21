using System;
using System.Windows.Media;

namespace Connect4WPF
{
    class Token : ICloneable
    {
        private Color _color;
        private int _x;
        private int _y;

        public Token(int x, int y)
        {
            this._color = Colors.White;
            this._x = x;
            this._y = y;
        }

        public Color GetColor()
        {
            return this._color;
        }

        public void SetColor(Color color)
        {
            this._color = color;
        }

        public int GetX()
        {
            return this._x;
        }

        public int GetY()
        {
            return this._y;
        }

        public bool IsSet()
        {
            return this._color != Colors.White;
        }

        public object Clone()
        {
            Token copy = new Token(this._x, this._y);
            copy.SetColor(this._color);
            return copy;
        }
    }
}
