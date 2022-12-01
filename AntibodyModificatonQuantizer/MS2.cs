using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.IO.Thermo;

namespace AntibodyModificatonQuantizer
{
    internal class MS2
    {
        public ThermoRawFile rawfile { get; private set; }
        public int ScanNumber { get; private set; }
        public double RetentionTime { get; private set; }
        public List<object> QuantResults { get; private set; }

        public MS2(ThermoRawFile rawfile, int spectrumNumber)
        {
            this.rawfile = rawfile;
            this.ScanNumber = spectrumNumber;
            this.RetentionTime = rawfile.GetRetentionTime(spectrumNumber);

            this.QuantResults = new List<object>();
        }
    }
}
