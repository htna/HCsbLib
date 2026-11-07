using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace HTLib2
{
    using DEBUG = System.Diagnostics.Debug;

    public partial class HDebug
    {
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void Assert(params bool[] conditions)
        {
            AssertAnd(conditions);
            //          System.Diagnostics.Debug.Assert(condition);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertAllEquals<T>(params T[] values)
            where T : IEquatable<T>
        {
            System.Diagnostics.Debug.Assert(IsEqualsAll(values));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertDouble(params double[] values)
        {
            foreach(double value in values)
            {
                System.Diagnostics.Debug.Assert(double.IsInfinity(value) == false);
                System.Diagnostics.Debug.Assert(double.IsNaN(value) == false);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerance(double tolerance, params double[] values)
        {
            System.Diagnostics.Debug.Assert(CheckTolerance(tolerance, values));
        }
        public static bool CheckTolerance(double tolerance, params double[] values)
        {
            for(int i = 0; i<values.Length; i++)
                if(Math.Abs(values[i]) > tolerance)
                    return false;
            //System.Diagnostics.Debug.Assert(assert);
            return true;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertToleranceIf(bool condition, double tolerance, params double[] values)
        {
            if(condition)
            {
                bool assert = true;
                for(int i = 0; i<values.Length; i++)
                    assert &= (Math.Abs(values[i]) <= tolerance);
                System.Diagnostics.Debug.Assert(assert);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerance(double tolerance, params double[][] values)
        {
            bool assert = true;
            for(int i = 0; i<values.Length; i++)
                for(int j = 0; j<values[i].Length; j++)
                    assert &= (Math.Abs(values[i][j]) <= tolerance);
            System.Diagnostics.Debug.Assert(assert);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertTolerance(double tolerance, double[,] values)
        {
            bool assert = CheckTolerance(tolerance, values);
            System.Diagnostics.Debug.Assert(assert);
        }
        public static bool CheckTolerance(double tolerance, double[,] values)
        {
            for(int c = 0; c<values.GetLength(0); c++)
                for(int r = 0; r<values.GetLength(1); r++)
                {
                    double value = values[c, r];
                    if(Math.Abs(value) > tolerance)
                        return false;
                }
            return true;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertSimilar(double left, double right, double tolerance)
        {
            System.Diagnostics.Debug.Assert(Math.Abs(left-right) <= tolerance);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertSimilar(double[] left, double[] right, double tolerance)
        {
            if(left.Length != right.Length) { DEBUG.Assert(false); return; }
            for(int i = 0; i<left.Length; i++)
            {
                if(Math.Abs(left[i]-right[i]) <= tolerance)
                    continue;
                DEBUG.Assert(false);
                return;
            }
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertSimilar(double[,] left, double[,] right, double tolerance)
        {
            if(left.GetLength(0) != right.GetLength(0)) { DEBUG.Assert(false); return; }
            if(left.GetLength(1) != right.GetLength(1)) { DEBUG.Assert(false); return; }
            for(int i = 0; i<left.GetLength(0); i++)
            {
                for(int j = 0; j<left.GetLength(1); j++)
                {
                    if(Math.Abs(left[i, j]-right[i, j]) <= tolerance)
                        continue;
                    DEBUG.Assert(false);
                    return;
                }
            }
        }
        //////////////////////////////////////////////
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotSimilar(double left, double right, double tolerance)
        {
            System.Diagnostics.Debug.Assert(Math.Abs(left-right) > tolerance);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotSimilar(double[] left, double[] right, double tolerance)
        {
            if(left.Length != right.Length) return;
            for(int i = 0; i<left.Length; i++)
            {
                if(Math.Abs(left[i]-right[i]) <= tolerance)
                    continue;
                return;
            }
            DEBUG.Assert(false);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotSimilar(double[,] left, double[,] right, double tolerance)
        {
            if(left.GetLength(0) != right.GetLength(0)) return;
            if(left.GetLength(1) != right.GetLength(1)) return;
            for(int i = 0; i<left.GetLength(0); i++)
            {
                for(int j = 0; j<left.GetLength(1); j++)
                {
                    if(Math.Abs(left[i, j]-right[i, j]) <= tolerance)
                        continue;
                    return;
                }
            }
            DEBUG.Assert(false);
        }
        //////////////////////////////////////////////
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotNull<T>(T value)
            where T : class
        {
            System.Diagnostics.Debug.Assert(value != null);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotNull<T>(params T[] value)
            where T : class
        {
            bool assert = true;
            if(value == null)
                assert = false;
            else
                for(int i = 0; i<value.Length; i++)
                    if(value[i] == null)
                        assert = false;
            DEBUG.Assert(assert);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertNotNull<T>(T[,] value)
            where T : class
        {
            bool assert = true;
            if(value == null)
                assert = false;
            else
                for(int i = 0; i<value.GetLength(0); i++)
                    for(int j = 0; j<value.GetLength(1); j++)
                        if(value[i, j] == null)
                            assert = false;
            DEBUG.Assert(assert);
        }

        //////////////////////////////////////////////
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertOr(params bool[] conditions)
        {
            foreach(bool condition in conditions)
            {
                if(condition == true)
                {
                    return;
                }
            }
            System.Diagnostics.Debug.Assert(false);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertAnd(params bool[] conditions)
        {
            bool success = true;
            foreach(bool condition in conditions)
            {
                if(condition == false)
                {
                    success = false;
                }
            }
            System.Diagnostics.Debug.Assert(success);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertXor(params bool[] conditions)
        {
            int numsuccess = 0;
            foreach(bool condition in conditions)
            {
                if(condition == true)
                {
                    numsuccess++;
                }
            }
            System.Diagnostics.Debug.Assert(numsuccess == 1);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertIf(bool condition, params bool[] asserts)
        {
            if(condition)
            {
                bool assert = true;
                for(int i = 0; i<asserts.Length; i++)
                    assert = assert && asserts[i];
                Assert(assert);
            }
        }

        // <hashcode_for_name, <name, is_assert>>
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerHiddenAttribute()]
        public static void AssertIf(string name, bool initial_condition, bool assert)
        {
            int hash = name.GetHashCode();
            if(_ConditionalAssert.ContainsKey(hash) == false)
            {
                _ConditionalAssert[hash] = new Dictionary<string, bool>();
                _ConditionalAssert[hash][name] = initial_condition;
                AssertIf(initial_condition, assert);
                return;
            }
            if(_ConditionalAssert[hash].ContainsKey(name) == false)
            {
                _ConditionalAssert[hash][name] = initial_condition;
                AssertIf(initial_condition, assert);
                return;
            }
            AssertIf(_ConditionalAssert[hash][name], assert);
        }
    }
}
