using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static MatrixBySparseMatrix ToSparseMatrix(this Matrix mat, int blksize)
        {
            if(mat is MatrixBySparseMatrix)
                return (mat as MatrixBySparseMatrix).CloneT();
            MatrixBySparseMatrix smat = new MatrixBySparseMatrix(mat.ColSize, mat.RowSize, blksize);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                {
                    double val = mat[c,r];
                    if(val != 0)
                        smat[c,r] = val;
                }
            return smat;
        }
    }
    [Serializable]
    public class MatrixBySparseMatrix : Matrix, ISerializable
    {
        MatrixSparse<MatrixByArr> blkmatrix;
        public readonly int BlkSize;
        public readonly int _ColSize;
        public readonly int _RowSize;

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Matrix

        public override int ColSize { get { return _ColSize; } }
        public override int RowSize { get { return _RowSize; } }

        public override double this[int c, int r]
        {
            get { return GetValue(c, r); }
            set { SetValue(c, r, value); }
        }
        public override double this[long c, long r]
        {
            get { return GetValue((int)c, (int)r); }
            set { SetValue((int)c, (int)r, value); }
        }

        public override Matrix Clone()
        {
            return CloneT();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // others
        public static bool _SelfTest = HDebug.IsDebuggerAttached;
        public static void SelfTest()
        {
            if(_SelfTest == false)
                return;
            _SelfTest = false;
            MatrixBySparseMatrix mat = new MatrixBySparseMatrix(4, 4, 2);
            HDebug.AssertToleranceMatrix(0, mat - MatrixByArr.Zeros(4,4));
            
            mat[0, 0] = mat[1, 1] = mat[2, 2] = mat[3, 3] = 1;
            HDebug.AssertToleranceMatrix(0, mat - LinAlg.Eye(4, 1));

            mat[0, 0] = mat[1, 1] = mat[2, 2] = mat[3, 3] = 0;
            HDebug.AssertToleranceMatrix(0, mat - MatrixByArr.Zeros(4, 4));
        }

        public MatrixBySparseMatrix(int colsize, int rowsize, int blksize)
        {
            if(_SelfTest)
                SelfTest();

            this._ColSize = colsize;
            this._RowSize = rowsize;
            this.BlkSize = blksize;
            //Func<MatrixByArr> GetZeroBlock = delegate() { return new double[blksize, blksize]; };
            int colblksize = (colsize % blksize == 0) ? (colsize/blksize) : (colsize/blksize+1);
            int rowblksize = (rowsize % blksize == 0) ? (rowsize/blksize) : (rowsize/blksize+1);
            //this.blkmatrix = new MatrixSparse<MatrixByArr>((colsize/blksize+1), (rowsize/blksize+1), GetZeroBlock);
            this.blkmatrix = new MatrixSparse<MatrixByArr>(colblksize, rowblksize, GetZeroBlock);
        }

        public MatrixByArr GetZeroBlock()
        {
            return new double[BlkSize, BlkSize];
        }
        public MatrixSparse<MatrixByArr> GetMatrixSparse()
        {
            return blkmatrix;
        }

        public static MatrixBySparseMatrix Zeros(int colsize, int rowsize, int blksize)
        {
            return new MatrixBySparseMatrix(colsize, rowsize, blksize);
        }
        public static MatrixBySparseMatrix Zeros_BlockSize2(int colsize, int rowsize)
        {
            return new MatrixBySparseMatrix(colsize, rowsize, 2);
        }
        public static MatrixBySparseMatrix Zeros_BlockSize3(int colsize, int rowsize)
        {
            return new MatrixBySparseMatrix(colsize, rowsize, 3);
        }

        public double GetValue(int c, int r)
        {
            HDebug.Assert(0<=c, c<_ColSize, 0<=r, r<_RowSize);
            int c0 = c/BlkSize;
            int r0 = r/BlkSize;
            if(blkmatrix.HasElement(c0, r0))
            {
                int c1 = c%BlkSize;
                int r1 = r%BlkSize;
                return blkmatrix[c0, r0][c1, r1];
            }
            return 0;
        }

        public void SetValue(int c, int r, double value)
        {
            int c0 = c/BlkSize; int c1 = c%BlkSize;
            int r0 = r/BlkSize; int r1 = r%BlkSize;
            if(value != 0)
            {
                // assign value
                MatrixByArr lmat = blkmatrix[c0, r0];
                lmat[c1, r1] = value;
                blkmatrix[c0, r0] = lmat;
                return;
            }

            HDebug.Assert(value == 0);
            if(blkmatrix.HasElement(c0, r0))
            {
                MatrixByArr lmat = blkmatrix[c0, r0];
                lmat[c1, r1] = value;
                blkmatrix[c0, r0] = lmat;
                return;
            }
            else
            {
                // (blkmatrix[c0,r0] == null) && (value == 0)
                // do nothing
                return;
            }
        }

        public int NumUsedBlocks { get {
            return blkmatrix.NumElements;
        }}
        public double RatioUsedBlocks { get {
            return (double)blkmatrix.NumElements / (blkmatrix.ColSize * blkmatrix.RowSize);
        }}
        public MatrixByArr GetBlock(int bc, int br)
        {
            return blkmatrix[bc, br];
        }
        public MatrixByArr GetBlockLock(int bc, int br)
        {
            return blkmatrix.GetAtLock(bc, br);
        }
        public void SetBlock(int bc, int br, MatrixByArr bval)
        {
            if(bval != null)
                HDebug.Assert(bval.ColSize == BlkSize, bval.RowSize == BlkSize);
            blkmatrix[bc, br] = bval;
        }
        public void SetBlockLock(int bc, int br, MatrixByArr bval)
        {
            if(bval != null)
                HDebug.Assert(bval.ColSize == BlkSize, bval.RowSize == BlkSize);
            blkmatrix.SetAtLock(bc, br, bval);
        }
        public bool HasBlock(int bc, int br)
        {
            return blkmatrix.HasElement(bc, br);
        }
        public bool HasBlockLock(int bc, int br)
        {
            return blkmatrix.HasElementLock(bc, br);
        }
        public IEnumerable<Tuple<int,int,MatrixByArr>> EnumBlocks()
        {
            return blkmatrix.EnumElements();
        }
        public IEnumerable<Tuple<int, int, MatrixByArr>> EnumBlocks_dep(params int[] lstBlkCol)
        {
            return blkmatrix.EnumElements_dep(lstBlkCol);
        }
        public IEnumerable<ValueTuple<int, int, MatrixByArr>> EnumBlocks(params int[] lstBlkCol)
        {
            return blkmatrix.EnumElements(lstBlkCol);
        }
        public IEnumerable<Tuple<int, int>> EnumIndices()
        {
            return blkmatrix.EnumIndices();
        }

        public MatrixBySparseMatrix CloneT()
        {
            MatrixBySparseMatrix clone = new MatrixBySparseMatrix(_ColSize, _RowSize, BlkSize);
            foreach(var bc_br_bval in blkmatrix.EnumElements())
            {
                int bc = bc_br_bval.Item1;
                int br = bc_br_bval.Item2;
                MatrixByArr bval = bc_br_bval.Item3;

                clone.blkmatrix[bc, br] = bval.CloneT();
            }
            return clone;
        }

        public override string ToString()
        {
            string msg = "";
            msg += string.Format("size({0}, {1}), elements({2}*({3},{3}))", _ColSize, _RowSize, blkmatrix.NumElements, BlkSize);
            return msg;
        }

        public void Compress()
        {
            foreach(var c_r_val in blkmatrix.EnumElements())
            {
                int c = c_r_val.Item1;
                int r = c_r_val.Item2;
                MatrixByArr lmat = c_r_val.Item3;

                if(lmat.HNumZeros() < BlkSize*BlkSize)
                    continue;
                HDebug.Assert(lmat.HNumZeros() == BlkSize*BlkSize);

                blkmatrix[c, r] = null;
            }
        }

        public double[,] ToArray()
        {
            double[,] arr = new double[_ColSize, _RowSize];
            foreach(var c_r_val in blkmatrix.EnumElements())
            {
                int    c0  = c_r_val.Item1;
                int    r0  = c_r_val.Item2;
                MatrixByArr val = c_r_val.Item3;
                HDebug.Assert(val.ColSize == BlkSize, val.RowSize == BlkSize);
                for(int c1=0; c1<BlkSize; c1++)
                    for(int r1=0; r1<BlkSize; r1++)
                        arr[BlkSize*c0+c1, BlkSize*r0+r1] = val[c1, r1];
            }
            return arr;
        }

        ////////////////////////////////////////////////////////////////////////////////////
		// Serializable
		public MatrixBySparseMatrix(SerializationInfo info, StreamingContext ctxt)
		{
            this.BlkSize  = info.GetInt32("BlkSize");
            this._ColSize = info.GetInt32("_ColSize");
            this._RowSize = info.GetInt32("_RowSize");

            int colblksize = (ColSize % BlkSize == 0) ? (ColSize/BlkSize) : (ColSize/BlkSize+1);
            int rowblksize = (RowSize % BlkSize == 0) ? (RowSize/BlkSize) : (RowSize/BlkSize+1);
            this.blkmatrix = new MatrixSparse<MatrixByArr>(colblksize, rowblksize, GetZeroBlock);

            int         block_num   =              info.GetInt32("block_num");
            int[]       block_Item1 = (int[]      )info.GetValue("block_Item1", typeof(int[]      ));
            int[]       block_Item2 = (int[]      )info.GetValue("block_Item2", typeof(int[]      ));
            double[,][] block_Item3 = (double[,][])info.GetValue("block_Item3", typeof(double[,][]));
            block_num = block_Item1.Length;
            HDebug.Assert(block_num == block_Item1.Length);
            HDebug.Assert(block_num == block_Item2.Length);
            HDebug.Assert(block_Item3.GetLength(0) == BlkSize);
            HDebug.Assert(block_Item3.GetLength(1) == BlkSize);
            for(int c=0; c<BlkSize; c++)
                for(int r=0; r<BlkSize; r++)
                    HDebug.Assert(block_Item3[c, r].Length == block_num);
            for(int i=0; i<block_num; i++)
            {
                int bc = block_Item1[i];
                int br = block_Item2[i];
                MatrixByArr bval = new double[BlkSize, BlkSize];
                for(int c=0; c<BlkSize; c++)
                    for(int r=0; r<BlkSize; r++)
                        bval[c, r] = block_Item3[c, r][i];
                SetBlock(bc, br, bval);
            }

            //throw new NotImplementedException();
        }
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("BlkSize" , this.BlkSize );
            info.AddValue("_ColSize", this._ColSize);
            info.AddValue("_RowSize", this._RowSize);
            //info.AddValue("block_num", block_num);
            int         block_num  = blkmatrix.NumElements;
            int[]       block_Item1 = new int[block_num];
            int[]       block_Item2 = new int[block_num];
            double[,][] block_Item3 = new double[BlkSize, BlkSize][]; for(int c=0; c<BlkSize; c++) for(int r=0; r<BlkSize; r++) block_Item3[c, r] = new double[block_num];
            int i = 0;
            foreach(Tuple<int,int,MatrixByArr> block in blkmatrix.EnumElements())
            {
                block_Item1[i] = block.Item1;
                block_Item2[i] = block.Item2;
                for(int c=0; c<BlkSize; c++)
                    for(int r=0; r<BlkSize; r++)
                        block_Item3[c, r][i] = block.Item3[c, r];
                i++;
            }
            info.AddValue("block_num"  , block_num  );
            info.AddValue("block_Item1", block_Item1);
            info.AddValue("block_Item2", block_Item2);
            info.AddValue("block_Item3", block_Item3);

            //throw new NotImplementedException();
        }
    }
}
