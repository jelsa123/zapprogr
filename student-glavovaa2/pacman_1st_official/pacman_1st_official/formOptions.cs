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
    public partial class optnsForm : Form
    {
        /// <summary>
        /// 
        /// Trieda slúžiaca na zmenu nastavení hry. Obsahuje zmenu obtiažnosti, rýchlosti a typu algoritmu.
        /// 
        /// </summary>
        public static bool gameDifficulty = true; // true = easy, false = hard
        public static int level = 1;
        public static int playerSpeedVal = 6; // default = 6
        public static int currentAlgorithm = 0; // 0 = Dijkstra, 1 = BFS
        public optnsForm()
        {
            InitializeComponent();
        }

        private void btnDifficulty_Click(object sender, EventArgs e)
        {
            // Mení obtiažnosť hry
            if (gameDifficulty)
            {
                btnDifficulty.Text = "HARD";
                btnDifficulty.ForeColor = Color.Red;
                
            }
            else
            {
                btnDifficulty.Text = "EASY";
                btnDifficulty.ForeColor = Color.LawnGreen;
                
            }
            gameDifficulty = !gameDifficulty;
        }

        private void backToMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trackbarPlayerSpd_Scroll(object sender, EventArgs e)
        {
            // Mení rýchlosť hráča
            playerSpeedVal = trackbarPlayerSpd.Value;
        }

        private void btnAlgorithm_Click(object sender, EventArgs e)
        {
            // Prepína typ algoritmu z BFS na Dijkstru
            if (currentAlgorithm == 0)
            {
                currentAlgorithm = 1;
                btnAlgorithm.Text = "BFS";
            }
            else
            {
                currentAlgorithm = 0;
                btnAlgorithm.Text = "DIJKSTRA";
            }
        }
    }
}
