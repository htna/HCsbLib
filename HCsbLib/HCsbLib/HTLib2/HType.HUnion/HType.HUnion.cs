using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HTLib2
{
    public partial class HType
    {
        [ StructLayout(LayoutKind.Explicit) ]
        public struct HUnion
        {
            [ FieldOffset(0) ] public char   cval;
            [ FieldOffset(0) ] public int    ival;
            [ FieldOffset(0) ] public bool   bval;
            [ FieldOffset(0) ] public double dval;
          //[ FieldOffset(0) ] public object oval;
        }
    }
}
