(* ::Package:: *)

p1={p1x,p1y,p1z};
p2={p2x,p2y,p2z};
p3={p3x,p3y,p3z};
dp1={dp1x,dp1y,dp1z};
dp2={dp2x,dp2y,dp2z};
dp3={dp3x,dp3y,dp3z};

(*
http://www.mathalino.com/reviewer/derivation-of-formulas/derivation-of-formula-for-radius-of-circumcircle
Radius R of triangle is
   R = abc / 4A,
where a,b,c are the lengths of sides of triangle, and A is the area of triangle.

http://en.wikipedia.org/wiki/Triangle#Using_vectors
Area of triangle is
  0.5 |AB x AC|,
where A,B,C are the location of points, AB is the vector from A to B, and 'x' is cross-product.
*)
R2[p1_,p2_,p3_] := ((p1-p2).(p1-p2)) * ((p2-p3).(p2-p3)) * ((p3-p1).(p3-p1)) / (4*Cross[p1-p2, p2-p3].Cross[p1-p2, p2-p3]);

R2a[p1_,p2_,p3_]:= ((p1-p2).(p1-p2)) * ((p2-p3).(p2-p3)) * ((p3-p1).(p3-p1));
R2b[p1_,p2_,p3_]:=                                                               (4*Cross[p1-p2, p2-p3].Cross[p1-p2, p2-p3]);
R2[p1_,p2_,p3_] := R2a[p1,p2,p3] / R2b[p1,p2,p3];

R2at[p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := R2a[p1+dp1*t,p2+dp2*t,p3+dp3*t];
R2bt[p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := R2b[p1+dp1*t,p2+dp2*t,p3+dp3*t];
R2t [p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := R2 [p1+dp1*t,p2+dp2*t,p3+dp3*t];

dR2at[p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := D[R2at[p1,p2,p3,dp1,dp2,dp3,t],t];
dR2bt[p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := D[R2bt[p1,p2,p3,dp1,dp2,dp3,t],t];
dR2t [p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := D[R2t [p1,p2,p3,dp1,dp2,dp3,t],t];


f[a_,b_,c_,da_,db_,dc_] := ((da-db).(a-b)) * ((a-c).(a-c)) * ((b-c).(b-c));
ff[a_,b_,c_,da_,db_,dc_] := f[a,b,c,da,db,dc]+f[c,a,b,dc,da,db]+f[b,c,a,db,dc,da];
(* ff[...] = f(a,b,c) + f(c,a,b) + f(b,c,a) *)
dR2at0[p1_,p2_,p3_,dp1_,dp2_,dp3_] := Limit[dR2at[p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2at0[p1,p2,p3,dp1,dp2,dp3] - 2*ff[p1,p2,p3,dp1,dp2,dp3] //Simplify
(******************************* should be 0 *******************************)


gg[a_,b_,c_,da_,db_,dc_] := Cross[a-b,b-c] . (Cross[a-b, db-dc] + Cross[da-db, b-c]);
(* g[...] = (Pab x Pbc)^tr (Pab x Wbc + Wab x Pbc)
      Wbc = Wa - Wb = da - db
      Pab = Pa - Pb = a - b
      a x b = cross[a,b]
*)
dR2bt0[p1_,p2_,p3_,dp1_,dp2_,dp3_] := Limit[dR2bt[p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2bt0[p1,p2,p3,dp1,dp2,dp3] - 2*(4*gg[p1,p2,p3,dp1,dp2,dp3]) //Simplify
(******************************* should be 0 *******************************)


( dR2at0[p1,p2,p3,dp1,dp2,dp3]*R2bt[p1,p2,p3,dp1,dp2,dp3,0]
- Limit[dR2bt[p1,p2,p3,dp1,dp2,dp3,tt],tt->0]*R2at[p1,p2,p3,dp1,dp2,dp3,0]
) - 2*(
    ff[p1,p2,p3,dp1,dp2,dp3]*R2bt[p1,p2,p3,dp1,dp2,dp3,0]
- 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2at[p1,p2,p3,dp1,dp2,dp3,0]
) //Simplify
(******************************* should be 0 *******************************)

dABdBAt[p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := (Limit[dR2at[p1,p2,p3,dp1,dp2,dp3,tt],tt->0]*R2bt[p1,p2,p3,dp1,dp2,dp3,t]
                                         - R2at[p1,p2,p3,dp1,dp2,dp3,t]*Limit[dR2bt[p1,p2,p3,dp1,dp2,dp3,tt],tt->0]);
dABdBAt0[p1_,p2_,p3_,dp1_,dp2_,dp3_] := Limit[dABdBA[p1,p2,p3,dp1,dp2,dp3,t],t->0];
dABdBAt0[p1,p2,p3,dp1,dp2,dp3] - 2*(
    ff[p1,p2,p3,dp1,dp2,dp3]*R2bt[p1,p2,p3,dp1,dp2,dp3,0]
- 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2at[p1,p2,p3,dp1,dp2,dp3,0]
) //Simplify
(******************************* should be 0 *******************************)


dR2t0[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := Limit[dR2t [p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2tx[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := dR2t0[p1,p2,p3,dp1,dp2,dp3] - dABdBAt0[p1,p2,p3,dp1,dp2,dp3] / R2bt[p1,p2,p3,dp1,dp2,dp3,0]^2 //Simplify;
(* dR2tx[p1,p2,p3,dp1,dp2,dp3] *)

xp1={1,2,3};
xp2={0,1,0};
xp3={1,0,0};
xdp1={1,2,3};
xdp2={-1,2,5};
xdp3={0,2,9};

dR2tx[xp1,xp2,xp3,xdp1,xdp2,xdp3]
(******************************* should be 0 *******************************)


dABdBAtTest1[p1_,p2_,p3_,dp1_,dp2_,dp3_] := 2*(    ff[p1,p2,p3,dp1,dp2,dp3]*R2bt[p1,p2,p3,dp1,dp2,dp3,0]
                                               - 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2at[p1,p2,p3,dp1,dp2,dp3,0]
                                               ) //Simplify;

dR2t0[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := Limit[dR2t [p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2tx[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := dR2t0[p1,p2,p3,dp1,dp2,dp3] - dABdBAtTest1[p1,p2,p3,dp1,dp2,dp3] / R2bt[p1,p2,p3,dp1,dp2,dp3,0]^2 //Simplify;
(* dR2tx[p1,p2,p3,dp1,dp2,dp3] *)

xp1={1,2,3};
xp2={0,1,0};
xp3={1,0,0};
xdp1={1,2,3};
xdp2={-1,2,5};
xdp3={0,2,9};

dR2tx[xp1,xp2,xp3,xdp1,xdp2,xdp3]
(******************************* should be 0 *******************************)


dABdBAtTest2[p1_,p2_,p3_,dp1_,dp2_,dp3_] := 2*(    ff[p1,p2,p3,dp1,dp2,dp3]*R2b[p1,p2,p3]
                                               - 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2a[p1,p2,p3]
                                              ) //Simplify;

dR2t0[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := Limit[dR2t [p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2tx[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := dR2t0[p1,p2,p3,dp1,dp2,dp3] - dABdBAtTest2[p1,p2,p3,dp1,dp2,dp3] / R2b[p1,p2,p3]^2 //Simplify;
(* dR2tx[p1,p2,p3,dp1,dp2,dp3] *)

xp1={1,2,3};
xp2={0,1,0};
xp3={1,0,0};
xdp1={1,2,3};
xdp2={-1,2,5};
xdp3={0,2,9};

dR2tx[xp1,xp2,xp3,xdp1,xdp2,xdp3]
(******************************* should be 0 *******************************)


dABdBAtTest2[p1_,p2_,p3_,dp1_,dp2_,dp3_] := 2*(    ff[p1,p2,p3,dp1,dp2,dp3]*R2b[p1,p2,p3]
                                                - 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2a[p1,p2,p3]
                                               )                           / R2b[p1,p2,p3]^2 //Simplify;

dR2t0[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := Limit[dR2t [p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2tx[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := dR2t0[p1,p2,p3,dp1,dp2,dp3] - dABdBAtTest2[p1,p2,p3,dp1,dp2,dp3]  //Simplify;
(* dR2tx[p1,p2,p3,dp1,dp2,dp3] *)

xp1={1,2,4};
xp2={0,1,0};
xp3={1,0,0};
xdp1={1,2,3};
xdp2={-1,2,5};
xdp3={0,2,9};

dR2tx[xp1,xp2,xp3,xdp1,xdp2,xdp3]
(******************************* should be 0 *******************************)


R2[p1_,p2_,p3_] := ((p1-p2).(p1-p2)) * ((p2-p3).(p2-p3)) * ((p3-p1).(p3-p1)) / (4*Cross[p1-p2, p2-p3].Cross[p1-p2, p2-p3]);
R2a[p1_,p2_,p3_]:= ((p1-p2).(p1-p2)) * ((p2-p3).(p2-p3)) * ((p3-p1).(p3-p1));
R2b[p1_,p2_,p3_]:=                                                               (4*Cross[p1-p2, p2-p3].Cross[p1-p2, p2-p3]);
R2[p1_,p2_,p3_] := R2a[p1,p2,p3] / R2b[p1,p2,p3];
R2t [p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := R2 [p1+dp1*t,p2+dp2*t,p3+dp3*t];
dR2t [p1_,p2_,p3_,dp1_,dp2_,dp3_,t_] := D[R2t [p1,p2,p3,dp1,dp2,dp3,t],t];
(* R2 = R2a / R2b *)
f[a_,b_,c_,da_,db_,dc_] := ((da-db).(a-b)) * ((a-c).(a-c)) * ((b-c).(b-c));
ff[a_,b_,c_,da_,db_,dc_] := f[a,b,c,da,db,dc]+f[c,a,b,dc,da,db]+f[b,c,a,db,dc,da];
(* ff[...] = f(a,b,c) + f(c,a,b) + f(b,c,a) *)
gg[a_,b_,c_,da_,db_,dc_] := Cross[a-b,b-c] . (Cross[a-b, db-dc] + Cross[da-db, b-c]);
(* g[...] = (Pab x Pbc)^tr (Pab x Wbc + Wab x Pbc)
      Wbc = Wa - Wb = da - db
      Pab = Pa - Pb = a - b
      a x b = cross[a,b]
*)
dABdBAtTest2[p1_,p2_,p3_,dp1_,dp2_,dp3_] := 2*(    ff[p1,p2,p3,dp1,dp2,dp3]*R2b[p1,p2,p3]
                                                - 4*gg[p1,p2,p3,dp1,dp2,dp3]*R2a[p1,p2,p3]
                                               )                           / R2b[p1,p2,p3]^2 //Simplify;

dR2t0[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := Limit[dR2t [p1,p2,p3,dp1,dp2,dp3,t],t->0];
dR2tx[p1_,p2_,p3_,dp1_,dp2_,dp3_ ] := dR2t0[p1,p2,p3,dp1,dp2,dp3] - dABdBAtTest2[p1,p2,p3,dp1,dp2,dp3]  //Simplify;
(* dR2tx[p1,p2,p3,dp1,dp2,dp3] *)

xp1={1,2,4};
xp2={0,1,0};
xp3={1,0,0};
xdp1={1,2,3};
xdp2={-1,2,5};
xdp3={0,2,9};

dR2tx[xp1,xp2,xp3,xdp1,xdp2,xdp3]
(******************************* should be 0 *******************************)



















p1={1,2,4};
p2={0,1,0};
p3={1,0,0};
dp1={1,2,3};
dp2={-1,2,5};
dp3={0,2,9};



P12dist2 = (p1-p2).(p1-p2);
P23dist2 = (p2-p3).(p2-p3);
P31dist2 = (p3-p1).(p3-p1);
Dotp12dp12 = (dp1-dp2).(p1-p2);
Dotp23dp31 = (dp2-dp3).(p2-p3);
Dotp31dp31 = (dp1-dp3).(p1-p3);
DotCroP12P23CroP12Dp23CroDp12P23 = Cross[p1-p2, p2-p3] . (Cross[p1-p2, dp2-dp3] + Cross[dp1-dp2, p2-p3]);
(* area = 0.5 |AB x AC| *)
Div2 = Cross[p1-p2, p2-p3].Cross[p1-p2, p2-p3];
Div4 = Div2^2;


dR2dt = 2*(+ ( Dotp31dp31 * P12dist2 * P23dist2
             + Dotp23dp31 * P12dist2 * P31dist2
             + Dotp12dp12 * P23dist2 * P31dist2 )/(4 * Div2)
           - ( DotCroP12P23CroP12Dp23CroDp12P23 * P12dist2 * P23dist2 * P31dist2)/(4 * Div4)
          )
(* notation in implementation *)
(******************************* all values should be the same *******************************)
dR2t0[p1,p2,p3,dp1,dp2,dp3]
(******************************* all values should be the same *******************************)
dABdBAtTest2[p1,p2,p3,dp1,dp2,dp3]
(******************************* all values should be the same *******************************)


R0 = 0.5*Sqrt[P12dist2 * P23dist2 * P31dist2 / Div2];
dRdR2 = 1/(2* R0);
dR2dtTest1 = dRdR2 * dR2dt
(******************************* all values should be the same *******************************)
dR2dtTest2 = (( Dotp31dp31*P12dist2*P23dist2 + Dotp23dp31*P12dist2*P31dist2 + Dotp12dp12*P23dist2*P31dist2 )/(4 * R0 * Div2)
        -( DotCroP12P23CroP12Dp23CroDp12P23 * R0 )/(1 * Div2)
          )
(* notation in the paper *)
(******************************* all values should be the same *******************************)
dR2dtTest1 - dR2dtTest2
(******************************* should be 0 *******************************)



