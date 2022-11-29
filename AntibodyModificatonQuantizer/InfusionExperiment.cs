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
        private static int expectedNumberOfMsnOnPeak = 8;
        private static int toleratedNumberofMsnOnPeak = (int)Math.Round(InfusionExperiment.expectedNumberOfMsnOnPeak * 0.6);
        private static int missedScanTolerance = 1;

        bool isMissedInjection;
        List<MS1> experimentParentScans;
        List<object> antibodyTargets;

        public InfusionExperiment(Dictionary<int, MS1> scanGroup, ThermoRawFile rawfile)
        {

        }

        // to group MS1 scans together by experiment, use an MS1 TIC intensity cutoff of 1E3
        // if there are more than tolerated MS1 scans with < 1E3 TIC total,
        //      group it.
        //          call it a missed injection.
        //          Don't digest until you hit signal again or the end of the raw file
        //  
        // if there are more than 5 MS1 scans with > 1E3 TIC in a row followed by a blank,
        //      group it.
        //          Call it a successful injection and process once you lose signal again for x2 scans
        public static bool ExperimentComplete(Dictionary<int, MS1> currentScanQueue, out string message)
        {
            // no point peak detecting when we just started aggregating scans
            if (currentScanQueue.Count < InfusionExperiment.expectedNumberOfMsnOnPeak * 0.6)
            {
                message = "Not enough points in group!";
                return false;
            }

            var sequentialSampleScans = 0;
            var sequentialBlankScans = 0;

            // now see how many "Sample" msn scans you have in a row
            foreach (var key in currentScanQueue.Keys)
            {
                if (sequentialSampleScans == 0)
                {
                    if (currentScanQueue[key].IsSample)
                    {
                        sequentialSampleScans++;
                    }
                }
                // we're theoretically in a sample peak now. start peak detection
                else
                {
                    if (currentScanQueue[key].IsSample)
                    {
                        sequentialSampleScans++;
                    }
                    else
                    {
                        // if we've hit the tolerance indicating the end of a peak, and we have hit enough points to be considered a peak
                        if (sequentialBlankScans == 2 && sequentialSampleScans > InfusionExperiment.expectedNumberOfMsnOnPeak * 0.6)
                        {
                            message = "Good Sample Peak!";
                            return true;
                        }
                        // if we've hit the tolerance indicating the end of a peak, and we don't have enough points to be considered a peak
                        else if (sequentialBlankScans == 2 && sequentialSampleScans <= InfusionExperiment.expectedNumberOfMsnOnPeak * 0.6)
                        {
                            message = "Bad Sample Peak!";
                            return true;
                        }
                        // use this to ignore accidental drop in electrospray?
                        else
                        {
                            sequentialBlankScans++;
                        }
                    }
                }
            }

            message = "Have not hit peak detection conditions yet";
            return false;
        }
    }
}
