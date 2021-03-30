using System;
using System.Drawing;
using System.Windows.Forms;

namespace Triangles
{
    public partial class Form1 : Form
    {

        bool isInputTypeAngle = false;

        bool isCtrl = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkIfTriangleIsPossible((float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value))
            {
                saveVisualToolStripMenuItem.Enabled = true;
                copyToolStripMenuItem.Enabled = true;
                if (!isInputTypeAngle)
                    createVertexPoints((float)numericUpDown1.Value, (float)numericUpDown3.Value, (float)numericUpDown2.Value,
                    delegate (Point baseStart, Point thirdVertex, Point baseEnd,
                              SideData2 side_1, SideData2 side_2, SideData2 side_3)
                    {
                        pictureBox1.Image = drawFilledRectangle(pictureBox1.Size.Width, pictureBox1.Size.Height);
                        Graphics gfx = Graphics.FromImage(pictureBox1.Image);

                        resolveYSubtraction(baseStart, baseEnd, thirdVertex,
                            delegate (Point baseStart2, Point baseEnd2, Point thirdVertex2)
                            {
                                PointF[] pts = new PointF[] { baseStart2, baseEnd2, thirdVertex2, baseStart2 };
                                gfx.DrawLines(Pens.DarkGray, pts);

                                drawLabels2(gfx, baseStart2, baseEnd2, thirdVertex2, side_1, side_2, side_3);
                            });
                    });
                else
                {
                    calculateSidesFromAngles2((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value,
                        delegate (float side1, float side2, float side3) {
                            int multiplier = 10;

                            side1 *= multiplier;
                            side2 *= multiplier;
                            side3 *= multiplier;

                            createVertexPoints(side1, side3, side2,
                                delegate (Point baseStart, Point thirdVertex, Point baseEnd,
                                        SideData2 side_1, SideData2 side_2, SideData2 side_3)
                                {
                                    pictureBox1.Image = drawFilledRectangle(pictureBox1.Size.Width, pictureBox1.Size.Height);
                                    Graphics gfx = Graphics.FromImage(pictureBox1.Image);

                                    resolveYSubtraction(baseStart, baseEnd, thirdVertex,
                                        delegate (Point baseStart2, Point baseEnd2, Point thirdVertex2)
                                        {
                                            PointF[] pts = new PointF[] { baseStart2, baseEnd2, thirdVertex2, baseStart2 };
                                            gfx.DrawLines(Pens.DarkGray, pts);

                                            drawLabels2(gfx, baseStart2, baseEnd2, thirdVertex2, side_1, side_2, side_3);
                                        });
                                });
                        });
                }
            }
            else
            {
                //(AB + AC > BC) && (AB + BC > AC) && (AC + BC > AB) : AB + AC + BC == 180
                MessageBox.Show("Triangle with specified " + (isInputTypeAngle ? "angles" : "sides") + " is not possible." + Environment.NewLine + Environment.NewLine + "Reason: " + 
                    (isInputTypeAngle ? (numericUpDown1.Value + "° + " + numericUpDown2.Value + "° + " + numericUpDown3.Value + "° ≠ 180°") : ((numericUpDown1.Value + numericUpDown2.Value <= numericUpDown3.Value) ? "Sum of two sides is always greater than the third side." + Environment.NewLine + "But " + numericUpDown1.Value + " + " + numericUpDown2.Value + " is not greater than " + numericUpDown3.Value + "." : ((numericUpDown2.Value + numericUpDown3.Value <= numericUpDown1.Value) ? "Sum of two sides is always greater than the third side." + Environment.NewLine + "But " + numericUpDown2.Value + " + " + numericUpDown3.Value + " is not greater than " + numericUpDown1.Value + "." : "Sum of two sides is always greater than the third side." + Environment.NewLine + "But " + numericUpDown1.Value + " + " + numericUpDown3.Value + " is not greater than " + numericUpDown2.Value + "."))));
            }
        }

        private void calculateSidesFromAngles2(int C, int B, int A, Action<float, float, float> callback)
        {
            float side1 = Math.Abs(1f);
            float side2 = (float)Math.Abs(Math.Sin(B * 3.142f / 180) / Math.Sin(A * 3.142f / 180));
            float side3 = (float)Math.Abs(Math.Sin(C * 3.142f / 180) / Math.Sin(A * 3.142f / 180));

            callback(side1, side2, side3);
        }

        private void drawLabels2(Graphics gfx,
            Point baseStart, Point baseEnd, Point thirdVertex,
            SideData2 side1, SideData2 side2, SideData2 side3)
        {
            if (checkBox1.Checked)
            {
                Point vertexLabel1, vertexLabel2, vertexLabel3;
                Point sideLabel1, sideLabel2, sideLabel3;

                Size temp;

                bool showCoords = checkBox3.Checked;

                string vertexText1 = (side3.side == "AB" ? "C" : side3.side == "BC" ? "A" : "B") + (showCoords ? ("(0, 0)") : "");
                string vertexText3 = (side2.side == "AB" ? "C" : side2.side == "BC" ? "A" : "B") + (showCoords ? ("(" + Decimal.Parse(Math.Round((baseEnd.X - baseStart.X) / ((baseEnd.X - baseStart.X) / side1.data), 2, MidpointRounding.ToEven).ToString()) + ", 0)") : "");
                string vertexText2 = (side1.side == "AB" ? "C" : side1.side == "BC" ? "A" : "B") + (showCoords ? ("(" + Decimal.Parse(Math.Round((thirdVertex.X - baseStart.X) / ((baseEnd.X - baseStart.X) / side1.data), 2, MidpointRounding.ToEven).ToString()) + ", " + Decimal.Parse(Math.Round(Math.Abs(thirdVertex.Y - baseStart.Y) / ((baseEnd.X - baseStart.X) / side1.data), 2, MidpointRounding.ToEven).ToString()) + ")") : "");

                temp = gfx.MeasureString(vertexText1, Properties.Settings.Default.vertexLabelFont).ToSize();
                vertexLabel1 = new Point(baseStart.X - (temp.Width / 2), baseStart.Y + temp.Height - 5);
                temp = gfx.MeasureString(vertexText3, Properties.Settings.Default.vertexLabelFont).ToSize();
                vertexLabel3 = new Point(baseEnd.X - (temp.Width / 2), baseEnd.Y + temp.Height - 5);
                temp = gfx.MeasureString(vertexText2, Properties.Settings.Default.vertexLabelFont).ToSize();
                vertexLabel2 = new Point(thirdVertex.X - (temp.Width / 2), thirdVertex.Y - temp.Height);

                gfx.DrawString(vertexText1, Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, vertexLabel1);
                gfx.DrawString(vertexText2, Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, vertexLabel2);
                gfx.DrawString(vertexText3, Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, vertexLabel3);

                temp = gfx.MeasureString((Decimal.Round((decimal)side1.data, 2, MidpointRounding.ToEven) == (int)side1.data ? (int)side1.data : Decimal.Round((decimal)side1.data, 2, MidpointRounding.ToEven)) + " unit" + (side1.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont).ToSize();
                sideLabel1 = new Point((baseEnd.X + baseStart.X) / 2 - (temp.Width / 2), vertexLabel1.Y);
                temp = gfx.MeasureString((Decimal.Round((decimal)side2.data, 2, MidpointRounding.ToEven) == (int)side2.data ? (int)side2.data : Decimal.Round((decimal)side2.data, 2, MidpointRounding.ToEven)) + " unit" + (side2.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont).ToSize();
                sideLabel2 = new Point(((thirdVertex.X + baseStart.X) / 2) - (temp.Width / 2), ((thirdVertex.Y + baseStart.Y) / 2));
                temp = gfx.MeasureString((Decimal.Round((decimal)side3.data, 2, MidpointRounding.ToEven) == (int)side3.data ? (int)side3.data : Decimal.Round((decimal)side3.data, 2, MidpointRounding.ToEven)) + " unit" + (side3.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont).ToSize();
                sideLabel3 = new Point(((baseEnd.X + thirdVertex.X) / 2) - (temp.Width / 2), ((baseEnd.Y + thirdVertex.Y) / 2));

                gfx.DrawString((Decimal.Round((decimal)side1.data, 2, MidpointRounding.ToEven) == (int)side1.data ? (int)side1.data : Decimal.Round((decimal)side1.data, 2, MidpointRounding.ToEven)) + " unit" + (side1.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, sideLabel1);
                gfx.DrawString((Decimal.Round((decimal)side2.data, 2, MidpointRounding.ToEven) == (int)side2.data ? (int)side2.data : Decimal.Round((decimal)side2.data, 2, MidpointRounding.ToEven)) + " unit" + (side2.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, sideLabel2);
                gfx.DrawString((Decimal.Round((decimal)side3.data, 2, MidpointRounding.ToEven) == (int)side3.data ? (int)side3.data : Decimal.Round((decimal)side3.data, 2, MidpointRounding.ToEven)) + " unit" + (side3.data != 1 ? "s" : ""), Properties.Settings.Default.vertexLabelFont, Brushes.IndianRed, sideLabel3);
            }

            if (!checkBox2.Checked)
                return;

            // CALCULATING ALL THREE ANGLES
            double numerator = (side1.data * side1.data) + (side2.data * side2.data) - (side3.data * side3.data);
            double denominator = (2 * side1.data * side2.data);
            int angle = (int)decimal.Round((decimal)(Math.Acos(numerator / denominator) * 180 / 3.142), 0, MidpointRounding.ToEven);

            numerator = (side2.data * side2.data) + (side3.data * side3.data) - (side1.data * side1.data);
            denominator = (2 * side2.data * side3.data);
            int angle2 = (int)decimal.Round((decimal)(Math.Acos(numerator / denominator) * 180 / 3.142), 0, MidpointRounding.ToEven);

            numerator = (side3.data * side3.data) + (side1.data * side1.data) - (side2.data * side2.data);
            denominator = (2 * side1.data * side3.data);
            int angle3 = (int)decimal.Round((decimal)(Math.Acos(numerator / denominator) * 180 / 3.142), 0, MidpointRounding.ToEven);
            //

            int radius;

            // DRAWING SIDE1-SIDE2 ANGLE
            radius = (side2.data / side1.data) > 0.45 ? 20 : 10;
            PointF[] pts = getAnglePoints2(baseStart, radius, 0, angle);
            gfx.DrawLines(Pens.Black, pts);
            Size characterSize = gfx.MeasureString(angle.ToString() + "°", Properties.Settings.Default.vertexLabelFont).ToSize();
            Point loc = new Point((int)pts[pts.Length / 2].X, (int)(pts[pts.Length / 2].Y - (characterSize.Height / 2)));
            PointF additionFactorPoint = new PointF((pts[pts.Length / 2].X - baseStart.X), (pts[pts.Length / 2].Y - baseStart.Y));
            if (additionFactorPoint.X > additionFactorPoint.Y)
                additionFactorPoint = new Point(1, (int)(additionFactorPoint.Y / additionFactorPoint.X));
            else
                additionFactorPoint = new Point((int)(additionFactorPoint.X / additionFactorPoint.Y), 1);
            loc = new Point((int)(loc.X + (5 * additionFactorPoint.X)), (int)(loc.Y - (5 * additionFactorPoint.X)));
            //
            gfx.DrawString(angle.ToString() + "°", Properties.Settings.Default.vertexLabelFont, Brushes.Black, loc);
            //

            // DRAWING SIDE2-SIDE3 ANGLE
            radius = ((side2.data / side1.data) > 0.45 || (side3.data / side1.data) > 0.45) ? 20 : 10;
            int startAngle = 270 - (90 - angle);
            int endAngle = startAngle + angle2;
            pts = getAnglePoints2(thirdVertex, radius, startAngle, endAngle);
            gfx.DrawLines(Pens.Black, pts);
            characterSize = gfx.MeasureString(angle2.ToString() + "°", Properties.Settings.Default.vertexLabelFont).ToSize();
            loc = new Point((int)pts[pts.Length / 2].X - (characterSize.Width / 2), (int)(pts[pts.Length / 2].Y - (characterSize.Height / 2)));
            additionFactorPoint = new Point((int)(pts[pts.Length / 2].X - thirdVertex.X), (int)(pts[pts.Length / 2].Y - thirdVertex.Y));
            if (additionFactorPoint.X > additionFactorPoint.Y)
                additionFactorPoint = new Point(1, (int)(additionFactorPoint.Y / additionFactorPoint.X));
            else
                additionFactorPoint = new Point((int)(additionFactorPoint.X / additionFactorPoint.Y), 1);
            loc = new Point(loc.X, (int)(loc.Y + (10 * additionFactorPoint.Y)));
            //
            gfx.DrawString(angle2.ToString() + "°", Properties.Settings.Default.vertexLabelFont, Brushes.Black, loc);
            //

            // DRAWING SIDE3-SIDE1 ANGLE
            radius = (side3.data / side1.data) > 0.45 ? 20 : 10;
            pts = getAnglePoints2(baseEnd, radius, 180 - angle3, 180);
            gfx.DrawLines(Pens.Black, pts);
            characterSize = gfx.MeasureString(angle3.ToString() + "°", Properties.Settings.Default.vertexLabelFont).ToSize();
            loc = new Point((int)pts[pts.Length / 2].X - (characterSize.Width / 2), (int)(pts[pts.Length / 2].Y - (characterSize.Height / 2)));
            additionFactorPoint = new PointF(pts[pts.Length / 2].X - baseEnd.X, pts[pts.Length / 2].Y - baseEnd.Y);
            if (additionFactorPoint.X > additionFactorPoint.Y)
                additionFactorPoint = new Point(1, (int)(additionFactorPoint.Y / additionFactorPoint.X));
            else
                additionFactorPoint = new Point((int)(additionFactorPoint.X / additionFactorPoint.Y), 1);
            loc = new Point((int)(loc.X - (5 * additionFactorPoint.X)), (int)(loc.Y - (5 * additionFactorPoint.Y)));
            //
            gfx.DrawString(angle3.ToString() + "°", Properties.Settings.Default.vertexLabelFont, Brushes.Black, loc);
            //
        }

        private void resolveYSubtraction(Point baseStart, Point baseEnd, Point thirdVertex, Action<Point, Point, Point> callback)
        {
            Point centroid = new Point((baseStart.X + baseEnd.X + thirdVertex.X) / 3, (baseStart.Y + baseEnd.Y + thirdVertex.Y) / 3);
            Point canvasCenter = new Point(pictureBox1.Size.Width / 2, pictureBox1.Size.Height / 2);
            int yDifference = centroid.Y - canvasCenter.Y;

            callback(new Point(baseStart.X, baseStart.Y - yDifference), new Point(baseEnd.X, baseEnd.Y - yDifference), new Point(thirdVertex.X, thirdVertex.Y - yDifference));
        }

        private PointF[] getAnglePoints2(Point center, int radius, int startAngle, int endAngle)
        {
            if (true)//Math.Abs(startAngle - endAngle) != 90)
            {
                PointF[] pts = new PointF[Math.Abs(endAngle - startAngle) + 2];
                for (int i = 0; i < pts.Length; i++)
                {
                    double currentAngle = (startAngle + (i * (endAngle > startAngle ? 1 : -1))) * 3.142f / 180;
                    pts[i] = new PointF((float)(radius * Math.Cos(currentAngle)) + center.X, center.Y - (float)(radius * Math.Sin(currentAngle)));
                }

                return pts;
            } else
            {
                PointF[] pts = new PointF[3];
                pts[0] = new PointF((float)(radius * Math.Cos(startAngle)) + center.X, center.Y - (float)(radius * Math.Sin(startAngle)));
                pts[2] = new PointF((float)(radius * Math.Cos(endAngle)) + center.X, center.Y - (float)(radius * Math.Sin(endAngle)));
                pts[1] = new PointF((float)(radius / 1.4142f * Math.Cos((startAngle + endAngle) / 2)) + center.X, center.Y + (float)(radius / 1.4142f * Math.Sin((startAngle + endAngle) / 2)));
                return pts;
            }
        }

        private void createVertexPoints(float AB, float BC, float AC, 
            Action<Point, Point, Point, SideData2, SideData2, SideData2> callback)
        {
            string baseSide = chooseABaseSide(AB, AC, BC);

            float side1, side2, side3;
            string side_1, side_2, side_3;

            if (baseSide == "AB")
            {
                side2 = AC;
                side_2 = "AC";
                side1 = AB;
                side_1 = "AB";
                side3 = BC;
                side_3 = "BC";
            }
            else if (baseSide == "AC")
            {
                side1 = AC;
                side_1 = "AC";
                side2 = AB;
                side_2 = "AB";
                side3 = BC;
                side_3 = "BC";
            } else
            {
                side3 = AC;
                side_3 = "AC";
                side2 = AB;
                side_2 = "AB";
                side1 = BC;
                side_1 = "BC";
            }

            Point baseStart, baseEnd;
            int borderDistance = 40;
            baseStart = new Point(borderDistance, pictureBox1.Size.Height - borderDistance);
            baseEnd = new Point(pictureBox1.Size.Width - borderDistance, baseStart.Y);

            Point thirdVertex;
            double pixelsPerUnit = (baseEnd.X - baseStart.X) / side1;
            double x = (Math.Pow(side1, 2) + Math.Pow(side2, 2) - Math.Pow(side3, 2)) / (2 * side1);
            double y = -1 * Math.Sqrt(Math.Pow(side2, 2) - Math.Pow(x, 2)) * pixelsPerUnit;
            x *= pixelsPerUnit;
            thirdVertex = new Point((int)(baseStart.X + x), (int)(baseStart.Y + y));

            callback(baseStart, thirdVertex, baseEnd, 
                new SideData2(side_1, side1), new SideData2(side_2, side2), new SideData2(side_3, side3));
        }

        private Bitmap drawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
            return bmp;
        }

        private bool checkIfTriangleIsPossible(float AB, float AC, float BC)
        {
            return (!isInputTypeAngle ? (AB + AC > BC) && (AB + BC > AC) && (AC + BC > AB) : AB + AC + BC == 180);
        }

        private string chooseABaseSide(float AB, float AC, float BC)
        {
            string largestSide = "AB";
            float largest = AB;

            if (largest < AC)
            {
                largest = AC;
                largestSide = "AC";
            }

            if (largest < BC)
                largestSide = "BC";

            return largestSide;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox3.Checked = false && checkBox1.Checked;
            checkBox3.Enabled = checkBox1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isInputTypeAngle)
            {
                button2.Text = "Input data as Angles";
                label1.Text = "Side AB: ";
                label2.Text = "Side AC: ";
                label3.Text = "Side BC: ";
                numericUpDown1.Value = 1;
                numericUpDown2.Value = 1;
                numericUpDown3.Value = 1;
            } else
            {
                button2.Text = "Input data as Lengths";
                label1.Text = "Angle 1: ";
                label2.Text = "Angle 2: ";
                label3.Text = "Angle 3: ";
                numericUpDown1.Value = 60;
                numericUpDown2.Value = 60;
                numericUpDown3.Value = 60;
            }

            isInputTypeAngle = !isInputTypeAngle;
        }

        private void saveVisualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = new Random().Next(10000000, 99999999).ToString() + ".PNG";

            saveFileDialog1.FileName = fileName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                pictureBox1.Image.Save(saveFileDialog1.FileName);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox1.Image);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = drawFilledRectangle(pictureBox1.Size.Width, pictureBox1.Size.Height);
            saveVisualToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isCtrl)
                numericUpDown1.Select(0, numericUpDown1.Value.ToString().Length);
            else
                try
                {
                    if (isInputTypeAngle)
                        numericUpDown1.Value = 180 - (numericUpDown2.Value + numericUpDown3.Value);
                    else
                        numericUpDown1.Value = numericUpDown2.Value + numericUpDown3.Value - 1;
                }
                catch (Exception) { }
            isCtrl = false;
        }

        private void numericUpDown2_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isCtrl)
                numericUpDown2.Select(0, numericUpDown2.Value.ToString().Length);
            else
                try
                {
                    if (isInputTypeAngle)
                        numericUpDown2.Value = 180 - (numericUpDown1.Value + numericUpDown3.Value);
                    else
                        numericUpDown2.Value = numericUpDown1.Value + numericUpDown3.Value - 1;
                }
                catch (Exception) { }
            isCtrl = false;
        }

        private void numericUpDown3_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isCtrl)
                numericUpDown3.Select(0, numericUpDown3.Value.ToString().Length);
            else
                try {
                    if (isInputTypeAngle)
                        numericUpDown3.Value = 180 - (numericUpDown2.Value + numericUpDown1.Value);
                    else
                        numericUpDown3.Value = numericUpDown2.Value + numericUpDown1.Value - 1;
                } catch (Exception) { }
            isCtrl = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            isCtrl = e.Control;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (isCtrl)
                isCtrl = !e.Control;
        }
    }

    public class SideData
    {
        public int data;
        public string side;

        public SideData(string side, int val)
        {
            this.side = side;
            data = val;
        }
    }

    public class SideData2
    {
        public float data;
        public string side;

        public SideData2(string side, float val)
        {
            this.side = side;
            data = val;
        }
    }
}
