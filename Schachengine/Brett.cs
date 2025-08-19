using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schachengine 
{
    class Brett
    {
        //GameState
        public Figur[] Figuren = new Figur[64]; //8x8 Brett
        public bool weißamzug;
        public int fzregel;
        public int zugnr;
        public bool[] Rochaderechte = new bool[4];
        public string enpassant;
        public int enpassant_Index;
        public bool Endspiel = false;

        //Position des Königs
        public int whiteKingPos;
        public int blackKingPos;
        public int PosKönig; //König von dem der am Zug ist

        //Abstände zum Rand vom jeweiligen Figur
        public static int[] AbstandNord = new int[64];
        public static int[] AbstandOst = new int[64];
        public static int[] AbstandSüd = new int[64];
        public static int[] AbstandWest = new int[64];

        public static List<Brett> history = new List<Brett>(); 

        //Piece-Lists
        public List<int> whitePawns = new();
        public List<int> whiteKnights = new();
        public List<int> whiteBishops = new();
        public List<int> whiteRooks = new();
        public List<int> whiteQueens = new();

        public List<int> blackPawns = new();
        public List<int> blackKnights = new();
        public List<int> blackBishops = new();
        public List<int> blackRooks = new();
        public List<int> blackQueens = new();
        private Brett Copy()
        {
            Brett copy = new Brett();
            for (int i = 0; i <= 63; i++)
            {
                copy.Figuren[i] = Figuren[i];
            }
            copy.fzregel = fzregel;
            copy.zugnr = zugnr;
            for (int i = 0; i <= 3; i++)
            {
                copy.Rochaderechte[i] = Rochaderechte[i];
            }
            copy.enpassant = enpassant;
            copy.enpassant_Index = enpassant_Index;
            copy.whiteKingPos = whiteKingPos;
            copy.blackKingPos = blackKingPos;
            copy.PosKönig = PosKönig;
            copy.Endspiel = Endspiel;
            return copy;
        }
        public void MakeMove(Zug zug)
        {
            history.Add(Copy()); // Save the current state before making the move
            //Fünfzig Züge-Regel
            if (Figuren[zug.Startfeld].Art == 9 || Figuren[zug.Startfeld].Art == 17 || Figuren[zug.Zielfeld].Farbe != "leer")
            {
                fzregel = 0;
            } else
            {
                fzregel++;
            }
            Figur[] tempBrett = new Figur[64];
            bool[] tempRochade = new bool[4];
            for (int i = 0; i<=63; i++)
            {
                tempBrett[i] = Figuren[i];
            }
            for (int i = 0; i<=3; i++)
            {
                tempRochade[i] = Rochaderechte[i];
            }
            //Rochaderechte-Entzug
            if (zug.Startfeld == 4)
            {
                Rochaderechte[0] = false;
                Rochaderechte[1] = false;
            }
            if (zug.Startfeld == 60)
            {
                Rochaderechte[2] = false;
                Rochaderechte[3] = false;
            }
            if ((zug.Startfeld == 0) | (zug.Zielfeld == 0)) Rochaderechte[1] = false;
            if ((zug.Startfeld == 7) | (zug.Zielfeld == 7)) Rochaderechte[0] = false;
            if ((zug.Startfeld == 56) | (zug.Zielfeld == 56)) Rochaderechte[3] = false;
            if ((zug.Startfeld == 63) | (zug.Zielfeld == 63)) Rochaderechte[2] = false;   
            //Figuren auf ein Feld ziehen
            Figuren[zug.Zielfeld] = Figuren[zug.Startfeld];
            Figuren[zug.Startfeld] = new Figur(0);
            //en passant Felder und besondere Züge
            enpassant_Index = -1;
            if (weißamzug)
            {
                if (zug.moveType == MoveType.BauerDoppelzug)
                    enpassant_Index = zug.Zielfeld - 8;    
                if (zug.moveType == MoveType.Enpassant)
                    Figuren[zug.Zielfeld - 8] = new Figur(0);
                if (zug.moveType == MoveType.RochadeKS)
                {
                    Figuren[7] = new Figur(0);
                    Figuren[5] = new Figur(12);
                }
                if (zug.moveType == MoveType.RochadeQS)
                {
                    Figuren[0] = new Figur(0);
                    Figuren[3] = new Figur(12);
                }
                //Bauernumwandlung
                if ((zug.Zielfeld / 8 == 7) & (Figuren[zug.Zielfeld].Art == 9)) 
                {
                    if (zug.Promotion == 1) Figuren[zug.Zielfeld] = new Figur(10);
                    if (zug.Promotion == 2) Figuren[zug.Zielfeld] = new Figur(11);
                    if (zug.Promotion == 3) Figuren[zug.Zielfeld] = new Figur(12);
                    if (zug.Promotion == 4) Figuren[zug.Zielfeld] = new Figur(13);
                }
                //Update der Königsposition
                if (Figuren[zug.Zielfeld].Art == 14)
                    whiteKingPos = zug.Zielfeld;
            }
            else
            {
                zugnr += 1;
                if (zug.moveType == MoveType.BauerDoppelzug)
                    enpassant_Index = zug.Zielfeld + 8; 
                
                if (zug.moveType == MoveType.Enpassant)
                    Figuren[zug.Zielfeld + 8] = new Figur(0);
                if (zug.moveType == MoveType.RochadeKS)
                {
                    Figuren[63] = new Figur(0);
                    Figuren[61] = new Figur(20);
                }
                if (zug.moveType == MoveType.RochadeQS)
                {
                    Figuren[56] = new Figur(0);
                    Figuren[59] = new Figur(20);
                }
                //Bauernumwandlung
                if ((zug.Zielfeld / 8 == 0) & (Figuren[zug.Zielfeld].Art == 17))
                {
                    if (zug.Promotion == 1) Figuren[zug.Zielfeld] = new Figur(18);
                    if (zug.Promotion == 2) Figuren[zug.Zielfeld] = new Figur(19);
                    if (zug.Promotion == 3) Figuren[zug.Zielfeld] = new Figur(20);
                    if (zug.Promotion == 4) Figuren[zug.Zielfeld] = new Figur(21);
                }
                //Update der Königsposition
                if (Figuren[zug.Zielfeld].Art == 22)
                    blackKingPos = zug.Zielfeld;
            }
            UpdateKingPos();
            weißamzug = !weißamzug; // Zugwechsel
        }
        public void UnmakeMove()
        {
            if(zugnr == 1 && weißamzug) 
                return; // Keine Züge zum Rückgängigmachen
            Figuren = history[history.Count - 1].Figuren;          
            fzregel = history[history.Count - 1].fzregel;
            zugnr = history[history.Count - 1].zugnr;
            Rochaderechte = history[history.Count - 1].Rochaderechte;
            enpassant = history[history.Count - 1].enpassant;
            enpassant_Index = history[history.Count - 1].enpassant_Index;
            blackKingPos = history[history.Count - 1].blackKingPos;
            whiteKingPos = history[history.Count - 1].whiteKingPos;
            PosKönig = history[history.Count - 1].PosKönig;
            weißamzug = !weißamzug;
            Endspiel = history[history.Count - 1].Endspiel;
            history.RemoveAt(history.Count - 1); // Letzten Zustand entfernen
        }
		
        public void Clear()
        {
            for (int i = 0; i <= 63; i++) Figuren[i] = new Figur(0);
            fzregel = 0;
            zugnr = 0;
            enpassant = "";
            enpassant_Index = -1;
            for (int i = 0; i<=3; i++)
            {
                Rochaderechte[i] = false;
            }
        }
        public void FenParser(string FEN)
        {
            FEN += ' ';
            Clear();
            int i = 56;
            int group = 1;
            string fzregel_digits = "0";
            string zugnr_digits = "0";
            foreach (char zeichen in FEN)
            {
                //Zugnummer
                if (group == 6)
                {
                    if (zeichen == ' ')
                    {
                        zugnr = Int32.Parse(zugnr_digits);
                    }
                    else
                    {
                        if (zeichen != '-') zugnr_digits += zeichen;
                    }
                }
                //Fünfzig-Züge-Regel
                if (group == 5)
                {
                    if (zeichen == ' ')
                    {
                        fzregel = Int32.Parse(fzregel_digits);
                        group = 6;
                    }
                    else
                    {
                        if (zeichen != '-') fzregel_digits += zeichen;
                    }
                }
                //En passant
                if (group == 4)
                {
                    if (zeichen == ' ')
                    {
                        if (enpassant != "none") enpassant_Index = SquareToIndex(enpassant);
                        group = 5;
                    }
                    else if (zeichen == '-')
                        enpassant = "none";
                    else
                    {
                        enpassant += zeichen;
                    }
                }
                //Rochaderecht
                if (group == 3)
                {
                    switch (zeichen)
                    {
                        case ' ':
                            group = 4;
                            break;
                        case 'K':
                            Rochaderechte[0] = true;
                            break;
                        case 'Q':
                            Rochaderechte[1] = true;
                            break;
                        case 'k':
                            Rochaderechte[2] = true;
                            break;
                        case 'q':
                            Rochaderechte[3] = true;
                            break;

                    }
                }
                //Zugrecht
                if (group == 2)
                {
                    switch (zeichen)
                    {
                        case 'w':
                            weißamzug = true;
                            break;
                        case 'b':
                            weißamzug = false;
                            break;
                        case ' ':
                            group++;
                            break;
                    }
                }
                if (group == 1)
                {
                    switch (zeichen)
                    {
                        //Schwarz
                        case 'r':
                            Figuren[i] = new Figur(20);
                            blackRooks.Add(i);
                            break;
                        case 'n':
                            Figuren[i] = new Figur(18);
                            blackKnights.Add(i);
                            break;
                        case 'b':   
                            Figuren[i] = new Figur(19);
                            blackBishops.Add(i);
                            break;
                        case 'q':
                            Figuren[i] = new Figur(21);
                            blackQueens.Add(i);
                            break;
                        case 'k':
                            Figuren[i] = new Figur(22);
                            blackKingPos = i;
                            break;
                        case 'p':
                            Figuren[i] = new Figur(17);
                            blackPawns.Add(i);
                            break;
                        //Weiß
                        case 'R':
                            Figuren[i] = new Figur(12);
                            whiteRooks.Add(i);
                            break;
                        case 'N':
                            Figuren[i] = new Figur(10);
                            whiteKnights.Add(i);
                            break;
                        case 'B':
                            Figuren[i] = new Figur(11);
                            whiteBishops.Add(i);
                            break;
                        case 'Q':
                            Figuren[i] = new Figur(13);
                            whiteQueens.Add(i);
                            break;
                        case 'K':
                            Figuren[i] = new Figur(14);
                            whiteKingPos = i;
                            break;
                        case 'P':
                            Figuren[i] = new Figur(9);
                            whitePawns.Add(i);
                            break;
                        //Nummern
                        case '/':
                            i -= 17;
                            break;
                        case '1':
                            i += 0;
                            break;
                        case '2':
                            i += 1;
                            break;
                        case '3':
                            i += 2;
                            break;
                        case '4':
                            i += 3;
                            break;
                        case '5':
                            i += 4;
                            break;
                        case '6':
                            i += 5;
                            break;
                        case '7':
                            i += 6;
                            break;
                        case '8':
                            i += 7;
                            break;
                        case ' ':
                            group = 2;
                            break;
                    }
                    i++;
                } 
            }          
        }
        public void Berechne_Randabstand()
        {
            for (int i = 0; i<=63; i++)
            {
                AbstandNord[i] = 7 - i / 8;
                AbstandOst[i] = 7 - i % 8;
                AbstandSüd[i] = 7 - AbstandNord[i];
                AbstandWest[i] = 7 - AbstandOst[i];
            }
        }
        public int SquareToIndex(string Square)
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
        public string IndexToSquare(int Index)
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
        public void UpdateKingPos()
        {
            if (weißamzug)
            {
                PosKönig = whiteKingPos;
            }
            else
            {
                PosKönig = blackKingPos;
            }
        }
    }
    public struct Figur
    {
        public string Farbe;
        public int Art;
        public int Wert;
        public int Type;
        
        public Figur(int _Art)
        {
            int nocArt = _Art;
            Art = _Art;
            if (Art < 16)
            {
                Farbe = "Weiß";
                nocArt -= 8;
            }
            else
            {
                Farbe = "Schwarz";
                nocArt -= 16;
            }
            if (Art == 0)
            {
                Farbe = "leer";
            }
            //Figurenwerte nach Hans Berliner
            switch (nocArt)
            {
                case 1: //Bauer
                    Wert = 100;
                    break;
                case 2: //Springer
                    Wert = 320;
                    break;
                case 3:
                    Wert = 333; //Läufer
                    break;
                case 4:
                    Wert = 510; //Turm
                    break;
                case 5:
                    Wert = 880; //Dame
                    break;
                default:
                    Wert = 0;
                    break;
            }
            Type = nocArt;
        }
    }
}
