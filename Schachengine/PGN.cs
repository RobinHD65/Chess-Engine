using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schachengine
{
    static class PGN
    {
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
        public static string ToPGN(List<Zug> moves)
        {
            int i = 0;
            string pgn = "";
            foreach (Zug zug in moves)
            {
                bool isCapture = false;
                switch(zug.moveType)
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
                        continue;
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
                        continue;
                    case MoveType.Enpassant:
                        isCapture = true;
                        break;

                }
                if (Brett.history[i].Figuren[zug.Zielfeld].Type != 0)
                {
                    isCapture = true;
                }
                if (i % 2 == 0)
                {
                    pgn += (1 + i / 2).ToString() + ". ";
                }
                string movedPiece = TypeToString(Brett.history[i].Figuren[zug.Startfeld].Type);
                pgn += movedPiece;
                if (isCapture)
                {
                    if (movedPiece == "") // wenn Bauer schlägt
                    {
                        pgn += IndexToSquare(zug.Startfeld)[0];
                    }
                    pgn += "x";
                }
                pgn += IndexToSquare(zug.Zielfeld);
                //Bauerumwandlung
                if (zug.Promotion != 0)
                {
                    pgn += TypeToString(zug.Promotion + 1);
                }
                //Mach ein Plus wenn Schach
                if (isCheck(Brett.history[i], zug, movedPiece, i))
                {
                    pgn += "+";
                }
                pgn += " ";
                //Zeilenumbruch nach jedem zweiten Zug
                if (i % 2 == 1)
                {
                    pgn += "\n";
                }
                i++;
            }
            return pgn;
        }
        public static bool isCheck(Brett brett, Zug zug, string movedPiece, int i) 
        {
            Brett tempBrett = brett.Copy();
            tempBrett.MakeMove(zug);
            tempBrett.weißamzug = i % 2 == 1;
            tempBrett.UpdateKingPos();
            if (tempBrett.SquareInCheck(tempBrett.PosKönig))
            {
                tempBrett.UnmakeMove();
                return true;
            }
            tempBrett.UnmakeMove();
            return false;
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
