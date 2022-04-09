using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamiteDexter
{
    public class CharacterSet
    {
        #region Decorative Colors and Images
        public Color BackgroundColor = Color.Transparent;
        public Texture2D BackgroundImage = null;
        public Texture2D BorderCornerImage = null;
        public Texture2D BorderHorizontalImage = null;
        public Texture2D BorderVerticalImage = null;
        #endregion

        #region Settings
        public int FontWidth = 20;
        public int FontHeight = 20;
        public int Margin = 30;
        #endregion

        #region Lowercase Letters
        public Texture2D a = null;
        public Texture2D b = null;
        public Texture2D c = null;
        public Texture2D d = null;
        public Texture2D e = null;
        public Texture2D f = null;
        public Texture2D g = null;
        public Texture2D h = null;
        public Texture2D i = null;
        public Texture2D j = null;
        public Texture2D k = null;
        public Texture2D l = null;
        public Texture2D m = null;
        public Texture2D n = null;
        public Texture2D o = null;
        public Texture2D p = null;
        public Texture2D q = null;
        public Texture2D r = null;
        public Texture2D s = null;
        public Texture2D t = null;
        public Texture2D u = null;
        public Texture2D v = null;
        public Texture2D w = null;
        public Texture2D x = null;
        public Texture2D y = null;
        public Texture2D z = null;
        #endregion

        #region Uppercase Letters
        public Texture2D A = null;
        public Texture2D B = null;
        public Texture2D C = null;
        public Texture2D D = null;
        public Texture2D E = null;
        public Texture2D F = null;
        public Texture2D G = null;
        public Texture2D H = null;
        public Texture2D I = null;
        public Texture2D J = null;
        public Texture2D K = null;
        public Texture2D L = null;
        public Texture2D M = null;
        public Texture2D N = null;
        public Texture2D O = null;
        public Texture2D P = null;
        public Texture2D Q = null;
        public Texture2D R = null;
        public Texture2D S = null;
        public Texture2D T = null;
        public Texture2D U = null;
        public Texture2D V = null;
        public Texture2D W = null;
        public Texture2D X = null;
        public Texture2D Y = null;
        public Texture2D Z = null;
        #endregion

        #region Punctuation
        public Texture2D Period = null;
        public Texture2D Exclamation = null;
        public Texture2D Question = null;
        public Texture2D Comma = null;
        public Texture2D Colon = null;
        public Texture2D Semicolon = null;
        public Texture2D Space = null;
        #endregion

        #region Paired Symbols
        public Texture2D ParenthesisLeft = null;
        public Texture2D ParenthesisRight = null;
        public Texture2D BracketLeft = null;
        public Texture2D BracketRight = null;
        public Texture2D BraceLeft = null;
        public Texture2D BraceRight = null;
        public Texture2D QuoteSingle = null;
        public Texture2D QuoteDouble = null;
        #endregion

        #region Numerals
        public Texture2D Zero = null;
        public Texture2D One = null;
        public Texture2D Two = null;
        public Texture2D Three = null;
        public Texture2D Four = null;
        public Texture2D Five = null;
        public Texture2D Six = null;
        public Texture2D Seven = null;
        public Texture2D Eight = null;
        public Texture2D Nine = null;
        #endregion

        #region Mathematical Symbols
        public Texture2D Plus = null;
        public Texture2D Minus = null;
        public Texture2D Equality = null;
        public Texture2D LessThan = null;
        public Texture2D GreaterThan = null;
        #endregion

        #region Miscellaneous Symbols
        public Texture2D SlashBack = null;
        public Texture2D SlashForward = null;
        public Texture2D Bar = null;
        public Texture2D Underscore = null;
        public Texture2D Tilde = null;
        public Texture2D ObscureThingy = null;
        public Texture2D At = null;
        public Texture2D Pound = null;
        public Texture2D Dollars = null;
        public Texture2D Percent = null;
        public Texture2D Carot = null;
        public Texture2D Ampersand = null;
        public Texture2D Asterisk = null;
        #endregion

        public CharacterSet()
        {
            if (a != null)
            {
                FontHeight = a.Width;
                FontHeight = a.Height;
                Margin = a.Width * 2;
            }
        }

        public CharacterSet(int fontWidth, int fontHeight)
        {
            FontWidth = fontWidth;
            FontHeight = fontHeight;
            Margin = fontWidth * 2;
        }

        public CharacterSet(int fontWidth, int fontHeight, int margin)
        {
            FontWidth = fontWidth;
            FontHeight = fontHeight;
            Margin = margin;
        }

        public Texture2D GetCharacterImage(char ch)
        {
            switch (ch)
            {
                #region Lowercase Letters
                case 'a': return a;
                case 'b': return b;
                case 'c': return c;
                case 'd': return d;
                case 'e': return e;
                case 'f': return f;
                case 'g': return g;
                case 'h': return h;
                case 'i': return i;
                case 'j': return j;
                case 'k': return k;
                case 'l': return l;
                case 'm': return m;
                case 'n': return n;
                case 'o': return o;
                case 'p': return p;
                case 'q': return q;
                case 'r': return r;
                case 's': return s;
                case 't': return t;
                case 'u': return u;
                case 'v': return v;
                case 'w': return w;
                case 'x': return x;
                case 'y': return y;
                case 'z': return z;
                #endregion

                #region Uppercase Letters
                case 'A': return A;
                case 'B': return B;
                case 'C': return C;
                case 'D': return D;
                case 'E': return E;
                case 'F': return F;
                case 'G': return G;
                case 'H': return H;
                case 'I': return I;
                case 'J': return J;
                case 'K': return K;
                case 'L': return L;
                case 'M': return M;
                case 'N': return N;
                case 'O': return O;
                case 'P': return P;
                case 'Q': return Q;
                case 'R': return R;
                case 'S': return S;
                case 'T': return T;
                case 'U': return U;
                case 'V': return V;
                case 'W': return W;
                case 'X': return X;
                case 'Y': return Y;
                case 'Z': return Z;
                #endregion

                #region Punctuation
                case '.': return Period;
                case '!': return Exclamation;
                case '?': return Question;
                case ',': return Comma;
                case ':': return Colon;
                case ';': return Semicolon;
                case ' ': return Space;
                #endregion

                #region Paired Symbols
                case '(': return ParenthesisLeft;
                case ')': return ParenthesisRight;
                case '[': return BracketLeft;
                case ']': return BracketRight;
                case '{': return BraceLeft;
                case '}': return BraceRight;
                case '\'': return QuoteSingle;
                case '\"': return QuoteDouble;
                #endregion

                #region Numerals
                case '0': return Zero;
                case '1': return One;
                case '2': return Two;
                case '3': return Three;
                case '4': return Four;
                case '5': return Five;
                case '6': return Six;
                case '7': return Seven;
                case '8': return Eight;
                case '9': return Nine;
                #endregion

                #region Mathematical Symbols
                case '+': return Plus;
                case '-': return Minus;
                case '=': return Equality;
                case '<': return LessThan;
                case '>': return GreaterThan;
                #endregion

                #region Miscellaneous Symbols
                case '\\': return SlashBack;
                case '/': return SlashForward;
                case '|': return Bar;
                case '_': return Underscore;
                case '~': return Tilde;
                case '`': return ObscureThingy;
                case '@': return At;
                case '#': return Pound;
                case '$': return Dollars;
                case '%': return Percent;
                case '^': return Carot;
                case '&': return Ampersand;
                case '*': return Asterisk;
                #endregion

                default: return null;
            }
        }
    }
}