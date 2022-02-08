using System;
using Amplitude.Mercury.Data.Simulation;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class ConstructibleStore
    {
        
        private static string LastName(string name)
        {
            string result;
            
            try
            {
                result = name.Substring(name.LastIndexOf('_') + 1);
            }
            catch (Exception)
            {
                result = name;
            }

            return result;
        }

        private static int EraLevel(string name)
        {
            int result = 0;
            
            try
            {
                result = int.TryParse(name.Substring(name.LastIndexOf('_') - 1, 1), out int level) ? level : 0;
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        private static string UnitLastName(string name)
        {
            string result;
            
            try
            {
                result = name.Substring(name.LastIndexOf('_', name.LastIndexOf('_') - 1) + 1);
            }
            catch (Exception)
            {
                result = name;
            }

            return result;
        }

        private static int UnitEraLevel(string name)
        {
            int result = 0;
            
            try
            {
                result = int.TryParse(name.Substring(name.LastIndexOf('_', name.LastIndexOf('_') - 1) - 1, 1), out int level) ? level : 0;
            }
            catch (Exception)
            {
                result = 0;
            }

            return result;
        }

        private static bool CommonDistrictsClause(DistrictDefinition d)
        {
            return d.Prototype == "Extension_Prototype_BaseEconomy" ||
                   d.Prototype == "Extension_Prototype_BaseMilitary" ||
                   d.Prototype == "Extension_Prototype_Base" ||
                   d.name == "Extension_Base_Harbour" ||
                   d.name == "Extension_Base_Extractor" ||
                   d.name == "Extension_Base_Luxury" ||
                   d.name == "Exploitation" ||
                   d.name == "Extension_Base_WondrousExtractor";
        }
        
        private static bool ResourceDistrictsClause(DistrictDefinition d)
        {
            return d.Prototype == "Extension_Base_Extractor" || d.Prototype == "Extension_Base_WondrousExtractor";
        }
        
        private static bool EmblematicDistrictsClause(DistrictDefinition d)
        {
            return d.Prototype == "Extension_Prototype_Emblematic";
        }
    }
}
