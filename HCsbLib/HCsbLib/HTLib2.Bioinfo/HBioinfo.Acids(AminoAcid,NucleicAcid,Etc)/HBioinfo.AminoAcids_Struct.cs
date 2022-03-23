﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Acid = HBioinfo.Acid;
    public static partial class HStaticBioinfo
    {
    public static partial class HBioinfo
    {
        public class Acid
        {
// https://www.i2symbol.com/symbols/arrows
// https://en.wikibooks.org/wiki/Unicode/List_of_useful_symbols
// https://w3c.github.io/xml-entities/025.html
// https://www.ssec.wisc.edu/~tomw/java/unicode.html
// ─│
// ═║
// ╱╲⇖⇗⇘⇙
// ␥⑊ ╔╗╚╝
                static string strucdata =
@"
RESI ALA
!        │  
!  *HN───*N
!        │         *HB1
!        │        ╱
!  *HA───*CA───*CB───*HB2
!        │        ╲
!        │         *HB3
!  *O════*C
!        │ 
!

RESI ARG
!  *     *     *     *     *     *     *   *     *     *     *     *
!        │                                 *HH11
!  *HN───*N                                │  
!        │     *HB1  *HG1  *HD1  *HE       *NH1──*HH12
!        │     │     │     │     │        ╱╱          
!  *HA───*CA───*CB───*CG───*CD───*NE───*CZ    
!        │     │     │     │              ╲   
!        │     *HB2  *HG2  *HD2            *NH2──*HH22
!  *O════*C                                │  
!        │                                 *HH21

RESI ASN
!        │                  
!  *HN───*N           
!        │     *HB1  *OD1      *HD21
!        │     │     ║        ╱
!  *HA───*CA───*CB───*CG───*ND2
!        │     │              ╲
!        │     *HB2            *HD22
!  *O════*C                 
!        │                  

RESI ASP
!        │            
!  *HN───*N           
!        │     *HB1      *OD1
!        │     │        ╱╱
!  *HA───*CA───*CB───*CG
!        │     │        ╲
!        │     *HB2      *OD2
!  *O════*C               
!        │                

RESI CYS
!        │            
!  *HN───*N           
!        │     *HB1
!        │     │   
!  *HA───*CA───*CB───*SG
!        │     │        ╲
!        │     *HB2      *HG1
!  *O════*C               
!        │                

RESI GLN
!        │                                
!  *HN───*N              
!        │     *HB1  *HG1  *OE1      *HE21
!        │     │     │      ║       ╱
!  *HA───*CA───*CB───*CG───*CD───*NE2
!        │     │     │              ╲
!        │     *HB2  *HG2            *HE22
!  *O════*C              
!        │               

RESI GLU
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │                            
!  *HN───*N              
!        │     *HB1  *HG1      *OE1
!        │     │     │        ╱╱
!  *HA───*CA───*CB───*CG───*CD
!        │     │     │        ╲
!        │     *HB2  *HG2      *OE2
!  *O════*C              
!        │               

RESI GLY
!        │ 
!  *H────*N
!        │   
!        │   
!  *HA1──*CA───*HA2
!        │   
!        │   
!  *O════*C          
!        │ 

RESI HSD
!        │               *HD1      *HE1
!  *HN───*N              │        ╱
!        │     *HB1      *ND1──*CE1
!        │     │        ╱      ║ 
!  *HA───*CA───*CB───*CG       ║ 
!        │     │        ╲╲     ║ 
!        │     *HB2      *CD2──*NE2
!  *O════*C              │  
!        │               *HD2

RESI HSE
!        │                         *HE1
!  *HN───*N                       ╱
!        │     *HB1      *ND1══*CE1
!        │     │        ╱      │
!  *HA───*CA───*CB───*CG       │
!        │     │        ╲╲     │
!        │     *HB2      *CD2──*NE2
!  *O════*C              │        ╲
!        │               *HD2      *HE2

RESI HSP
!        │               *HD1      *HE1
!  *HN───*N              │        ╱
!        │     *HB1      *ND1──*CE1
!        │     │        ╱      ║ 
!  *HA───*CA───*CB───*CG       ║ 
!        │     │        ╲╲     ║ 
!        │     *HB2      *CD2──*NE2
!  *O════*C              │        ╲
!        │               *HD2      *HE2

RESI ILE
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │       *HG21 *HG22
!  *HN───*N       ╲   ╱ 
!        │         *CG2──*HG23
!        │        ╱
!  *HA───*CA───*CB─HB        *HD1
!        │        ╲         ╱
!        │         *CG1──*CD───*HD2
!  *O════*C       ╱   ╲     ╲	 
!        │       *HG11 *HG12 *HD3

RESI LEU
!        │             *HD11 *HD12
!  *HN───*N             ╲   ╱
!        │     *HB1      *CD1──*HD13
!        │     │        ╱
!  *HA───*CA───*CB───*CG─HG
!        │     │        ╲ 
!        │     *HB2      *CD2──*HD23
!  *O════*C             ╱   ╲
!        │             *HD21 *HD22

RESI LYS
!        │                              
!  *HN───*N                             
!        │     *HB1  *HG1  *HD1  *HE1      *HZ1
!        │     │     │     │     │        ╱   
!  *HA───*CA───*CB───*CG───*CD───*CE───*NZ───*HZ2
!        │     │     │     │     │        ╲
!        │     *HB2  *HG2  *HD2  *HE2      *HZ3
!  *O════*C                             
!        │                              

RESI MET
!        │                            
!  *HN───*N                           
!        │     *HB1  *HG1        *HE1 
!        │     │     │           │   
!  *HA───*CA───*CB───*CG───*SD───*CE───*HE3
!        │     │     │           │   
!        │     *HB2  *HG2        *HE2 
!  *O════*C                          
!        │                           

RESI PHE
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │               *HD1  *HE1    
!  *HN───*N              │     │   
!        │     *HB1      *CD1──*CE1
!        │      │      ╱╱         ╲╲
!  *HA───*CA───*CB───*CG           *CZ───*HZ
!        │      │      ╲          ╱
!        │     *HB2     *CD2═══*CE2
!  *O════*C             │      │    
!        │              *HD2   *HE2   

RESI PRO
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │   *HD1 *HD2
!        │    ╲  ╱
!        *N────*CD     *HG1
!        │        ╲   ╱
!        │         *CG
!        │        ╱   ╲
!  *HA───*CA───*CB     *HG2
!        │    ╱  ╲
!        │   *HB1 *HB2
!  *O════*C
!        │ 

RESI SER
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │          
!  *HN───*N         
!        │     *HB1
!        │     │   
!  *HA───*CA───*CB───*OG
!        │     │        ╲
!        │     *HB2      *HG1
!  *O════*C             
!        │              

RESI THR
!        │     
!  *HN───*N    
!        │         *OG1──*HG1
!        │       ╱
!  *HA───*CA───*CB───*HB  
!        │       ╲     
!        │         *CG2──*HG21
!  *O════*C      ╱   ╲    
!        │     *HG21   *HG22 

RESI TRP
!        │                    *HE3
!  *HN───*N                   │   
!        │     *HB1           *CE3
!        │     │             ╱ ╲╲
!  *HA───*CA───*CB───*CG───*CD2  *CZ3──*HZ3
!        │     │     ║     ║     │  
!        │     *HB2  *CD1  *CE2  *CH2──*HH2
!  *O════*C        ╱   ╲ ╱   ╲ ╱╱
!        │       *HD1   *NE1  * CZ2
!                       │     │  
!                       *HE1  *HZ2

RESI TYR
!        │              *HD1  *HE1    
!  *HN───*N             │     │   
!        │     *HB1     *CD1──*CE1
!        │      │     ╱╱       ╲╲
!  *HA───*CA───*CB───*CG         *CZ───*OH
!        │      │      ╲       ╱         ╲
!        │     *HB2     *CD2══*CE2        *HH
!  *O════*C             │     │  
!        │              *HD2  *HE2   

RESI VAL
!  *     *     *     *     *     *     *     *     *     *     *     *
!        │     *HG11 *HG12
!  *HN───*N     ╲   ╱ 
!        │        *CG1──*HG13
!        │       ╱
!  *HA───*CA───*CB─HB  
!        │       ╲     
!        │        *CG2──*HG21
!  *O════*C     ╱   ╲   
!        │     *HG21 *HG22

";

        }
    }
    }
}
