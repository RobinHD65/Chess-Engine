using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace Schachengine
{
    static class Zobrist
    {
        public static ulong[,] figures = new ulong[13, 64]; //12 größter Index meiner Figuren +1 //64 Felder hat das Schachfeld
        public static ulong sideToMove;
        public static ulong[] castlingrights = new ulong[16];
        public static ulong[] epFile = new ulong[8];
        public static Random random = new Random(1583678);
        static Zobrist()
        {
            int i, j;
            for (i = 0; i < 13; i++)
            {
                for (j = 0; j < 64; j++)
                {
                    figures[i, j] = randomULong();
                }
            }

            sideToMove = randomULong();

            for (i = 0; i < 16; i++)
            {
                castlingrights[i] = randomULong();
            }

            for (i = 0; i < 8; i++)
            {
                epFile[i] = randomULong();
            }
        }
        static ulong randomULong()
        {    
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong CalcZobristKey(Brett brett)
        {
            ulong key = 0;
            int i;
            for (i = 0; i < 64; i++)
            {
                int figurIndex = brett.Figuren[i].Art;
                if (figurIndex != 0)
                {
                    key ^= figures[figurIndex, i];
                }
                
            }
            if (brett.enpassant_Index != -1)
            {
                int epFileIndex = brett.enpassant_Index % 8;
                key ^= epFile[epFileIndex];
            } 
            
            if (!brett.weißamzug)
            {
                key ^= sideToMove;
            }
            key ^= castlingrights[brett.Rochaderechte];

            return key;
        }
    }
}
