using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.IO.Thermo;

namespace AntibodyModificatonQuantizer
{
    internal class InfusionExperiment
    {
        private static double MaximumLog10BackgroundSignal = 6;

        public List<MS1> experimentParentScans;
        public List<object> antibodyTargets;

        public InfusionExperiment(List<MS1> scanGroup)
        {
            this.experimentParentScans = scanGroup;
        }

        public static void DeterminePeakDetectionThresholds(ThermoRawFile rawfile)
        {
            // the background signal for flagging an MS1 as Sample or NotSample is somewhat dynamic.
            // I've seen thresholds which could be placed from 1E3 to 1E6.
            // We can detect an appropriate threshold by log10 tranforming the MS1 TIC intensity and picking FWHM of the lower distribution.
            // Lazy approach currently, detect log10 threshold and add log10 unit of 1 as "cutoff".
            MS1.SetSampleIntensityThreshold(InfusionExperiment.SetDynamicBackgroundThreshold(rawfile));
        }

        private static double SetDynamicBackgroundThreshold(ThermoRawFile rawfile)
        {
            var Ms1TicHistogram = HistogramBuilder(rawfile);

            // get max value in histogram < MaximumToleratedBackgroundSignal
            var maxValue = -1;
            double threshold = -1;

            for (var i = 0.0; i <= InfusionExperiment.MaximumLog10BackgroundSignal; i += 0.1)
            {
                // round i to prevent floating point math errors
                var roundedThreshold = Math.Round(i, 1);
                if (maxValue <= Ms1TicHistogram[roundedThreshold])
                {
                    maxValue = Ms1TicHistogram[roundedThreshold];
                    threshold = roundedThreshold;
                }
            }

            // untransform from log10 spacae plus a fudge factor
            // TODO: calculate fudge factor from histogram distribution
            return Math.Pow(10, threshold + 1);
        }

        private static Dictionary<double, int> HistogramBuilder(ThermoRawFile rawfile)
        {
            // create histogram to store log10 transformed TIC 
            var returnDictionary = new Dictionary<double, int>();

            // signal should never be larger than 1e11
            for (var i = 0.0; i <= 11; i += 0.1)
            {
                returnDictionary.Add(Math.Round(i, 1), 0);
            }

            for (var i = rawfile.FirstSpectrumNumber; i <= rawfile.LastSpectrumNumber; i++)
            {
                if (rawfile.GetMsnOrder(i) == 1)
                {
                    var ms1TIC = rawfile.GetTIC(i);
                    double log10TransformedTIC = -1;

                    if (ms1TIC == 0)
                    {
                        // can't log10 transform 0;
                        returnDictionary[0]++;
                    }
                    else
                    {
                        // log10 transformTic and round to nearest tenth's place
                        log10TransformedTIC = Math.Round(Math.Log10(ms1TIC), 1);
                        var t = "";

                        returnDictionary[log10TransformedTIC]++;
                    }
                }
            }

            return returnDictionary;
        }

        
        // lazy algorithm.
        // in short, anytime we hit a sample-flagged MS1, start aggregating scans
        // start adding MS1 scans to it
        // once we hit a nonsample-flagged MS1, start a new list
        // Will not account for drops in spray stability or find missed injections
        public static List<InfusionExperiment> NaivePeakDetection(Dictionary<int, MS1> ms1Scans)
        {
            var returnList = new List<InfusionExperiment>();

            var sortedKeys = ms1Scans.Keys.ToList();
            sortedKeys.Sort();

            List<MS1> experimentScanList = new List<MS1>();
            var inExperiment = false;

            foreach (var key in sortedKeys)
            {
                var thisMs1 = ms1Scans[key];

                if (inExperiment)
                {
                    if (thisMs1.IsSample)
                    {
                        experimentScanList.Add(thisMs1);
                    }
                    else
                    {
                        inExperiment = false;
                        returnList.Add(new InfusionExperiment(experimentScanList));
                    }
                }
                else
                {
                    // currently not aggregating MS1 scans
                    if (thisMs1.IsSample) 
                    {
                        inExperiment = true;

                        experimentScanList = new List<MS1>();
                        experimentScanList.Add(thisMs1);
                    }
                }
            }

            return returnList;
        }
        
    }
}
