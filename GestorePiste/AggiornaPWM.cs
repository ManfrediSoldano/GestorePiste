using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Newtonsoft.Json;

namespace GestorePiste
{
    public class AggiornaPWM
    {
        int i = 5;
        int valoreint = 0;
        public int valoredaimpostare { get; set; }
        public int numPin { get; set; }

        private bool active = false;
        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        public socket connessione { get; set; }
        public AggiornaPWM (int valore, int pin)
        {
            valoredaimpostare = valore;
            numPin = pin;
            
            InitGPIO();
            timer = new DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;

            timer.Start();
        }

        public bool Update (int valore)
        {
            valoredaimpostare = valore;
            if (valoredaimpostare > 100)
                valoredaimpostare = 100;
            if (valoredaimpostare < 0)
                valoredaimpostare = 0;

            if (valoredaimpostare == 100)
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                active = false;
            }
            else if (valoredaimpostare == 0)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                active = false;

            }
            else
            {
                active = true;
                valoreint = valoredaimpostare/20 ;
                timer.Interval = System.TimeSpan.FromMilliseconds(1);
               
            }

            return true;
        }
        private void Timer_Tick(object sender, object e)
        {
            if (i == 5)
            {
                i = 0;
            }

            if(active)
            {
                if (i<=valoreint)
                {
                    i = i + 1;
                    pinValue = GpioPinValue.High;
                    pin.Write(pinValue);

                }
                else
                {
                    i = i + 1;
                    pinValue = GpioPinValue.Low;
                    pin.Write(pinValue);

                }
            }
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;

                return;
            }

            pin = gpio.OpenPin(numPin);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

       
    }
}
