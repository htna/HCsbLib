using System;
using Wolfram.NETLink;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Mathematica
	{
        public sealed class Expr : IDisposable, ISerializable
        {
            public Wolfram.NETLink.Expr expr;

            public Expr(object obj)                         { expr = new Wolfram.NETLink.Expr(obj); }
            public Expr(ExpressionType type, string val)    { expr = new Wolfram.NETLink.Expr(type, val); }
            public Expr(object head, params object[] args)  { expr = new Wolfram.NETLink.Expr(head, args); }

            public Expr this[int part]                      { get { return new Expr(expr[part]); } }

            public Expr[] Args                              { get { Expr[] args = new Expr[expr.Args.Length];
                                                                    for(int i=0; i<args.Length; i++) args[i] = new Expr(expr.Args[i]);
                                                                    return args;
                                                                  } }
            public Expr Head                                { get { return new Expr(expr.Head); } }  
            public int Length                               { get { return expr.Length    ; } }   
            public int[] Dimensions                         { get { return expr.Dimensions; } }

            public static Expr CreateFromLink(IMathLink ml)                             { return new Expr(Wolfram.NETLink.Expr.CreateFromLink(ml)); }
            public Array AsArray(ExpressionType reqType, int depth)                     { return          expr.AsArray(reqType, depth); }
            public double AsDouble()                                                    { return          expr.AsDouble(); }
            public long AsInt64()                                                       { return          expr.AsInt64(); }
            public bool AtomQ()                                                         { return          expr.AtomQ(); }
            public bool ComplexQ()                                                      { return          expr.ComplexQ(); }
            public Expr Delete(int n)                                                   { return new Expr(expr.Delete(n)); }
            public void Dispose()                                                       {                 expr.Dispose(); }
            public override bool Equals(object obj)                                     { return          expr.Equals(obj); }
            public override int GetHashCode()                                           { return          expr.GetHashCode(); }
            public void GetObjectData(SerializationInfo info, StreamingContext context) {                 expr.GetObjectData(info, context); }
            public Expr Insert(Expr e, int n)                                           { return new Expr(expr.Insert(e.expr, n)); }
            public bool IntegerQ()                                                      { return          expr.IntegerQ(); }
            public bool ListQ()                                                         { return          expr.ListQ(); }
            public bool MatrixQ()                                                       { return          expr.MatrixQ(); }
            public bool MatrixQ(ExpressionType elementType)                             { return          expr.MatrixQ(elementType); }
            public bool NumberQ()                                                       { return          expr.NumberQ(); }
            public Expr Part(int[] ia)                                                  { return new Expr(expr.Part(ia)); }
            public Expr Part(int i)                                                     { return new Expr(expr.Part(i)); }
            public void Put(IMathLink ml)                                               {                 expr.Put(ml); }
            public bool RationalQ()                                                     { return          expr.RationalQ(); }
            public bool RealQ()                                                         { return          expr.RealQ(); }
            public bool StringQ()                                                       { return          expr.StringQ(); }
            public bool SymbolQ()                                                       { return          expr.SymbolQ(); }
            public Expr Take(int n)                                                     { return new Expr(expr.Take(n)); }
            public override string ToString()                                           { return          expr.ToString(); }
            public bool TrueQ()                                                         { return          expr.TrueQ(); }
            public bool VectorQ()                                                       { return          expr.VectorQ(); }
            public bool VectorQ(ExpressionType elementType)                             { return          expr.VectorQ(elementType); }

            public static bool operator ==(Expr x, Expr y)                              { return (x.expr == y.expr); }
            public static bool operator !=(Expr x, Expr y)                              { return (x.expr != y.expr); }

            public static explicit operator string(Expr e)                              { return (string)e.expr; }
            public static explicit operator double(Expr e)                              { return (double)e.expr; }
            public static explicit operator long  (Expr e)                              { return (long  )e.expr; }
        }
    }
}
