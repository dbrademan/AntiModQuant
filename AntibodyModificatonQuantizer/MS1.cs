using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO.Thermo;

namespace AntibodyModificatonQuantizer
{
    internal class MS1
    {
        public static double SampleIntensityThreshold = 1000; // 1E3

        public ThermoRawFile Rawfile { get; private set; }
        public int ScanNumber { get; private set; }
        public double RetentionTime { get; private set; }
        public bool IsSample { get; private set; }
        public List<MS2> ChildSpectra { get; private set; }
        public List<object> QuantResults { get; private set; }

        public MS1(ThermoRawFile rawfile, int scanNumber)
        {
            this.Rawfile = rawfile;
            this.ScanNumber = scanNumber;
            this.RetentionTime = rawfile.GetRetentionTime(this.ScanNumber);
            this.ChildSpectra = new List<MS2>();
            this.QuantResults = new List<object>();

            if (rawfile.GetTIC(this.ScanNumber) > MS1.SampleIntensityThreshold)
            {
                this.IsSample = true;
            }
            else
            {
                this.IsSample = false;
            }
        }

        public static void SetSampleIntensityThreshold(double newThreshold)
        {
            MS1.SampleIntensityThreshold = newThreshold;
        }
    }
}
