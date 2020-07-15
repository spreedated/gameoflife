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
using System.Windows.Forms;

namespace GameOfLife.Classes
{
    public class Life
    {
        public bool InProcess;
        private readonly Matrix TheMatrix;
        private readonly Timer LifeCycle = new Timer();
        private bool CycleInProcess;
        public Life(ref Matrix TheMatrix)
        {
            this.TheMatrix = TheMatrix;
            LifeCycle.Tick += LifeCycleElapsed;
        }
        public void Start()
        {
            LifeCycle.Interval = 1;
            LifeCycle.Start();
            InProcess = true;
        }
        public void End()
        {
            LifeCycle.Stop();
            InProcess = false;
        }

        private void LifeCycleElapsed(object sender, EventArgs e)
        {
            if (CycleInProcess || !TheMatrix.IsMatrixReady)
            {
                return;
            }
            CycleInProcess = true;
            LifeCycleSequence();
            CycleInProcess = false;
        }

        public void LifeCycleSequence()
        {
            List<Matrix.Universe> FutureUniverse = new List<Matrix.Universe>();
            TheMatrix.Universes.All(x=> { Matrix.Universe acc = x; FutureUniverse.Add(x); return true; });

            if (this.TheMatrix.Universes == null || this.TheMatrix.Universes.Count <= 0)
            {
                return;
            }
            foreach (Matrix.Universe uni in TheMatrix.Universes)
            {
                int count = GetLivingSurroundings(uni);

                if (uni.IsAlive && count < 2)
                {
                    FutureUniverse.Where(x => { return x.ID == uni.ID; }).FirstOrDefault().Die();
                    //uni.Die();
                }
                if (uni.IsAlive && (count == 2 | count == 3))
                {
                    FutureUniverse.Where(x => { return x.ID == uni.ID; }).FirstOrDefault().Born();
                    //uni.Born();
                }
                if (uni.IsAlive && count > 3)
                {
                    FutureUniverse.Where(x => { return x.ID == uni.ID; }).FirstOrDefault().Die();
                    //uni.Die();
                }
                if (!uni.IsAlive && count == 3)
                {
                    FutureUniverse.Where(x => { return x.ID == uni.ID; }).FirstOrDefault().Born();
                    //uni.Born();
                }
            }

            TheMatrix.Universes = FutureUniverse;

            ulong aliveCount = Convert.ToUInt64(this.TheMatrix.Universes.Where(x => x.IsAlive).Count());
            if (aliveCount <= 0)
            {
                Log.Information("[Life][LifeCycleSequence] Remaining " + aliveCount.ToString() + " universes alive");
                Log.Information("[Life][LifeCycleSequence] Everything is dead... restarting life");
                TheMatrix.RestartLife();
            }
        }
        private int GetLivingSurroundings(Matrix.Universe uni)
        {
            int count = 0;
            Matrix.Universe universe;

            //Right
            universe = TheMatrix.Universes.Where(x => x.X == uni.X + 1 && x.Y == uni.Y).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Bottom Right
            universe = TheMatrix.Universes.Where(x => x.X == uni.X + 1 && x.Y == uni.Y + 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Bottom
            universe = TheMatrix.Universes.Where(x => x.X == uni.X && x.Y == uni.Y + 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Bottom Left
            universe = TheMatrix.Universes.Where(x => x.X == uni.X - 1 && x.Y == uni.Y + 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Left
            universe = TheMatrix.Universes.Where(x => x.X == uni.X - 1 && x.Y == uni.Y).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Top Left
            universe = TheMatrix.Universes.Where(x => x.X == uni.X - 1 && x.Y == uni.Y - 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Top
            universe = TheMatrix.Universes.Where(x => x.X == uni.X && x.Y == uni.Y - 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            //Top Right
            universe = TheMatrix.Universes.Where(x => x.X == uni.X + 1 && x.Y == uni.Y - 1).FirstOrDefault();
            if (universe != null && universe.IsAlive)
            {
                count -= -1;
            }
            return count;
        }
    }
}
