using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Schachengine
{
    public enum Result 
    { 
        whiteWin,
        blackWin,
        draw,
        undecided
    }
    class Suche : MoveGenerator
    {
        public Zug gespeicherterZug;
        public int Suchtiefe;
        public int Evalve;
        public int AlphaCuts;
        public int BetaCuts;
        public int QuiesceCalls;
        public int nodes;
        public int maxZeit = 10;

        public List<Zug> legalMoves = new List<Zug>();

        public readonly int[][] Bewertungstabellen =
        {
            //Mittelspielbewertung Tabellen
            //Bauern
           new int[]{  0,  0,  0,  0,  0,  0,  0,  0,
                       50, 50, 50, 50, 50, 50, 50, 50,
                       10, 10, 20, 30, 30, 20, 10, 10,
                       5,  5, 10, 25, 25, 10,  5,  5,
                       0,  0,  0, 20, 20,  0,  0,  0,
                       5, -5,-10,  0,  0,-10, -5,  5,
                       5, 10, 10,-20,-20, 10, 10,  5,
                       0,  0,  0,  0,  0,  0,  0,  0},
            //Springer
           new int[]{-50,-40,-30,-30,-30,-30,-40,-50,
                     -40,-20,  0,  0,  0,  0,-20,-40,
                     -30,  0, 10, 15, 15, 10,  0,-30,
                     -30,  5, 15, 20, 20, 15,  5,-30,
                     -30,  0, 15, 20, 20, 15,  0,-30,
                     -30,  5, 10, 15, 15, 10,  5,-30,
                     -40,-20,  0,  5,  5,  0,-20,-40,
                     -50,-40,-30,-30,-30,-30,-40,-50 },
           //Läufer
           new int[]{-20,-10,-10,-10,-10,-10,-10,-20,
                     -10,  0,  0,  0,  0,  0,  0,-10,
                     -10,  0,  5, 10, 10,  5,  0,-10,
                     -10,  5,  5, 10, 10,  5,  5,-10,
                     -10,  0, 10, 10, 10, 10,  0,-10,
                     -10,  5, 10,  5,  5, 10,  5,-10,
                     -10, 10,  0,  0,  0,  0, 10,-10,
                     -20,-10,-10,-10,-10,-10,-10,-20,},
           //Turm
           new int[]{ 0,  0,  0,  0,  0,  0,  0,  0,
                      5, 10, 10, 10, 10, 10, 10,  5,
                     -5,  0,  0,  0,  0,  0,  0, -5,
                     -5,  0,  0,  0,  0,  0,  0, -5,
                     -5,  0,  0,  0,  0,  0,  0, -5,
                     -5,  0,  0,  0,  0,  0,  0, -5,
                     -5,  0,  0,  0,  0,  0,  0, -5,
                      0,  0,  0,  5,  5,  0,  0,  0},
           // Dame
           new int[]{-20,-10,-10, -5, -5,-10,-10,-20,
                     -10,  0,  0,  0,  0,  0,  0,-10,
                     -10,  0,  5,  5,  5,  5,  0,-10,
                      -5,  0,  5,  5,  5,  5,  0, -5,
                       0,  0,  5,  5,  5,  5,  0, -5,
                     -10,  5,  5,  5,  5,  5,  0,-10,
                     -10,  0,  5,  0,  0,  0,  0,-10,
                     -20,-10,-10, -5, -5,-10,-10,-20},
           //König 
           new int[]{-30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -20,-30,-30,-40,-40,-30,-30,-20,
                     -10,-20,-20,-20,-20,-20,-20,-10,
                      20, 20,  0,  0,  0,  0, 20, 20,
                      20, 30, 10,  0,  0, 10, 30, 20},

           //Endspielbewertung Tabellen
           //Bauern
           new int[]{  0,  0,  0,  0,  0,  0,  0,  0,
                       50, 50, 50, 50, 50, 50, 50, 50,
                       30, 30, 30, 30, 30, 30, 30, 30,
                       10, 10, 10, 30, 30, 10,  10, 10,
                       0,  0,  0, 20, 20,  0,  0,  0,
                       -30,-30,-30,-40,-40,-30,-30,-30,
                       -30,-30,-30,-40,-40,-30,-30,-30,
                       0,  0,  0,  0,  0,  0,  0,  0},
            //Springer
           new int[]{-50,-40,-30,-30,-30,-30,-40,-50,
                     -40,-20,  0,  0,  0,  0,-20,-40,
                     -30,  0, 10, 15, 15, 10,  0,-30,
                     -30,  5, 15, 20, 20, 15,  5,-30,
                     -30,  0, 15, 20, 20, 15,  0,-30,
                     -30,  5, 10, 15, 15, 10,  5,-30,
                     -40,-20,  0,  5,  5,  0,-20,-40,
                     -50,-40,-30,-30,-30,-30,-40,-50},
           //Läufer
           new int[]{-20,-10,-10,-10,-10,-10,-10,-20,
                     -10,  0,  0,  0,  0,  0,  0,-10,
                     -10,  0,  5, 10, 10,  5,  0,-10,
                     -10,  5,  5, 10, 10,  5,  5,-10,
                     -10,  0, 10, 10, 10, 10,  0,-10,
                     -10,  5, 10,  5,  5, 10,  5,-10,
                     -10, 10,  0,  0,  0,  0, 10,-10,
                     -20,-10,-10,-10,-10,-10,-10,-20,},
           //Turm
           new int[]{ 0,  0,  0,  0,  0,  0,  0,  0,
                      10, 10, 10, 10, 10, 10, 10, 10,
                      0,  0,  0,  0,  0,  0,  0,  0,
                      0,  0,  0,  0,  0,  0,  0,  0,
                      0,  0,  0,  0,  0,  0,  0,  0,
                      0,  0,  0,  0,  0,  0,  0,  0,
                      0,  0,  0,  0,  0,  0,  0,  0,
                      5,  5,  5,  5,  5,  5,  5,  5},
           // Dame
           new int[]{-20,-10,-10, -5, -5,-10,-10,-20,
                     -10,  0,  0,  0,  0,  0,  0,-10,
                     -10,  0,  5,  5,  5,  5,  0,-10,
                      -5,  0,  5,  5,  5,  5,  0, -5,
                       0,  0,  5,  5,  5,  5,  0, -5,
                     -10,  5,  5,  5,  5,  5,  0,-10,
                     -10,  0,  5,  0,  0,  0,  0,-10,
                     -20,-10,-10, -5, -5,-10,-10,-20},
           //König 
           new int[]{-50,-40,-30,-20,-20,-30,-40,-50,
                     -30,-20,-10,  0,  0,-10,-20,-30,
                     -30,-10, 20, 30, 30, 20,-10,-30,
                     -30,-10, 30, 40, 40, 30,-10,-30,
                     -30,-10, 30, 40, 40, 30,-10,-30,
                     -30,-10, 20, 30, 30, 20,-10,-30,
                     -30,-30,  0,  0,  0,  0,-30,-30,
                     -50,-30,-30,-30,-30,-30,-30,-50}
        };
        public Suche()
        {
            timer.Elapsed += new ElapsedEventHandler(timedEvent);
            Berechne_Randabstand();
        }
        //Bewertungstabellen für
        public int Bewertung(int pyCount)
        {
            int MaterialWeiß = 0;
            int MaterialSchwarz = 0;
            int Typ = 0;
            int Bewertung = 0;
            for (int i = 0; i <= 63; i++)
            {
                if (Figuren[i].Farbe == Farbe.Weiß)
                {
                    Typ = Figuren[i].ID;
                    MaterialWeiß += Figuren[i].Wert;
                    //Kriterium Figurenaktivität
                    if (Endspiel)
                    {
                        MaterialWeiß += Bewertungstabellen[Typ + 5][63 - i];
                    }
                    else
                    {
                        MaterialWeiß += Bewertungstabellen[Typ - 1][63 - i];
                    }
                }
                else if (Figuren[i].Farbe == Farbe.Schwarz)
                {
                    Typ = Figuren[i].ID - 6;
                    MaterialSchwarz += Figuren[i].Wert;
                    //Kriterium Figurenaktivität
                    if (Endspiel)
                    {
                        MaterialSchwarz += Bewertungstabellen[Typ + 5][i];
                    }
                    else
                    {
                        MaterialSchwarz += Bewertungstabellen[Typ - 1][i];
                    }
                }
            }
            if (MaterialWeiß <= 1250 || MaterialSchwarz <= 1250)
                Endspiel = true;
            else
                Endspiel = false;

            Bewertung = MaterialWeiß - MaterialSchwarz;
            switch (WinChecker(true))
            {
                case Result.whiteWin:
                    Bewertung = 100000 - pyCount;
                    break;
                case Result.blackWin:
                    Bewertung = -100000 + pyCount;
                    break;
                case Result.draw:
                    Bewertung = 0;
                    break;
            }
            return Bewertung;
        }
        public Result WinChecker(bool drawDoubleRepeat)
        {
            //Matt und Patt
            if (LegalMoveGenerator(false).Count == 0)
            {
                UpdateKingPos();
                if (SquareInCheck(PosKönig))
                {
                    if (weißamzug)
                    {
                        return Result.blackWin;
                    }
                    else
                    {
                        return Result.whiteWin;
                    }
                }
                else
                {
                    return Result.draw;
                }
            }
            //Fünfzig-Züge-Regel
            if (fzregel >= 100)
                return Result.draw;
            //Dreifache-Stellungswiederholung
            int repCount = zobrist_history.Count(x => x == zobrist_key);
            int drawCount = 2;
            if (drawDoubleRepeat)
                drawCount = 1;
            if (repCount == drawCount)
            {
                return Result.draw;
            }
            return Result.undecided;
        }
        public Result WinChecker(bool drawDoubleRepeat, List<Zug> legalMoves)
        {
            //Matt und Patt
            if (legalMoves.Count == 0)
            {
                UpdateKingPos();
                if (SquareInCheck(PosKönig))
                {
                    if (weißamzug)
                    {
                        return Result.blackWin;
                    }
                    else
                    {
                        return Result.whiteWin;
                    }
                }
                else
                {
                    return Result.draw;
                }
            }
            //Fünfzig-Züge-Regel
            if (fzregel >= 100)
                return Result.draw;
            //Dreifache-Stellungswiederholung
            int repCount = zobrist_history.Count(x => x == zobrist_key);
            int drawCount = 2;
            if (drawDoubleRepeat)
                drawCount = 1;
            if (repCount == drawCount)
            {
                return Result.draw;
            }
            return Result.undecided;
        }
        public int Quiesce(int tiefe, int spieler, int alpha, int beta)
        {
            int static_eval = spieler * Bewertung(tiefe);
            //Stand Pat
            int max = static_eval;
            if (max >= beta)
            {
                return max;
            }
                
            if (max >= alpha) 
            {
                alpha = max;
            }            
            // delta pruning
            if (!Endspiel)
            {
                int delta = 1000;
                if (max < alpha - delta)
                    return alpha;
            } 
            //Erst ab hier zählen, da es erst ab hier teuer wird
            int wert;
            List<Zug> Captures = LegalMoveGenerator(true);
            foreach (Zug capture in Captures)
            {
                MakeMove(capture);
                wert = -Quiesce(tiefe + 1, -spieler, -beta, -alpha);
                UnmakeMove();
                if (wert > max)
                {
                    max = wert;
                    if (wert > alpha)
                    {
                        alpha = wert;
                    }
                }
                if (wert > beta)
                {
                    return max;
                }
            }
            return max;
        }

        //Alpha-Beta failsoft
        public int NegaMax(int tiefe, int spieler, int alpha, int beta)
        {
            if (zSekunden >= maxZeit && Suchtiefe > 1) return -1; 
            legalMoves = LegalMoveGenerator(false);
            if ((tiefe == Suchtiefe) || (WinChecker(true, legalMoves) != Result.undecided)) return Quiesce(tiefe, spieler, alpha, beta);            
            int max = int.MinValue + 2;
            foreach (Zug move in legalMoves)
            {
                MakeMove(move);
                int wert = -NegaMax(tiefe + 1, -spieler, -beta, -alpha);
                UnmakeMove();
                if (zSekunden >= maxZeit && Suchtiefe > 1) return -1;
                if (wert > max)
                {
                    max = wert;
                    if (wert > alpha)
                    {
                        alpha = wert;
                    }
                    if (tiefe == 0)
                    {
                        gespeicherterZug = move;
                    }
                }
                if (wert >= beta)
                {
                    return max;
                }
            }
            return max;
        }
        public async Task<Zug> PlayMove(int thinkTime)
        {
            return await Task.Run(() =>
            {
                gespeicherterZug = default;
                zSekunden = 0;
                maxZeit = thinkTime;
                timer.Start();

                Zug besterZug = default;
                int besterWert = 0;
                AlphaCuts = 0;
                BetaCuts = 0;
                QuiesceCalls = 0;
                nodes = 0;
                // Iterative Deepening
                for (Suchtiefe = 1; Suchtiefe <= 20; Suchtiefe++) // 20 nur als Maximalgrenze(wird nie erreicht)
                {
                    if (weißamzug)
                    {
                        besterWert = NegaMax(0, 1, int.MinValue + 2, int.MaxValue - 2);
                    }
                    else
                    {
                        besterWert = -NegaMax(0, -1, int.MinValue + 2, int.MaxValue - 2);
                    }             
                    if (Suchtiefe == 1)
                    {
                        besterZug = gespeicherterZug;
                        Evalve = besterWert;
                    }
                    if (zSekunden >= maxZeit)
                    {
                        Suchtiefe--;
                        break;
                    }
                    besterZug = gespeicherterZug;
                    pvHash = zobrist_key;
                    pvMove = besterZug;
                    Evalve = besterWert;
                }

                timer.Stop();
                return besterZug;
            });
        }
        private void timedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            zSekunden += 1;
        }
    }
}
