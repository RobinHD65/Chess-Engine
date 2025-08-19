using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        public bool pieceSelected = false;
        static Suche instance = new(); //Objekt Suche erbt von allem
        Zug Input = new Zug();
        List<Zug> history = new List<Zug>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            instance.FenParser("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            instance.Berechne_Randabstand();
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
                    Feld = new Rectangle(i % 8 * 100, j * 100, 100, 100);
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
                float j = 7 - i / 8;
                switch (instance.Figuren[i].Art)
                {
                    case 9:
                        e.Graphics.DrawImage(Properties.Resources.Bauer_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 10:
                        e.Graphics.DrawImage(Properties.Resources.Springer_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 11:
                        e.Graphics.DrawImage(Properties.Resources.Läufer_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 12:
                        e.Graphics.DrawImage(Properties.Resources.Turm_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 13:
                        e.Graphics.DrawImage(Properties.Resources.Dame_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 14:
                        e.Graphics.DrawImage(Properties.Resources.König_Weiß, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 17:
                        e.Graphics.DrawImage(Properties.Resources.Bauer_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 18:
                        e.Graphics.DrawImage(Properties.Resources.Springer_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 19:
                        e.Graphics.DrawImage(Properties.Resources.Läufer_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 20:
                        e.Graphics.DrawImage(Properties.Resources.Turm_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 21:
                        e.Graphics.DrawImage(Properties.Resources.Dame_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                    case 22:
                        e.Graphics.DrawImage(Properties.Resources.König_schwarz, i % 8 * 100, j * 100);
                        e.Dispose();
                        break;
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //Lokalisierung des Klicks auf dem Brett = Welches Feld wird angeklickt?
            Point p = PointToClient(Cursor.Position);
            float MousePosx = p.X / 100;
            float MousePosy = 8 - p.Y / 100;
            int Feldx = (int)Math.Round(MousePosx);
            int Feldy = (int)Math.Round(MousePosy) - 1;
            int Feld = 8 * Feldy + Feldx;
            //Ziehen einer Figur
            if (pieceSelected & (instance.Figuren[Feld].Farbe != instance.Figuren[Input.Startfeld].Farbe))
            {
                Input.Zielfeld = Feld;
                List<Zug> LegaleZüge = instance.LegalMoveGenerator(false);
                if (LegaleZüge.Any(move => move.Zielfeld == Input.Zielfeld & (move.Startfeld == Input.Startfeld)))
                {
                    Input = LegaleZüge[LegaleZüge.FindLastIndex(move => move.Zielfeld == Input.Zielfeld & (move.Startfeld == Input.Startfeld))];
                    if (radioButton1.Checked) Input.Promotion = 1;
                    if (radioButton2.Checked) Input.Promotion = 2;
                    if (radioButton3.Checked) Input.Promotion = 3;
                    if (radioButton4.Checked) Input.Promotion = 4;
                    pieceSelected = false;
                    history.Add(Input);
                    instance.MakeMove(Input);
                    label26.Text = instance.Bewertung(0).ToString();
                }
                else
                {
                    pieceSelected = false;
                }
            }
            else
            {
                if ((instance.weißamzug & instance.Figuren[Feld].Farbe == "Weiß") | (!instance.weißamzug & instance.Figuren[Feld].Farbe == "Schwarz"))
                {
                    pieceSelected = true;
                    Input.Startfeld = Feld;
                }
            }
            UpdateUI();

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                instance.enpassant = instance.IndexToSquare(instance.enpassant_Index);
            }
            label5.Text = instance.zugnr.ToString();
            label11.Text = instance.Rochaderechte[0].ToString();
            label12.Text = instance.Rochaderechte[1].ToString();
            label13.Text = instance.Rochaderechte[2].ToString();
            label14.Text = instance.Rochaderechte[3].ToString();
            label16.Text = instance.enpassant;
            label18.Text = instance.fzregel.ToString();
            label20.Text = instance.weißamzug.ToString();
            label23.Text = instance.blackKingPos.ToString();
            label24.Text = instance.whiteKingPos.ToString();
            UpdateHistory();
            string win = instance.WinChecker();
            if (win != "") MessageBox.Show(win);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Zug Computerzug = instance.PlayMove();
            label26.Text = instance.Evalve.ToString();
            label28.Text = instance.Suchtiefe.ToString();
            history.Add(Computerzug);
            instance.MakeMove(Computerzug);
            UpdateUI();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<Zug> captures = instance.LegalMoveGenerator(true);
            MessageBox.Show(captures.Count.ToString());
            foreach (Zug capture in captures)
            {
                MessageBox.Show(instance.IndexToSquare(capture.Startfeld) + "-" + instance.IndexToSquare(capture.Zielfeld));
            }
        }
        private void UpdateHistory()
        {
            int i = 0;
            richTextBox1.Text = "";
            foreach (Zug zug in history)
            {
                if (i % 2 == 0)
                {
                    richTextBox1.Text += (1 + i / 2).ToString() + ". ";
                }
                richTextBox1.Text += instance.IndexToSquare(zug.Startfeld) + "-" + instance.IndexToSquare(zug.Zielfeld) + " ";
                if (i % 2 == 1)
                {
                    richTextBox1.Text += "\n";
                }
                i++;
            }
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
                    richTextBox2.Text += $"Tiefe {i+1}: Nodes: {nodes} Zeit: {instance.zSekunden}\n";
                }
                instance.timer.Stop();
                instance.timer.Interval = 100;
            }
            
        }
    }
}


