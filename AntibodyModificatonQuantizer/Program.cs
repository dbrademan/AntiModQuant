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
            ///         - Adapting this to a dynamic intensity cutoff. 
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

            var rawfilePath = @"P:\DRB_ASH_AntibodyCharacterization\withMS2\17jun2022_NM_oxCurve_20220617022316.raw";
            var rawfile = new ThermoRawFile(rawfilePath);
            rawfile.Open();

            //## COMPLETE SOPHISTICATED EXPERIMENT DETECTION LATER
            //## For now, just use signal > threshold, experiment on. signal < Threshold, experiment off;
            InfusionExperiment.DeterminePeakDetectionThresholds(rawfile);

            // go through raw file. Aggregate MS1 scans and flag as Sample or NotSample
            Dictionary<int, MS1> ms1Scans = new Dictionary<int, MS1>();
            var currentMs1Scan = -1;

            for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
            {
                if (rawfile.GetMsnOrder(i) == 1)
                {
                    currentMs1Scan = i;
                    ms1Scans.Add(i, new MS1(rawfile, i));
                }
                else
                {
                    ms1Scans[currentMs1Scan].ChildSpectra.Add(new MS2(rawfile, i));
                }
            }

            // using your lazy algorithm, try and group experiments
            // seems to work fine for the better data of Alex's
            var infusionExperiments = InfusionExperiment.NaivePeakDetection(ms1Scans);
            var t = "";
            
            // organise your quantification targets. 
            List<QuantificationGroup> quantificationGroups = new List<QuantificationGroup>();



        }
    }
}
