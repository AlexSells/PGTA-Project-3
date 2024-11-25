using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class TimeOfDay : DataItem
    {
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public string totalTime {  get; private set; }
        public TimeOfDay( string info)
            : base(info)
        {

        }
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                totalTime = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al TimeOfDay");
                long total = Convert.ToInt64(base.info.Substring(0, 24), 2);
                //Debug.WriteLine("Hem tallat la string: "+ total);
                total = total / 128;

                //Debug.WriteLine("Tenim el int");
                TimeSpan time = TimeSpan.FromSeconds(total);
                //Debug.WriteLine("Hem agafat el TimeSpan");
     
                totalTime = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                                time.Hours,
                                time.Minutes,
                                time.Seconds,
                                time.Milliseconds);
               
            }
        }
        public override string ObtenerAtributos()
        {
            string mensaje = totalTime + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Time=totalTime;
            return grid;
        }
    }
}