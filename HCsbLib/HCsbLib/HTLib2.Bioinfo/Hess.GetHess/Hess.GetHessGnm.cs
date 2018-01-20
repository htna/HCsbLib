using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static bool GetHessGnmSelfTest()
        {
            if(HDebug.Selftest() == false)
                return true;

            Pdb pdb = Pdb.FromPdbid("1MJC");
            for(int i=0; i<pdb.atoms.Length; i++)
            {
                HDebug.Assert(pdb.atoms[0].altLoc  == pdb.atoms[i].altLoc);
                HDebug.Assert(pdb.atoms[0].chainID == pdb.atoms[i].chainID);
            }
            List<Vector> coords = pdb.atoms.ListCoord();
            double cutoff = 13;
            Matlab.Execute("clear");
            Matlab.PutMatrix("x", MatrixByArr.FromRowVectorList(coords).ToArray());
            Matlab.PutValue("cutoffR", cutoff);
            Matlab.Execute(@"%  function cx = contactsNew(x, cutoffR)
                                % Contact matrix within cutoff distance.
                                % Author: Guang Song
                                % New: 10/25/2006
                                %

                                %n = size(x,1); 
                                % Method 1: slow
                                %for i=1:n
                                %  center = x(i,:);
                                %  distSqr(:,i) = sum((x-center(ones(n,1),:)).^2,2);
                                %end
                                %cx = sparse(distSqr<=cutoffR^2);

                                % Method 2: fast! about 28 times faster when array size is 659x3
                                %tot = zeros(n,n);
                                %for i=1:3
                                %  xi = x(:,ones(n,1)*i);
                                %  %tmp = (xi - xi.').^2;
                                %  %tot = tot + tmp;
                                %  tot = tot +  (xi - xi.').^2;
                                %end
                                %cx = sparse(tot<=cutoffR^2);

                                % Method 3: this implementation is the shortest! but sligtly slower than 
                                % method 2
                                %xn = x(:,:,ones(n,1)); % create n copy x
                                %xnp = permute(xn,[3,2,1]);
                                %tot = sum((xn-xnp).^2,2); % sum along x, y, z
                                %cx = sparse(permute(tot,[1,3,2])<=cutoffR^2);
                                % put it into one line like below actually slows it down. Don't do that.
                                %cx =  sparse(permute(sum((xn-permute(xn,[3,2,1])).^2,2),[1,3,2])<=cutoffR^2);

                                %Method 4: using function pdist, which I just know
                                % this one line implementation is even faster. 2 times than method 2.
                                cx = sparse(squareform(pdist(x)<=cutoffR));
                            ");
            Matlab.Execute(@"%  function gnm = kirchhoff(cx)
                                % the returned gnm provide the kirchhoff matrix
                                % cx is the contact map.
                                % Guang Song
                                % 11/09/06
                                gnm = diag(sum(cx)) - cx;
                            ");
            Matlab.Execute("gnm = full(gnm);");
            Matrix gnm_gsong = Matlab.GetMatrix("gnm");
            Matlab.Execute("clear;");

            Matrix gnm = GetHessGnm(coords.ToArray(), cutoff);

            if(gnm_gsong.RowSize != gnm.RowSize) { HDebug.Assert(false); return false; }
            if(gnm_gsong.ColSize != gnm.ColSize) { HDebug.Assert(false); return false; }

            for(int c=0; c<gnm.ColSize; c++)
                for(int r=0; r<gnm.RowSize; r++)
                    if(Math.Abs(gnm_gsong[c, r] - gnm[c, r]) >= 0.00000001)
                    { HDebug.Assert(false); return false; }

            return true;
        }
        public static Matrix GetHessGnm(IList<Vector> coords, double cutoff)
        {
            HDebug.Assert(GetHessGnmSelfTest());

            int n = coords.Count;
            double cutoff2 = cutoff * cutoff;
            Matrix hess = Matrix.Zeros(n, n);
            for(int i=0; i<n; i++)
            {
                if(coords[i] == null)
                    continue;
                for(int j=0; j<n; j++)
                {
                    if(i == j)
                        continue;
                    if(coords[j] == null)
                        continue;
                    Vector vec_ij = coords[j] - coords[i];
                    double dist2 = vec_ij.Dist2;
                    if(dist2 > cutoff2)
                        continue;
                    hess[i, j]  = -1;
                    hess[i, i] +=  1;
                }
            }
            return hess;
        }
    }
}
