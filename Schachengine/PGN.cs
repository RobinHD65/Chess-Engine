using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schachengine
{

    static class PGN
    {
        static Suche instance = new Suche();
        public static string TypeToString(int type)
        {
            switch (type)
            {
                case 1:
                    return "";
                case 2:
                    return "N";
                case 3:
                    return "B";
                case 4:
                    return "R";
                case 5:
                    return "Q";
                case 6:
                    return "K";
                default:
                    return "";
            }
        }
        public static string AddMove(Zug zug, int i)
        {
            string pgn = "";
            switch (zug.moveType)
            {
                case MoveType.RochadeKS:
                    if (i % 2 == 0)
                    {
                        pgn += (1 + i / 2).ToString() + ". ";
                    }
                    pgn += "O-O ";
                    if (i % 2 == 1)
                    {
                        pgn += "\n";
                    }
                    i++;
                    instance.MakeMove(zug);
                    return pgn;
                case MoveType.RochadeQS:
                    if (i % 2 == 0)
                    {
                        pgn += (1 + i / 2).ToString() + ". ";
                    }
                    pgn += "O-O-O ";
                    if (i % 2 == 1)
                    {
                        pgn += "\n";
                    }
                    i++;
                    instance.MakeMove(zug);
                    return pgn;

            }
            if (i % 2 == 0)
            {
                pgn += (1 + i / 2).ToString() + ". ";
            }
            //Schreibe Figurtyp wenn kein Bauer
            if (instance.Figuren[zug.Startfeld].Type != 1)
            {
                pgn += TypeToString(instance.Figuren[zug.Startfeld].Type);
                pgn += IndexToSquare(zug.Startfeld);
            }
                
            //Schlagzüge
            if (instance.Figuren[zug.Zielfeld].ID != 0 || zug.moveType == MoveType.Enpassant)
            {
                if (instance.Figuren[zug.Startfeld].Type == 1)
                    pgn += IndexToSquare(zug.Startfeld)[0];
                pgn += "x";
            }
            // Schreibe Zielfeld
            pgn += IndexToSquare(zug.Zielfeld);
            //Bauerumwandlung
            if (zug.Promotion != 0)
            {
                pgn += TypeToString(zug.Promotion + 1);
            }
            //Schach

            pgn += " ";      
            //Zeilenumbruch nach jedem zweiten Zug
            if (i % 2 == 1)
            {
                pgn += "\n";
            }
            i++;
            instance.MakeMove(zug);
            return pgn;
        }
        public static string ToPGN(List<Zug> moves, string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") // fen = .. ist default-Wert
        {
            instance.FenParser(fen);
            int i = 0;
            string pgn = "";
            foreach (Zug zug in moves)
            {
                pgn += AddMove(zug, i);
                i++;
            }
            return pgn;
        }

        public static int SquareToIndex(string Square)
        {
            int Index = 0;
            switch (Square[0])
            {
                case 'a':
                    Index = 0;
                    break;
                case 'b':
                    Index = 1;
                    break;
                case 'c':
                    Index = 2;
                    break;
                case 'd':
                    Index = 3;
                    break;
                case 'e':
                    Index = 4;
                    break;
                case 'f':
                    Index = 5;
                    break;
                case 'g':
                    Index = 6;
                    break;
                case 'h':
                    Index = 7;
                    break;

            }
            Index += 8 * ((int)char.GetNumericValue(Square[1]) - 1);
            MessageBox.Show(Index.ToString());
            return Index;
        }
        public static string IndexToSquare(int Index)
        {
            string Square = "";
            switch (Index % 8)
            {
                case 0:
                    Square = "a";
                    break;
                case 1:
                    Square = "b";
                    break;
                case 2:
                    Square = "c";
                    break;
                case 3:
                    Square = "d";
                    break;
                case 4:
                    Square = "e";
                    break;
                case 5:
                    Square = "f";
                    break;
                case 6:
                    Square = "g";
                    break;
                case 7:
                    Square = "h";
                    break;
            }
            Square = Square + (1 + Index / 8).ToString();
            return Square;
        }
    }
}
