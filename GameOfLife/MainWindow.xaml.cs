// Copyright (c) 2020 Markus Wackermann
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using GameOfLife.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using Serilog.Core;
using System.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        public Matrix TheMatrix;
        public Life LifeCycle;
        public MainWindow()
        {
            InitializeComponent();
            HelperFunctions.PositionWindow(ref MainWindowN); // Set Position of MainWindow
            Log.Logger = new LoggerConfiguration().WriteTo.Debug().CreateLogger(); // Create logging object
            TheMatrix = new Matrix(ref LifeGrid, 8, 16); //Init Rectangles/Universes/Matrix
            TheMatrix.Generate(); // Create Rectangles/Universes/Matrix
            LifeCycle = new Life(ref TheMatrix); // Begin the circle of Life
            LifeCycle.Start();
        }

        private void MainWindowN_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                if (LifeCycle.InProcess)
                {
                    LifeCycle.End();
                    TheMatrix.Universes.All((x) => { x.MouseDown += Universe_MouseDown; return true; });
                }
                else
                {
                    LifeCycle.Start();
                }
            }
            if (e.Key == System.Windows.Input.Key.C)
            {
                TheMatrix.Universes.All((x) => { x.Die(); return true; });

                for (int i = 0; i < 32; i++)
                {
                    TheMatrix.Universes.Where((x) => { return x.X == Convert.ToUInt64(i) && x.Y == Convert.ToUInt64(i); }).First().Born();
                    TheMatrix.Universes.Where((x) => { return x.X == Convert.ToUInt64(31 - i) && x.Y == Convert.ToUInt64(i); }).First().Born();
                }
            }
            if (e.Key == System.Windows.Input.Key.S)
            {
                LifeCycle.LifeCycleSequence();
            }
            if (e.Key == System.Windows.Input.Key.O)
            {
                TheMatrix.Universes.All((x) => { x.Die(); return true; });

                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 15; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 16 && x.Y == 15; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 16; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 17; }).First().Born();
            }
            if (e.Key == System.Windows.Input.Key.B)
            {
                TheMatrix.Universes.All((x) => { x.Die(); return true; });

                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 15; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 16; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 15 && x.Y == 17; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 16 && x.Y == 15; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 16 && x.Y == 16; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 16 && x.Y == 17; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 17 && x.Y == 15; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 17 && x.Y == 16; }).First().Born();
                TheMatrix.Universes.Where((x) => { return x.X == 17 && x.Y == 17; }).First().Born();
            }
        }

        private void Universe_MouseDown(object sender, EventArgs e)
        {
            Matrix.Universe uni = (Matrix.Universe)sender;
            if (uni.IsAlive)
            {
                uni.Die();
            }
            else
            {
                uni.Born();
            }
        }
    }
}
