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

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife.Classes
{
    public partial class Matrix
    {
        public int UniverseCellSize { get; set; }
        public int UniverseDimensionCount { get; set; }
        public int LifeSize { get { return (UniverseDimensionCount * UniverseDimensionCount); } }
        public List<Universe> Universes { get; set; } = new List<Universe>();

        public bool IsMatrixReady;

        private Grid RefGrid { get; set; }

        public Matrix(ref Grid RefGrid, int Cellsize = 16, int DimensionCount = 16)
        {
            this.RefGrid = RefGrid;
            this.UniverseCellSize = Cellsize;
            this.UniverseDimensionCount = DimensionCount;
            Log.Information("[Matrix][New] Init Matrix");
        }
        public async void RestartLife()
        {
            this.IsMatrixReady = false;
            foreach (Universe x in Universes)
            {
                Task<bool> t = GenerateLife();
                await t;
                switch (t.Result)
                {
                    case true:
                        x.Born();
                        break;
                    case false:
                        x.Die();
                        break;
                };
            }
            this.IsMatrixReady = true;
        }
        private Task<bool> GenerateLife()
        {
            Task<bool> t = Task.Factory.StartNew<bool>(() =>
            {
                    bool color = Convert.ToBoolean(new Random(DateTime.Now.Millisecond).Next(0, 2));
                    //Thread.Sleep(10);
                    Log.Information("[Matrix][GenerateLife] Celluniverse will be " + (color ? "alive" : "dead"));
                    return color;
            });
            return t;
        }
        public async void Generate()
        {
            int x = 0;
            int y = 0;
            ulong uniX = 0;
            ulong uniY = 0;
            int count = 0;

            while (x < LifeSize & y < LifeSize)
            {
                Task<bool> t = GenerateLife();
                await t;
                Universe acc = new Universe()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Height = UniverseCellSize,
                    Width = UniverseCellSize,
                    Margin = new Thickness(x, y, 0, 0),
                    Name = "Universe" + count.ToString(),
                    X = uniX,
                    Y = uniY,
                    ID = count
                };
                switch (t.Result)
                {
                    case true:
                        acc.Born();
                        break;
                    case false:
                        acc.Die();
                        break;
                };
                Universes.Add(acc);

                Log.Information("[Matrix][Generate] Added Universe(" + (t.Result?"alive":"dead") + ") at " + x.ToString() + "," + y.ToString() + " -- #" + count.ToString() + " --- Coords X: " + uniX + " Y: " + uniY);
                await GenerateLife();
                
                count -= -1;
                uniX += 1;
                x -= -UniverseCellSize;
                if (x >= LifeSize & y <= LifeSize)
                {
                    x = 0;
                    y -= -UniverseCellSize;
                    uniY += 1;
                    uniX = 0;
                }
            }
            Universes.All(u => { this.RefGrid.Children.Add(u); return true; });

            this.RefGrid.Children.Remove((UIElement)(this.RefGrid.Children.OfType<object>().Where(b => b.GetType().Equals(typeof(Label))).FirstOrDefault()));
            IsMatrixReady = true;
            Log.Information(new string('=', 50));
            Log.Information("[Matrix][Generate] Generated " + Universes.Count.ToString() + " universes in total.");
            Log.Information("[Matrix][Generate] Generation complete!");
            Log.Information(new string('=', 50));
        }
    }
}
