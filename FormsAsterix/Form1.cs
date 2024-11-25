//using Microsoft.VisualBasic.Devices;
//using Microsoft.VisualBasic.Logging;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LibAsterix;
//using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using System.Diagnostics;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Drawing;
//using System.Reflection.Metadata;
using MultiCAT6.Utils;
using System.Collections.Generic;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using System.Windows;
using SharpKml.Dom;
using SharpKml.Base;
using SharpKml.Engine;
using Vector = SharpKml.Base.Vector;
using Document = SharpKml.Dom.Document;
using TimeSpan = System.TimeSpan;
using System.Globalization;
using Size = System.Drawing.Size;
using System.Xml;
using System.Runtime.ConstrainedExecution;
using Amazon.IdentityManagement.Model;
using System.ComponentModel;
using Amazon.DeviceFarm.Model;
using OfficeOpenXml;


namespace FormsAsterix

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // set some initialization parameters
            Start_sim.FlatAppearance.BorderSize = 0;
            Start_sim.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Start_sim.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Start_sim.MouseEnter += (s, e) => Start_sim.Cursor = Cursors.Hand;
            Start_sim.MouseLeave += (s, e) => Start_sim.Cursor = Cursors.Default;
            DescodBUT.MouseEnter += (s, e) => DescodBUT.Cursor = Cursors.Hand;
            DescodBUT.MouseLeave += (s, e) => DescodBUT.Cursor = Cursors.Default;
        }

        // Create some lists and variables to save some data 
        List<List<DataItem>> bloque = new List<List<DataItem>>();
        List<AsterixGrid> asterixGrids = new List<AsterixGrid>();

        List<long> time = new List<long>();
        List<double> longitudList = new List<double>();
        List<double> latitudList = new List<double>();
        List<String> AircraftIDList = new List<String>();
        List<double> rhoList = new List<double>();

        List<string> AircraftAddrList = new List<string>();
        List<string> TrackNumList = new List<string>();
        List<string> Mode3AList = new List<string>();
        List<string> SACList = new List<string>();
        List<string> SICList = new List<string>();
        List<double> AltitudeList = new List<double>();

        List<double> DistHor = new List<double>();

        long timeInicial;

        private Dictionary<string, PointLatLng> lastPositions = new Dictionary<string, PointLatLng>(); // Dictionary to track previous positions
        private HashSet<string> Sim_diccionary = new HashSet<string>(); // Using HashSet to improve performance
        private GMapOverlay aircraftOverlay = new GMapOverlay("aircraftOverlay"); // Single overlay for all markers



        // Get the file ".ast" to apply decodification function

        private void DescodBUT_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set the title and file filter for the dialog
            openFileDialog.Title = "Selecciona un archivo .ast";
            openFileDialog.Filter = "Todos los arxivos (*.ast*)|*ast*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Decodification
                ReadBinaryFile(openFileDialog.FileName);
                Corrected_Altitude(bloque);
                Calcular_Lat_Long(bloque);
                GenerarAsterix(bloque);

                // Initialize Simulation 
                groupBox1.Hide();
                groupBox2.Show();
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(40, 40);
                imageList.Images.Add(Properties.Resources.play_button);
                Start_sim.Image = imageList.Images[0];
                Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                Start_sim.ImageAlign = ContentAlignment.TopCenter;
                Start_sim.TextAlign = ContentAlignment.BottomCenter;
                timeInicial = time[0];
                timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));
                gMapControl1.Overlays.Add(aircraftOverlay);
            }
        }





        public void ReadBinaryFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {

                        // Read first octet (8 bits)
                        byte firstOctet = reader.ReadByte();

                        if (firstOctet == 48) //Només agafem els data blocks de la cat 48
                        {
                            // Leer los siguientes dos octetos(16 bits) como un short
                            byte[] longitud = new byte[2];
                            longitud[0] = reader.ReadByte();
                            longitud[1] = reader.ReadByte();
                            int variableLength = BitConverter.ToInt16(longitud, 0);
                            variableLength = (ushort)((variableLength >> 8) | (variableLength << 8)); // Corregir la endianidad



                            // Calcular cuántos bits a leer según la longitud variable
                            int bitsToRead = variableLength * 8 - 3 * 8; // Restamos los octetos de cat y length

                            // Asegurarse de que hay suficientes bytes para leer
                            if (bitsToRead > 0)
                            {
                                byte[] buffer = new byte[(bitsToRead + 7) / 8]; // Redondear hacia arriba
                                reader.Read(buffer, 0, buffer.Length);
                                string DataBlock = ConvertirByte2String(buffer); //Dades que tenim en un DataBlock
                                //Ara hem de mirar el FSPEC per saber quants DataItems tenim al record 
                                int FSPEC_bits = FSPEC(DataBlock); //Obtenim quants bits té el FSPEC
                                int[] FSPEC_vector = new int[FSPEC_bits]; //Creem un vector amb la longitud del FSPEC
                                FSPEC_vector = ConvertirBits(DataBlock, FSPEC_bits);
                                DataBlock = DataBlock.Substring(FSPEC_bits); //eliminem del missatge els bits del FSPEC
                                ReadPacket(FSPEC_vector, DataBlock); //Cridem a la funció per a llegir el paquet
                                //MessageBox.Show("Hem llegit el paquet");

                            }
                            else
                            {
                                Console.WriteLine("No hay bits suficientes para leer después de restar 3 octetos.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("El paquete no pertenece a la ct 48 y no se lee");
                        }
                    }

                    MessageBox.Show("Tot el fitxer s'ha descodificat correctament");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
        }

        static string ConvertirByte2String(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0')); //El PadLeft nos asegura que cada byte se representa con 8 bits, rellenando si es necesario
                // Com que sempre hi han octets jo crec que està bé que faci això
            }
            return sb.ToString();
        }

        //Llegim el FSPEC per saber on comencen els Data Items
        static int FSPEC(string DataBlock)
        {
            int length = DataBlock.Length; //No hem de superar mai la longitud
            for (int i = 0; i < length; i = i + 8)
            {

                if (i + 8 <= length)
                {
                    string aux = DataBlock.Substring(i, 8);
                    char ultimbit = aux[7]; //Busquem el valor de l'últim bit per saber si hi ha més FSPEC
                    if (ultimbit == '0')
                    {
                        return i + 8; //Obtenim quants bits de FSPEC hi ha 
                    }
                }
            }
            return -1; //Si hi ha algun error 
        }

        //Convertim els bits del FSPEC en un vector
        static int[] ConvertirBits(string DataBlock, int length)
        {

            int numBits = (length / 8) * 7;
            int[] vectorBits = new int[numBits];
            int indexBit = 0;
            for (int i = 0; i < length; i += 8)
            {
                //Eliminem els bits FX, ja que no ens indiquen res
                if (i + 8 <= length)
                {
                    string octeto = DataBlock.Substring(i, 8);
                    for (int j = 0; j < 7; j++)
                    {
                        vectorBits[indexBit] = octeto[j] == '1' ? 1 : 0;
                        indexBit++;
                    }
                }

            }

            return vectorBits;
        }
        public void ReadPacket(int[] read, string DataBlock)
        {
            string mensaje;
            int octet = 8; // Octet longitude
            int bitsleidos = 0;
            int final;
            int j;
            List<string> cadena = new List<string>();
            List<DataItem> di = new List<DataItem>();


            for (int i = 0; i < read.Length; i++)
            {

                switch (i)
                {
                    case 0:
                        if (read[i] == 1)
                        {
                            // Decodification SAC and SIC
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet); //La longitud és fixa en aquest cas
                            di.Add(new LibAsterix.DataSourceIdentifier(mensaje));

                            int length = 8; //Cada octeto tiene 8 bits

                            // Convertir SAC y SIC de binario a decimal

                            SACList.Add(Convert.ToString(Convert.ToInt32(mensaje.Substring(0, length), 2)));
                            SICList.Add(Convert.ToString(Convert.ToInt32(mensaje.Substring(length), 2)));

                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.DataSourceIdentifier("N/A"));
                            SACList.Add("N/A");
                            SICList.Add("N/A");
                        }
                        break;
                    case 1:
                        if (read[i] == 1)
                        {
                            // Decodification ACTUAL TIME
                            mensaje = DataBlock.Substring(bitsleidos, 3 * octet);

                            long timeActual = Convert.ToInt64(mensaje, 2) / 128;
                            time.Add(timeActual);
                            di.Add(new LibAsterix.TimeOfDay(mensaje));
                            bitsleidos = bitsleidos + 3 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.TimeOfDay("N/A"));
                        }

                        break;
                    case 2:
                        if (read[i] == 1)
                        {
                            // Decodification TARGET REPORT
                            final = 0;
                            j = 0;
                            while (final == 0)
                            {
                                cadena.Add(DataBlock.Substring(bitsleidos, octet));
                                bitsleidos = bitsleidos + octet;
                                if (cadena[j][j + octet - 1] == '1')
                                {
                                    j = j + 1;
                                }
                                else
                                {
                                    final = 1;
                                }
                            }

                            mensaje = System.String.Join("", cadena); //Unim tots els bits en una sola string
                            di.Add(new LibAsterix.TargetReportDescriptor(mensaje));
                            cadena.Clear(); //Buidem la llista per a no gastar memòria
                        }
                        else
                        {
                            di.Add(new LibAsterix.TargetReportDescriptor("N/A"));
                        }
                        break;
                    case 3:
                        if (read[i] == 1)
                        {
                            // Decodification POLAR POSITION
                            mensaje = DataBlock.Substring(bitsleidos, 4 * octet);
                            di.Add(new LibAsterix.Position_Polar(mensaje));
                            double Rho = (Convert.ToInt32(mensaje.Substring(0, 16), 2)) * ((double)1 / 256);
                            rhoList.Add(Rho);
                            bitsleidos = bitsleidos + 4 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.Position_Polar("N/A"));
                        }
                        break;
                    case 4:
                        if (read[i] == 1)
                        {
                            // Decodification MODE 3A
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet);
                            di.Add(new LibAsterix.Mode3A(mensaje));

                            string message = Convert.ToString(Convert.ToInt32(mensaje.Substring(4), 2), 8).PadLeft(4, '0');

                            Mode3AList.Add(message);

                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.Mode3A("N/A"));
                            Mode3AList.Add("N/A");
                        }
                        break;
                    case 5:
                        if (read[i] == 1)
                        {
                            // Decodification FLIGHT LEVEL
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet);
                            di.Add(new LibAsterix.FlightLevel(mensaje));
                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.FlightLevel("N/A"));
                        }
                        break;
                    case 6:
                        if (read[i] == 1)
                        {
                            // Decodification RADAR PLOT CHAR
                            string[] dades = new string[8]; //La longitud màxima serà de 8 octets

                            int length = 0;
                            for (int t = 0; t < dades.Length; t++)
                            {
                                dades[t] = DataBlock.Substring(bitsleidos + t, 1);
                                if (dades[t] == "1")
                                {
                                    length = length + octet; //Així trobarem la longitud del missatge a llegir
                                }
                            }
                            mensaje = DataBlock.Substring(bitsleidos, octet + length);
                            di.Add(new LibAsterix.RadarPlotChar(mensaje));
                            bitsleidos = bitsleidos + octet + length;
                        }
                        else
                        {
                            di.Add(new LibAsterix.RadarPlotChar("N/A"));
                        }
                        break;
                    case 7:
                        if (read[i] == 1)
                        {
                            // Decodification AIRCRAFT ADDRESS
                            mensaje = DataBlock.Substring(bitsleidos, 3 * octet);
                            di.Add(new LibAsterix.AircraftAdd(mensaje));
                            string add = string.Empty;
                            string address = string.Empty;
                            for (int m = 0; m < mensaje.Length; m += 4)
                            {
                                string bits = mensaje.Substring(m, 4); //Agafem grups de 4 per a passar-ho a hexadecimal
                                int decval = Convert.ToInt32(bits, 2); //Ho passem a decimal
                                string address_char = decval.ToString("X");
                                add += address_char;
                            }
                            AircraftAddrList.Add(add);
                            bitsleidos = bitsleidos + 3 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.AircraftAdd("N/A"));
                            AircraftAddrList.Add("N/A");
                        }
                        break;
                    case 8:
                        if (read[i] == 1)
                        {
                            // Decodification AIRCRAFT ID
                            mensaje = DataBlock.Substring(bitsleidos, 6 * octet);
                            di.Add(new LibAsterix.AircraftID(mensaje));
                            string ID = "";
                            for (int m = 0; m < mensaje.Length; m += 6)
                            {
                                string block = mensaje.Substring(m, 6);
                                ID += ConvertirBitsAChar(block);
                            }
                            AircraftIDList.Add(ID);
                            bitsleidos = bitsleidos + 6 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.AircraftID("N/A"));
                            AircraftIDList.Add(Convert.ToString(di[i]));
                        }
                        break;
                    case 9:
                        if (read[i] == 1)
                        {
                            // Decodification BDS
                            mensaje = DataBlock.Substring(bitsleidos, octet);
                            bitsleidos = bitsleidos + octet;
                            int rep = Convert.ToInt32(mensaje, 2); // Passem a int per saber el nombre de repeticions
                            int flag4 = 0;
                            int flag5 = 0;
                            int flag6 = 0;
                            for (int k = 0; k < rep; k++)
                            {
                                mensaje = DataBlock.Substring(bitsleidos, 8 * octet);

                                int BDS1 = Convert.ToInt32(mensaje.Substring(56, 4), 2);
                                int BDS2 = Convert.ToInt32(mensaje.Substring(60, 4), 2);


                                if (BDS1 == 4 & BDS2 == 0)
                                {
                                    di.Add(new LibAsterix.ModeS4(mensaje));
                                    flag4 = 1;
                                }
                                else if (BDS1 == 5 & BDS2 == 0)
                                {
                                    di.Add(new LibAsterix.ModeS5(mensaje));
                                    flag5 = 1;
                                }
                                else if (BDS1 == 6 & BDS2 == 0)
                                {
                                    di.Add(new LibAsterix.ModeS6(mensaje));
                                    flag6 = 1;
                                }
                                bitsleidos = bitsleidos + 8 * octet;
                            }
                            if (flag4 == 0 & flag5 == 0 & flag6 == 0)
                            {
                                di.Add(new LibAsterix.ModeS4("N/A"));
                                di.Add(new LibAsterix.ModeS5("N/A"));
                                di.Add(new LibAsterix.ModeS6("N/A"));
                            }
                            else if (flag4 == 0 & flag5 == 0)
                            {
                                di.Add(new LibAsterix.ModeS4("N/A"));
                                di.Add(new LibAsterix.ModeS5("N/A"));
                            }
                            else if (flag4 == 0 & flag6 == 0)
                            {
                                di.Add(new LibAsterix.ModeS4("N/A"));
                                di.Add(new LibAsterix.ModeS6("N/A"));
                            }
                            else if (flag5 == 0 & flag6 == 0)
                            {
                                di.Add(new LibAsterix.ModeS5("N/A"));
                                di.Add(new LibAsterix.ModeS6("N/A"));
                            }
                            else if (flag4 == 0)
                            {
                                di.Add(new LibAsterix.ModeS4("N/A"));
                            }
                            else if (flag5 == 0)
                            {
                                di.Add(new LibAsterix.ModeS5("N/A"));
                            }
                            else if (flag6 == 0)
                            {
                                di.Add(new LibAsterix.ModeS6("N/A"));
                            }


                        }
                        else
                        {
                            di.Add(new LibAsterix.ModeS4("N/A"));
                            di.Add(new LibAsterix.ModeS5("N/A"));
                            di.Add(new LibAsterix.ModeS6("N/A"));
                        }
                        break;
                    case 10:

                        if (read[i] == 1)
                        {
                            // Decodification TRACK NUMBER
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet);
                            di.Add(new LibAsterix.TrackNum(mensaje));

                            TrackNumList.Add(Convert.ToString(Convert.ToInt32(mensaje.Substring(4, 12), 2)));

                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.TrackNum("N/A"));
                            TrackNumList.Add("N/A");
                        }
                        break;
                    case 11:
                        if (read[i] == 1)
                        {
                            // Decodification POSITION CARTESIAN
                            mensaje = DataBlock.Substring(bitsleidos, 4 * octet);
                            di.Add(new LibAsterix.Position_Cartesian(mensaje));
                            bitsleidos = bitsleidos + 4 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.Position_Cartesian("N/A"));
                        }
                        break;
                    case 12:
                        if (read[i] == 1)
                        {
                            // Decodification TRACK VELOCITY POLAR
                            mensaje = DataBlock.Substring(bitsleidos, 4 * octet);

                            di.Add(new LibAsterix.TrackVelocityPolar(mensaje));
                            bitsleidos = bitsleidos + 4 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.TrackVelocityPolar("N/A"));
                        }
                        break;
                    case 13:
                        if (read[i] == 1)
                        {
                            // Decodification TRACK STATUS
                            final = 0;
                            j = 0;
                            while (final == 0)
                            {
                                cadena.Add(DataBlock.Substring(bitsleidos, octet));
                                bitsleidos = bitsleidos + octet;
                                if (cadena[j][j + octet - 1] == '1')
                                {
                                    j = j + 1;
                                }
                                else
                                {
                                    final = 1;
                                }
                            }

                            mensaje = System.String.Join("", cadena); //Unim tots els bits en una sola string
                            di.Add(new LibAsterix.TrackStatus(mensaje));
                            cadena.Clear(); //Buidem la llista per a no gastar memòria

                        }
                        else
                        {
                            di.Add(new LibAsterix.TrackStatus("N/A"));
                        }
                        break;
                    case 14:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 4 * octet;
                        }
                        break;
                    case 15:
                        if (read[i] == 1)
                        {
                            final = 0;
                            j = 0;
                            while (final == 0)
                            {
                                cadena.Add(DataBlock.Substring(bitsleidos, octet));
                                bitsleidos = bitsleidos + octet;
                                if (cadena[j][j + octet - 1] == '1')
                                {
                                    j = j + 1;
                                }
                                else
                                {
                                    final = 1;
                                }
                            }
                            cadena.Clear();
                        }
                        break;
                    case 16:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 2 * octet;
                        }

                        break;
                    case 17:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 4 * octet;
                        }

                        break;

                    case 18:
                        if (read[i] == 1)
                        {
                            // Decodification HEIGHT 3D RADAR
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet);
                            di.Add(new LibAsterix.H_3D_RADAR(mensaje));
                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.H_3D_RADAR("N/A"));
                        }
                        break;
                    case 19:
                        if (read[i] == 1)
                        {
                            final = 0;
                            j = 0;
                            while (final == 0)
                            {
                                cadena.Add(DataBlock.Substring(bitsleidos, octet));
                                bitsleidos = bitsleidos + octet;
                                if (cadena[j][j + octet - 1] == '1')
                                {
                                    j = j + 1;
                                }
                                else
                                {
                                    final = 1;
                                }
                            }
                            cadena.Clear();
                        }
                        break;

                    case 20:
                        if (read[i] == 1)
                        {
                            // Decodification COMMUNICATION ACAS
                            mensaje = DataBlock.Substring(bitsleidos, 2 * octet);
                            di.Add(new LibAsterix.CommACAS(mensaje));
                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        else
                        {
                            di.Add(new LibAsterix.CommACAS("N/A"));
                        }
                        break;
                    case 21:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 7 * octet;
                        }
                        break;
                    case 22:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + octet;
                        }
                        break;
                    case 23:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        break;
                    case 24:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + octet;
                        }
                        break;
                    case 25:
                        if (read[i] == 1)
                        {
                            bitsleidos = bitsleidos + 2 * octet;
                        }
                        break;
                    case 26:
                        if (read[i] == 1)
                        {

                        }
                        break;
                    case 27:
                        if (read[i] == 1)
                        {

                        }
                        break;


                }
            }

            Descodificar(di); //Cridem a la funció descodificar
            bloque.Add(di);

        }

        private static readonly Dictionary<char, string> ia5Mapping = new Dictionary<char, string>
    {
        {'A', "000001"}, {'B', "000010"}, {'C', "000011"}, {'D', "000100"}, {'E', "000101"},
        {'F', "000110"}, {'G', "000111"}, {'H', "001000"}, {'I', "001001"}, {'J', "001010"},
        {'K', "001011"}, {'L', "001100"}, {'M', "001101"}, {'N', "001110"}, {'O', "001111"},
        {'P', "010000"}, {'Q', "010001"}, {'R', "010010"}, {'S', "010011"}, {'T', "010100"},
        {'U', "010101"}, {'V', "010110"}, {'W', "010111"}, {'X', "011000"}, {'Y', "011001"},
        {'Z', "011010"}, {'0', "110000"}, {'1', "110001"}, {'2', "110010"}, {'3', "110011"},
        {'4', "110100"}, {'5', "110101"}, {'6', "110110"}, {'7', "110111"}, {'8', "111000"},
        {'9', "111001"}, {' ', "100000"},
    };
        static char ConvertirBitsAChar(string cadena)
        {
            foreach (var entry in ia5Mapping)
            {
                if (entry.Value == cadena)
                {
                    return entry.Key;
                }
            }
            return '?';
        }

        private void Descodificar(List<DataItem> data)
        {

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Descodificar();

            }

        }


        // To open a new ".ast" file without closing the simulation
        private void NewDataBut_Click(object sender, EventArgs e)
        {
            // Create an OpenFileDialog to allow the user to select a file.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Selecciona un archivo .ast",
                Filter = "Todos los archivos (*.ast*)|*ast*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Clear lists and variables from the previous simulation
                bloque.Clear();
                asterixGrids.Clear();
                time.Clear();
                longitudList.Clear();
                latitudList.Clear();
                AircraftIDList.Clear();
                rhoList.Clear();
                AircraftAddrList.Clear();
                TrackNumList.Clear();
                Mode3AList.Clear();
                SACList.Clear();
                SICList.Clear();
                AltitudeList.Clear();
                DistHor.Clear();

                lastPositions.Clear(); 
                Sim_diccionary.Clear();

                // Decode the new file
                ReadBinaryFile(openFileDialog.FileName);
                Corrected_Altitude(bloque);
                Calcular_Lat_Long(bloque);
                GenerarAsterix(bloque);

                // Check if data was loaded correctly
                if (time.Count > 0)
                {
                    // Set up the simulation layout with the new initial time
                    timeInicial = time[0];
                    num_loop = 0;
                    Click_times = 0;

                    gMapControl1.Overlays.Clear(); // Clear all previous overlays from the map

                    aircraftOverlay = new GMapOverlay("aircraftOverlay");
                    gMapControl1.Overlays.Add(aircraftOverlay); 
                    gMapControl1.ReloadMap(); 



                    // Update the time in the text control
                    timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    (int)(timeInicial / 3600),
                                                 (int)((timeInicial % 3600) / 60),
                                                 (int)(timeInicial % 60));

                    // Set up the "Start" button
                    ImageList imageList = new ImageList();
                    imageList.ImageSize = new Size(40, 40);
                    imageList.Images.Add(Properties.Resources.play_button);
                    Start_sim.Image = imageList.Images[0];
                    Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                    Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                    Start_sim.ImageAlign = ContentAlignment.TopCenter;
                    Start_sim.TextAlign = ContentAlignment.BottomCenter;
                    Start_sim.Text = " Start";
                    Start_sim.Visible = true;

                    timer1.Interval = 1000;
                    Velocity_label_bar.Text = "Sim. Speed x1";
                    trackBar1.Value = 0;
                }
                else
                {
                    // Show an error message if the file does not contain valid data
                    MessageBox.Show("El archivo seleccionado no contiene datos válidos o no se pudo cargar.",
                                    "Error al cargar archivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Open DataGridView Form to show the decodified data
        private void ShowDataBut_Click(object sender, EventArgs e)
        {
            DataGridView formulari = new DataGridView(asterixGrids);
            formulari.Show();
        }


        // This method is designed to initialize the GMapControl with a set of configurations to ensure the map behaves as expected during the app's lifecycle
        int zoom = 7;
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            gMapControl1.PolygonsEnabled = true;
            gMapControl1.MarkersEnabled = true;
            gMapControl1.NegativeMode = false;
            gMapControl1.RetryLoadTile = 0;
            gMapControl1.ShowTileGridLines = false;
            gMapControl1.AllowDrop = true;
            gMapControl1.IgnoreMarkerOnMouseWheel = true;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.DisableFocusOnMouseEnter = true;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = zoom;
            gMapControl1.Position = new PointLatLng(41.300702, 2.102058);
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.ShowCenter = false;


        }

        // Functions to define the clock for the simulation
        int num_loop = 0;


        private void timer1_Tick(object sender, EventArgs e)
        {
            // Increment the initial time each time the timer is triggered
            timeInicial++;

            // Run the simulation and update markers only when necessary
            Tick(ref timeInicial, ref num_loop);

            // Update the time in the interface
            timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));

            // Refresh the map only every few ticks to improve performance
            gMapControl1.Refresh();

        }


        private void Tick(ref long timeTick, ref int num_loop)
        {

            // Check if there are aircraft to add to the simulation
            if (num_loop != AircraftIDList.Count)
            {
                for (int i = 0; num_loop < AircraftIDList.Count && timeTick >= time[num_loop]; num_loop++)
                {
                    // Add or update the marker on the map only if necessary
                    AddMarkerToMap(latitudList[num_loop], longitudList[num_loop], AircraftIDList[num_loop], num_loop);
                }
            }
            else
            {
                // Stop the simulation if all aircraft have been processed
                timer1.Stop();
                timeTick -= 2;
                Start_sim.Visible = false;
            }
        }

        // Functions to show the Markers on map
        private void AddMarkerToMap(double lat, double lon, string name, int currentIndex)
        {
            if (AircraftAddrList[currentIndex] != "N/A")
            {
                var newPosition = new PointLatLng(lat, lon);
                double lastRelevantTime = time.Last() - 60;
                bool isLastMoment = currentIndex == AircraftIDList.Count - 1;
                bool nameAppearsInFuture = AircraftIDList.Skip(currentIndex + 1).Contains(name);
                bool isInLastRelevantSecond = time[currentIndex] >= lastRelevantTime;

                // Only remove the marker if it will not appear in the future, is not in the last relevant second, and is not the last one
                if (!nameAppearsInFuture && !isInLastRelevantSecond && !isLastMoment)
                {
                    GMapMarker existingMarker = aircraftOverlay.Markers.FirstOrDefault(m => m.Tag?.ToString() == name);

                    if (existingMarker != null)
                    {
                        aircraftOverlay.Markers.Remove(existingMarker);
                    }

                    Sim_diccionary.Remove(name); // Remove the aircraft from Sim_diccionary
                    return;
                }

                // If the aircraft's position has not changed, there is no need to update the marker
                if (lastPositions.TryGetValue(name, out var lastPosition) && lastPosition == newPosition)
                {
                    return;
                }

                // Update the position in the dictionary
                lastPositions[name] = newPosition;

                // Find the existing marker or create a new one
                GMapMarker marker = aircraftOverlay.Markers.FirstOrDefault(m => m.Tag?.ToString() == name);
                if (marker != null)
                {
                    // Update the position of the existing marker
                    MoveMarkerSmoothly(marker, newPosition); // Call interpolation for smooth movement
                }
                else
                {
                    // Create a new marker if it doesn't exist and add it to the overlay
                    marker = new GMarkerGoogle(newPosition, GMarkerGoogleType.blue_dot) { Tag = name };
                    aircraftOverlay.Markers.Add(marker);

                    // Add the name to the HashSet if it is not already registered
                    Sim_diccionary.Add(name);
                }
            }
        }
        private void MoveMarkerSmoothly(GMapMarker marker, PointLatLng targetPosition)
        {
            // Calculate the distance in latitude and longitude between the current position and the target
            double latDiff = targetPosition.Lat - marker.Position.Lat;
            double lonDiff = targetPosition.Lng - marker.Position.Lng;

            // Define a lower speed factor to make the movement more gradual and precise
            double moveFactor = 0.12;

            // Ensure the difference values are not too small, or the movement will appear choppy
            if (Math.Abs(latDiff) > 0.000001 || Math.Abs(lonDiff) > 0.000001)
            {
                // Calculate the new position by applying the movement factor
                marker.Position = new PointLatLng(
                    marker.Position.Lat + latDiff * moveFactor,
                    marker.Position.Lng + lonDiff * moveFactor
                );
            }
            else
            {
                // If the difference is too small, place the marker directly at the target position
                marker.Position = targetPosition;
            }
        }

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            // Get the aircraft name from the marker's tag
            string name = item.Tag?.ToString();

            // Search for the corresponding marker within the single overlay
            GMapMarker existingMarker = aircraftOverlay.Markers.FirstOrDefault(m => m.Tag?.ToString() == name);

            // Find the index of the aircraft in AircraftIDList based on its name
            int indexAir = AircraftIDList.FindIndex(id => id == name);

            // Verify if the aircraft name exists in the simulation dictionary (Sim_diccionary)
            if (existingMarker != null && Sim_diccionary.Contains(name))
            {
                // Get the position of the existing marker on the map
                var position = existingMarker.Position;
                
                List<int> index_ID = new List<int>();
                for (int i = 0; i < AircraftIDList.Count; i++)
                {
                    if (AircraftIDList[i] == name)
                    {
                        index_ID.Add(i);
                    }
                }

                List<int> index_time = new List<int>();

                for (int i = 0; i < time.Count; i++)
                {
                    if(time[i] < timeInicial)
                    {
                        index_time.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                int indexAlt = indexAir; // Initial value to indicate no match was found

                List<int> index_sub = new List<int>();

                foreach (int id in index_ID)
                {
                    if (index_time.Contains(id))
                    {
                        index_sub.Add(id);
                    }
                }

                if(index_sub.Count == 0)
                {
                    indexAlt = indexAir;
                }
                else
                {
                    indexAlt = index_sub.Last();
                }



                // Prepare the information to display about the selected aircraft
                string info = $"Aircraft address: {AircraftAddrList[indexAir]}\n" +
                              $"Track number: {TrackNumList[indexAir]}\n" +
                              $"Mode 3A Reply: {Mode3AList[indexAir]}\n" +
                              $"\n" +
                              $"Lat: {position.Lat}º\n" +
                              $"Lon: {position.Lng}º\n" +
                              $"Altitude: {AltitudeList[indexAlt]} ft\n" +
                              $"\n" +
                              $"SAC: {SACList[indexAir]}\n" +
                              $"SIC: {SICList[indexAir]}\n";

                // Display the information in a message box with the aircraft name as the title
                MessageBox.Show(info, $"Aircraft identification: {name}");
            }
        }

        // This function handles the Start/Stop simulation button click event
        int Click_times = 0;
        private void Start_sim_Click(object sender, EventArgs e)
        {
            // Si el texto del botón es "Start" o "Continue"
            if (Start_sim.Text == " Start" || Start_sim.Text == " Continue")
            {
                // Configuración de la imagen de botón de "play"
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(40, 40);
                imageList.Images.Add(Properties.Resources.play_button);
                Start_sim.Image = imageList.Images[0];
                Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                Start_sim.ImageAlign = ContentAlignment.TopCenter;
                Start_sim.TextAlign = ContentAlignment.BottomCenter;

                // Configura el botón en el primer clic
                if (Click_times == 0)
                {
                    // Configuración de la imagen de botón de "stop"
                    ImageList imageListStop = new ImageList();
                    imageListStop.ImageSize = new Size(40, 40);
                    imageListStop.Images.Add(Properties.Resources.pause);
                    Start_sim.Image = imageListStop.Images[0];
                    Start_sim.Text = " Stop";

                    // Ordena los datos de 'bloque' en función del tercer elemento (index 2)
                    bloque = bloque.OrderBy(data => Convert.ToString(data[2])).ToList();

                    // Muestra el control del tiempo y empieza el temporizador
                    timeTXT.Show();
                    timer1.Start();

                    // Incrementa el contador de clics y limpia el diccionario de simulación
                    Click_times++;
                    Sim_diccionary.Clear();
                    lastPositions.Clear();  // Limpia las posiciones para reiniciar
                }
                else
                {
                    // Configuración de la imagen de botón de "stop"
                    ImageList imageListStop = new ImageList();
                    imageListStop.ImageSize = new Size(40, 40);
                    imageListStop.Images.Add(Properties.Resources.pause);
                    Start_sim.Image = imageListStop.Images[0];
                    Start_sim.Text = " Stop";
                    timer1.Start();
                }
            }
            else
            {
                // Configuración de la imagen de botón de "play" al pausar
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(40, 40);
                imageList.Images.Add(Properties.Resources.play_button);
                Start_sim.Image = imageList.Images[0];
                Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                Start_sim.ImageAlign = ContentAlignment.TopCenter;
                Start_sim.TextAlign = ContentAlignment.BottomCenter;
                Start_sim.Text = " Continue";
                timer1.Stop();
            }
        }
        private void RestartSimBut_Click(object sender, EventArgs e)
        {
            // Stop the simulation timer to pause the simulation
            timer1.Stop();

            // Reset the initial time to the first time in the time array (time[0])
            timeInicial = time[0];

            // Reset the loop counter and click count
            num_loop = 0;
            Click_times = 0;

            lastPositions.Clear();
            Sim_diccionary.Clear();

            // Clear all overlays from the map (removes any markers or other graphical elements)
            gMapControl1.Overlays.Clear();

            aircraftOverlay = new GMapOverlay("aircraftOverlay");
            gMapControl1.Overlays.Add(aircraftOverlay);

            gMapControl1.ReloadMap();

            // Set the time text to show the formatted initial time (HH:mm:ss)
            timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));

            // Set up the ImageList to display the play button image
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);
            imageList.Images.Add(Properties.Resources.play_button);
            Start_sim.Image = imageList.Images[0];
            Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
            Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
            Start_sim.ImageAlign = ContentAlignment.TopCenter;
            Start_sim.TextAlign = ContentAlignment.BottomCenter;
            Start_sim.Text = " Start";
            Start_sim.Visible = true;

            groupBox3.Hide();

            timer1.Interval = 1000;
            Velocity_label_bar.Text = "Sim. Speed x1";
            trackBar1.Value = 0;
        }

        // This method is triggered whenever the value of trackBar1 is changed (when the user scrolls the trackbar)
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Get the current value of the trackbar (the velocity level selected by the user)
            int velocity = trackBar1.Value;
            switch (velocity)
            {
                case 0:
                case 1:
                    timer1.Interval = 1000;
                    Velocity_label_bar.Text = "Sim. Speed x1";
                    break;

                case 2:
                case 3:
                    timer1.Interval = 500;
                    Velocity_label_bar.Text = "Sim. Speed x2";
                    break;

                case 4:
                case 5:
                    timer1.Interval = 250;
                    Velocity_label_bar.Text = "Sim. Speed x4";
                    break;

                case 6:
                case 7:
                    timer1.Interval = 200;
                    Velocity_label_bar.Text = "Sim. Speed x5";
                    break;

                case 8:
                case 9:
                    timer1.Interval = 100;
                    Velocity_label_bar.Text = "Sim. Speed x10";
                    break;

                case 10:
                    timer1.Interval = 10;
                    Velocity_label_bar.Text = "Sim. Speed x100";
                    break;
            }
        }

        // Method to generate a KML file
        private void GetKMLBut_Click(object sender, EventArgs e)
        {
            // Initialize a SaveFileDialog to allow the user to choose where to save the KML file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo KML|*.kml";
            saveFileDialog.Title = "Guardar archivo KML";


            DialogResult result = saveFileDialog.ShowDialog();

            // If the user selects a file path and clicks OK
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName; // Saves the file name 

                // Dictionary to store aircraft data with the name of the aircraft as the key
                Dictionary<string, KML_DATA> posicionesDeRepeticiones = new Dictionary<string, KML_DATA>();

                // Iterate over each item in the "bloque" list (aircraft data)
                for (int i = 0; i < bloque.Count; i++)
                {
                    string nombre = AircraftIDList[i];
                    if (!posicionesDeRepeticiones.ContainsKey(nombre))
                    {
                        posicionesDeRepeticiones[nombre] = new KML_DATA();
                        posicionesDeRepeticiones[nombre].Positions = new List<Vector>();
                        posicionesDeRepeticiones[nombre].Description = "Aircraft address: " + nombre + " ; Aircraft indentification: " + AircraftIDList[i] + " ; Track number: " + TrackNumList[i] + " ; Mode 3A Reply: " + Mode3AList[i] + " ; SAC: " + SACList[i] + " ; SIC: " + SICList[i];
                    }
                    posicionesDeRepeticiones[nombre].Positions.Add(new Vector(latitudList[i], longitudList[i], AltitudeList[i]));
                }

                // Create the KML document and KML object
                var document = new Document();
                var kml = new Kml();

                int styleCount = 0; // Counter to create unique style IDs for each aircraft
                // Loop through the dictionary to create KML elements for each aircraft
                foreach (var kvp in posicionesDeRepeticiones)
                {
                    string nombreAeronave = kvp.Key;
                    string description = kvp.Value.Description;

                    var placemark = new SharpKml.Dom.Placemark();
                    placemark.Name = nombreAeronave;
                    placemark.Description = new Description { Text = description };

                    // Create a custom style for each placemark
                    var style = new Style();
                    style.Id = "Style" + styleCount;
                    style.Line = new LineStyle
                    {
                        Color = new Color32(255, 0, 0, 255),
                        Width = 0.5
                    };

                    placemark.StyleUrl = new Uri("#" + style.Id, UriKind.Relative); // Link the style to the placemark

                    // Create a LineString geometry (path of the aircraft's trajectory)
                    var lineString = new LineString();
                    lineString.Coordinates = new CoordinateCollection();

                    var point = new SharpKml.Dom.Point();

                    // Iterate through the positions of the aircraft and add them as coordinates in the LineString
                    foreach (Vector posicion in kvp.Value.Positions)
                    {
                        lineString.Coordinates.Add(posicion);
                        if (posicion.Altitude <= 1828.8)
                        {
                            lineString.AltitudeMode = AltitudeMode.RelativeToGround;
                        }
                        else
                        {
                            lineString.AltitudeMode = AltitudeMode.Absolute;
                        }

                    }

                    placemark.Geometry = lineString;

                    // Add the custom style to the KML
                    document.AddStyle(style);
                    document.AddFeature(placemark);

                    styleCount++;
                }

                Style lineStyle = new Style();
                lineStyle.Line = new LineStyle();
                lineStyle.Line.Width = 5;
                lineStyle.Line.Color = new Color32(0, 255, 0, 255);

                document.AddStyle(lineStyle);

                // Add the document (with all the placemarks) to the KML object
                kml.Feature = document;

                // Create a KML file from the document object
                KmlFile kmlFile = KmlFile.Create(kml, false);

                // Save file to a memory stream
                MemoryStream memStream = new MemoryStream();
                kmlFile.Save(memStream);

                // Write the KML data from the memory stream to the file
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.CopyTo(fileStream);
                }
            }
        }

        private class KML_DATA
        {
            public List<Vector> Positions { get; set; }
            public string Description { get; set; }
        }
        // Method to write CSV file
        private void EscribirFichero(List<List<DataItem>> bloque, string nombreFichero)
        {
            // Variable to track line number for the file
            int NumLinea = 1;
            DataItem.SetNombreFichero(nombreFichero);

            // Header of the CSV file with column names (separated by semicolons)
            string cabecera = "Num Linea;SAC;SIC;Time of Day;Latitud;Longitud;Altura;TYP;SIM;RDP;SPI;RAB;TST;ERR;XPP;ME;MI;FOE;ADSBEP;ADSBVAL;SCNEP;SCNVAL;PAIEP;PAIVAL;RHO;THETA;Mode-3/A V;Mode-3/A G;Mode-3/A L;Mode-3/A reply;FL V;FL G;Flight level;Mode C Corrected;SRL;SRR;SAM;PRL;PAM;RPD;APD;Aircraft address;Aircraft Identification;BDS4;MCPU/FCU Selected altitude;FMS Selected Altitude;Barometric pressure setting;Mode status;VNAV;ALTHOLD;Approach;Target status;Target altitude source;BDS5;Roll angle;True track angle;Ground Speed;Track angle rate;True Airspeed;BDS6;Magnetic heading;Indicated airspeed;Mach;Barometric altitude rate;Inertial Vertical Velocity;Track Number;X-Cartesian;Y-Cartesian;Calculated groundspeed;Calculated heading;CNF;RAD;DOU;MAH;CDM;TRE;GHO;SUP;TCC;Height Measured by a 3D Radar;COM;STATUS;SI;MSSC;ARC;AIC;B1A_message;B1B_message";

            // Check if the 'bloque' list (which contains the Aircrafts data) has elements
            if (bloque.Count > 0)
            {
                // Write the header to the file (using the first DataItem in the first block of data)
                bloque[0][0].EscribirEnFichero(cabecera + "\n", false);
            }

            // Iterate through each "block" of data in the "bloque" list
            foreach (var data in bloque)
            {
                List<string> atributosDI = new List<string>();

                // Iterate through each DataItem in the block (data)
                foreach (DataItem item in data)
                {
                    // Add the attributes (or string representation) of each DataItem to the list
                    atributosDI.Add(item.ObtenerAtributos());
                }
                string mensaje = string.Join("", atributosDI);

                // If the current block has at least one item
                if (data.Count > 0)
                {
                    // Write the line number to the file
                    data[0].EscribirEnFichero($"{NumLinea}" + ";", false);

                    NumLinea++;
                }

                // Write the joined string of attributes (message) to the file
                data[0].EscribirEnFichero(mensaje, false);

                // If the current block has at least one item, add a newline after writing the data
                if (data.Count > 0)
                {
                    data[0].EscribirEnFichero("\n", true);
                }
            }
        }


        private void CSV_File_Click(object sender, EventArgs e)
        {
            // Create a new SaveFileDialog instance to allow the user to select a file path
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Set the file filter so that the user can choose between CSV files or all files
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.Title = "Seleccionar la ubicación y el nombre del fichero";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the file path selected by the user
                    string filePath = saveFileDialog.FileName;

                    // Call the method to write data to the file (passing the file path)
                    EscribirFichero(bloque, filePath);
                    MessageBox.Show("S'ha escrit el fitxer correctament");
                }
            }
        }


        public void Corrected_Altitude(List<List<DataItem>> bloques)
        {
            foreach (var bloque in bloques)
            {
                // Primero, obtenemos la presión del bloque
                double? presion = null;
                foreach (var di in bloque)
                {
                    if (di is ModeS4 diConPresion)
                    {
                        if (diConPresion.BARtxt != "N/A")
                        {
                            string p = diConPresion.BARtxt;
                            presion = Convert.ToDouble(p);
                            break; // Obtenemos la presión una vez y salimos
                        }
                    }
                }

                // Si tenemos presión, procesamos los elementos que necesitan la corrección
                if (presion.HasValue)
                {
                    foreach (var di in bloque)
                    {
                        if (di is FlightLevel fl)
                        {
                            if (fl.FL != "N/A")
                            {
                                // Llamamos a CorrectedAltitude y almacenamos el valor en CorrectedAltitudeValue
                                fl.CorrectedAltitude(presion.Value);
                            }
                            else
                            {
                                fl.Alt_correct = " ";
                            }

                        }
                    }
                }
                else
                {
                    foreach (var di in bloque)
                    {
                        if (di is FlightLevel fl)
                        {
                            fl.Alt_correct = " ";
                        }
                    }
                }

            }

        }
        public void Calcular_Lat_Long(List<List<DataItem>> bloques)
        {
            // Iterate over each block of data
            foreach (var elemento in bloques)
            {
                double? Flight = null;

                // Iterate over each DataItem in the current block to find flight level
                foreach (var di in elemento)
                {
                    if (di is FlightLevel fl)
                    {
                        // Check if flight level data is available; convert it to double or set to 0 if "N/A"
                        if (fl.FL != "N/A")
                        {
                            Flight = Convert.ToDouble(fl.FL);
                        }
                        else
                        {
                            Flight = 0;
                        }


                    }
                }

                // Proceed if a flight level value was found
                if (Flight.HasValue)
                {
                    double Rho;
                    double Theta;
                    double Elev;
                    double Lat;
                    double Long;
                    double h;
                    string mensaje;

                    // Initialize radar coordinates (latitude, longitude, and height)
                    CoordinatesWGS84 coord = new CoordinatesWGS84(41.300702 * GeoUtils.DEGS2RADS, 2.102058 * GeoUtils.DEGS2RADS, 2.007 + 25.25);
                    GeoUtils geoUtils = new GeoUtils();
                    for (int i = 0; i < elemento.Count; i++)
                    {
                        if (elemento[i] is Position_Polar polar)
                        {
                            // Convert polar coordinates (rho and theta) to Cartesian and geodetic coordinates
                            Rho = Convert.ToDouble(polar.rho) * GeoUtils.NM2METERS;
                            Theta = Convert.ToDouble(polar.theta);
                            double RadEarth = geoUtils.CalculateEarthRadius(coord);                   
                            Elev = GeoUtils.CalculateElevation(coord, RadEarth, Rho, Math.Abs(Convert.ToDouble(Flight)) * 100 * GeoUtils.FEET2METERS);
                            
                            // Transform radar spherical coordinates to Cartesian
                            CoordinatesXYZ cartesian = GeoUtils.change_radar_spherical2radar_cartesian(new CoordinatesPolar(Rho, Theta * GeoUtils.DEGS2RADS, Elev));

                            // Transform radar Cartesian coordinates to geocentric coordinates
                            CoordinatesXYZ geocentric = geoUtils.change_radar_cartesian2geocentric(coord, cartesian);

                            // Transform geocentric coordinates to geodesic (latitude, longitude, height)
                            CoordinatesWGS84 geodesic = geoUtils.change_geocentric2geodesic(geocentric);

                            // Extract latitude and longitude in degrees + height 
                            double Lat_rad = geodesic.Lat;
                            double Long_rad = geodesic.Lon;
                            Lat = Lat_rad * GeoUtils.RADS2DEGS;
                            latitudList.Add(Lat);
                            Long = Long_rad * GeoUtils.RADS2DEGS;
                            longitudList.Add(Long);
                            h = geodesic.Height;
                            AltitudeList.Add(h);
                            mensaje = Convert.ToString(Lat) + ";" + Convert.ToString(Long) + ";" + Convert.ToString(h);
                            Geodesic_Coord geocoord = new Geodesic_Coord(mensaje);
                            geocoord.Descodificar();

                            // Insert the geocoord object into the current block of data
                            elemento.Insert(2, geocoord);
                            break;



                        }
                    }

                }
            }

        }

        public void GenerarAsterix(List<List<DataItem>> bloque)
        {
            int Num = 1; // Initialize a counter for numbering each grid

            foreach (var elemento in bloque)
            {
                // Create a new AsterixGrid object for each list in bloque
                AsterixGrid grid = new AsterixGrid();

                // Assign a unique number to the grid based on the counter (Num)
                grid.Num = Convert.ToString(Num);

                // Loop through each DataItem (di) in the current list (elemento)
                foreach (var di in elemento)
                {
                    // Retrieve an AsterixGrid representation from the DataItem (di)
                    AsterixGrid parcial = di.ObtenerAsterix();

                    // Assign values from 'parcial' to 'grid' only if they are non-empty (not null or empty strings)
                    if (!string.IsNullOrEmpty(parcial.SAC)) grid.SAC = parcial.SAC;
                    if (!string.IsNullOrEmpty(parcial.SIC)) grid.SIC = parcial.SIC;
                    if (!string.IsNullOrEmpty(parcial.Time)) grid.Time = parcial.Time;
                    if (!string.IsNullOrEmpty(parcial.TYP)) grid.TYP = parcial.TYP;
                    if (!string.IsNullOrEmpty(parcial.SIM)) grid.SIM = parcial.SIM;
                    if (!string.IsNullOrEmpty(parcial.RDP)) grid.RDP = parcial.RDP;
                    if (!string.IsNullOrEmpty(parcial.SPI)) grid.SPI = parcial.SPI;
                    if (!string.IsNullOrEmpty(parcial.RAB)) grid.RAB = parcial.RAB;
                    if (!string.IsNullOrEmpty(parcial.TST)) grid.TST = parcial.TST;
                    if (!string.IsNullOrEmpty(parcial.ERR)) grid.ERR = parcial.ERR;
                    if (!string.IsNullOrEmpty(parcial.XPP)) grid.XPP = parcial.XPP;
                    if (!string.IsNullOrEmpty(parcial.ME)) grid.ME = parcial.ME;
                    if (!string.IsNullOrEmpty(parcial.MI)) grid.MI = parcial.MI;
                    if (!string.IsNullOrEmpty(parcial.FOE)) grid.FOE = parcial.FOE;
                    if (!string.IsNullOrEmpty(parcial.ADS_EP)) grid.ADS_EP = parcial.ADS_EP;
                    if (!string.IsNullOrEmpty(parcial.ADS_VAL)) grid.ADS_VAL = parcial.ADS_VAL;
                    if (!string.IsNullOrEmpty(parcial.SCN_EP)) grid.SCN_EP = parcial.SCN_EP;
                    if (!string.IsNullOrEmpty(parcial.SCN_VAL)) grid.SCN_VAL = parcial.SCN_VAL;
                    if (!string.IsNullOrEmpty(parcial.PAI_EP)) grid.PAI_EP = parcial.PAI_EP;
                    if (!string.IsNullOrEmpty(parcial.PAI_VAL)) grid.PAI_VAL = parcial.PAI_VAL;
                    if (!string.IsNullOrEmpty(parcial.Rho)) grid.Rho = parcial.Rho;
                    if (!string.IsNullOrEmpty(parcial.Theta)) grid.Theta = parcial.Theta;
                    if (!string.IsNullOrEmpty(parcial.Latitude)) grid.Latitude = parcial.Latitude;
                    if (!string.IsNullOrEmpty(parcial.Longitude)) grid.Longitude = parcial.Longitude;
                    if (!string.IsNullOrEmpty(parcial.Height)) grid.Height = parcial.Height;
                    if (!string.IsNullOrEmpty(parcial.V_70)) grid.V_70 = parcial.V_70;
                    if (!string.IsNullOrEmpty(parcial.G_70)) grid.G_70 = parcial.G_70;
                    if (!string.IsNullOrEmpty(parcial.L_70)) grid.L_70 = parcial.L_70;
                    if (!string.IsNullOrEmpty(parcial.Mode3_A_Reply)) grid.Mode3_A_Reply = parcial.Mode3_A_Reply;
                    if (!string.IsNullOrEmpty(parcial.V_90)) grid.V_90 = parcial.V_90;
                    if (!string.IsNullOrEmpty(parcial.G_90)) grid.G_90 = parcial.G_90;
                    if (!string.IsNullOrEmpty(parcial.Flight_Level)) grid.Flight_Level = parcial.Flight_Level;
                    if (!string.IsNullOrEmpty(parcial.Mode_C_Correction)) grid.Mode_C_Correction = parcial.Mode_C_Correction;
                    if (!string.IsNullOrEmpty(parcial.SRL)) grid.SRL = parcial.SRL;
                    if (!string.IsNullOrEmpty(parcial.SRR)) grid.SRR = parcial.SRR;
                    if (!string.IsNullOrEmpty(parcial.SAM)) grid.SAM = parcial.SAM;
                    if (!string.IsNullOrEmpty(parcial.PRL)) grid.PRL = parcial.PRL;
                    if (!string.IsNullOrEmpty(parcial.PAM)) grid.PAM = parcial.PAM;
                    if (!string.IsNullOrEmpty(parcial.RPD)) grid.RPD = parcial.RPD;
                    if (!string.IsNullOrEmpty(parcial.APD)) grid.APD = parcial.APD;
                    if (!string.IsNullOrEmpty(parcial.Aircraft_Address)) grid.Aircraft_Address = parcial.Aircraft_Address;
                    if (!string.IsNullOrEmpty(parcial.Aircraft_Indentification)) grid.Aircraft_Indentification = parcial.Aircraft_Indentification;


                    // Asignamos las propiedades solo si parcial las tiene llenas
                    if (!string.IsNullOrEmpty(parcial.BDS_4_0)) grid.BDS_4_0 = parcial.BDS_4_0;
                    if (!string.IsNullOrEmpty(parcial.MCP_FCUtxt)) grid.MCP_FCUtxt = parcial.MCP_FCUtxt;
                    if (!string.IsNullOrEmpty(parcial.FMStxt)) grid.FMStxt = parcial.FMStxt;
                    if (!string.IsNullOrEmpty(parcial.BARtxt)) grid.BARtxt = parcial.BARtxt;
                    if (!string.IsNullOrEmpty(parcial.Mode_stat_txt)) grid.Mode_stat_txt = parcial.Mode_stat_txt;
                    if (!string.IsNullOrEmpty(parcial.VNAVMODEtxt)) grid.VNAVMODEtxt = parcial.VNAVMODEtxt;
                    if (!string.IsNullOrEmpty(parcial.ALTHOLDtxt)) grid.ALTHOLDtxt = parcial.ALTHOLDtxt;
                    if (!string.IsNullOrEmpty(parcial.Approachtxt)) grid.Approachtxt = parcial.Approachtxt;
                    if (!string.IsNullOrEmpty(parcial.StatusTargAlt)) grid.StatusTargAlt = parcial.StatusTargAlt;
                    if (!string.IsNullOrEmpty(parcial.TargetAltSourcetxt)) grid.TargetAltSourcetxt = parcial.TargetAltSourcetxt;

                    if (!string.IsNullOrEmpty(parcial.BDS_5_0)) grid.BDS_5_0 = parcial.BDS_5_0;
                    if (!string.IsNullOrEmpty(parcial.Rolltxt)) grid.Rolltxt = parcial.Rolltxt;
                    if (!string.IsNullOrEmpty(parcial.TrueTracktxt)) grid.TrueTracktxt = parcial.TrueTracktxt;
                    if (!string.IsNullOrEmpty(parcial.GroundSpeedtxt)) grid.GroundSpeedtxt = parcial.GroundSpeedtxt;
                    if (!string.IsNullOrEmpty(parcial.TrackAngletxt)) grid.TrackAngletxt = parcial.TrackAngletxt;
                    if (!string.IsNullOrEmpty(parcial.TrueAirspeedtxt)) grid.TrueAirspeedtxt = parcial.TrueAirspeedtxt;

                    if (!string.IsNullOrEmpty(parcial.BDS_6_0)) grid.BDS_6_0 = parcial.BDS_6_0;
                    if (!string.IsNullOrEmpty(parcial.MagHeadtxt)) grid.MagHeadtxt = parcial.MagHeadtxt;
                    if (!string.IsNullOrEmpty(parcial.IndAirtxt)) grid.IndAirtxt = parcial.IndAirtxt;
                    if (!string.IsNullOrEmpty(parcial.MACHtxt)) grid.MACHtxt = parcial.MACHtxt;
                    if (!string.IsNullOrEmpty(parcial.BarAlttxt)) grid.BarAlttxt = parcial.BarAlttxt;
                    if (!string.IsNullOrEmpty(parcial.InerVerttxt)) grid.InerVerttxt = parcial.InerVerttxt;

                    if (!string.IsNullOrEmpty(parcial.Track_Number)) grid.Track_Number = parcial.Track_Number;
                    if (!string.IsNullOrEmpty(parcial.X_Component)) grid.X_Component = parcial.X_Component;
                    if (!string.IsNullOrEmpty(parcial.Y_Component)) grid.Y_Component = parcial.Y_Component;

                    if (!string.IsNullOrEmpty(parcial.Ground_Speed)) grid.Ground_Speed = parcial.Ground_Speed;
                    if (!string.IsNullOrEmpty(parcial.Heading)) grid.Heading = parcial.Heading;

                    if (!string.IsNullOrEmpty(parcial.CNF)) grid.CNF = parcial.CNF;
                    if (!string.IsNullOrEmpty(parcial.RAD)) grid.RAD = parcial.RAD;
                    if (!string.IsNullOrEmpty(parcial.DOU)) grid.DOU = parcial.DOU;
                    if (!string.IsNullOrEmpty(parcial.MAH)) grid.MAH = parcial.MAH;
                    if (!string.IsNullOrEmpty(parcial.CDM)) grid.CDM = parcial.CDM;
                    if (!string.IsNullOrEmpty(parcial.TRE)) grid.TRE = parcial.TRE;
                    if (!string.IsNullOrEmpty(parcial.GHO)) grid.GHO = parcial.GHO;
                    if (!string.IsNullOrEmpty(parcial.SUP)) grid.SUP = parcial.SUP;
                    if (!string.IsNullOrEmpty(parcial.TCC)) grid.TCC = parcial.TCC;

                    if (!string.IsNullOrEmpty(parcial.Height_3D)) grid.Height_3D = parcial.Height_3D;

                    if (!string.IsNullOrEmpty(parcial.COM)) grid.COM = parcial.COM;
                    if (!string.IsNullOrEmpty(parcial.STAT)) grid.STAT = parcial.STAT;
                    if (!string.IsNullOrEmpty(parcial.SI)) grid.SI = parcial.SI;
                    if (!string.IsNullOrEmpty(parcial.MSSC)) grid.MSSC = parcial.MSSC;
                    if (!string.IsNullOrEmpty(parcial.ARC)) grid.ARC = parcial.ARC;
                    if (!string.IsNullOrEmpty(parcial.AIC)) grid.AIC = parcial.AIC;
                    if (!string.IsNullOrEmpty(parcial.B1A)) grid.B1A = parcial.B1A;
                    if (!string.IsNullOrEmpty(parcial.B1B)) grid.B1B = parcial.B1B;



                }

                // Add the completed 'grid' object to the list of 'asterixGrids'
                asterixGrids.Add(grid);

                // Increment the counter for the next grid number
                Num++;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            groupBox3.Show();
        }

        private void AcceptBut_Click(object sender, EventArgs e)
        {
            // Set up the ImageList to display the play button image
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);
            imageList.Images.Add(Properties.Resources.play_button);
            Start_sim.Image = imageList.Images[0];
            Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
            Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
            Start_sim.ImageAlign = ContentAlignment.TopCenter;
            Start_sim.TextAlign = ContentAlignment.BottomCenter;
            Start_sim.Text = " Continue";
            timer1.Stop();

            // Retrieve the aircraft IDs from the input fields
            string Aircraft1 = A1_IDbox.Text.Trim();
            string Aircraft2 = A2_IDbox.Text.Trim();

            int markerA1 = 0;
            int markerA2 = 0;

            // Check if the provided aircraft IDs exist in the list
            foreach (string ID in AircraftIDList)
            {
                string trimmedID = ID.Trim();
                if (trimmedID == Aircraft1)
                {
                    markerA1 = 1;
                }
                else if (trimmedID == Aircraft2)
                {
                    markerA2 = 1;
                }
                if (markerA1 == 1 && markerA2 == 1)
                {
                    break;
                }
            }


            int flag = 0;

            if (markerA1 == 1 && markerA2 == 1)
            {
                // Initialize lists for filtered data
                List<double> longitudList_sub = new List<double>();
                List<double> latitudList_sub = new List<double>();
                List<String> AircraftIDList_sub = new List<String>();

                List<string> AircraftAddrList_sub = new List<string>();
                List<string> TrackNumList_sub = new List<string>();
                List<string> Mode3AList_sub = new List<string>();
                List<string> SACList_sub = new List<string>();
                List<string> SICList_sub = new List<string>();
                List<double> AltitudeList_sub = new List<double>();


                // Filter the data for the two selected aircraft
                int positID = 0;
                foreach (string ID_Air in AircraftIDList)
                {
                    if (ID_Air.Trim() == Aircraft1 || ID_Air.Trim() == Aircraft2)
                    {
                        longitudList_sub.Add(longitudList[positID]);
                        latitudList_sub.Add(latitudList[positID]);
                        AircraftAddrList_sub.Add(AircraftAddrList[positID]);
                        AircraftIDList_sub.Add(AircraftIDList[positID].Trim());
                        TrackNumList_sub.Add(TrackNumList[positID]);
                        Mode3AList_sub.Add(Mode3AList[positID]);
                        SACList_sub.Add(SACList[positID]);
                        SICList_sub.Add(SICList[positID]);
                        AltitudeList_sub.Add(AltitudeList[positID]);
                    }
                    else
                    {
                        // Fill in "N/A" or default values for non-selected aircraft
                        longitudList_sub.Add(0.0);
                        latitudList_sub.Add(0.0);
                        AircraftAddrList_sub.Add("N/A");
                        AircraftIDList_sub.Add("N/A");
                        TrackNumList_sub.Add("N/A");
                        Mode3AList_sub.Add("N/A");
                        SACList_sub.Add("N/A");
                        SICList_sub.Add("N/A");
                        AltitudeList_sub.Add(0.0);
                    }
                    positID++;
                }

                // Calculate the horizontal distance between the two selected aircraft
                DistanciaHoritzontal(longitudList_sub, latitudList_sub, AltitudeList_sub, AircraftIDList_sub, Aircraft1, Aircraft2);

                // Open the simulation form, passing the selected aircraft
                DistHoritzontal formDistHor = new DistHoritzontal(Aircraft1, Aircraft2, longitudList_sub, latitudList_sub, AircraftIDList_sub, AircraftAddrList_sub, TrackNumList_sub, Mode3AList_sub, SACList_sub, SICList_sub, AltitudeList_sub, time, bloque, DistHor);
                formDistHor.Show();
            }
            else if (markerA1 == 0 && markerA2 == 0)
            {
                // Display an error message if both aircraft IDs are not found
                DialogResult result = MessageBox.Show("Both aircraft IDs are not found in the list.", "Continue", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    flag = 1;
                }
            }
            else if (markerA1 == 0)
            {
                // Display an error message if Aircraft 1 ID is not found
                DialogResult result = MessageBox.Show("Aircraft 1 ID is not found in the list.", "Continue", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    flag = 1;
                }
            }
            else if (markerA2 == 0)
            {
                // Display an error message if Aircraft 2 ID is not found
                DialogResult result = MessageBox.Show("Aircraft 2 ID is not found in the list.", "Continue", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    flag = 1;
                }
            }
        }

        internal const double height_radar_tang = 3438.954;

        internal const double Lat_deg_tang = 41.065656 * GeoUtils.DEGS2RADS;

        internal const double Lon_deg_tang = 1.413301 * GeoUtils.DEGS2RADS;

        internal CoordinatesWGS84 system_center_tang = new CoordinatesWGS84(Lat_deg_tang, Lon_deg_tang, height_radar_tang);
        public CoordinatesUVH GetUV(double latitude, double longitude, double height)
        {
            CoordinatesWGS84 Plane_lat_lon = new CoordinatesWGS84(latitude, longitude, height);

            GeoUtils geoUtils = new GeoUtils();

            geoUtils.setCenterProjection(system_center_tang);

            CoordinatesXYZ geocentric_coordinates = geoUtils.change_geodesic2geocentric(Plane_lat_lon);

            CoordinatesXYZ cartesian_system = geoUtils.change_geocentric2system_cartesian(geocentric_coordinates);

            CoordinatesUVH stereographic_system = geoUtils.change_system_cartesian2stereographic(cartesian_system);

            return stereographic_system;
        }




        public void DistanciaHoritzontal(List<double> longitudList_DH, List<double> latitudList_DH, List<double> AltitudeList_DH, List<String> AircraftIDList_sub, string A1, string A2)
        {
            double lat1 = 0, long1 = 0, height1 = 0;
            double lat2 = 0, long2 = 0, height2 = 0;

            CoordinatesUVH coord1 = null, coord2 = null;

            int flag1 = 0, flag2 = 0; // Initialize flags to 0, indicating not found

            for (int i = 0; i < AircraftIDList_sub.Count; i++)
            {
                string currentID = AircraftIDList_sub[i].Trim();

                
                if (currentID == A1.Trim() && flag1 == 0)
                {
                    // Find coordinates for Aircraft 1 (A1)
                    lat1 = latitudList_DH[flag1];
                    long1 = longitudList_DH[flag1];
                    height1 = AltitudeList_DH[flag1];
                    coord1 = GetUV(lat1 * GeoUtils.DEGS2RADS, long1 * GeoUtils.DEGS2RADS, height1);
                    flag1 = i;
                }
                else if (currentID == A2.Trim() && flag2 == 0)
                {
                    // Find coordinates for Aircraft 2 (A2)
                    lat2 = latitudList_DH[flag2];
                    long2 = longitudList_DH[flag2];
                    height2 = AltitudeList_DH[flag2];
                    coord2 = GetUV(lat2 * GeoUtils.DEGS2RADS, long2 * GeoUtils.DEGS2RADS, height2);
                    flag2 = i;
                }

            }

            // Iterate over the list of aircraft IDs to find the positions of the two selected aircraft (A1 and A2)
            for (int i = 0; i < AircraftIDList_sub.Count; i++)
            {
                string currentID = AircraftIDList_sub[i].Trim();

                if (currentID == A1.Trim())
                {
                    lat1 = latitudList_DH[i]; 
                    long1 = longitudList_DH[i];
                    height1 = AltitudeList_DH[i];

                    // Convert the latitude, longitude, and altitude to UVH coordinates (U, V, and height)
                    coord1 = GetUV(lat1 * GeoUtils.DEGS2RADS, long1 * GeoUtils.DEGS2RADS, height1);

                }
                else if (currentID == A2.Trim())
                {
                    lat2 = latitudList_DH[i]; 
                    long2 = longitudList_DH[i];
                    height2 = AltitudeList_DH[i];

                    // Convert the latitude, longitude, and altitude to UVH coordinates (U, V, and height)
                    coord2 = GetUV(lat2 * GeoUtils.DEGS2RADS, long2 * GeoUtils.DEGS2RADS, height2);
                }

                // Calculate the Euclidean distance between the two aircraft's UV coordinates (U and V)
                double distancia = Math.Round(Math.Sqrt(Math.Pow(coord2.U - coord1.U, 2) + Math.Pow(coord2.V - coord1.V, 2)), 3);

                // Add the calculated distance (converted to kilometers) to the DistHor list
                DistHor.Add(distancia * Math.Pow(10, -3));

            }
        }
        /*### PROJECT 3 ########################################################################################################################################*/
        internal int click = 0;
        List<(string ID, string AircraftID, double secs, string Rute, string TypePlane, string EstelaType, string TakeoffProcess, string TakeoffRWY)> rowData;
        List<PlaneFilter> ListPlanes;
        private List<(double time, double lat, double lon, double altitude, string AircraftAddress, string AircraftID, string TrackNum, string Mode_3A_Reply, string SAC, string SIC, double rho, bool ground, double WGS_altitude)> sim_data;
        private List<(string ID, string AircraftID, double sec, string, string, string, string, string)> ReadExcel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            List<(string ID, string AircraftID, double sec, string, string, string, string, string)> data = new List<(string, string, double, string, string, string, string, string)>();

            openFileDialog.Title = "Select the desired file";
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                string filePath = openFileDialog.FileName;

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(filePath)))
                {
                    // Assume data is in the first worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    int countRow = worksheet.Dimension.Rows;
                    int countCol = worksheet.Dimension.Columns;
                    int countAux = 0;

                    for (int rows = 2; rows <= countRow; rows++)
                    {
                        // ID
                        var colValue1 = worksheet.Cells[rows, 1].Value?.ToString() ?? "";
                        // Indicative
                        var colValue2 = worksheet.Cells[rows, 2].Value?.ToString() ?? "";

                        if (colValue1 != "" && colValue2 != "")
                        {
                            var colValue7 = worksheet.Cells[rows, 7].Value?.ToString() ?? "";
                            var colValue8 = worksheet.Cells[rows, 8].Value?.ToString() ?? "";

                            var TakeoffProcess = " ";

                            if (colValue7.Length > 2)
                            {
                                var TakeoffRWY = colValue8.Split('-');
                                var aux = colValue7.Substring(0, colValue7.Length - 2);
                                if (TakeoffRWY[1] == "24L")
                                {
                                    TakeoffProcess = aux + "-Q";
                                }
                                else if (TakeoffRWY[1] == "06R")
                                {
                                    TakeoffProcess = aux + "-R";
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            // Time
                            var colValue3 = worksheet.Cells[rows, 3].Value?.ToString() ?? "";
                            DateTime dateTime = DateTime.FromOADate(Convert.ToDouble(colValue3));
                            double sec = dateTime.Hour * 3600 + dateTime.Minute * 60 + dateTime.Second + dateTime.Millisecond;

                            var colValue4 = worksheet.Cells[rows, 4].Value?.ToString() ?? "";
                            var colValue5 = worksheet.Cells[rows, 5].Value?.ToString() ?? "";
                            var colValue6 = worksheet.Cells[rows, 6].Value?.ToString() ?? "";


                            data.Add((colValue1, colValue2, sec, colValue4, colValue5, colValue6, TakeoffProcess, colValue8));
                        }
                        else
                        {
                            countAux++;
                            if (countAux == 5) { break; }
                        }
                    }
                }
                return data;
            }
            else { return data; }
        }
        public double ConvertToSeconds(string time)
        {
            //MessageBox.Show(time);
            // Split the time string by the colon
            string[] timeParts = time.Split(':');
            double sec = Convert.ToDouble(timeParts[2]);
            return Convert.ToDouble(timeParts[0]) * 3600 + Convert.ToDouble(timeParts[1]) * 60 + Convert.ToDouble(sec);
        }
        private void Project3Btn_Click(object sender, EventArgs e)
        {
            click++;
            if (click == 1)
            {
                rowData = ReadExcel();

                if (rowData.Count != 0)
                {
                    ListPlanes = new List<PlaneFilter>();

                    for (int i = 0; i < asterixGrids.Count; i++)
                    {
                        bool find = false;
                        int j = 0;
                        // Buscar el AircraftID correspondiente en rowData
                        while (j < rowData.Count)
                        {
                            if (rowData[j].AircraftID.Trim().Equals(asterixGrids[i].Aircraft_Indentification.Trim())) //(Convert.ToString(rowData[j].AircraftID) == Convert.ToString(asterixGrids[i].Aircraft_Indentification))
                            {
                                find = true;
                                break; // Detener la búsqueda en el primer match
                            }
                            j++;
                        }
                        // Si se encontró un match, procesar el AircraftData
                        if (find)
                        {
                            var AircraftData = rowData[j];
                            if (AircraftData != default)
                            {
                                // Validación de tiempo
                                if (AircraftData.secs <= ConvertToSeconds(asterixGrids[i].Time))
                                {
                                    // Validación de coordenadas
                                    if ((40.9 > Convert.ToDouble(asterixGrids[i].Latitude) || Convert.ToDouble(asterixGrids[i].Latitude) > 41.7) && (1.5 > Convert.ToDouble(asterixGrids[i].Longitude) || Convert.ToDouble(asterixGrids[i].Longitude) > 2.6))
                                    {
                                        continue; // Skip invalid coordinates
                                    }
                                    else if (Convert.ToDouble(asterixGrids[i].Height) > 20000 || asterixGrids[i].Flight_Level == "NAN") //|| asterixGrids[i].GHO == true
                                    {
                                        continue; // Skip invalid heights or NAN flight levels
                                    }
                                    else
                                    {
                                        UVCoordinates WGS84Coordinates = new LibAsterix.UVCoordinates();
                                        CoordinatesUVH steorographicSys = WGS84Coordinates.GetUV(Convert.ToDouble(asterixGrids[i].Latitude) * GeoUtils.DEGS2RADS, Convert.ToDouble(asterixGrids[i].Latitude) * GeoUtils.DEGS2RADS, Convert.ToDouble(asterixGrids[i].Height));
                                        PlaneFilter planeFilter = new PlaneFilter
                                        {
                                            num = asterixGrids[i].Num,
                                            EstelaType = AircraftData.EstelaType,
                                            TakeoffProcess = AircraftData.TakeoffProcess,
                                            TakeoffRWY = AircraftData.TakeoffRWY,
                                            U = steorographicSys.U * GeoUtils.METERS2NM,
                                            V = steorographicSys.V * GeoUtils.METERS2NM,
                                            ID = Convert.ToInt32(AircraftData.ID),
                                            AircraftType = AircraftData.TypePlane,
                                            AircraftID = asterixGrids[i].Aircraft_Indentification,
                                            AircraftAddress = asterixGrids[i].Aircraft_Address,
                                            TrackNum = asterixGrids[i].Track_Number,
                                            time_sec = ConvertToSeconds(asterixGrids[i].Time),
                                            Lat = Convert.ToDouble(asterixGrids[i].Latitude),
                                            Lon = Convert.ToDouble(asterixGrids[i].Longitude),
                                            Altitude = Convert.ToDouble(asterixGrids[i].Height),
                                            BDS50 = asterixGrids[i].BDS_5_0,
                                            BDS60 = asterixGrids[i].BDS_6_0,
                                            RollAngle = asterixGrids[i].Rolltxt,
                                            TrueTrackAngle = asterixGrids[i].TrackAngletxt,
                                            GroundSpeed = asterixGrids[i].GroundSpeedtxt,
                                            MagneticHeading = asterixGrids[i].MagHeadtxt,
                                            IndicatedAirspeed = asterixGrids[i].IndAirtxt,
                                            Mach = asterixGrids[i].MACHtxt,
                                            BarometricAltitudeRate = asterixGrids[i].BarAlttxt,
                                            InertialVerticalVelocity = asterixGrids[i].InerVerttxt
                                        };
                                        ListPlanes.Add(planeFilter);
                                    }
                                }
                                //ListPlanes = ListPlanes.OrderBy(data => data.time_sec).ToList();
                            }
                        }

                    }

                    Projecte3 projecte3 = new Projecte3(ListPlanes);
                    MessageBox.Show(Convert.ToString(ListPlanes.Count));
                    projecte3.Show();
                }
            }
            else
            {
                //ListPlanes = ListPlanes.OrderBy(data => data.time_sec).ToList();
                Projecte3 projecte3 = new Projecte3(ListPlanes);
                projecte3.Show();
            }
        }
    
    }
}
