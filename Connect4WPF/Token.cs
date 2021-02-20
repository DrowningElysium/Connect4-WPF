using System.Windows.Media;

namespace Connect4WPF
{
    class Token
    {
        private Color color;
        private int x;
        private int y;

        public Token(int x, int y)
        {
            this.color = Colors.White;
            this.x = x;
            this.y = y;
        }

        public Color GetColor()
        {
            return this.color;
        }

        public void SetColor(Color color)
        {
            this.color = color;
        }

        public int GetX()
        {
            return this.x;
        }

        public int GetY()
        {
            return this.y;
        }

        public bool IsSet()
        {
            return this.color != Colors.White;
        }
    }
}
