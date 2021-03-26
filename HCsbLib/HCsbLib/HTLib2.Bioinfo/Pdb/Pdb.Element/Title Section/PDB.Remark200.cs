using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    /// https://www.wwpdb.org/documentation/file-format-content/format33/remarks1.html#REMARK%20200
    ///
    /// REMARK 200
    ///
    /// REMARK 200 is mandatory if single crystal, fiber, or polycrystalline X-ray
    /// diffraction experiments were performed. The format of date in this remark is
    /// DD-MMM-YY. DD is the day of the month (a number 01 through 31), MMM is the
    /// English 3-letter abbreviation for the month, and YY is the year.
    /// 
    /// Template
    /// 
    ///          1         2         3         4         5         6         7         8
    /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
    /// --------------------------------------------------------------------------------
    /// REMARK 200
    /// REMARK 200 EXPERIMENTAL DETAILS
    /// REMARK 200  EXPERIMENT TYPE                :  X-RAY DIFFRACTION
    /// REMARK 200  DATE OF  DATA COLLECTION       : 
    /// REMARK 200  TEMPERATURE           (KELVIN) : 
    /// REMARK 200  PH                             : 
    /// REMARK 200  NUMBER  OF CRYSTALS USED       : 
    /// REMARK 200
    /// REMARK 200  SYNCHROTRON              (Y/N) : 
    /// REMARK 200  RADIATION SOURCE               : 
    /// REMARK 200  BEAMLINE                       : 
    /// REMARK 200  X-RAY  GENERATOR MODEL         : 
    /// REMARK 200  MONOCHROMATIC OR LAUE    (M/L) : 
    /// REMARK 200  WAVELENGTH OR RANGE        (A) : 
    /// REMARK 200  MONOCHROMATOR                  : 
    /// REMARK 200  OPTICS                         : 
    /// REMARK 200
    /// REMARK 200  DETECTOR  TYPE                 : 
    /// REMARK 200  DETECTOR  MANUFACTURER         : 
    /// REMARK 200  INTENSITY-INTEGRATION SOFTWARE : 
    /// REMARK 200  DATA  SCALING SOFTWARE         : 
    /// REMARK 200  
    /// REMARK 200  NUMBER  OF UNIQUE REFLECTIONS  : 
    /// REMARK 200  RESOLUTION RANGE HIGH      (A) : 
    /// REMARK 200  RESOLUTION RANGE LOW       (A) : 
    /// REMARK 200  REJECTION CRITERIA  (SIGMA(I)) : 
    /// REMARK 200
    /// REMARK 200 OVERALL.
    /// REMARK 200  COMPLETENESS FOR RANGE     (%) : 
    /// REMARK 200  DATA  REDUNDANCY               : 
    /// REMARK 200  R  MERGE                    (I): 
    /// REMARK 200  R  SYM                      (I): 
    /// REMARK 200  <I/SIGMA(I)> FOR THE DATA SET  : 
    /// REMARK 200
    /// REMARK 200 IN THE HIGHEST RESOLUTION SHELL.
    /// REMARK 200  HIGHEST RESOLUTION SHELL, RANGE HIGH (A) : 
    /// REMARK 200  HIGHEST RESOLUTION SHELL, RANGE LOW  (A) : 
    /// REMARK 200  COMPLETENESS FOR SHELL     (%) : 
    /// REMARK 200  DATA  REDUNDANCY IN SHELL      : 
    /// REMARK 200  R MERGE  FOR SHELL          (I): 
    /// REMARK 200  R SYM  FOR SHELL            (I): 
    /// REMARK 200  <I/SIGMA(I)> FOR SHELL   : 
    /// REMARK 200
    /// REMARK 200 METHOD USED TO DETERMINE THE STRUCTURE: 
    /// REMARK 200 SOFTWARE USED: 
    /// REMARK 200 STARTING MODEL: 
    /// REMARK 200
    /// REMARK 200 REMARK:  
    /// Examples
    /// 
    /// The following example illustrates the how REMARK 200 will be used in cases in
    /// which multiple data collections are described. In this example, data items
    /// corresponding to different data collection sessions are separated by semi-colons.
    /// Multiple data values within a single session (e.g. wavelength) are separated by
    /// commas.
    /// 
    /// REMARK 200                                                                      
    /// REMARK 200 EXPERIMENTAL DETAILS                                                 
    /// REMARK 200  EXPERIMENT TYPE                : X-RAY DIFFRACTION                  
    /// REMARK 200  DATE OF  DATA COLLECTION       : 17-MAR-02;  17-MAR-02           
    /// REMARK 200  TEMPERATURE           (KELVIN) : 100; 100                           
    /// REMARK 200  PH                             : 8.00                               
    /// REMARK 200  NUMBER  OF CRYSTALS USED       : 2                                  
    /// REMARK 200                                                                      
    /// REMARK 200  SYNCHROTRON              (Y/N) : Y; Y                               
    /// REMARK 200  RADIATION SOURCE               : APS ; APS                          
    /// REMARK 200  BEAMLINE                       : 17ID; 17ID                         
    /// REMARK 200  X-RAY  GENERATOR MODEL         : NULL                               
    /// REMARK 200  MONOCHROMATIC OR LAUE    (M/L) : M; M                               
    /// REMARK 200  WAVELENGTH OR RANGE        (A) : 1.5545; 1.0720, 1.0723, 
    /// REMARK 200                                   1.0543     
    /// REMARK 200  MONOCHROMATOR                  : SI (111); SI (111)                 
    /// REMARK 200  OPTICS                         : NULL                               
    /// REMARK 200                                                                      
    /// REMARK 200  DETECTOR  TYPE                 : CCD; CCD                           
    /// REMARK 200  DETECTOR  MANUFACTURER         : ADSC QUANTUM 210;  ADSC
    /// REMARK 200                                   QUANTUM  210                       
    /// REMARK 200  INTENSITY-INTEGRATION SOFTWARE : DENZO                              
    /// REMARK 200  DATA  SCALING SOFTWARE         : HKL                                
    /// REMARK 200                                                                      
    /// REMARK 200  NUMBER  OF UNIQUE REFLECTIONS  : 29132                              
    /// REMARK 200  RESOLUTION RANGE HIGH      (A) : 1.900                             
    /// REMARK 200  RESOLUTION RANGE LOW       (A) : 30.000                             
    /// REMARK 200  REJECTION CRITERIA  (SIGMA(I)) : 0.000                             
    /// REMARK 200                                                                      
    /// REMARK 200 OVERALL.                                                             
    /// REMARK 200  COMPLETENESS FOR RANGE     (%) : 98.3                               
    /// REMARK 200  DATA  REDUNDANCY               : 19.800                             
    /// REMARK 200  R  MERGE                    (I): NULL                               
    /// REMARK 200  R  SYM                      (I): 0.07500                            
    /// REMARK 200  <I/SIGMA(I)> FOR THE DATA SET  : 17.0000
    /// REMARK 200                                                                      
    /// REMARK 200 IN THE HIGHEST RESOLUTION SHELL.                                     
    /// REMARK 200  HIGHEST RESOLUTION SHELL, RANGE HIGH (A) : 1.90                     
    /// REMARK 200  HIGHEST RESOLUTION SHELL, RANGE LOW  (A) : 1.97                     
    /// REMARK 200  COMPLETENESS FOR SHELL     (%) : 83.4                               
    /// REMARK 200  DATA  REDUNDANCY IN SHELL      : 3.00                               
    /// REMARK 200  R MERGE  FOR SHELL          (I): NULL                               
    /// REMARK 200  R SYM  FOR SHELL            (I): 0.65000
    /// REMARK 200  <I/SIGMA(I)> FOR SHELL         : 1.500
    /// REMARK 200                                                                      
    /// REMARK 200 DIFFRACTION PROTOCOL: SINGLE WAVELENGTH; MAD                        
    /// REMARK 200 METHOD USED TO DETERMINE THE STRUCTURE: MAD                          
    /// REMARK 200 SOFTWARE USED: SOLVE 2.02                                            
    /// REMARK 200 STARTING MODEL: NULL                                                 
    /// REMARK 200                                                                      
    /// REMARK 200 REMARK: NULL
    /// 
    public static partial class PdbStatic
    {
        public static double? GetRemark200Temperature(this IEnumerable<Pdb.Remark> remarks)
        {
            double? temp = null;
            foreach(var remark in remarks.HEnumByRemarkNum(200))
            {
                string contents = remark.contents;
                if(contents.Contains("TEMPERATURE") == false)
                    continue;
                HDebug.Assert(temp == null);
                HDebug.Assert(contents.Contains("(KELVIN)"));
                contents = contents.Replace("TEMPERATURE", "");
                contents = contents.Replace("(KELVIN)"   , "");
                contents = contents.Replace(":"   , "");
                string[] tokens = contents.Split(';');
                foreach(string token in tokens)
                {
                    double dbl;
                    if(double.TryParse(contents, out dbl))
                    {
                        temp = dbl;
                        break;
                    }
                }
            }
            return temp;
        }
    }
}
