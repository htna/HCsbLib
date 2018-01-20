using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Vector : Vector<double>
    {
        public static bool operator_mul_Vector_double = Debug.IsDebuggerAttached;
        public static Vector operator*(Vector val1, double val2)
        {
            if(operator_mul_Vector_double)
                #region selftest
            {
                operator_mul_Vector_double = false;
                Vector tv1 = new double[] { 1, 2, 3 };
                double tv2 = -2;
                Vector tvo = tv1 * tv2;
                Debug.AssertTolerant(0, tvo - new Vector(-2, -4, -6));
            }
                #endregion

            double[] ret = new double[val1.data.Length];
            for(int i=0; i<ret.Length; i++)
                ret[i] = val1[i] * val2;
            return ret;
        }
        public static Vector operator*(double val1, Vector val2)
        {
            return val2*val1;
        }
        public static bool operator_div_Vector_double = Debug.IsDebuggerAttached;
        public static Vector operator/(Vector val1, double val2)
        {
            if(operator_div_Vector_double)
                #region selftest
            {
                operator_div_Vector_double = false;
                Vector tv1 = new double[] { 2, 6, -8 };
                double tv2 = -2;
                Vector tvo = tv1 / tv2;
                Debug.AssertTolerant(0, tvo - new Vector(-1, -3, 4));
            }
                #endregion

            double[] ret = new double[val1.data.Length];
            for(int i=0; i<ret.Length; i++)
                ret[i] = val1[i] / val2;
            return ret;
        }
        public static bool operator_add_vector_vector = Debug.IsDebuggerAttached;
        public static Vector operator+(Vector val1, double[] val2)
        {
            if(operator_add_vector_vector)
                #region selftest
            {
                operator_add_vector_vector = false;
                Vector tv1 = new double[] { 2, 6, -8 };
                Vector tv2 = new double[] { 1, 2, 3  };
                Vector tvo = tv1 + tv2;
                Debug.AssertTolerant(0, tvo - new Vector(3, 8, -5));
            }
                #endregion

            Debug.Assert(val1.data.Length == val2.Length);
            double[] ret = new double[val1.data.Length];
            for(int i=0; i<ret.Length; i++)
                ret[i] = val1[i] + val2[i];
            return ret;
        }
        public static bool operator_sub_vector_vector = Debug.IsDebuggerAttached;
        public static Vector operator-(Vector val1, double[] val2)
        {
            if(operator_sub_vector_vector)
                #region selftest
            {
                operator_sub_vector_vector = false;
                Vector tv1 = new double[] { 2, 6, -8 };
                Vector tv2 = new double[] { 1, 2, 3  };
                Vector tvo = tv1 - tv2;
                Debug.AssertTolerant(0, tvo - new Vector(1, 4, -11));
            }
                #endregion

            Debug.Assert(val1.data.Length == val2.Length);
            double[] ret = new double[val1.data.Length];
            for(int i=0; i<ret.Length; i++)
                ret[i] = val1[i] - val2[i];
            return ret;
        }
        //public static bool operator>(Vector<T> val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i > val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator>=(Vector<T> val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i >= val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator<(Vector<T> val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i < val2) == false)
        //            return false;
        //    return true;
        //}
        //public static bool operator<=(Vector<T> val1, T val2)
        //{
        //    foreach(dynamic val1i in val1.data)
        //        if((val1i <= val2) == false)
        //            return false;
        //    return true;
        //}
    }
}
