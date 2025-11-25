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
        public int Rochaderechte;        
        public string enpassant;
        public int enpassant_Index;
        public bool Endspiel = false;
        public ulong zobrist_key;

        //Rochaderechte-Konstanten
        const int whiteKS = 0b0001;
        const int whiteQS = 0b0010;
        const int blackKS = 0b0100;
        const int blackQS = 0b1000;

        //Position des Königs
        public int whiteKingPos;
        public int blackKingPos;
        public int PosKönig; //König von dem der am Zug ist

        //Abstände zum Rand vom jeweiligen Figur
        public static int[] AbstandNord = new int[64];
        public static int[] AbstandOst = new int[64];
        public static int[] AbstandSüd = new int[64];
        public static int[] AbstandWest = new int[64];
        public static int[] AbstandNO = new int[64];
        public static int[] AbstandNW = new int[64];
        public static int[] AbstandSO = new int[64];
        public static int[] AbstandSW = new int[64];

        public static List<Brett> history = new List<Brett>();
        public static List<ulong> zobrist_history = new List<ulong>();
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
        public Brett Copy()
        {
            Brett copy = new Brett();
            for (int i = 0; i <= 63; i++)
            {
                copy.Figuren[i] = Figuren[i];
            }
            copy.fzregel = fzregel;
            copy.zugnr = zugnr;
            copy.Rochaderechte = Rochaderechte;
            copy.enpassant = enpassant;
            copy.enpassant_Index = enpassant_Index;
            copy.whiteKingPos = whiteKingPos;
            copy.blackKingPos = blackKingPos;
            copy.PosKönig = PosKönig;
            copy.Endspiel = Endspiel;
            return copy;
        }
        public string ColorToMove()
        {
            if (weißamzug)
                return "Weiß";
            else
                return "Schwarz";
        }
        public bool hasCastlingRight(int Index) // 0: whiteKS 1: whiteQS 2:blackKS 3:blackQS 
        {
            switch (Index)
            {
                case 0: //whiteKS                   
                    return (Rochaderechte & whiteKS) != 0;
                case 1: //whiteQS
                    return (Rochaderechte & whiteQS) != 0;
                case 2: //blackKS
                    return (Rochaderechte & blackKS) != 0;
                case 3: //blackQS
                    return (Rochaderechte & blackQS) != 0;
            }
            return false;
        }
        public void MakeMove(Zug zug)
        {
            history.Add(Copy()); // Save the current state before making the move
            zobrist_history.Add(zobrist_key);
            //Fünfzig Züge-Regel
            if (Figuren[zug.Startfeld].Art == 1 || Figuren[zug.Startfeld].Art == 7 || Figuren[zug.Zielfeld].Farbe != "leer")
            {
                fzregel = 0;
            } else
            {
                fzregel++;
            }
            //Rochaderechte-Entzug
            //Königszüge
            if (zug.Startfeld == 4)
            {
                if (hasCastlingRight(0))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= whiteKS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
                if (hasCastlingRight(1))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= whiteQS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            if (zug.Startfeld == 60)
            {
                if (hasCastlingRight(2))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= blackKS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
                if (hasCastlingRight(3))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= blackQS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            //Turmzüge oder Turm wird geschlagen
            if ((zug.Startfeld == 0) || (zug.Zielfeld == 0)) //a1
            {
                if (hasCastlingRight(1))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= whiteQS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            if ((zug.Startfeld == 7) || (zug.Zielfeld == 7)) //h1
            {
                if (hasCastlingRight(0))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= whiteKS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            if ((zug.Startfeld == 56) || (zug.Zielfeld == 56)) //a8
            {
                if (hasCastlingRight(3))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= blackQS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            if ((zug.Startfeld == 63) || (zug.Zielfeld == 63)) //h8
            {
                if (hasCastlingRight(2))
                {
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                    Rochaderechte -= blackKS;
                    zobrist_key ^= Zobrist.castlingrights[Rochaderechte];
                }
            }
            //Figuren auf ein Feld ziehen
            int gezogeneFigur = Figuren[zug.Startfeld].Art;
            if (Figuren[zug.Zielfeld].Art != 0) // Wenn Figur geschlagen wird
            {
                zobrist_key ^= Zobrist.figures[Figuren[zug.Zielfeld].Art, zug.Zielfeld];
            }
            Figuren[zug.Zielfeld] = Figuren[zug.Startfeld];     
            Figuren[zug.Startfeld] = new Figur(0);
            zobrist_key ^= Zobrist.figures[gezogeneFigur, zug.Startfeld];
            zobrist_key ^= Zobrist.figures[gezogeneFigur, zug.Zielfeld];
            //en passant Felder und besondere Züge
            if (enpassant_Index != - 1) 
                zobrist_key ^= Zobrist.epFile[enpassant_Index % 8];
            enpassant_Index = -1;
            if (weißamzug)
            {
                if (zug.moveType == MoveType.BauerDoppelzug)
                {
                    enpassant_Index = zug.Zielfeld - 8;
                }
                if (zug.moveType == MoveType.Enpassant)
                {
                    zobrist_key ^= Zobrist.figures[7, zug.Zielfeld - 8];
                    Figuren[zug.Zielfeld - 8] = new Figur(0);
                }
                if (zug.moveType == MoveType.RochadeKS)
                {
                    zobrist_key ^= Zobrist.figures[4, 7];
                    zobrist_key ^= Zobrist.figures[4, 5];
                    Figuren[7] = new Figur(0);              
                    Figuren[5] = new Figur(4);
                }
                if (zug.moveType == MoveType.RochadeQS)
                {
                    zobrist_key ^= Zobrist.figures[4, 0];
                    zobrist_key ^= Zobrist.figures[4, 3];
                    Figuren[0] = new Figur(0);
                    Figuren[3] = new Figur(4);
                }
                //Bauernumwandlung
                if ((zug.Zielfeld / 8 == 7) && (Figuren[zug.Zielfeld].Art == 1)) 
                {
                    zobrist_key ^= Zobrist.figures[1, zug.Zielfeld];
                    zobrist_key ^= Zobrist.figures[zug.Promotion + 1, zug.Zielfeld];
                    if (zug.Promotion == 1) Figuren[zug.Zielfeld] = new Figur(2);
                    if (zug.Promotion == 2) Figuren[zug.Zielfeld] = new Figur(3);
                    if (zug.Promotion == 3) Figuren[zug.Zielfeld] = new Figur(4);
                    if (zug.Promotion == 4) Figuren[zug.Zielfeld] = new Figur(5);
                }
                //Update der Königsposition
                if (Figuren[zug.Zielfeld].Art == 6)
                    whiteKingPos = zug.Zielfeld;
            }
            else
            {
                zugnr += 1;
                if (zug.moveType == MoveType.BauerDoppelzug)
                    enpassant_Index = zug.Zielfeld + 8;
                if (zug.moveType == MoveType.Enpassant)
                {
                    zobrist_key ^= Zobrist.figures[1, zug.Zielfeld + 8];
                    Figuren[zug.Zielfeld + 8] = new Figur(0);
                }
                if (zug.moveType == MoveType.RochadeKS)
                {
                    zobrist_key ^= Zobrist.figures[10, 63];
                    zobrist_key ^= Zobrist.figures[10, 61];
                    Figuren[63] = new Figur(0);
                    Figuren[61] = new Figur(10);
                }
                if (zug.moveType == MoveType.RochadeQS)
                {

                    zobrist_key ^= Zobrist.figures[10, 56];
                    zobrist_key ^= Zobrist.figures[10, 59];
                    Figuren[56] = new Figur(0);
                    Figuren[59] = new Figur(10);
                }
                //Bauernumwandlung
                if ((zug.Zielfeld / 8 == 0) && (Figuren[zug.Zielfeld].Art == 7))
                {
                    zobrist_key ^= Zobrist.figures[7, zug.Zielfeld];
                    zobrist_key ^= Zobrist.figures[zug.Promotion + 7, zug.Zielfeld];
                    if (zug.Promotion == 1) Figuren[zug.Zielfeld] = new Figur(8);
                    if (zug.Promotion == 2) Figuren[zug.Zielfeld] = new Figur(9);
                    if (zug.Promotion == 3) Figuren[zug.Zielfeld] = new Figur(10);
                    if (zug.Promotion == 4) Figuren[zug.Zielfeld] = new Figur(11);
                }
                //Update der Königsposition
                if (Figuren[zug.Zielfeld].Art == 12)
                    blackKingPos = zug.Zielfeld;
            }
            UpdateKingPos();
            zobrist_key ^= Zobrist.sideToMove;
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
            zobrist_key = zobrist_history[zobrist_history.Count - 1];
            history.RemoveAt(history.Count - 1); // Letzten Zustand entfernen
            zobrist_history.RemoveAt(zobrist_history.Count - 1);
        }
		
        public void Clear()
        {
            for (int i = 0; i <= 63; i++) Figuren[i] = new Figur(0);
            history.Clear();
            zobrist_history.Clear();
            fzregel = 0;
            zugnr = 0;
            enpassant = "";
            enpassant_Index = -1;
            Rochaderechte = 0;
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
                        if (enpassant != "none") enpassant_Index = PGN.SquareToIndex(enpassant);
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
                            Rochaderechte += whiteKS;
                            break;
                        case 'Q':
                            Rochaderechte += whiteQS;
                            break;
                        case 'k':
                            Rochaderechte += blackKS;
                            break;
                        case 'q':
                            Rochaderechte += blackQS;
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
                            Figuren[i] = new Figur(10);
                            blackRooks.Add(i);
                            break;
                        case 'n':
                            Figuren[i] = new Figur(8);
                            blackKnights.Add(i);
                            break;
                        case 'b':   
                            Figuren[i] = new Figur(9);
                            blackBishops.Add(i);
                            break;
                        case 'q':
                            Figuren[i] = new Figur(11);
                            blackQueens.Add(i);
                            break;
                        case 'k':
                            Figuren[i] = new Figur(12);
                            blackKingPos = i;
                            break;
                        case 'p':
                            Figuren[i] = new Figur(7);
                            blackPawns.Add(i);
                            break;
                        //Weiß
                        case 'R':
                            Figuren[i] = new Figur(4);
                            whiteRooks.Add(i);
                            break;
                        case 'N':
                            Figuren[i] = new Figur(2);
                            whiteKnights.Add(i);
                            break;
                        case 'B':
                            Figuren[i] = new Figur(3);
                            whiteBishops.Add(i);
                            break;
                        case 'Q':
                            Figuren[i] = new Figur(5);
                            whiteQueens.Add(i);
                            break;
                        case 'K':
                            Figuren[i] = new Figur(6);
                            whiteKingPos = i;
                            break;
                        case 'P':
                            Figuren[i] = new Figur(1);
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
            zobrist_key = Zobrist.CalcZobristKey(this);
        }
        public void Berechne_Randabstand()
        {
            for (int i = 0; i<=63; i++)
            {
                AbstandNord[i] = 7 - i / 8;
                AbstandOst[i] = 7 - i % 8;
                AbstandSüd[i] = 7 - AbstandNord[i];
                AbstandWest[i] = 7 - AbstandOst[i];
                AbstandNO[i] = Math.Min(AbstandNord[i], AbstandOst[i]);
                AbstandNW[i] = Math.Min(AbstandNord[i], AbstandWest[i]);
                AbstandSO[i] = Math.Min(AbstandSüd[i], AbstandOst[i]);
                AbstandSW[i] = Math.Min(AbstandSüd[i], AbstandWest[i]);
            }
            
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
        public bool SquareInCheck(int Feld)
        {
            int i, j;
            int[] diagonalOffsets = { -9, -7, 7, 9 };
            int[] horivertiOffsets = { -8, -1, 1, 8 };
            int[] diagonalAbstände =
            {
                AbstandSW[Feld],
                AbstandSO[Feld],
                AbstandNW[Feld],
                AbstandNO[Feld],
            };
            int[] horivertiAbstände =
            {
                AbstandSüd[Feld],
                AbstandWest[Feld],
                AbstandOst[Feld],
                AbstandNord[Feld]
            };
            //diagonal            
            for (i = 0; i < 4; i++)
            {
                for (j = 1; j <= diagonalAbstände[i]; j++)
                {
                    int checkFeld = Feld + j * diagonalOffsets[i];
                    Figur checkFeldFigur = Figuren[checkFeld];

                    if (ColorToMove() == Figuren[checkFeld].Farbe) //Schach wird von eigener Figur abgeblockt
                        break;

                    if (j == 1) // direkt benachbarte Felder
                    {
                        if (!weißamzug && (checkFeldFigur.Type == 1) && (i < 2)) //weißer Bauer gibt Schach von Süden
                            return true;
                        if (weißamzug && (checkFeldFigur.Type == 1) && (i >= 2)) //schwarzer Bauer gibt Schach von Norden
                            return true;
                        if (checkFeldFigur.Type == 6) // König gibt nur benachbart Schach
                            return true;
                    }

                    if (checkFeldFigur.Art != 0)
                    {
                        if (checkFeldFigur.Type != 3 && checkFeldFigur.Type != 5) //Schach wird abgeblockt durch gegnerische Figur
                            break;
                    }

                    //Läufer und Damenschach
                    if (checkFeldFigur.Type == 3 || checkFeldFigur.Type == 5)
                        return true;
                }
            }
            //horizontal und vertikal
            for (i = 0; i < 4; i++)
            {
                for (j = 1; j <= horivertiAbstände[i]; j++)
                {
                    int checkFeld = Feld + j * horivertiOffsets[i];
                    Figur checkFeldFigur = Figuren[checkFeld];

                    if (ColorToMove() == Figuren[checkFeld].Farbe) //Schach wird von eigener Figur abgeblockt
                        break;

                    if (j == 1) // direkt benachbarte Felder
                    {
                        if (checkFeldFigur.Type == 6) // König gibt nur benachbart Schach
                            return true;
                    }
                    if (checkFeldFigur.Art != 0)
                    {
                        if (checkFeldFigur.Type != 4 && checkFeldFigur.Type != 5) //Schach wird abgeblockt durch gegnerische Figur
                            break;
                    }
                    //Trum und Damenschach
                    if (checkFeldFigur.Type == 4 || checkFeldFigur.Type == 5)
                        return true;
                }
            }
            //Springerschachs
            if (AbstandNord[Feld] >= 2)
            {
                if (AbstandWest[Feld] >= 1)
                {
                    int checkFeld = Feld + 15;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
                if (AbstandOst[Feld] >= 1)
                {
                    int checkFeld = Feld + 17;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
            }
            if (AbstandNord[Feld] >= 1)
            {
                if (AbstandWest[Feld] >= 2)
                {
                    int checkFeld = Feld + 6;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
                if (AbstandOst[Feld] >= 2)
                {
                    int checkFeld = Feld + 10;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
            }
            if (AbstandSüd[Feld] >= 1)
            {
                if (AbstandWest[Feld] >= 2)
                {
                    int checkFeld = Feld - 10;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
                if (AbstandOst[Feld] >= 2)
                {
                    int checkFeld = Feld - 6;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
            }
            if (AbstandSüd[Feld] >= 2)
            {
                if (AbstandWest[Feld] >= 1)
                {
                    int checkFeld = Feld - 17;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
                if (AbstandOst[Feld] >= 1)
                {
                    int checkFeld = Feld - 15;
                    Figur checkFeldFigur = Figuren[checkFeld];
                    if (checkFeldFigur.Type == 2 && ColorToMove() != checkFeldFigur.Farbe)
                        return true;
                }
            }
            return false;
        }
    }
}
