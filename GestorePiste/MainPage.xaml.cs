using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// Il modello di elemento per la pagina vuota è documentato all'indirizzo http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace GestorePiste
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AggiornaPWM regolatore1;
        AggiornaPWM regolatore2;
        socket connessione;
        GpioController gpio = GpioController.GetDefault();
        private DispatcherTimer timer;
       // private bool aggiornam = false;

        public MainPage()
        {
            this.InitializeComponent();
            connessione = new socket(ip.Text);
            connessione.Listen();
            connessione.ValueChanged += TheValueChanged;

           
            // Show an error if there is no GPIO controller
            if (gpio != null)
            {
                regolatore1 = new AggiornaPWM(0,26);
                regolatore2 = new AggiornaPWM(0,13);
            }

            else
            {
                
               
            } 

        }

        private void Timer_Tick(object sender, object e)
        {
           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            connessione.ip = ip.Text;
            connessione.Connect();
        }

        private void TheValueChanged(Object sender, EventArgs e)
        {
            int val1 = ((socket)sender).datoricevuto.valoreprimaauto;
            int val2 = ((socket)sender).datoricevuto.valoresecondaauto;
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                aggiorna(val1, val2);
                info.Text = "Nuovi valori> Auto 1: "+val1.ToString()+ " Auto 2: " +val2.ToString();
               
            }).AsTask().Wait();
              
             // info.Text = a;
        }

        private void button1_Click(object sender, RoutedEventArgs e)

        {
            connessione.Send(Int32.Parse(dati.Text), Int32.Parse(dati2.Text));

        }

        private void aggiorna(int i, int j)
        {
            regolatore1.Update(i);
            regolatore2.Update(j);
        }
    }
}
