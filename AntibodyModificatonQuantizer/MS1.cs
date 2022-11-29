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
        private static double SampleIntensityThreshold = 1000; // 1E3

        public int ScanNumber { get; private set; }
        public double RetentionTime { get; private set; }
        public bool IsSample { get; private set; }
        public List<MS2> ChildSpectra { get; private set; }
        public List<object> QuantResults { get; private set; }

        public MS1(ThermoRawFile rawfile, int scanNumber)
        {
            this.ScanNumber = scanNumber;
            this.RetentionTime = rawfile.GetRetentionTime(this.ScanNumber);
            this.ChildSpectra = new List<MS2>();
            this.QuantResults = new List<object>();

            if (rawfile.GetSpectrum(scanNumber).GetBasePeakIntensity() > MS1.SampleIntensityThreshold)
            {
                this.IsSample = true;
            }
            else
            {
                this.IsSample = false;
            }
        }
    }
}
