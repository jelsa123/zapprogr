using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pacman_1st_official
{
    public partial class MainMenu : Form
    {
        /// <summary>
        /// 
        /// Hlavné menu, dá sa z neho ísť do Options formy alebo do samotnej hry
        /// 
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Tlačítko na prepnutie sa do hry
            pacmanGame formG = new pacmanGame();
            formG.ShowDialog();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            // Tlačítko na prepnutie sa do options menu
            optnsForm formO = new optnsForm();
            formO.ShowDialog();
        }
    }
}
