using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static double SimFluc(IList<Mode> modes1, IList<Mode> modes2)
        {
            /// need to confirm again...
            /// 

            Matrix M1 = modes1.ToModeMatrix();
            Vector V1 = modes1.ToArray().ListEigval();
            Matrix M2 = modes2.ToModeMatrix();
            Vector V2 = modes2.ToArray().ListEigval();

            using(new Matlab.NamedLock("SimFluc"))
            {
                Matlab.Execute("");
                Matlab.Execute("clear");
                Matlab.PutMatrix("MM1", M1); Matlab.PutVector("VV1", V1);
                Matlab.PutMatrix("MM2", M2); Matlab.PutVector("VV2", V2);

                Matlab.Execute("[U2,S2,V2] = svd(MM2);");
                Matlab.Execute("U2 = U2(:, 1:length(VV2));");
                Matlab.Execute("S2 = S2(1:length(VV2), :);");
                Matlab.Execute("invSV2 = diag(1./diag(S2))*V2';");
                // covariance of mode2
                Matlab.Execute("C2 = invSV2*diag(1./VV2)*invSV2';");
                Matlab.Execute("C2 = (C2 + C2')/2;");
                // covariance of mode 1 projected onto U2
                Matlab.Execute("C1 = U2'*(MM1*diag(VV1)*MM1')*U2;");
                Matlab.Execute("C1 = pinv((C1 + C1')/2);");
                Matlab.Execute("C1 = (C1 + C1')/2;");
                // compute the fluctuation similarity
                Matlab.Execute("detInvC1 = det(inv(C1));");
                Matlab.Execute("detInvC2 = det(inv(C2));");
                Matlab.Execute("detInvC1InvC2 = det((inv(C1)+inv(C2))/2);");
                Matlab.Execute("simfluc0 = ((detInvC1*detInvC2)^0.25);");
                Matlab.Execute("simfluc1 = (detInvC1InvC2)^0.5;");
                Matlab.Execute("simfluc = simfluc0 / simfluc1;");

                double simfluc = Matlab.GetValue("simfluc");
                Matlab.Execute("clear");
                return simfluc;
            }
        }
    }
}
