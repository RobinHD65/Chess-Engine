using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schachengine
{
    public struct Zug
    {
        public int Startfeld;
        public int Zielfeld;
        public int Promotion;
        public MoveType moveType;
        public int GuessedScore;
        public Zug(int _Startfeld, int _Zielfeld)
        {
            Startfeld = _Startfeld;
            Zielfeld = _Zielfeld;
            Promotion = 0;
            moveType = MoveType.Normal;
        }
    }
    public enum MoveType
    {
        Normal,
        RochadeKS,
        RochadeQS,
        BauerDoppelzug,
        Enpassant
    }
}
