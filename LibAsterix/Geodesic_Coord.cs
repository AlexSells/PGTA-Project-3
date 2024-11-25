using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class Geodesic_Coord: DataItem
    {
        public string Lat {  get; private set; }
        public string Long { get; private set; }
        public string Height { get; private set; }
        public Geodesic_Coord(string info)
            : base(info)
        {


        }
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                Lat = " ";
                Long = " ";
                Height = " ";
            }
            else
            {
                string[] mensaje = base.info.Split(';');
                Lat = mensaje[0];
                Long = mensaje[1];
                Height = mensaje[2];
            }



        }
        public override string ObtenerAtributos()
        {
            string mensaje = Lat + ";" + Long + ";" + Height + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Latitude = Lat;
            grid.Longitude = Long;
            grid.Height = Height;

            return grid;


        }
    }
}
