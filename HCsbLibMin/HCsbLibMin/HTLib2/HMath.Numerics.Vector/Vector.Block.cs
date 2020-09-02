using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Vector
	{
        public Vector Round(int digit)
        {
            Vector vec = new double[Size];
            for(int i=0; i<Size; i++)
                vec[i] = Math.Round(this[i], digit);
            return vec;
        }
        public Vector[] Reshape(int dim)
        {
            HDebug.Assert(Size%dim == 0);
            Vector[] vecs = new Vector[Size/dim];
            for(int i=0; i<vecs.Length; i++)
            {
                vecs[i] = new double[dim];
                for(int j=0; j<dim; j++)
                    vecs[i][j] = this[i*dim+j];
            }
            return vecs;
        }

        public Vector[] ToVectors(int vecsize)
        {
            return Reshape(vecsize);
        }

        public static Vector[] NewVectors(int numcopy, Vector vec)
        {
            Vector[] vecs = new Vector[numcopy];
            for(int i=0; i<numcopy; i++)
                vecs[i] = vec.Clone();
            return vecs;
        }
        public static Vector From(Vector[] blockvector)
        {
            return FromBlockvector(blockvector);
        }
        public static Vector FromBlockvector(Vector[] blockvector)
        {
            List<double> vec = new List<double>();
            foreach(Vector vector in blockvector)
                for(int i=0; i<vector.Size; i++)
                    vec.Add(vector[i]);
            return vec.ToArray();
        }
        public static Vector[] Clone(Vector[] source)
        {
            Vector[] target = new Vector[source.Length];
            for(int i=0; i<target.Length; i++)
                target[i] = source[i].Clone();
            return target;
        }
        public static Vector[] Mul(Vector[] l, double r, bool allowNullVec=false)
        {
            Vector[] result = Clone(l);
            MulTo(result, r, allowNullVec: allowNullVec);
            return result;
        }
        public static void MulTo(Vector[] target, double value, bool allowNullVec=false)
        {
            for(int i=0; i<target.Length; i++)
            {
                if(target[i] != null) { target[i] = target[i] * value; continue; }
                if(allowNullVec)
                {
                    if(target[i] == null) continue;
                }
                HDebug.Assert(false);
            }
        }
        public static void AddTo(Vector[] target, Vector[] value, bool allowNullVec=false)
        {
            HDebug.Exception(target.Length == value.Length);
            for(int i=0; i<target.Length; i++)
            {
                if(target[i] != null && value[i] != null) { target[i] = target[i] + value[i]; continue; }
                if(allowNullVec)
                {
                    if(target[i] == null && value[i] == null) continue;
                    if(target[i] == null && value[i] != null) { target[i] = value[i].Clone(); continue; }
                    if(target[i] != null && value[i] == null) { target[i] = value[i].Clone(); continue; }
                }
                HDebug.Assert(false);
            }
        }
        public static Vector[] Sub(Vector[] l, Vector[] r, bool allowNullVec=false)
        {
            Vector[] result = Clone(l);
            SubFrom(result, r, allowNullVec: allowNullVec);
            return result;
        }
        public static void SubFrom(Vector[] target, Vector[] value, bool allowNullVec=false)
        {
            HDebug.Assert(target.Length == value.Length);
            for(int i=0; i<target.Length; i++)
            {
                if(target[i] != null && value[i] != null) { target[i] = target[i] - value[i]; continue; }
                if(allowNullVec)
                {
                    if(target[i] == null && value[i] == null) continue;
                    if(target[i] == null && value[i] != null) { target[i] = value[i].Clone(); continue; }
                    if(target[i] != null && value[i] == null) { target[i] = value[i].Clone(); continue; }
                }
                HDebug.Assert(false);
            }
        }

        public static Vector VtV(Vector[] l, Vector[] r)
        {
            HDebug.Assert(l.Length == r.Length);
            int length = l.Length;
            Vector result = new double[length];
            for(int i=0; i<length; i++)
                result[i] = LinAlg.VtV(l[i], r[i]);
            return result;
        }
    }
    public static class VectorBlock
    {
        public static IList<Vector> GetOrthonormals(this IList<Vector> vectors)
        {
            Vector[] omvecs = new Vector[vectors.Count];
            omvecs[0] = vectors[0].UnitVector();
            for(int i=1; i<vectors.Count; i++)
            {
                Vector nomvec = vectors[i].UnitVector();
                foreach(Vector omvec in omvecs)
                {
                    if(omvec == null) continue;
                    double proj = LinAlg.DotProd(nomvec, omvec);
                    nomvec = (nomvec - proj * omvec).UnitVector();
                }
                omvecs[i] = nomvec.UnitVector();
            }
            if(HDebug.IsDebuggerAttached)
            {
                for(int i=0; i<omvecs.Length-1; i++)
                    for(int j=i+1; j<omvecs.Length; j++)
                    {
                        double proj = LinAlg.DotProd(omvecs[i].UnitVector(), omvecs[j].UnitVector());
                        HDebug.AssertTolerance(0.00000001, proj);
                    }
            }
            return omvecs;
        }

        public static List<Vector> CreateList(int numvector, int sizevector)
        {
            return new List<Vector>(CreateArray(numvector, sizevector));
        }
        public static Vector[] CreateArray(int numvector, int sizevector)
        {
            Vector[] vecs = new Vector[numvector];
            for(int i=0; i<numvector; i++)
                vecs[i] = new double[sizevector];
            return vecs;
        }

		public static bool IsNaN             (this Vector[] vecs) { foreach(Vector vec in vecs) if(vec.IsNaN             ) return true; return false; }
		public static bool IsInfinity        (this Vector[] vecs) { foreach(Vector vec in vecs) if(vec.IsInfinity        ) return true; return false; }
		public static bool IsPositiveInfinity(this Vector[] vecs) { foreach(Vector vec in vecs) if(vec.IsPositiveInfinity) return true; return false; }
		public static bool IsNegativeInfinity(this Vector[] vecs) { foreach(Vector vec in vecs) if(vec.IsNegativeInfinity) return true; return false; }
		public static bool IsComputable      (this Vector[] vecs) { return ((IsNaN(vecs) == false) && (IsInfinity(vecs) == false)); }

        public static IList<Vector> ToUnitVectors(this IList<Vector> vecs)
        {
            Vector[] nvecs = new Vector[vecs.Count];
            for(int iv=0; iv < nvecs.Length; iv++)
                nvecs[iv] = vecs[iv].UnitVector();
            return nvecs;
        }

        public static Vector HSum   (this IList<Vector> vecs) { Vector sum = vecs[0].Clone(); for(int i=1; i<vecs.Count; i++) sum += vecs[i]; return sum; }
        public static Vector Average(this IList<Vector> vecs) { Vector avg = vecs.HSum(); avg /= vecs.Count; return avg; }
        public static Vector[] Clone(this Vector[] vecs) { Vector[] clone = new Vector[vecs.Length]; for(int i=0; i<vecs.Length; i++) clone[i] = vecs[i].Clone(); return clone; }
        public static Vector[] Clone(this Vector[] vecs, int numcopy)
        {
            HDebug.Assert(numcopy >= 1);
            Vector[] clone = new Vector[vecs.Length*numcopy];
            for(int i=0; i<clone.Length; i++)
                clone[i] = vecs[i/numcopy].Clone();
            return clone;
        }

        public static Vector ToVector(this IList<Vector> vecs)
        {
            List<double> vals = new List<double>();
            for(int i=0; i<vecs.Count; i++)
                vals.AddRange(vecs[i].ToArray());
            return vals.ToArray();
        }

        //public static void Add(this IList<Vector> src1, IList<Vector> src2, double src2scale=1)
        //{
        //    Debug.Assert(src1.Count == src2.Count);
        //    int size = src1.Count;
        //    for(int i=0; i<size; i++)
        //    {
        //        Debug.Assert(src1[i].Size == src2[i].Size);
        //        for(int j=0; j<src1[i].Size; j++)
        //            src1[i][j] += src2[i][j] * src2scale;
        //    }
        //}
        public static Vector[] Add(IList<Vector> src1, IList<Vector> src2)
        {
            HDebug.Assert(src1.Count == src2.Count);
            int size = src1.Count;
            Vector[] result = new Vector[size];
            for(int i=0; i<size; i++)
                result[i] = src1[i] + src2[i];
            return result;
        }
        public static Vector[] Sub(IList<Vector> src1, IList<Vector> src2)
        {
            HDebug.Assert(src1.Count == src2.Count);
            int size = src1.Count;
            Vector[] result = new Vector[size];
            for(int i=0; i<size; i++)
                result[i] = src1[i] - src2[i];
            return result;
        }
        public static Vector[] Mul(IList<Vector> src1, double src2)
        {
            int size = src1.Count;
            Vector[] result = new Vector[size];
            for(int i=0; i<size; i++)
                result[i] = src1[i]*src2;
            return result;
        }
        public static double[] Dist2(this IList<Vector> src)
        {
            double[] dist2s = new double[src.Count];
            for(int i=0; i<src.Count; i++)
                dist2s[i] = src[i].Dist2;
            return dist2s;
        }
        public static double[] Dist(this IList<Vector> src)
        {
            double[] dists = new double[src.Count];
            for(int i=0; i<src.Count; i++)
                dists[i] = src[i].Dist;
            return dists;
        }

        public static double[,] Pwdist2(this IList<Vector> src)
        {
            int size = src.Count;
            double[,] pwdist2 = new double[size, size];
            for(int i=0; i<size; i++)
                for(int j=i+1; j<size; j++)
                {
                    double dist2 = (src[i] - src[j]).Dist2;
                    pwdist2[i, j] = dist2;
                    pwdist2[j, i] = dist2;
                }
            return pwdist2;
        }

        public static double[,] Pwdist(this IList<Vector> src)
        {
            int size = src.Count;
            double[,] pwdist = new double[size, size];
            for(int i=0; i<size; i++)
                for(int j=i+1; j<size; j++)
                {
                    double dist2 = (src[i] - src[j]).Dist2;
                    double dist = Math.Sqrt(dist2);
                    pwdist[i, j] = dist;
                    pwdist[j, i] = dist;
                }
            return pwdist;
        }

        public static List<Vector> PwAdd(this IList<Vector> src1, IList<Vector> src2, double src2mul=1)
        {
            int size = src1.Count;
            HDebug.Assert(size == src2.Count);
            Vector[] vecs = new Vector[size];
            for(int i=0; i<size; i++)
                vecs[i] = src1[i] + src2[i] * src2mul;
            return new List<Vector>(vecs);
        }
        public static List<Vector> PwSub(this IList<Vector> src1, IList<Vector> src2)
        {
            return PwAdd(src1, src2, src2mul:-1);
        }
    }
}
