using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.StateMachine;

namespace StateMachine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var demo = new RxWithCancelation();
            //IDisposable subscription = demo.ObservableCollection.Subscribe(i=> );
            //Console.WriteLine("Press any key to cancel");
            //Console.ReadKey();
            //subscription.Dispose();
            //Console.WriteLine("Press any key to quit");
            //Console.ReadKey();  // give background thread chance to write the cancel acknowledge message
        }
    }
}
