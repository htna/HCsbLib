﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class PdbStatic
    {
        public static double GetResolution(this IEnumerable<Pdb.Remark2> remark2s)
        {
            double? resolution = null;
            foreach(var remark2 in remark2s)
            {
                {
                    string line = remark2.line;
                    line = line.Replace("REMARK   2" , "");
                    line = line.Replace("RESOLUTION.", "");
                    line = line.Replace("ANGSTROMS." , "");
                    line = line.Trim();
                    double res;
                    if(double.TryParse(line, out res))
                    {
                        HDebug.Assert(resolution == null);
                        resolution = res;
                        continue;
                    }
                }
                {
                    if(remark2._RESOLUTION != "RESOLUTION.") continue;
                    if(remark2._ANGSTROMS  != "ANGSTROMS." ) continue;
                    HDebug.Assert(resolution == null);
                    resolution = remark2.TryResolution();
                }
            }

            return resolution.Value;
        }
    }
    public partial class Pdb
    {
        [Serializable]
        public class Remark2 : Element, IBinarySerializable
        {
            /// http://www.wwpdb.org/documentation/format23/remarks.html
            ///
            /// REMARK 2
            /// REMARK records present experimental details, annotations, comments, and
            /// 
            /// REMARK 2 states the highest resolution, in Angstroms, that was used in
            /// building the model. As with all the remarks, the first REMARK 2 record
            /// is empty and is used as a spacer.
            /// 
            /// Record Format and Details
            /// 
            /// The second REMARK 2 record has one of two formats. The first is used for
            /// diffraction studies, the second for other types of experiments in which
            /// resolution is not relevant, e.g., NMR.
            /// 
            /// For diffraction experiments:
            /// COLUMNS    DATA TYPE      FIELD        DEFINITION
            /// -----------------------------------------------------------------------
            ///  1 -  6    Record name    "REMARK"
            /// 10         LString(1)     "2"
            /// 12 - 22    LString(11)    "RESOLUTION."
            /// 24 - 30    Real(7.2)      resolution   Resolution.
            /// 32 - 41    LString(10)    "ANGSTROMS."
            /// 
            /// REMARK 2 when not a diffraction experiment:
            /// 
            /// COLUMNS        DATA TYPE     FIELD          DEFINITION
            /// --------------------------------------------------------------------------------
            ///  1 -  6        Record name   "REMARK"
            /// 10             LString(1)    "2"
            /// 12 - 38        LString(28)   "RESOLUTION.  NOT APPLICABLE."
            /// 
            /// Example
            /// 
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// --------------------------------------------------------------------------------
            /// REMARK   2                                                            
            /// REMARK   2 RESOLUTION.    1.74 ANGSTROMS.
            /// REMARK   2                                                            
            /// REMARK   2 RESOLUTION.NOT APPLICABLE.
            /// REMARK   2                                                                      
            /// REMARK   2 RESOLUTION.    7.50  ANGSTROMS.
            /// 
            public static string RecordName { get { return "REMARK"; } }

            Remark2(string line)
                : base(line)
            {
            }
            public static Remark2 FromString(string line)
            {
                HDebug.Assert(IsRemark2(line));
                return new Remark2(line);
            }
            public static bool IsRemark2(string line) { return (line.Substring(0,10) == "REMARK   2"); }
            public char   _REMARKNUM  { get { return Char   (idxs__REMARKNUM);       } } static readonly int[] idxs__REMARKNUM= new int[]{10,10}; // 10         LString(1)     "2"
            public string _RESOLUTION { get { return String (idxs__RESOLUTIO);       } } static readonly int[] idxs__RESOLUTIO= new int[]{12,22}; // 12 - 22    LString(11)    "RESOLUTION."
            public double resolution  { get { return Double (idxs_resolution).Value; } } static readonly int[] idxs_resolution= new int[]{24,30}; // 24 - 30    Real(7.2)      resolution   Resolution.
            public string _ANGSTROMS  { get { return String (idxs__ANGSTROMS);       } } static readonly int[] idxs__ANGSTROMS= new int[]{32,41}; // 32 - 41    LString(10)    "ANGSTROMS."

            public double? TryResolution()
            {
                //try
                //{
                    if( '2'           != Char  (idxs__REMARKNUM) ) return null;
                    if( "RESOLUTION." != String(idxs__RESOLUTIO) ) return null;
                    if( "ANGSTROMS."  != String(idxs__ANGSTROMS) ) return null;
                    double? resolution = Double(idxs_resolution);
                    return  resolution;
                //}
                //catch
                //{
                //    return null;
                //}
            }

            // IComparable<Atom>
            //int IComparable<Atom>.CompareTo(Atom other)
            //{
            //    return serial.CompareTo(other.serial);
            //}

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Remark2(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public Remark2(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }
}
