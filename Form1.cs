using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Rows = 8;
            Columns = 10;
            //InitializeComponent();
            InitializeComponent2();
        }

        public static int Rows { get; set; }
        public static int Columns { get; set; }

        public static int BombCount = 10;

        private classButton[,] _buttons;

        Label output = new Label();

        private Button newGame = new Button();


        private void InitializeComponent2()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();

            _buttons = new classButton[Rows, Columns];

            SetupBoard();

            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = Columns;
            for (int c = 0; c < Columns; c++)
                this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F / Columns));

            for (int r = 0; r < Rows; r++)
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F / Rows));

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    this.tableLayoutPanel1.Controls.Add(_buttons[r, c], c, r);

            output.Name = "OutputLabel";
            output.Text = " ";
            output.AutoSize = true;

            newGame.Name = "NewGame";
            newGame.Text = "New Game";
            newGame.AutoSize = true;
            newGame.Click += new EventHandler(this.newGame_Click);

            this.tableLayoutPanel1.Controls.Add(newGame);
            this.tableLayoutPanel1.Controls.Add(output);

            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = Rows;

            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;




            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
        }

        private void newGame_Click(object? sender, EventArgs e)
        {
            cellsFound = 0;
            this.Controls.Clear();
            this.InitializeComponent2();
            
        }

        private void SetupBoard()
        {
            var bombLocations = GetBombLocations();

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    _buttons[r, c] = new classButton();
                    _buttons[r, c].Dock = DockStyle.Fill;
                    _buttons[r, c].TabIndex = r * Columns + c;

                    //We set up some random squares to be bombs
                    var currentPair = new KeyValuePair<int, int>(r, c);
                    if (bombLocations.Contains(currentPair))
                    {
                        _buttons[r, c].HiddenText = "\uD83D\uDCA3";
                    }

                    _buttons[r, c].Click += new EventHandler(this.btn_click);
                }
            }

            //After bombs are set we will set the rest of the squares with the proper numbers
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    int surroundingBombCount = 0;
                    if (_buttons[r, c].HiddenText != "\uD83D\uDCA3")
                    {
                        for (int row = -1; row <= 1; row++)
                        {
                            for (int col = -1; col <= 1; col++)
                            {
                                //We don't need to check the square against itself
                                if (row == 0 && col == 0)
                                {
                                    continue;
                                }

                                // We skip checking any squares that would be out of bounds
                                if (row + r < 0 || col + c < 0 || row + r >= Rows || col + c >= Columns)
                                {
                                    continue;
                                }

                                if (_buttons[row + r, col + c].HiddenText == "\uD83D\uDCA3")
                                {
                                    surroundingBombCount++;
                                }
                            }
                        }

                        _buttons[r, c].HiddenText = surroundingBombCount.ToString();
                        _buttons[r, c].row = r;
                        _buttons[r, c].col = c;
                    }
                }
            }
        }

        private int cellsFound = 0;

        private void btn_click(object sender, EventArgs e)
        {

            classButton btn = (classButton)sender;

            if (btn.HiddenText == "\uD83D\uDCA3") 
                {
                    btn.showText();
                    output.Text = "You lose.";
                    cellsFound = 0;
                }

                else if (btn.HiddenText != "0")
                {
                    btn.showText();
                    cellsFound++;
                }

                else
                {
                    btn.Enabled = false;
                    cellsFound++;

                    for (int row = -1; row <= 1; row++)
                    {
                        for (int col = -1; col <= 1; col++)
                        {
                            //We don't need to check the square against itself
                            if (row == 0 && col == 0)
                            {
                                continue;
                            }

                            // We skip checking any squares that would be out of bounds
                            if (row + btn.row < 0 || col + btn.col < 0 || row + btn.row >= Rows ||
                                col + btn.col >= Columns)
                            {
                                continue;
                            }

                            if (_buttons[row + btn.row, col + btn.col].isTextShown)
                            {
                                continue;
                            }

                            _buttons[row + btn.row, col + btn.col].PerformClick();
                         
                        }

                    }

                    if (cellsFound >= (Rows * Columns) - BombCount)
                {
                    output.Text = "You Win!";
                }

            }
           
            

        }

        public static List<KeyValuePair<int, int>> GetBombLocations()
        {
            Random rand = new Random();

            var bombLocations = new List<KeyValuePair<int, int>>() { };

            while (bombLocations.Count < BombCount)
            {
                KeyValuePair<int, int> randomPair = new KeyValuePair<int, int>
                    (rand.Next(Rows), rand.Next(Columns));

                if (!bombLocations.Contains(randomPair))
                {
                    bombLocations.Add(randomPair);
                }
            }

            return bombLocations;
        }
    }
    public class classButton : Button
    {

        public string strHiddenText;

        public int row;
        public int col;
        public bool isTextShown = false;
        public string HiddenText
        {
            get { return strHiddenText; }
            set { strHiddenText = value; }
        }

        public void hideText()
        {
            Text = "";
            isTextShown = false;
        }
        public void showText()
        {
            Text = strHiddenText;
            isTextShown = true;
        }


    }


}
