using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
//Hi roman TEST
namespace CustomControls
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new HauptForm());
        }
    }

    public class HauptForm : Form
    {
        string Previous = "0";
        string Copy = "0";
        bool calculated;
        private TextBox Input;

        public HauptForm()
        {
            Text = "Taschen Rechner Roman & Jari";
            ClientSize = new Size(400, 500);

            Input = new TextBox
            {
                Text = "0",
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Center,
                Font = new Font("Fredoka One", 24)
            };
            Input.SetBounds(10, 10, 380, 40);
            Controls.Add(Input);


            string[] digits =
            {
                "C", "Del", "/", "*",
                "7", "8", "9", "-",
                "4", "5", "6", "+",
                "1", "2", "3", "D",
                "0", ".", "=", "L",
                "^", "Copy","Paste","Previous"            };

            for (int i = 0; i < digits.Length; i++)
            {

                Button template = new Button
                {
                    Text = digits[i],
                    Font = new Font("Fredoka One", 24),
                    FlatStyle = FlatStyle.Popup
                };
                template.SetBounds(10 + (i % 4) * 100, 100 + (i / 4) * 60, 100, 55);
                template.Click += clicked;
                Controls.Add(template);
            }
        }

        private void clicked(object sender, EventArgs e)
        {
            string t = ((Button)sender).Text;

            if (t == "Previous" && Previous != string.Empty)
            {
                Input.Text = Input.Text + Previous;
                return;
            }

            if (t == "D")
            {
                BackColor = Color.Black;
                Input.BackColor = Color.Black;
                Input.ForeColor = Color.White;

                foreach (Control c in Controls)
                {
                    if (c is Button b)
                    {
                        b.BackColor = Color.Black;
                        b.ForeColor = Color.White;
                    }
                }
                return;
            }

            if (t == "L")
            {
                BackColor = Color.White;
                Input.BackColor = Color.White;
                Input.ForeColor = Color.Black;

                foreach (Control c in Controls)
                {
                    if (c is Button b)
                    {
                        b.BackColor = Color.White;
                        b.ForeColor = Color.Black;
                    }
                }
                return;
            }

            if (t == "Paste")
            {
                string text = Clipboard.GetText().Trim().Replace(',', '.');

                if (Regex.IsMatch(text, @"^[\d+\-*/^.]+$"))
                {
                    bool beginntMitOperator = Regex.IsMatch(text, @"^[+*/^]");

                    if (Input.Text == "Fehler" ||
                        ((Input.Text == "0" || calculated) && !beginntMitOperator))
                    {
                        Input.Text = text;
                    }
                    else
                    {
                        Input.Text += text;
                    }

                    calculated = false;
                }
                return;
            }

            if (t == "Copy")
            {
                Clipboard.SetText(Input.Text);
                return;
            }

            if (Input.Text == "Fehler")
            {
                Input.Text = "0";
                calculated = false;
            }

            if (calculated)
            {
                calculated = false;
                if (t.Length == 1 && char.IsDigit(t[0]) || t == ".")
                {
                    Input.Text = "0";
                }
            }


            if (t == "+" || t == "-" || t == "*" || t == "/" || t == "^")

            {

                if (Input.Text.Length == 0)

                {

                    if (t != "-")

                        return;

                }

                else
                {

                    char letztes = Input.Text[Input.Text.Length - 1];


                    if ("+-*/^".Contains(letztes))

                    {

                        if (t != "-")

                            return;


                        if (letztes == '-')

                        {

                            if (Input.Text.Length < 2)

                                return;


                            char vorletztes = Input.Text[Input.Text.Length - 2];


                            if ("+-*/^".Contains(vorletztes))

                                return;

                        }

                    }

                }

            }

            if (Input.Text == "0" && t.Length == 1 && char.IsDigit(t[0]))
            {
                Input.Text = "";
            }

            if (t == ".")
            {
                string aktuelleZahl = Regex.Match(Input.Text, @"[\d.]*$").Value;
                if (aktuelleZahl.Contains("."))
                {
                    return;
                }
                if (aktuelleZahl == "")
                {
                    Input.Text += "0.";
                    return;
                }
            }

            if (t == "Del")
            {
                if (Input.Text.Length > 1)
                {
                    Input.Text = Input.Text.Substring(0, Input.Text.Length - 1);
                    if (Input.Text == "-")
                    {
                        Input.Text = "0";
                    }
                }
                else
                {
                    Input.Text = "0";
                }
                return;
            }

            if (t == "C")
            {
                Input.Text = "0";
                return;
            }

            if (t == "=")
            {
                MatchCollection treffer = Regex.Matches(Input.Text, @"(?<![\d.,])-?\d+([.,]\d+)?|[+\-*/^]");

                List<decimal> zahlen = new List<decimal>();
                List<string> operatoren = new List<string>();

                foreach (Match m in treffer)
                {
                    if (Regex.IsMatch(m.Value, @"^[+\-*/^]$"))
                    {
                        operatoren.Add(m.Value);
                    }
                    else
                    {
                        zahlen.Add(decimal.Parse(m.Value.Replace(',', '.'), CultureInfo.InvariantCulture));
                    }
                }

                if (zahlen.Count == 0 || zahlen.Count != operatoren.Count + 1)
                {
                    Input.Text = "Fehler";
                    return;
                }

                int p = 0;
                while (p < operatoren.Count)
                {
                    if (operatoren[p] == "^")
                    {
                        zahlen[p] = (decimal)Math.Pow((double)zahlen[p], (double)zahlen[p + 1]);
                        zahlen.RemoveAt(p + 1);
                        operatoren.RemoveAt(p);
                    }
                    else
                    {
                        p++;
                    }
                }

                int i = 0;
                while (i < operatoren.Count)
                {
                    if (operatoren[i] == "*" || operatoren[i] == "/")
                    {
                        if (operatoren[i] == "/" && zahlen[i + 1] == 0)
                        {
                            Input.Text = "Fehler";
                            return;
                        }

                        decimal ergebnis = operatoren[i] == "*" ? zahlen[i] * zahlen[i + 1]
                            : zahlen[i] / zahlen[i + 1];

                        zahlen[i] = ergebnis;
                        zahlen.RemoveAt(i + 1);
                        operatoren.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                decimal summe = zahlen[0];
                for (int j = 0; j < operatoren.Count; j++)
                {
                    summe = operatoren[j] == "+" ? summe + zahlen[j + 1] : summe - zahlen[j + 1];
                }

                Input.Text = summe.ToString(CultureInfo.InvariantCulture);
                Previous = Input.Text;
                calculated = true;
                return;
            }

            if (t == "2" && Input.Text != "0" && Input.Text != string.Empty)
            {

                char letztes = Input.Text[Input.Text.Length - 1];


                if ("^".Contains(letztes))

                {

                    if (Input.Text.Length > 1)

                    {

                        Input.Text = Input.Text.Substring(0, Input.Text.Length - 1);

                        if (Input.Text == "-")

                        {

                            Input.Text = "0";

                        }

                    }

                    else
                    {

                        Input.Text = "0";

                    }


                    t = "²";

                }

            }

            Input.Text = Input.Text + t;
        }
    }
}