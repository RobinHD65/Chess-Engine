using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Schachengine
{
    class Suche : MoveGenerator
    {
        public Zug gespeicherterZug;
        public int Suchtiefe;
        public int Evalve;
        public int maxZeit = 10;

        public List<Zug> legalMoves = new List<Zug>();

        public int[][] Bewertungstabellen =
        {
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
                     -10, 10, 10, 10, 10, 10, 10,-10,
                     -10,  5,  0,  0,  0,  0,  5,-10,
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
           //König Eröffnung/Mittelspiel
           new int[]{-30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -30,-40,-40,-50,-50,-40,-40,-30,
                     -20,-30,-30,-40,-40,-30,-30,-20,
                     -10,-20,-20,-20,-20,-20,-20,-10,
                      20, 20,  0,  0,  0,  0, 20, 20,
                      20, 30, 10,  0,  0, 10, 30, 20},
           //König Endspiel
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
                if (Figuren[i].Farbe == "Weiß")
                {
                    Typ = Figuren[i].Art - 8;
                    MaterialWeiß += Figuren[i].Wert;
                    //Kriterium Figurenaktivität
                    if (Typ == 6)
                    {
                        if (Endspiel)
                        {
                            MaterialWeiß += Bewertungstabellen[6][63 - i];
                        }
                        else
                        {
                            MaterialWeiß += Bewertungstabellen[5][63 - i];
                        }
                    }
                    else
                    {
                        MaterialWeiß += Bewertungstabellen[Typ - 1][63 - i];
                    }
                }
                else if (Figuren[i].Farbe == "Schwarz")
                {
                    Typ = Figuren[i].Art - 16;
                    MaterialSchwarz += Figuren[i].Wert;
                    //Kriterium Figurenaktivität
                    if (Typ == 6)
                    {
                        if (Endspiel)
                        {
                            MaterialSchwarz += Bewertungstabellen[6][i];
                        }
                        else
                        {
                            MaterialSchwarz += Bewertungstabellen[5][i];
                        }
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
            switch (WinChecker())
            {
                case "1-0":
                    Bewertung = 100000 - pyCount;
                    break;
                case "0-1":
                    Bewertung = -100000 + pyCount;
                    break;
                case "1/2-1/2":
                    Bewertung = 0;
                    break;
            }
            return Bewertung;
        }
        public string WinChecker()
        {
            //Matt und Patt
            if (LegalMoveGenerator(false).Count == 0)
            {
                UpdateKingPos();
                if (SquareInCheck(PosKönig))
                {
                    if (weißamzug)
                    {
                        return "0-1";
                    }
                    else
                    {
                        return "1-0";
                    }
                }
                else
                {
                    return "1/2-1/2";
                }
            }
            //Fünfzig-Züge-Regel
            if (fzregel >= 100)
                return "1/2-1/2";
            return "";
        }
        public int Quiesce(int tiefe, int spieler, int alpha, int beta)
        {
            int static_eval = spieler * Bewertung(tiefe);
            //Stand Pat
            int max = static_eval;
            if (max >= beta)
                return max;
            if (max >= alpha)
                alpha = max;

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
                    if (wert > alpha) alpha = wert;
                }
                if (wert > beta)
                    return max;
            }
            return max;
        }
        public int NegaMax(int tiefe, int spieler, int alpha, int beta)
        {
            if (zSekunden >= maxZeit && Suchtiefe > 1) return -1; 
            legalMoves = LegalMoveGenerator(false);
            if ((tiefe == Suchtiefe) || (legalMoves.Count == 0)) return Quiesce(tiefe, spieler, alpha, beta);
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
                        alpha = wert;
                    if (tiefe == 0)
                    {
                        gespeicherterZug = move;
                    }
                }
                if (wert >= beta)
                    return max;
            }
            return max;
        }
        public Zug PlayMove()
        {
            gespeicherterZug = default;
            zSekunden = 0;
            maxZeit = 20;
            timer.Start();

            Zug besterZug = default;
            int besterWert = 0;

            // Iterative Deepening
            for (Suchtiefe = 1; Suchtiefe <= 10; Suchtiefe++) // 10 nur als Maximalgrenze
            {
                if (weißamzug)
                {
                    besterWert = NegaMax(0, 1, int.MinValue + 2, int.MaxValue - 2);
                }
                else
                {
                    besterWert = -NegaMax(0, -1, int.MinValue + 2, int.MaxValue - 2);
                }
                // Zeitkontrolle: abbrechen, falls maxZeit überschritten
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
                Evalve = besterWert;
            }

            timer.Stop();
            return besterZug;
        }
        private void timedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            zSekunden += 1;
        }
    }
}
