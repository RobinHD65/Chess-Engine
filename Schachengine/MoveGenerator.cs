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
        List<Zug> pseudolegal = new List<Zug>();
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
            
            GenerateMoves(true, onlyTakes);
            List<Zug> legaleZüge = new();
            if (zobrist_key == pvHash)
            {
                pvMove.GuessedScore = 10000;
                int idx = pseudolegal.FindIndex(z => z.Startfeld == pvMove.Startfeld && z.Zielfeld == pvMove.Zielfeld); 
                if (idx >= 0) pseudolegal.RemoveAt(idx);
                legaleZüge.Add(pvMove);
            }
            for (int i = 0; i < pseudolegal.Count; i++)
            {
                Zug zug = pseudolegal[i];     
                MakeMove(zug);
                weißamzug = !weißamzug;
                UpdateKingPos();
                if (!SquareInCheck(PosKönig))
                {
                    if (Figuren[zug.Zielfeld].Farbe != Farbe.leer)
                    {
                        zug.GuessedScore = Figuren[zug.Zielfeld].Wert - Figuren[zug.Startfeld].Wert;
                    }
                    legaleZüge.Add(zug);
                }
                weißamzug = !weißamzug;
                UnmakeMove();
                
            
            }
            if (zobrist_key == pvHash)
            {
                for (int i = 0; i < legaleZüge.Count; i++)
                {
                    if (legaleZüge[i].Startfeld == pvMove.Startfeld && legaleZüge[i].Zielfeld == pvMove.Zielfeld)
                    {
                        Zug temp = legaleZüge[i];
                        temp.GuessedScore = 10000;
                        legaleZüge[i] = temp;
                        break;
                    }
                }
            }
            legaleZüge.Sort((a, b) => b.GuessedScore.CompareTo(a.GuessedScore));
            return legaleZüge;
        }
        public List<Zug> GenerateMoves(bool CheckCastles, bool onlyTakes)
        {
            pseudolegal.Clear();
            for (int i = 0; i <= 63; i++)
            {
                if ((Figuren[i].Farbe == Farbe.Weiß && weißamzug) || (Figuren[i].Farbe == Farbe.Schwarz && !weißamzug))
                {
                    switch (Figuren[i].Type)
                    {
                        case 1: //Bauern
                            GeneratePawnMoves(i, onlyTakes);
                            break;
                        case 2: //Springer
                            GenerateKnightMoves(i, onlyTakes);
                            break;
                        case 3: //Läufer
                            GenerateBishopMoves(i, onlyTakes);
                            break;
                        case 4: //Turm
                            GenerateRookMoves(i, onlyTakes);
                            break;
                        case 5: //Dame
                            GenerateBishopMoves(i, onlyTakes);
                            GenerateRookMoves(i, onlyTakes);
                            break;
                        case 6: //König
                            GenerateKingMoves(i, CheckCastles, onlyTakes);
                            break;

                    }
                }
            }
            return pseudolegal;
        }
        public void GenerateBishopMoves(int Feld, bool onlyTakes)
        {  
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
                    int dest = Feld + j * diagonalOffsets[i];
                    // Wenn eigene Figur auf dem Zielfeld -> nicht hinzufügen und abbrechen
                    if (Figuren[dest].Farbe == Figuren[Feld].Farbe)
                    {
                        break;
                    }
                    // ansonsten Zug hinzufügen

                    if (!onlyTakes) pseudolegal.Add(new Zug(Feld, dest));
                    // Wenn ein Gegner auf dem Feld steht (nicht leer) -> abbrechen (Schlagzug)
                    if (Figuren[dest].Art != 0)
                    {
                        if (onlyTakes) pseudolegal.Add(new Zug(Feld, dest));
                        break;
                    }

                }
            }
        }
        public void GenerateRookMoves(int Feld, bool onlyTakes)
        { 
            int[] Abstände = { AbstandSüd[Feld], AbstandWest[Feld], AbstandOst[Feld], AbstandNord[Feld] };
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 1; j <= Abstände[i]; j++)
                {
                    int dest = Feld + j * horivertiOffsets[i];
                    if (Figuren[dest].Farbe == Figuren[Feld].Farbe)
                    {
                        break;
                    }
                    if (!onlyTakes) pseudolegal.Add(new Zug(Feld, dest));
                    if (Figuren[dest].Art != 0)
                    {
                        if (onlyTakes) pseudolegal.Add(new Zug(Feld, dest));
                        break;
                    }
                }
            }
        }
        public void GeneratePawnMoves(int Feld, bool onlyTakes)
        {
            //Weiße Bauern
            if (Figuren[Feld].Farbe == Farbe.Weiß)
            {
                if (Figuren[Feld + 8].Farbe == Farbe.leer)
                {
                    if (Feld / 8 == 6)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            Zug move = new Zug(Feld, Feld + 8);
                            move.Promotion = i;
                            if (!onlyTakes) pseudolegal.Add(move);
                        }
                    }
                    else
                    {
                        if (!onlyTakes) pseudolegal.Add(new Zug(Feld, Feld + 8));
                    }
                    if (Feld / 8 == 1)
                    {
                        if (Figuren[Feld + 16].Farbe == Farbe.leer)
                        {
                            Zug move = new Zug(Feld, Feld + 16);
                            move.moveType = MoveType.BauerDoppelzug;
                            if (!onlyTakes) pseudolegal.Add(move);
                        }
                    }
                }
                //Schlagzüge
                if (AbstandWest[Feld] > 0)
                {
                    if (Figuren[Feld + 7].Farbe == Farbe.Schwarz)
                    {
                        if (Feld / 8 == 6)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld + 7);
                                move.Promotion = i;
                                pseudolegal.Add(move);
                            }
                        }
                        else
                        {
                            pseudolegal.Add(new Zug(Feld, Feld + 7));
                        }
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Figuren[Feld + 9].Farbe == Farbe.Schwarz)
                    {
                        if (Feld / 8 == 6)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld + 9);
                                move.Promotion = i;
                                pseudolegal.Add(move);
                            }
                        }
                        else
                        {
                            pseudolegal.Add(new Zug(Feld, Feld + 9));
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
                        pseudolegal.Add(move);
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Feld + 9 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld + 9);
                        move.moveType = MoveType.Enpassant;
                        pseudolegal.Add(move);
                    }   
                }

            }
            else
            //Schwarze Bauern
            {
                if (Figuren[Feld - 8].Farbe == Farbe.leer)
                {
                    if (Feld / 8 == 1)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            Zug move = new Zug(Feld, Feld - 8);
                            move.Promotion = i;
                            if (!onlyTakes) pseudolegal.Add(move);
                        }
                    }
                    else
                    {
                        if (!onlyTakes) pseudolegal.Add(new Zug(Feld, Feld - 8));
                    }
                    if (Feld / 8 == 6)
                    {
                        if ((Feld / 8 == 6) && (Figuren[Feld - 16].Farbe == Farbe.leer))
                        {
                            Zug move = new Zug(Feld, Feld - 16);
                            move.moveType = MoveType.BauerDoppelzug;
                            if (!onlyTakes) pseudolegal.Add(move);
                        }
                    }
                }
                //Schlagzüge
                if (AbstandWest[Feld] > 0)
                {
                    if (Figuren[Feld - 9].Farbe == Farbe.Weiß)
                    {
                        if (Feld / 8 == 1)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld - 9);
                                move.Promotion = i;
                                pseudolegal.Add(move);
                            }
                        }
                        else
                        {
                            pseudolegal.Add(new Zug(Feld, Feld - 9));
                        }
                    }
                }
                if (AbstandOst[Feld] > 0)
                {
                    if (Figuren[Feld - 7].Farbe == Farbe.Weiß)
                    {
                        if (Feld / 8 == 1)
                        {
                            for (int i = 1; i <= 4; i++)
                            {
                                Zug move = new Zug(Feld, Feld - 7);
                                move.Promotion = i;
                                pseudolegal.Add(move);
                            }
                        }
                        else
                        {
                            pseudolegal.Add(new Zug(Feld, Feld - 7));
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
                        pseudolegal.Add(move);

                    }
                }
                if (AbstandWest[Feld] > 0)
                {
                    if (Feld - 9 == enpassant_Index)
                    {
                        Zug move = new Zug(Feld, Feld - 9);
                        move.moveType = MoveType.Enpassant;
                        pseudolegal.Add(move);
                    }
                }
            }
        }
        public void GenerateKnightMoves(int Feld, bool onlyTakes)
        {
            if ((AbstandNord[Feld] >= 2) && (AbstandOst[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 17].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 17].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld + 17));
                }
            }

            if ((AbstandNord[Feld] >= 1) && (AbstandOst[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 10].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 10].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld + 10));
                }
            }
            if ((AbstandSüd[Feld] >= 1) && (AbstandOst[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 6].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 6].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld - 6));
                }
            }
            if ((AbstandSüd[Feld] >= 2) && (AbstandOst[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 15].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 15].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld - 15));
                }
            }
            if ((AbstandSüd[Feld] >= 2) && (AbstandWest[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 17].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld - 17].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld - 17));
                }
            }
            if ((AbstandSüd[Feld] >= 1) && (AbstandWest[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld - 10].Farbe)
                {   
                    if (!onlyTakes || Figuren[Feld - 10].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld - 10));
                }
            }
            if ((AbstandNord[Feld] >= 2) && (AbstandWest[Feld] >= 1))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 15].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 15].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld + 15));
                }
            }
            if ((AbstandNord[Feld] >= 1) && (AbstandWest[Feld] >= 2))
            {
                if (Figuren[Feld].Farbe != Figuren[Feld + 6].Farbe)
                {
                    if (!onlyTakes || Figuren[Feld + 6].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld + 6));
                }
            }
        }
        public void GenerateKingMoves(int Feld, bool CheckCastles, bool onlyTakes)
        {
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
                    if (Figuren[Feld].Farbe != Figuren[Feld + kingOffsets[i]].Farbe)
                    {
                        if (!onlyTakes || Figuren[Feld + kingOffsets[i]].Farbe != Farbe.leer) pseudolegal.Add(new Zug(Feld, Feld + kingOffsets[i]));
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
                            pseudolegal.Add(move);
                        }
                    }
                    if (hasCastlingRight(1) && (Figuren[1].Art == 0) && (Figuren[2].Art == 0) && (Figuren[3].Art == 0))
                    {
                        if (!SquareInCheck(3) && !SquareInCheck(4))
                        {
                            Zug move = new Zug(Feld, Feld - 2);
                            move.moveType = MoveType.RochadeQS;
                            pseudolegal.Add(move);
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
                            pseudolegal.Add(move);
                        }
                    }
                    if (hasCastlingRight(3) && (Figuren[57].Art == 0) && (Figuren[58].Art == 0) && (Figuren[59].Art == 0))
                    {
                        if (!SquareInCheck(59) && !SquareInCheck(60))
                        {
                            Zug move = new Zug(Feld, Feld - 2);
                            move.moveType = MoveType.RochadeQS;
                            pseudolegal.Add(move);
                        }

                    }
                }
            }
            return;
        }
    }
}
