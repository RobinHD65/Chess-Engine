using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Schachengine.Form1;

namespace Schachengine
{
    public partial class Form1 : Form
    {
        //Public-Deklarationen
        public bool invertion = false;
        public bool pieceSelected = false;
        static Suche instance = new(); //Objekt Suche erbt von allem
        Zug Input = new Zug();
        List<Zug> history = new List<Zug>();

        //Match vs AI
        public bool playerIsWhite;
        public bool Turnier_weißamzug = true;
        public bool isThinking = false;
        public bool matchMode = false;

        public int weiß_Minuten = 10;
        public int weiß_Sekunden;
        public int weiß_zSekunden;

        public int schwarz_Minuten = 10;
        public int schwarz_Sekunden;
        public int schwarz_zSekunden;
        public Form1()
        {
            InitializeComponent();
        }
        private async void playMove()
        {

            isThinking = true;
            int gestime;
            if (playerIsWhite)
            {
                gestime = schwarz_Minuten * 600 + schwarz_Sekunden * 10 + schwarz_zSekunden;
            }
            else
            {
                gestime = weiß_Minuten * 600 + weiß_Sekunden * 10 + weiß_zSekunden;
            }
            int thinktime = Math.Min(gestime / 20, 100);
            Zug Computerzug = await instance.PlayMove(thinktime); 
            label26.Text = instance.Evalve.ToString();
            label28.Text = instance.Suchtiefe.ToString();
            instance.MakeMove(Computerzug);
            history.Add(Computerzug);
            UpdateUI();
            Turnier_weißamzug = !Turnier_weißamzug;
            isThinking = false;

        }
        public void InitalizeMatch(int Minuten, bool isWhite)
        {
            instance.FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            history.Clear();
            playerIsWhite = isWhite;
            Turnier_weißamzug = true;
            weiß_Minuten = Minuten;
            weiß_Sekunden = 0;
            weiß_zSekunden = 0;

            schwarz_Minuten = Minuten;
            schwarz_Sekunden = 0;
            schwarz_zSekunden = 0;
            label33.Text = schwarz_Minuten.ToString("00") + ":" + schwarz_Sekunden.ToString("00") + "." + schwarz_zSekunden.ToString(); //Schwarze Zeit wird aktualisiert
            button1.Enabled = false; //Darfst keine Züge zurücknehmen, hehe
            button2.Enabled = false; //Darfst keine Stellung laden
            button3.Enabled = false; //Darfst nicht den Computerzug machen 
            checkBox1.Enabled = false; //Darfst nicht das Brett drehen
            matchMode = true;

            UpdateUI();
            timer1.Start();
            if (!playerIsWhite)
            {
                checkBox1.Checked = true;
                invertion = true;
                playMove();
            } else
            {
                checkBox1.Checked = false;
                invertion = false;
                UpdateUI();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            instance.FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            label26.Text = instance.Bewertung(0).ToString();
            UpdateUI();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int i;
            //Schachbrettfelder zeichnen
            Rectangle Feld;
            for (i = 0; i <= 63; i++)
            {
                int j = 7 - i / 8;
                if ((i + j) % 2 == 0)
                {
                    Feld = new Rectangle(i % 8 * 100, j * 100, 100, 100);
                    e.Graphics.FillRectangle(Brushes.White, Feld);
                }
                else
                {
                    Feld = new Rectangle(i % 8 * 100, j * 100, 100, 100);
                    e.Graphics.FillRectangle(Brushes.DarkCyan, Feld);
                }
                //ausgewählte Figur wird grau makiert
                if (i == Input.Startfeld & pieceSelected)
                {

                    int place;
                    if (invertion)
                    {
                        place = 63 - i;
                    }
                    else
                    {
                        place = i;
                    }
                    j = 7 - place / 8;
                    Feld = new Rectangle(place % 8 * 100, j * 100, 100, 100);
                    e.Graphics.FillRectangle(Brushes.Gray, Feld);
                }
            }
            //mögliche Felder auf die die Figur ziehen kann werden rot markiert
            if (pieceSelected)
            {
                foreach (Zug possibleInput in instance.LegalMoveGenerator(false))
                {
                    if (possibleInput.Startfeld == Input.Startfeld)
                    {
                        int x = possibleInput.Zielfeld;
                        if (invertion) x = 63 - x;
                        int y = 7 - x / 8;
                        if ((x + y) % 2 == 0)
                        {
                            Feld = new Rectangle(x % 8 * 100, y * 100, 100, 100);
                            e.Graphics.FillRectangle(Brushes.Red, Feld);
                        }
                        else
                        {
                            Feld = new Rectangle(x % 8 * 100, y * 100, 100, 100);
                            e.Graphics.FillRectangle(Brushes.DarkRed, Feld);
                        }
                    }
                }
            }
            //Zeichnen der Figuren
            for (i = 0; i <= 63; i++)
            {
                int place = 0;
                if (invertion)
                {
                    place = 63 - i;
                }
                else
                {
                    place = i;
                }
                float j = 7 - place / 8;
                switch (instance.Figuren[i].Art)
                {
                    case 1:
                        e.Graphics.DrawImage(Properties.Resources.Bauer_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 2:
                        e.Graphics.DrawImage(Properties.Resources.Springer_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 3:
                        e.Graphics.DrawImage(Properties.Resources.Läufer_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 4:
                        e.Graphics.DrawImage(Properties.Resources.Turm_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 5:
                        e.Graphics.DrawImage(Properties.Resources.Dame_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 6:
                        e.Graphics.DrawImage(Properties.Resources.König_Weiß, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 7:
                        e.Graphics.DrawImage(Properties.Resources.Bauer_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 8:
                        e.Graphics.DrawImage(Properties.Resources.Springer_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 9:
                        e.Graphics.DrawImage(Properties.Resources.Läufer_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 10:
                        e.Graphics.DrawImage(Properties.Resources.Turm_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 11:
                        e.Graphics.DrawImage(Properties.Resources.Dame_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 12:
                        e.Graphics.DrawImage(Properties.Resources.König_schwarz, place % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isThinking)
                return;
            bool makedTurn = false;
            //Lokalisierung des Klicks auf dem Brett = Welches Feld wird angeklickt?
            Point p = PointToClient(Cursor.Position);
            float MousePosx = p.X / 100;
            float MousePosy = 8 - p.Y / 100;
            int Feldx = (int)Math.Round(MousePosx);
            int Feldy = (int)Math.Round(MousePosy) - 1;
            int Feld = 8 * Feldy + Feldx;
            if (invertion)
            {
                Feld = 63 - Feld;
            }
            //Ziehen einer Figur
            if (pieceSelected & (instance.Figuren[Feld].Farbe != instance.Figuren[Input.Startfeld].Farbe))
            {
                Input.Zielfeld = Feld;
                List<Zug> LegaleZüge = instance.LegalMoveGenerator(false);
                if (LegaleZüge.Any(move => move.Zielfeld == Input.Zielfeld & (move.Startfeld == Input.Startfeld)))
                {
                    Input = LegaleZüge[LegaleZüge.FindLastIndex(move => move.Zielfeld == Input.Zielfeld & (move.Startfeld == Input.Startfeld))];
                    if ((instance.Figuren[Input.Startfeld].Type == 1) && (Input.Zielfeld < 8 || Input.Zielfeld >= 56))
                    {
                        if (radioButton1.Checked) Input.Promotion = 1;
                        if (radioButton2.Checked) Input.Promotion = 2;
                        if (radioButton3.Checked) Input.Promotion = 3;
                        if (radioButton4.Checked) Input.Promotion = 4;
                    }
                    pieceSelected = false;
                    history.Add(Input);
                    instance.MakeMove(Input);
                    label26.Text = instance.Bewertung(0).ToString();
                    if (matchMode)
                    {
                        makedTurn = true;
                        Turnier_weißamzug = !Turnier_weißamzug;
                    }
                }
                else
                {
                    pieceSelected = false;
                }
            }
            else
            {
                if ((instance.weißamzug & instance.Figuren[Feld].Farbe == Farbe.Weiß) || (!instance.weißamzug & instance.Figuren[Feld].Farbe == Farbe.Schwarz))
                {
                    pieceSelected = true;
                    Input.Startfeld = Feld;
                }
            }
            UpdateUI();
            if (makedTurn)
            {
                playMove();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (history.Count == 0)
            {
                MessageBox.Show("Es gibt keine Züge zum zurücknehmen");
                return;
            }
            history.RemoveAt(history.Count - 1);
            instance.UnmakeMove();
            UpdateUI();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            instance.FenParser(textBox1.Text);
            UpdateUI();
        }
        public void UpdateUI()
        {
            pictureBox1.Refresh();
            if (instance.enpassant_Index == -1)
            {
                instance.enpassant = "none";
            }
            else
            {
                instance.enpassant = PGN.IndexToSquare(instance.enpassant_Index);
            }
            label5.Text = instance.zugnr.ToString();
            label11.Text = instance.hasCastlingRight(0).ToString();
            label12.Text = instance.hasCastlingRight(1).ToString();
            label13.Text = instance.hasCastlingRight(2).ToString();
            label14.Text = instance.hasCastlingRight(3).ToString();
            label16.Text = instance.enpassant;
            label18.Text = instance.fzregel.ToString();
            label20.Text = instance.weißamzug.ToString();
            label23.Text = instance.blackKingPos.ToString();
            label24.Text = instance.whiteKingPos.ToString();
            UpdateHistory();
            string win = instance.WinChecker(false);
            if (win != "")
            {
                MessageBox.Show(win);
                if (matchMode)
                {
                    EndMatch();
                }
            }
        }
        private void EndMatch() {             
            matchMode = false;
            timer1.Stop();
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            checkBox1.Enabled = true;
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            Zug Computerzug = await instance.PlayMove(20); // 20 Zehntel als Standardzeit
            label26.Text = instance.Evalve.ToString();
            label28.Text = instance.Suchtiefe.ToString();
            history.Add(Computerzug);
            instance.MakeMove(Computerzug);
            UpdateUI();
            button3.Enabled = true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            List<Zug> captures = instance.LegalMoveGenerator(true);
            MessageBox.Show(captures.Count.ToString());
            foreach (Zug capture in captures)
            {
                MessageBox.Show(PGN.IndexToSquare(capture.Startfeld) + "-" + PGN.IndexToSquare(capture.Zielfeld));
            }
        }
        private void UpdateHistory()
        {
            richTextBox1.Text = PGN.ToPGN(history);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int tiefe))
            {
                instance.timer.Interval = 10;
                richTextBox2.Text = "";
                instance.zSekunden = 0;
                instance.timer.Start();
                for (int i = 0; i < tiefe; i++)
                {
                    ulong nodes = instance.Perft(i);
                    richTextBox2.Text += $"Tiefe {i + 1}: Nodes: {nodes} Zeit: {instance.zSekunden}\n";
                }
                instance.timer.Stop();
                instance.timer.Interval = 100;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            invertion = !invertion;
            UpdateUI();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = PointToClient(Cursor.Position);
            float MousePosx = p.X / 100;
            float MousePosy = 8 - p.Y / 100;
            int Feldx = (int)Math.Round(MousePosx);
            int Feldy = (int)Math.Round(MousePosy) - 1;
            int Feld = 8 * Feldy + Feldx;
            if (e.Button == MouseButtons.Right)
            {
                if (instance.SquareInCheck(Feld))
                {
                    MessageBox.Show("Feld im Schach");
                }
                else
                {
                    MessageBox.Show("Feld nicht im Schach");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 form2 = new(this);
            form2.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Turnier_weißamzug)
            {
                weiß_zSekunden -= 1;
                if (weiß_zSekunden < 0)
                {
                    weiß_Sekunden -= 1;
                    weiß_zSekunden = 9;
                }
                if (weiß_Sekunden < 0)
                {
                    weiß_Minuten -= 1;
                    weiß_Sekunden = 59;
                }
                label32.Text = weiß_Minuten.ToString("00") + ":" + weiß_Sekunden.ToString("00") + "." + weiß_zSekunden.ToString();
                if (weiß_Minuten <= 0 & weiß_Sekunden <= 0 & weiß_zSekunden <= 0)
                {
                    EndMatch();
                    MessageBox.Show("Weiß hat verloren!");                   
                }
            }
            else
            {
                schwarz_zSekunden -= 1;
                if (schwarz_zSekunden < 0)
                {
                    schwarz_Sekunden -= 1;
                    schwarz_zSekunden = 9;
                }
                if (schwarz_Sekunden < 0)
                {
                    schwarz_Minuten -= 1;
                    schwarz_Sekunden = 59;
                }
                label33.Text = schwarz_Minuten.ToString("00") + ":" + schwarz_Sekunden.ToString("00") + "." + schwarz_zSekunden.ToString();
                if (schwarz_Minuten <= 0 & schwarz_Sekunden <= 0 & schwarz_zSekunden <= 0)
                {
                    EndMatch();
                    MessageBox.Show("Schwarz hat verloren!");              
                }
            }
        }
    }
}


