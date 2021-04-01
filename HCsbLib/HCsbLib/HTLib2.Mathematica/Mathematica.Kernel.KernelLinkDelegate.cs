using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolfram.NETLink;
using System.Drawing;

namespace HTLib2
{
	public partial class Mathematica
	{
        public static KernelLinkDelegate CreateKernelLinkDelegate()
        {
            IKernelLink ml = HMathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            return new KernelLinkDelegate
            {
                ml = ml,
            };
        }
        public class KernelLinkDelegate
        {
            public Wolfram.NETLink.IKernelLink ml;

            public Exception LastError                                          { get { return ml.LastError          ; } }
            public bool TypesetStandardForm                                     { get { return ml.TypesetStandardForm; } set { value = ml.TypesetStandardForm; } }
            public string GraphicsFormat                                        { get { return ml.GraphicsFormat     ; } set { value = ml.GraphicsFormat     ; } }
            public bool UseFrontEnd                                             { get { return ml.UseFrontEnd        ; } set { value = ml.UseFrontEnd        ; } }
            public bool WasInterrupted                                          { get { return ml.WasInterrupted     ; } set { value = ml.WasInterrupted     ; } }
          //public event PacketHandler PacketArrived                            { get { return ml.PacketArrived; } }
            public void AbandonEvaluation()                                     {        ml.AbandonEvaluation()                                 ; }
            public void AbortEvaluation()                                       {        ml.AbortEvaluation()                                   ; }
            public void BeginManual()                                           {        ml.BeginManual()                                       ; }
            public void EnableObjectReferences()                                {        ml.EnableObjectReferences()                            ; }
            public void Evaluate(string s)                                      {        ml.Evaluate(s)                                         ; }
            public void Evaluate(Expr e)                                        {        ml.Evaluate(e.expr)                                    ; }
          //public Image EvaluateToImage(Expr e, int width, int height)         { return new Image(ml.EvaluateToImage(e.expr, width, height))   ; }
          //public Image EvaluateToImage(string s, int width, int height)       { return new Image(ml.EvaluateToImage(s, width, height))        ; }
            public string EvaluateToInputForm(string s, int pageWidth)          { return ml.EvaluateToInputForm(s, pageWidth)                   ; }
            public string EvaluateToInputForm(Expr e, int pageWidth)            { return ml.EvaluateToInputForm(e.expr, pageWidth)              ; }
            public string EvaluateToOutputForm(string s, int pageWidth)         { return ml.EvaluateToOutputForm(s, pageWidth)                  ; }
            public string EvaluateToOutputForm(Expr e, int pageWidth)           { return ml.EvaluateToOutputForm(e.expr, pageWidth)             ; }
          //public Image EvaluateToTypeset(Expr e, int width)                   { return new Image(ml.EvaluateToTypeset(e.expr, width))         ; }
          //public Image EvaluateToTypeset(string s, int width)                 { return new Image(ml.EvaluateToTypeset(s, width))              ; }
            public Array GetArray(Type leafType, int depth)                     { return ml.GetArray(leafType, depth)                           ; }
            public Array GetArray(Type leafType, int depth, out string[] heads) { return ml.GetArray(leafType, depth, out heads)                ; }
            public ExpressionType GetExpressionType()                           { return ml.GetExpressionType()                                 ; }
            public ExpressionType GetNextExpressionType()                       { return ml.GetNextExpressionType()                             ; }
            public object GetObject()                                           { return ml.GetObject()                                         ; }
            public void HandlePacket(PacketType pkt)                            {        ml.HandlePacket(pkt)                                   ; }
            public void InterruptEvaluation()                                   {        ml.InterruptEvaluation()                               ; }
            public void Message(string symtag, params string[] args)            {        ml.Message(symtag, args)                               ; }
            public bool OnPacketArrived(PacketType pkt)                         { return ml.OnPacketArrived(pkt)                                ; }
            public void Print(string s)                                         {        ml.Print(s)                                            ; }
            public void Put(object obj)                                         {        ml.Put(obj)                                            ; }
            public void PutReference(object obj, Type t)                        {        ml.PutReference(obj, t)                                ; }
            public void PutReference(object obj)                                {        ml.PutReference(obj)                                   ; }
            public void TerminateKernel()                                       {        ml.TerminateKernel()                                   ; }
            public void WaitAndDiscardAnswer()                                  {        ml.WaitAndDiscardAnswer()                              ; }
            public PacketType WaitForAnswer()                                   { return ml.WaitForAnswer()                                     ; }
        }
    }
}
