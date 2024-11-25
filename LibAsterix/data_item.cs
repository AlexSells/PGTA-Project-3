using System;
using System.IO;

namespace LibAsterix
{
    // Clase base
    public abstract class DataItem
    {
        // Atributos
        public string info;
        public static string NombreFichero {  get; set; }

        // Constructor
        public DataItem(string info)
        {
            
            this.info = info;
            
        }

        // Getters y Setters
       
        public string GetInfo()
        {
            return info;
        }

        public void SetInfo(string info)
        {
            this.info = info;
        }

      

        // Método abstracto que será implementado por las clases hijas
        public abstract void Descodificar();
        public abstract string ObtenerAtributos();
        public abstract AsterixGrid ObtenerAsterix();

        // Método que escribe en un fichero
        public void EscribirEnFichero(string mensaje, bool Saltolinea=false)
        {
            if (string.IsNullOrEmpty(NombreFichero))
            {
                throw new InvalidOperationException("La ruta del fichero no es válida");
            }

         
            using (StreamWriter escritor = new StreamWriter(NombreFichero, true))
            {
                if (Saltolinea)
                {
                    escritor.WriteLine(mensaje);
                }
                else
                {
                    escritor.Write(mensaje);
                }
               
            }
            

        }

        /*public void EscribirEnDataGridView (string mensaje, bool nuevaFila = false, string[] valores)
        {
            
                // If DataGridView has no columns, set them up based on the first line's structure
                if (dataGridView.Columns.Count == 0)
                {
                    // Assume that columns are separated by ";" as per your example
                    string[] headers = mensaje.Split(';');
                    foreach (var header in headers)
                    {
                        dataGridView.Columns.Add(header, header);  // Add columns with the header names
                    }
                    return; // Stop here as we only set up columns initially
                }

                // Now, if nuevaFila is true, we add a new row with `mensaje` values
                if (nuevaFila)
                {
                    string[] valores = mensaje.Split(';');  // Split by delimiter to get individual cell values
                    dataGridView.Rows.Add(valores);  // Add the entire array as a row
                }
                else
                {
                    // If nuevaFila is false, update the last row if it exists
                    if (dataGridView.Rows.Count > 0)
                    {
                        string[] valores = mensaje.Split(';');
                        for (int i = 0; i < valores.Length && i < dataGridView.ColumnCount; i++)
                        {
                            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[i].Value = valores[i];
                        }
                    }
                }

        }*/
        public static void SetNombreFichero(string nombreFichero)
        {
            NombreFichero = nombreFichero;
        }

        public virtual void CorrectedAltitude(double p)
        {

        }
    }
}
