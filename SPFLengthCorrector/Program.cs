using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPFLengthCorrector
{
    public enum NoteFace
    {
        FUFF = 1,    // Face up, face first
        FUFL,       // Face up, face last
        FDFF,       // Face down, face first
        FDFL        // Face down, face last
    }



    class Program
    {
        static CMySql m_MySql;
        public const byte SENSOR_DATA_LENGTH = 50;

        static void Main(string[] args)
        {
            string Database = args[0];
            string Table = args[1];
            int Face = int.Parse(args[2]);
            

            string FaceString = ((NoteFace)Face).ToString();

            int CalculatedSensorDataLength = 0;
            int CurrentSensorDataLength = 0;
            int MaxSensorDataLength = 0;
            int MinSensorDataLength = SENSOR_DATA_LENGTH;
            int NumberOfMismatches = 0;

            m_MySql = new CMySql(Database);
            Console.WriteLine("Finding Records");
            List<int> IDs = m_MySql.GetCorruptDataIDs(Table, Face);
            Console.WriteLine(IDs.Count.ToString() + " Found");

            foreach(int ID in IDs)
            {
                CalculatedSensorDataLength = m_MySql.GetSensorLength(Table, ID, SENSOR_DATA_LENGTH, out CurrentSensorDataLength);
                Console.Write("ID " + ID.ToString() + " Length: " + CalculatedSensorDataLength.ToString() + "\\" + CurrentSensorDataLength.ToString());

                //if (CalculatedSensorDataLength == 3)
                //{
                //    Console.WriteLine("222");
                //}

                if (CalculatedSensorDataLength != CurrentSensorDataLength)
                {
                    Console.WriteLine("***");
                    NumberOfMismatches++;
                    m_MySql.OverwriteSensorLength(Table, ID, CalculatedSensorDataLength);
                }
                else
                {
                    Console.WriteLine("");
                }

                MaxSensorDataLength = Math.Max(MaxSensorDataLength, CalculatedSensorDataLength);
                MinSensorDataLength = Math.Min(MinSensorDataLength, CalculatedSensorDataLength);
            }
            Console.WriteLine("");
            Console.WriteLine(Database.ToString() + "." + Table.ToString() + " " + Face.ToString() +" ("+  FaceString+ ")");
            Console.WriteLine("Records Found: " + IDs.Count.ToString() );
            Console.WriteLine("Mismatches: " + NumberOfMismatches.ToString());
            Console.WriteLine("Max Sensor Length: " + MaxSensorDataLength.ToString());
            Console.WriteLine("Min Sensor Length: " + MinSensorDataLength.ToString());
        }
    }
}
