using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schachengine
{
    public enum Farbe
    {
        Weiß,
        Schwarz,
        leer
    }
    public struct Figur
    {
        public Farbe Farbe;
        public int ID; //Figurart mit Farbe
        public int Wert;
        public int Type; //Figurart ohne Farbe

        public Figur(int _ID)
        {
            int nocArt = _ID;
            ID = _ID;
            if (ID <= 6)
            {
                Farbe = Farbe.Weiß;
                nocArt -= 0;
            }
            else
            {
                Farbe = Farbe.Schwarz;
                nocArt -= 6;
            }
            if (ID == 0)
            {
                Farbe = Farbe.leer;
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
