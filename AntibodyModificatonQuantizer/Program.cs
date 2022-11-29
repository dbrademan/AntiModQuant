using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.IO.Thermo;

namespace AntibodyModificatonQuantizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /// <remarks>
            /// 
            /// Goals of this project:
            ///     Alex has a set of raw files where he infuses antibodies under multiple experimental conditions in one raw file. 
            ///     He wants to go through each injection (which represents a tryptic digestion of an antibody)
            ///     and from MS1 and MS2 quantify the ratio of modified & unmodified peptides. 
            /// 
            /// Strategy: 
            ///     Go through a raw file containing multiple NanoMate injections (infusion).
            ///     - Group MS1 scans by experiment. 
            ///         - Try raw intensity cutoff first. 
            ///             - Lazy but likely effective solution.
            ///         - Point out suspected missed injections as well.
            ///     
            ///     - Add relevant MS2 scans to each experiment.
            ///     - Go through each MS1, try to quant the modified/unmodified peptide forms from intact m/z
            ///         - Do monoisotope first. 
            ///             - Maybe do isotopic envelope modeling later.
            ///     - Then go through each MS2 sampling a peptide of interest and quant ratios there as well. 
            ///     
            /// </remarks>

            var rawfilePath = @"T:\File_exchange\Pavel\from Alex\ms2\31oct2022_deamidation_nist3.raw";
            var rawfile = new ThermoRawFile(rawfilePath);
            rawfile.Open();

            // to group MS1 scans together by experiment, use an MS1 TIC intensity cutoff of 1E3
            // if there are more than 5 MS1 scans with < 1E3 TIC in a row,
            //      group it.
            //          call it a missed injection.
            //          Don't digest until you hit signal again or the end of the raw file
            //  
            // if there are more than 5 MS1 scans with > 1E3 TIC in a row,
            //      group it.
            //          Call it a successful injection and process once you lose signal again for x2 scans

            // go through raw file. Aggregate runs. Digest queue if it hits the above thresholds and restart
            Dictionary<int, MS1> runningScanQueue = new Dictionary<int, MS1>();
            
            for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
            {
                if (rawfile.GetMsnOrder(i) == 1)
                {
                    var outMessage = "";
                    // check if you need to digest the scan queue at this point.
                    if (InfusionExperiment.ExperimentComplete(runningScanQueue, out outMessage))
                    {
                        var t = "";
                        //InfusionExperiment.Digest(runningScanQueue);
                    }


                    runningScanQueue.Add(i, new MS1(rawfile, i));
                }
                // if this isn't an MS1, the parent scan should already be in the dictinary. Add it
                else
                {
                    runningScanQueue[rawfile.GetParentSpectrumNumber(i)].ChildSpectra.Add(new MS2());
                }
            }

            // finish off by digesting scan queue now that we're at the end of the raw file




        }
    }
}
