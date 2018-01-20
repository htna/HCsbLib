/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo.External
{
    public partial class Gromacs
    {
        public partial class Topol
        {
            public class Atom : Element
            {
                public readonly int    nr     ;
                public readonly string type   ;
                public readonly int    resnr  ;
                public readonly string residue;
                public readonly string atom   ;
                public readonly int    cgnr   ;
                public readonly double charge ;
                public readonly double mass   ;
                //typeB    chargeB      massB
                public Atom(string line,
                    int nr, string type, int resnr, string residue, string atom, int cgnr, double charge, double mass)
                    : base(line)
                {
                    this.nr      = nr     ;
                    this.type    = type   ;
                    this.resnr   = resnr  ;
                    this.residue = residue;
                    this.atom    = atom   ;
                    this.cgnr    = cgnr   ;
                    this.charge  = charge ;
                    this.mass    = mass   ;
                }

                public static Atom FromString(string line)
                {
                    //;   nr       type  resnr residue  atom   cgnr     charge       mass  typeB    chargeB      massB
                    //     1        NH3      1    ARG      N      1       -0.3     14.007   ; qtot -0.3
                    string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(tokens.Length == 8);
                    int    nr      =    int.Parse(tokens[0]);
                    string type    =              tokens[1];
                    int    resnr   =    int.Parse(tokens[2]);
                    string residue =              tokens[3];
                    string atom    =              tokens[4];
                    int    cgnr    =    int.Parse(tokens[5]);
                    double charge  = double.Parse(tokens[6]);
                    double mass    = double.Parse(tokens[7]);
                    return new Atom(line, nr, type, resnr, residue, atom, cgnr, charge, mass);
                }
            }
        }
    }
}
*/