using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schachengine
{
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
            if (Art <= 6)
            {
                Farbe = "Weiß";
                nocArt -= 0;
            }
            else
            {
                Farbe = "Schwarz";
                nocArt -= 6;
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
