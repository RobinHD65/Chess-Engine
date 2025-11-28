using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schachengine
{
    public struct UnmakeInfo
    {
        public Zug letzterZug;
        public Figur gezogeneFigur;
        public Figur geschlageneFigur;
        public int rochaderechte;
        public int fzregel;
        public int enpassantIndex;
    }
}
