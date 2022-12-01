using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.Proteomics;

namespace AntibodyModificatonQuantizer
{
    internal class QuantificationGroup
    {
        public string peptideName;
        public QuantificationTarget standardForm;
        public QuantificationTarget modifiedPeptide;

        public QuantificationGroup(string peptideName, string peptideSequence, int charge, Modification fixedModification, 
            Modification variableModification)
        {
            // create standard peptide
            var standardPeptide = new Peptide(peptideSequence);
            // add fixed modifications

            Modification carboxymethylCystine = null;
            ModificationDictionary.TryGetModification("Carboxymethyl", out carboxymethylCystine);

            standardPeptide.AddModification(carboxymethylCystine);

            // duplicate standardPeptide object
            var modifiedPeptide = new Peptide(standardPeptide, true);

            // add variable modification
            // hard-code specifically to LSC(Carboxymethyl)VASGFIFSNHWM*N*WVR
            // just to double check I'm doing everything correctly
            Modification oxidationModification;
            ModificationDictionary.TryGetModification("Oxidation", out oxidationModification);
        }
    }
}
