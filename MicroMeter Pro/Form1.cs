using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace MicroMeter_Pro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbox_waga.KeyPress += TylkoLiczbyIZnakDziesietny;
            tbox_wzrost.KeyPress += TylkoLiczbyIZnakDziesietny;
            UpdateAll();
        }

        private void guna2CustomRadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void ObliczBMI()
        {
            double waga = 0;
            double wzrostCm = 0;
            double wiek = 0;

            double.TryParse(tbox_waga.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out waga);

            double.TryParse(tbox_wzrost.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wzrostCm);

            double.TryParse(tbox_wiek.Text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out wiek);

            if (waga <= 0 || wzrostCm <= 0)
            {
                label16_BMI.Text = "0";
                return;
            }

            double wzrostM = wzrostCm / 100.0;
            double bmi = waga / (wzrostM * wzrostM);

            label16_BMI.Text = bmi.ToString("0.00");
            InterpretujBMI(bmi);

        }

        private void TylkoLiczbyIZnakDziesietny(object sender, KeyPressEventArgs e)
        {
            var tb = sender as Guna.UI2.WinForms.Guna2TextBox;
            if (tb == null)
                return;

            // Backspace, delete itp.
            if (char.IsControl(e.KeyChar))
                return;

            // Cyfry
            if (char.IsDigit(e.KeyChar))
                return;

            // Jeden separator dziesiętny
            if ((e.KeyChar == ',' || e.KeyChar == '.') &&
                !tb.Text.Contains(",") &&
                !tb.Text.Contains("."))
            {
                return;
            }

            // Reszta blokowana
            e.Handled = true;
        }

        private void tbox_waga_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void tbox_wzrost_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void tbox_wiek_TextChanged(object sender, EventArgs e)
        {
            ObliczBMI();
            ObliczPPM();
        }

        private void InterpretujBMI(double bmi)
        {
            if (bmi <= 0)
            {
                label16_BMI.ForeColor = Color.Black;
                label16_BMI.Text = "0";
                return;
            }

            if (bmi < 18.5)
            {
                label16_BMI.ForeColor = Color.DeepSkyBlue;
                label16_BMI.Text += "  (Niedowaga)";
            }
            else if (bmi < 25)
            {
                label16_BMI.ForeColor = Color.LimeGreen;
                label16_BMI.Text += "  (Norma)";
            }
            else if (bmi < 30)
            {
                label16_BMI.ForeColor = Color.Gold;
                label16_BMI.Text += "  (Nadwaga)";
            }
            else if (bmi < 35)
            {
                label16_BMI.ForeColor = Color.Orange;
                label16_BMI.Text += "  (Otyłość I)";
            }
            else if (bmi < 40)
            {
                label16_BMI.ForeColor = Color.OrangeRed;
                label16_BMI.Text += "  (Otyłość II)";
            }
            else
            {
                label16_BMI.ForeColor = Color.Red;
                label16_BMI.Text += "  (Otyłość III)";
            }
        }

        private void ObliczPPM()
        {
            // parsowanie danych
            double waga, wzrost, wiek;

            if (!double.TryParse(tbox_waga.Text, out waga) ||
                !double.TryParse(tbox_wzrost.Text, out wzrost) ||
                !double.TryParse(tbox_wiek.Text, out wiek))
            {
                label16_ppm.Text = "-";
                return;
            }

            if (waga <= 0 || wzrost <= 0 || wiek <= 0)
            {
                label16_ppm.Text = "-";
                return;
            }

            // płeć
            bool isMale = rb_male.Checked;
            bool isFemale = rb_female.Checked;

            if (!isMale && !isFemale)
            {
                label16_ppm.Text = "-";
                return;
            }

            // wybrany wzór
            if (cbox_wzor.SelectedIndex < 0)
            {
                label16_ppm.Text = "-";
                return;
            }

            double ppm = 0;

            switch (cbox_wzor.SelectedIndex)
            {
                case 0: // Mifflin–St Jeor
                    ppm = isMale
                        ? (10 * waga + 6.25 * wzrost - 5 * wiek + 5)
                        : (10 * waga + 6.25 * wzrost - 5 * wiek - 161);
                    break;

                case 1: // Harris–Benedict
                    ppm = isMale
                        ? (88.362 + 13.397 * waga + 4.799 * wzrost - 5.677 * wiek)
                        : (447.593 + 9.247 * waga + 3.098 * wzrost - 4.330 * wiek);
                    break;

                default:
                    label16_ppm.Text = "-";
                    return;
            }

            label16_ppm.Text = Math.Round(ppm).ToString();
            
        }

        private void rb_male_CheckedChanged(object sender, EventArgs e)
        {
            ObliczPPM();

        }

        private void rb_female_CheckedChanged(object sender, EventArgs e)
        {
            ObliczPPM();
        }

        private void cbox_wzor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObliczPPM();
        }

        private void ObliczCPM()
        {
            // najpierw musimy mieć PPM
            if (!double.TryParse(label16_ppm.Text, out double ppm) || ppm <= 0)
            {
                label16_cpm.Text = "-";
                return;
            }

            // PAL
            if (cbox_pal.SelectedIndex < 0)
            {
                label16_cpm.Text = "-";
                return;
            }

            double pal = 1.2;

            switch (cbox_pal.SelectedIndex)
            {
                case 0: pal = 1.2; break;
                case 1: pal = 1.4; break;
                case 2: pal = 1.6; break;
                case 3: pal = 1.8; break;
                case 4: pal = 2.0; break;
            }

            // cel
            int korekta = 0;

            if (rb_redukcja.Checked)
                korekta = -300;
            else if (rb_tycie.Checked)
                korekta = 300;
            else if (rb_miesniowa.Checked)
                korekta = 500;
            else if (rb_utrzymanie.Checked)
                korekta = 0;
            else
            {
                label16_cpm.Text = "-";
                return;
            }

            double cpm = ppm * pal + korekta;
            label16_cpm.Text = Math.Round(cpm).ToString();
        }

        private void cbox_pal_SelectedIndexChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void rb_utrzymanie_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void rb_redukcja_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void rb_tycie_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void rb_miesniowa_CheckedChanged(object sender, EventArgs e)
        {
            ObliczCPM();

        }

        private void UpdateAll()
        {
            ObliczBMI();
            ObliczPPM();
            ObliczCPM();
        }

    }
}
