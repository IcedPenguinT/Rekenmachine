using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rekenmachine2
{
    public partial class rForm : MetroFramework.Forms.MetroForm
    {
        //Variabelen
        public decimal beginGetal;
        public decimal werkGetal;
        public string getalHandler;
        public bool resetCalcTextbox = false;
        public bool resetCalcTextboxUpper = false;
        public bool firstTime = true;
        public bool opToEqual = true;
        public bool clickedOpperator = false;
        public bool equalClicked = false;
        public bool calcMode = false;
        public bool switchOff = false;
        public bool equalCalcModePressed = false;
        public string[] lines;
        public List<string> historyLines = new List<string>();
        MemoryRekenmachine memoryRekenmachiene = new MemoryRekenmachine();
        public rForm()
        {
            InitializeComponent();
            //select label zodat numpad werkt.
            btnZero.Select();
        }

        private void btnNumber_Click(object sender, EventArgs e)
        {
            // als er op is is gedrukt reset getal handler en begin getal anders worden
            // deze gebruikt voor de calculatie.
            if (equalClicked)
            {
                equalCalcModePressed = false;
                equalClicked = false;
                clickedOpperator = false;
                getalHandler = "";
                beginGetal = 0;
                if (resetCalcTextbox)
                    resetTextbox();
            }
            else 
            {
                // als resetCalcTextbox true is reset de textbox
                if (resetCalcTextbox)
                    resetTextbox();
            }

            // update textbox nummers.
            if (tbxCalc.Lines[3] != "0")
            {
                lines = tbxCalc.Lines;
                if (lines[3].Length < 10)
                    lines[3] += ((Button)sender).Text;
                else
                    lines[3] = (lines[3].Substring(0, lines[3].Length - 1) + ((Button)sender).Text);
            }
            else
            {
                lines = tbxCalc.Lines;
                lines[3] = ((Button)sender).Text;
            }
            tbxCalc.Lines = lines;
        }

        private void btnOperators_Click(object sender, EventArgs e)
        {
            // alle operatoren (+, - , *, etc.)
            resetCalcTextboxUpper = false;
            equalCalcModePressed = false;
            equalClicked = false;
            clickedOpperator = true;
            opToEqual = true;
            resetCalcTextbox = true;
            // eerste keer (want door kunnen rekenen met eerdere som)
            if (firstTime)
            {
                firstTime = false;
                beginGetal = decimal.Parse(tbxCalc.Lines[3]);
                getalHandler = ((Button)sender).Text;
                UpdateTbx($"{Math.Round(beginGetal, 2)} {getalHandler}", beginGetal.ToString());
            }
            // door rekenen met eerste som.
            else if (!calcMode)
            {
                werkGetal = decimal.Parse(tbxCalc.Lines[3]);
                beginGetal = RekenFuncties.AntwoordSom(getalHandler, beginGetal, werkGetal);
                getalHandler = ((Button)sender).Text;
                UpdateTbx($"{Math.Round(beginGetal, 2)} {getalHandler}", beginGetal.ToString());
                switchOff = true;
            }
            else
            {
                werkGetal = decimal.Parse(tbxCalc.Lines[3]);
                getalHandler = ((Button)sender).Text;
                lines = tbxCalc.Lines;
                lines[0] += $" {Math.Round(werkGetal, 2)} {getalHandler}";
                tbxCalc.Lines = lines;
                switchOff = false;
            }
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            // druk op is teken.
            resetCalcTextboxUpper = true;
            resetCalcTextbox = true;
            firstTime = true;
            equalClicked = true;
            switchOff = true;
            decimal oudBeginGetal = beginGetal;
            // als er geen nummer is ingevuld;
            if (beginGetal == 0 && !clickedOpperator)
            {
                beginGetal = decimal.Parse(tbxCalc.Lines[3]);
                UpdateTbx($"{Math.Round(beginGetal, 2)} =", beginGetal.ToString());
                UpdateHistory($"{Math.Round(beginGetal, 2)} =", lines[3]);
                beginGetal = 0;
            }
            else if(calcMode)
            {
                if (equalCalcModePressed == true)
                    return;
                equalCalcModePressed = true;
                werkGetal = decimal.Parse(tbxCalc.Lines[3]);
                lines = tbxCalc.Lines;
                lines[0] += ($" {Math.Round(werkGetal, 2)}");
                lines[3] = RekenFuncties.CalcMoreNumbers(lines[0]).ToString();
                lines[0] += (" =");
                tbxCalc.Lines = lines;
                UpdateHistory(lines[0], lines[3]);
            }
            else if(werkGetal != 0 && !opToEqual)
            {
                // telkens op = blijven drukken.
                beginGetal = RekenFuncties.AntwoordSom(getalHandler, beginGetal, werkGetal);
                UpdateTbx($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", beginGetal.ToString());
                UpdateHistory($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", lines[3]);
            }
            else if (opToEqual)
            {
                // meerdere nummers eerste keer = klikken.
                opToEqual = false;
                werkGetal = decimal.Parse(tbxCalc.Lines[3]);
                beginGetal = RekenFuncties.AntwoordSom(getalHandler, beginGetal, werkGetal);
                UpdateTbx($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", beginGetal.ToString());
                UpdateHistory($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", lines[3]);
            }
            else
            {
                // als andere specifieke statements niet overeen komen
                werkGetal = decimal.Parse(tbxCalc.Lines[3]);
                beginGetal = RekenFuncties.AntwoordSom(getalHandler, beginGetal, werkGetal);
                UpdateTbx($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", beginGetal.ToString());
                UpdateHistory($"{Math.Round(oudBeginGetal, 2)} {getalHandler} {Math.Round(werkGetal, 2)} =", lines[3]);
            }
        }

        private void btnOperatorsSpecial_Click(object sender, EventArgs e)
        {
            // speciale operatoren zoals tot de macht en wortels.
            string oudGetal = tbxCalc.Lines[3];
            // voor als de is knop is ingedrukt.
            if (resetCalcTextboxUpper)
            {
                UpdateTbx("", oudGetal);
                resetCalcTextboxUpper = false;
            }
            // +/- werkt net iets in de textbox.
            if (((Button)sender).Text == "+/-x")
            {
                UpdateTbx(tbxCalc.Lines[0], RekenFuncties.OperatorSpecial
                    (((Button)sender).Text, decimal.Parse(tbxCalc.Lines[3])).ToString());
            }
            else if (clickedOpperator)
            {
                // als er al een operator is aangeklikt.
                UpdateTbx(tbxCalc.Lines[0], RekenFuncties.OperatorSpecial
                    (((Button)sender).Text, decimal.Parse(tbxCalc.Lines[3])).ToString());
            }
            else
            {
                // Anders.
                UpdateTbx
                    ($"{((Button)sender).Text.Replace("x", Math.Round(decimal.Parse(oudGetal), 2).ToString())}",
                    RekenFuncties.OperatorSpecial(((Button)sender).Text, decimal.Parse(tbxCalc.Lines[3])).ToString());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // delet buttons.
            switch(((Button)sender).Text)
            {
                case "CE":
                    // clear onderste line.
                    lines = tbxCalc.Lines;
                    lines[3] = "0";
                    tbxCalc.Lines = lines;
                    break;
                case "C":
                    // clear hele textbox.
                    ResetVariabelen();
                    break;
                case "DEL":
                    // haal laatste nummer weg onderste line.
                        lines = tbxCalc.Lines;
                        lines[3] = lines[3].Remove(lines[3].Length - 1);
                        if (lines[3].Length == 0)
                        {
                            lines[3] = "0";
                        }
                        tbxCalc.Lines = lines;
                    break;
            }
        }
        
        public void ResetVariabelen()
        {
            // reset alle variabelen.
            beginGetal = 0;
            werkGetal = 0;
            getalHandler = "";
            resetCalcTextbox = false;
            resetCalcTextboxUpper = false;
            firstTime = true;
            opToEqual = true;
            clickedOpperator = false;
            lines = tbxCalc.Lines;
            lines[0] = "";
            lines[3] = "0";
            tbxCalc.Lines = lines;
            equalClicked = false;
            equalCalcModePressed = false;
        }

        private void btnMemory_Click(object sender, EventArgs e)
        {
            // memory buttons
            decimal x = decimal.Parse(tbxCalc.Lines[3]);
            switch(((Button)sender).Text)
            {
                case "M+":
                    // voeg toe aan memory getal. memory + getal = memory.
                    MemoryRekenmachine.MemoryAdd(memoryRekenmachiene, x);
                    resetCalcTextbox = true;
                    break;
                case "M-":
                    // afhalen van memory.
                    MemoryRekenmachine.MemorySubtract(memoryRekenmachiene, x);
                    resetCalcTextbox = true;
                    break;
                case "MC":
                    // leeg het memory.
                    MemoryRekenmachine.MemoryClear(memoryRekenmachiene);
                    resetCalcTextbox = true;
                    break;
                case "MS":
                    // save getal als memory
                    MemoryRekenmachine.MemorySave(memoryRekenmachiene, x);
                    resetCalcTextbox = true;
                    break;
                case "MR":
                    // lees het getal in de memory en laad deze.
                    lines = tbxCalc.Lines;
                    lines[3] = MemoryRekenmachine.MemoryRead(memoryRekenmachiene).ToString();
                    tbxCalc.Lines = lines;
                    resetCalcTextbox = true;
                    break;
            }
        }

        private void btnResetGeschiedenis_Click(object sender, EventArgs e)
        {
            // clear geschiedenis.
            historyLines.Clear();
            tbxHistory.Lines = historyLines.ToArray();
        }

        public void resetTextbox()
        {
            // reset textbox zodat hij weer werkt met onze event handlers.
            lines = tbxCalc.Lines;
            if (resetCalcTextboxUpper)
            {
                lines[0] = "";
            }
            lines[3] = "0";
            tbxCalc.Lines = lines;
            resetCalcTextbox = false;
            resetCalcTextboxUpper = false;
        }

        private void History_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // kijk welke key op het toetsenbord wordt ingedrukt.
            switch (e.KeyData)
            {
                case Keys.NumPad0:
                    btnZero.PerformClick();
                    break;
                case Keys.NumPad1:
                    btnOne.PerformClick();
                    break;
                case Keys.NumPad2:
                    btnTwo.PerformClick();
                    break;
                case Keys.NumPad3:
                    btnThree.PerformClick();
                    break;
                case Keys.NumPad4:
                    btnFour.PerformClick();
                    break;
                case Keys.NumPad5:
                    btnFive.PerformClick();
                    break;
                case Keys.NumPad6:
                    btnSix.PerformClick();
                    break;
                case Keys.NumPad7:
                    btnSeven.PerformClick();
                    break;
                case Keys.NumPad8:
                    btnEight.PerformClick();
                    break;
                case Keys.NumPad9:
                    btnNine.PerformClick();
                    break;
                case Keys.Add:
                    btnAdd.PerformClick();
                    break;
                case Keys.Subtract:
                    btnSubtract.PerformClick();
                    break;
                case Keys.Divide:
                    btnDevide.PerformClick();
                    break;
                case Keys.Multiply:
                    btnTimes.PerformClick();
                    break;
                case Keys.Enter:
                    btnEquals.PerformClick();
                    break;
                case Keys.Back:
                    btnBackspace.PerformClick();
                    break;

            }
        }

        private void btnSwapCalcMethode_Click(object sender, EventArgs e)
        {
            if (switchOff)
                calcMode = !calcMode;
        }

        public void UpdateHistory(string line1, string line3)
        {
            historyLines.Insert(0, line3);
            historyLines.Insert(0, line1);
            if (historyLines.Count >= 24)
            {
                historyLines.RemoveRange(historyLines.Count - 2, 2);
            }
            tbxHistory.Lines = historyLines.ToArray();
        }

        public void UpdateTbx(string line0, string line3)
        {
            lines = tbxCalc.Lines;
            lines[0] = line0;
            lines[3] = line3;
            tbxCalc.Lines = lines;
        }
    }


    /*public enum Operator
    Geen idee hoe ik het kon toepassen zonder de code uit te breiden.
        plus,
        min,
        keer,
        delen,
        macht,
        wortel,
        deelVanEen,
        module
    }*/
    public class MemoryRekenmachine
    {
        // Memory class.
        public decimal memory = 0;

        public static void MemorySave(MemoryRekenmachine memoryRekenmachiene, decimal save)
        {
            memoryRekenmachiene.memory = save;
        }
        public static void MemoryAdd(MemoryRekenmachine memoryRekenmachiene, decimal add)
        {
            memoryRekenmachiene.memory += add;
        }
        public static void MemorySubtract(MemoryRekenmachine memoryRekenmachiene, decimal subtract)
        {
            memoryRekenmachiene.memory -= subtract;
        }
        public static decimal MemoryRead(MemoryRekenmachine memoryRekenmachiene)
        {
            return memoryRekenmachiene.memory;
        }
        public static void MemoryClear(MemoryRekenmachine memoryRekenmachiene)
        {
            memoryRekenmachiene.memory = 0;
        }
    }

    public class RekenFuncties
    {
        // Reken class.
        public static decimal AntwoordSom(string handler, decimal gtl1, decimal gtl2)
        {
            decimal antw;
            try
            {
                switch (handler)
                {
                    // normale reken functies.
                    case "%":
                        antw = gtl1 % gtl2;
                        return antw;
                    case "/":
                        try
                        {
                            antw = gtl1 / gtl2;
                        }
                        catch (DivideByZeroException)
                        {
                            antw = 0;
                        }
                        return antw;
                    case "x":
                        antw = gtl1 * gtl2;
                        return antw;
                    case "-":
                        antw = gtl1 - gtl2;
                        return antw;
                    case "+":
                        antw = gtl1 + gtl2;
                        return antw;
                    default:
                        return gtl1;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static decimal OperatorSpecial(string handler, decimal gtl1)
        {
            // special reken functie.
            decimal ans;
            switch (handler)
            {
                case "+/-x":
                    ans = gtl1 * -1;
                    return ans;
                case "1/x":
                    ans = 1 / gtl1;
                    return ans;
                case "x²":
                    ans = gtl1 * gtl1;
                    return ans;
                case "√x":
                    ans = (decimal)Math.Sqrt(decimal.ToDouble(gtl1));
                    return ans;
                default:
                    return gtl1;
            }
        }
        public static decimal CalcMoreNumbers(string sum)
        {
            decimal x = 0;
            sum = sum.Replace("x", "*");
            DataTable dt = new DataTable();
            var v = dt.Compute(sum, "");
            x = Convert.ToDecimal(v);
            return x; 
        }
    }
}
