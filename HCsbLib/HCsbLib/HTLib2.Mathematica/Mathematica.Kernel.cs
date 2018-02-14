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
        public static object   EvaluateObject     (string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetObject     (); }); }
        public static double   EvaluateDouble     (string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetDouble     (); }); }
        public static double[] EvaluateDoubleArray(string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetDoubleArray(); }); }

        public static T Evaluate<T>(string evaluate, Func<IKernelLink,T> GetT)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            ml.Evaluate(evaluate);
            ml.WaitForAnswer();

            T result;
            try
            {
                result = GetT(ml);
                //result = ml.GetDoubleArray();
                //result = ml.GetObject();
            }
            catch
            {
                result = default(T);
                //result = null;
            }
            ml.Close();
            return result;
        }

        public static void EvaluatePng(string evaluate, int width, int height, string pngpath)
        {
        }
        public static Image EvaluateImage(string evaluate, int width, int height)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            Image image;
            try
            {
                ml.WaitAndDiscardAnswer();
                image = ml.EvaluateToImage(evaluate, width, height);
                //ml.WaitForAnswer();
            }
            catch
            {
                image = null;
            }
            ml.Close();
            return image;
        }
        public static void EvaluatePng(string evaluate, int width, string pngpath)
        {
            Image image = EvaluateImage(evaluate, width);
            image.Save(pngpath);
        }
        public static Image EvaluateImage(string evaluate, int width)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            Image image;
            try
            {
                ml.WaitAndDiscardAnswer();
                image = ml.EvaluateToTypeset(evaluate, width);
                //ml.WaitForAnswer();
            }
            catch
            {
                image = null;
            }
            ml.Close();
            return image;
        }
    }
}
