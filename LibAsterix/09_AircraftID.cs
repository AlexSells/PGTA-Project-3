using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class AircraftID : DataItem
    {

        public string ID {  get; private set; }


        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public AircraftID(string info)
            : base(info)
        {

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


        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                ID = "N/A";
            }
            else
            {
                ID = "";
                if (base.info.Length % 6 != 0)
                {
                    throw new ArgumentException("La cadena no és múltiple de 6");
                }
                for (int i = 0; i < base.info.Length; i += 6)
                {
                    string block = base.info.Substring(i, 6);
                    ID += ConvertirBitsAChar(block);
                }
            }
        }

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
        public override string ObtenerAtributos()
        {
            string mensaje = ID + ";";
            return mensaje;
        }

        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Aircraft_Indentification = ID;
            return grid;
        }
    }
}
