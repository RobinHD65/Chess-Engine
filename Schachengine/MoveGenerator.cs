using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Windows.Forms;

namespace Schachengine
{
    class MoveGenerator : Brett
    {
        public System.Timers.Timer timer = new(100);
        public int zSekunden;
        public ulong pvHash;
        public Zug pvMove;
        public UInt64 Perft(int tiefe)
        {
            UInt64 n = 0;      
            List<Zug> legalmoves = LegalMoveGenerator(false);
            if (tiefe == 0)
                return (ulong)legalmoves.Count;
            foreach (Zug move in legalmoves)
            { 
                    MakeMove(move);
                    n += Perft(tiefe - 1);
                    UnmakeMove();
            }
            return n;
            
        }
        public List<Zug> LegalMoveGenerator(bool onlyTakes)
        {
            List<Zug> pseudolegal = GenerateMoves(true, onlyTakes);
            List<Zug> legaleZüge = new();
            if (zobrist_key == pvHash) 
            {
                pvMove.GuessedScore = 10000;
            }
            for (int i = 0; i < pseudolegal.Count; i++)
            {
                Zug zug = pseudolegal[i];
                MakeMove(zug);
                weißamzug = !weißamzug;
                UpdateKingPos();
                if (!SquareInCheck(PosKönig))
                {
                    if (Figuren[zug.Zielfeld].Farbe != "leer")
                    {
                        zug.GuessedScore = Figuren[zug.Zielfeld].Wert - Figuren[zug.Startfeld].Wert;
                    }
                    legaleZüge.Add(zug);
                }
                weißamzug = !weißamzug;
                UnmakeMove();
            }
            legaleZüge.OrderByDescending(z => z.GuessedScore);
            return legaleZüge;
        }
        public List<Zug> GenerateMoves(bool CheckCastles, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            for (int i = 0; i <= 63; i++)
            {
                if ((Figuren[i].Farbe == "Weiß" && weißamzug) || (Figuren[i].Farbe == "Schwarz" && !weißamzug))
                {
                    switch (Figuren[i].Type)
                    {
                        case 1: //Bauern
                            moves.AddRange(GeneratePawnMoves(i, onlyTakes));
                            break;
                        case 2: //Springer
                            moves.AddRange(GenerateKnightMoves(i, onlyTakes));
                            break;
                        case 3: //Läufer
                            moves.AddRange(GenerateBishopMoves(i, onlyTakes));
                            break;
                        case 4: //Turm
                            moves.AddRange(GenerateRookMoves(i, onlyTakes));
                            break;
                        case 5: //Dame
                            moves.AddRange(GenerateBishopMoves(i, onlyTakes));
                            moves.AddRange(GenerateRookMoves(i, onlyTakes));
                            break;
                        case 6: //König
                            moves.AddRange(GenerateKingMoves(i, CheckCastles, onlyTakes));
                            break;

                    }
                }
            }
            return moves;
        }
        public List<Zug> GenerateBishopMoves(int Feld, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            int[] Offsets = { -9, -7, 7, 9 };
            int[] Abstände = {
                Math.Min(AbstandSüd[Feld], AbstandWest[Feld]),
                Math.Min(AbstandSüd[Feld], AbstandOst[Feld]),
                Math.Min(AbstandNord[Feld], AbstandWest[Feld]),
                Math.Min(AbstandNord[Feld], AbstandOst[Feld])
            };
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 1; j <= Abstände[i]; j++)
                {
                    int dest = Feld + j * Offsets[i];
                    // Wenn eigene Figur auf dem Zielfeld -> nicht hinzufügen und abbrechen
                    if (Figuren[dest].Farbe == Figuren[Feld].Farbe)
                    {
                        break;
                    }
                    // ansonsten Zug hinzufügen

                    if (!onlyTakes) moves.Add(new Zug(Feld, dest));
                    // Wenn ein Gegner auf dem Feld steht (nicht leer) -> abbrechen (Schlagzug)
                    if (Figuren[dest].Art != 0)
                    {
                        if (onlyTakes) moves.Add(new Zug(Feld, dest));
                        break;
                    }

                }
            }
            return moves;
        }
        public List<Zug> GenerateRookMoves(int Feld, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            int[] Offsets = { 8, 1, -8, -1 };
            int[] Abstände = { AbstandNord[Feld], AbstandOst[Feld], AbstandSüd[Feld], AbstandWest[Feld] };
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 1; j <= Abstände[i]; j++)
                {
                    int dest = Feld + j * Offsets[i];
                    if (Figuren[dest].Farbe == Figuren[Feld].Farbe)
                    {
                        break;
                    }
                    if (!onlyTakes) moves.Add(new Zug(Feld, dest));
                    if (Figuren[dest].Art != 0)
                    {
                        if (onlyTakes) moves.Add(new Zug(Feld, dest));
                        break;
                    }
                }
            }
            return moves;
        }
        public List<Zug> GeneratePawnMoves(int Feld, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            //Weiße Bauern
            if (Figuren[Feld].Farbe == "Weiß")
            {
                if (Figuren[Feld + 8].Farbe == "leer")
                {
                    if (Feld / 8 == 6)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            Zug move = new Zug(Feld, Feld + 8);
                            move.Promotion = i;
                            if (!onlyTakes) moves.Add(move);
                        }
                    }
                    else
                    {
                        if (!onlyTakes) moves.Add(new Zug(Feld, Feld + 8));
                    }
                    if (Feld / 8 == 1)
                    {
                        if (Figuren[Feld + 16].Farbe == "leer")
                        {
                            Zug move = new Zug(Feld, Feld + 16);
                            move.moveType = MoveType.BauerDoppelzug;
                            if (!onlyTakes) moves.Add(move);
                        }
                    }
                }
                //Schlagzüge
                if (AbstandWest[Feld] > 0)
                {
                    if (Figuren[Feld + 7].Farbe == "Schwarz")
                    {
                        if (Feld / 8 == 6)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld + 7);
                                move.Promotion = i;
                                moves.Add(move);
                            }
                        }
                        else
                        {
                            moves.Add(new Zug(Feld, Feld + 7));
                        }
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Figuren[Feld + 9].Farbe == "Schwarz")
                    {
                        if (Feld / 8 == 6)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld + 9);
                                move.Promotion = i;
                                moves.Add(move);
                            }
                        }
                        else
                        {
                            moves.Add(new Zug(Feld, Feld + 9));
                        }
                    }
                }
                //en passant
                if (AbstandWest[Feld] > 0)
                {
                    if (Feld + 7 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld + 7);
                        move.moveType = MoveType.Enpassant;
                        moves.Add(move);
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Feld + 9 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld + 9);
                        move.moveType = MoveType.Enpassant;
                        moves.Add(move);
                    }
                }

            }
            else
            //Schwarze Bauern
            {
                if (Figuren[Feld - 8].Farbe == "leer")
                {
                    if (Feld / 8 == 1)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            Zug move = new Zug(Feld, Feld - 8);
                            move.Promotion = i;
                            if (!onlyTakes) moves.Add(move);
                        }
                    }
                    else
                    {
                        if (!onlyTakes) moves.Add(new Zug(Feld, Feld - 8));
                    }
                    if (Feld / 8 == 6)
                    {
                        if ((Feld / 8 == 6) && (Figuren[Feld - 16].Farbe == "leer"))
                        {
                            Zug move = new Zug(Feld, Feld - 16);
                            move.moveType = MoveType.BauerDoppelzug;
                            if (!onlyTakes) moves.Add(move);
                        }
                    }
                }
                //Schlagzüge
                if (AbstandWest[Feld] > 0)
                {
                    if (Figuren[Feld - 9].Farbe == "Weiß")
                    {
                        if (Feld / 8 == 1)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld - 9);
                                move.Promotion = i;
                                moves.Add(move);
                            }
                        }
                        else
                        {
                            moves.Add(new Zug(Feld, Feld - 9));
                        }
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Figuren[Feld - 7].Farbe == "Weiß")
                    {
                        if (Feld / 8 == 1)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld - 7);
                                move.Promotion = i;
                                moves.Add(move);
                            }
                        }
                        else
                        {
                            moves.Add(new Zug(Feld, Feld - 7));
                        }
                    }
                }
                //enpassant
                if (AbstandOst[Feld] > 0)
                {
                    if (Feld - 7 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld - 7);
                        move.moveType = MoveType.Enpassant;
                        moves.Add(move);

                    }
                }
                if (AbstandWest[Feld] > 0)
                {
                    if (Feld - 9 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld - 9);
                        move.moveType = MoveType.Enpassant;
                        moves.Add(move);
                    }
                }
            }
            return moves;
        }
        public List<Zug> GenerateKnightMoves(int Feld, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            if ((AbstandNord[Feld] >= 2) && (AbstandOst[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 17].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 17].Farbe != "leer") moves.Add(new Zug(Feld, Feld + 17));
                }
            }

            if ((AbstandNord[Feld] >= 1) && (AbstandOst[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 10].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 10].Farbe != "leer") moves.Add(new Zug(Feld, Feld + 10));
                }
            }
            if ((AbstandSüd[Feld] >= 1) && (AbstandOst[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 6].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 6].Farbe != "leer") moves.Add(new Zug(Feld, Feld - 6));
                }
            }
            if ((AbstandSüd[Feld] >= 2) && (AbstandOst[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 15].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 15].Farbe != "leer") moves.Add(new Zug(Feld, Feld - 15));
                }
            }
            if ((AbstandSüd[Feld] >= 2) && (AbstandWest[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 17].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 17].Farbe != "leer") moves.Add(new Zug(Feld, Feld - 17));
                }
            }
            if ((AbstandSüd[Feld] >= 1) && (AbstandWest[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 10].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 10].Farbe != "leer") moves.Add(new Zug(Feld, Feld - 10));
                }
            }
            if ((AbstandNord[Feld] >= 2) && (AbstandWest[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 15].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 15].Farbe != "leer") moves.Add(new Zug(Feld, Feld + 15));
                }
            }
            if ((AbstandNord[Feld] >= 1) && (AbstandWest[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 6].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 6].Farbe != "leer") moves.Add(new Zug(Feld, Feld + 6));
                }
            }
            return moves;
        }
        public List<Zug> GenerateKingMoves(int Feld, bool CheckCastles, bool onlyTakes)
        {
            List<Zug> moves = new List<Zug>();
            int[] Offsets = { 7, 8, 9, 1, -7, -8, -9, -1 };
            int[] Abstände =
            {
                Math.Min(AbstandNord[Feld], AbstandWest[Feld]),
                AbstandNord[Feld],
                Math.Min(AbstandNord[Feld], AbstandOst[Feld]),
                AbstandOst[Feld],
                Math.Min(AbstandSüd[Feld], AbstandOst[Feld]),
                AbstandSüd[Feld],
                Math.Min(AbstandSüd[Feld], AbstandWest[Feld]),
                AbstandWest[Feld]
            };
            for (int i = 0; i <= 7; i++)
            {
                if (Abstände[i] > 0)
                {
                    if (Figuren[Feld].Farbe != Figuren[Feld + Offsets[i]].Farbe)
                    {
                        if (!onlyTakes || Figuren[Feld + Offsets[i]].Farbe != "leer") moves.Add(new Zug(Feld, Feld + Offsets[i]));
                    }
                }
            }
            //Rochaden Weiß
            if (CheckCastles && !onlyTakes)
            {
                if (weißamzug)
                {
                    if (hasCastlingRight(0) && (Figuren[5].Art == 0) && (Figuren[6].Art == 0))
                    {
                        if (!SquareInCheck(5) && !SquareInCheck(4))
                        {
                            Zug move = new Zug(Feld, Feld + 2);
                            move.moveType = MoveType.RochadeKS;
                            moves.Add(move);
                        }
                    }
                    if (hasCastlingRight(1) && (Figuren[1].Art == 0) && (Figuren[2].Art == 0) && (Figuren[3].Art == 0))
                    {
                        if (!SquareInCheck(3) && !SquareInCheck(4))
                        {
                            Zug move = new Zug(Feld, Feld - 2);
                            move.moveType = MoveType.RochadeQS;
                            moves.Add(move);
                        }
                    }                      
                }
                else
                //Rochaden Schwarz
                {
                    if (hasCastlingRight(2) && (Figuren[61].Art == 0) && (Figuren[62].Art == 0))
                    {
                        if (!SquareInCheck(61) && !SquareInCheck(60))
                        {
                            Zug move = new Zug(Feld, Feld + 2);
                            move.moveType = MoveType.RochadeKS;
                            moves.Add(move);
                        }
                    }
                    if (hasCastlingRight(3) && (Figuren[57].Art == 0) && (Figuren[58].Art == 0) && (Figuren[59].Art == 0))
                    {
                        if (!SquareInCheck(59) && !SquareInCheck(60))
                        {
                            Zug move = new Zug(Feld, Feld - 2);
                            move.moveType = MoveType.RochadeQS;
                            moves.Add(move);
                        }
                        
                    }
                }
            }
            return moves;
        }
    }
}
