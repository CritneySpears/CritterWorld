﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCG.TurboSprite;

namespace CritterWorld
{
    public partial class CritterScorePanel : UserControl
    {
        private void UpdateScore(int currentScore, int overallScore)
        {
            labelScore.Text = currentScore + "/" + overallScore;
        }

        public CritterScorePanel(Critter critter)
        {
            InitializeComponent();

            // TODO - set image of critter in score area here

            Timer timer = new Timer
            {
                Interval = 500
            };
            timer.Tick += (e, evt) =>
            {
                UpdateScore(critter.CurrentScore, critter.OverallScore);
                progressBarHealth.Value = critter.Health;
                progressBarEnergy.Value = critter.Energy;
                if (critter.IsEscaped)
                {
                    labelEscaped.Visible = true;
                    UpdateScore(critter.CurrentScore, critter.OverallScore);
                    timer.Stop();
                }
                if (critter.IsDead)
                {
                    labelDead.Visible = true;
                    timer.Stop();
                }
            };
            timer.Start();
        }
    }
}