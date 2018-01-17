using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Site = Pdb.Site;
    public static partial class PdbStatic
    {
        public class ResInfo : IEquatable<ResInfo>
        {
            public string resName;
            public char   chainID;
            public int    resSeq ;
            public char   iCode  ;

            public bool Equals(ResInfo other)
            {
                if(resName != other.resName) return false;
                if(resSeq  != other.resSeq ) return false;
                if(chainID!=' ' && other.chainID!=' ') if(chainID != other.chainID) return false;
                if(iCode  !=' ' && other.iCode  !=' ') if(iCode   != other.iCode  ) return false;
                return true;
            }
            public static bool Equals(ResInfo v1, ResInfo v2)
            {
                return v1.Equals(v2);
            }
            public override int GetHashCode()
            {
 	             return (resName.GetHashCode() + chainID.GetHashCode() + resSeq.GetHashCode() + iCode.GetHashCode());
            }
            public override string ToString()
            {
                string str = string.Format("{0}{1:000}, {2}, {3}", resName, resSeq, chainID, iCode);
                return str;
            }
        }
        public static Tuple<char, int>[] HListChainidResseq(this IList<ResInfo> resinfos)
        {
            List<Tuple<char,int>> lstChnSeq = new List<Tuple<char, int>>();
            foreach(var resinfo in resinfos)
                lstChnSeq.Add(new Tuple<char, int>(resinfo.chainID, resinfo.resSeq));
            return lstChnSeq.ToArray();
        }
        
        public class PPdbSiteInfo
        {
            public string    siteID;
            public ResInfo[] resinfos;
            public override string ToString()
            {
                string str = siteID + " : ";
                foreach(var resinfo in resinfos)
                    str += string.Format("{0}{1}, ", resinfo.resName, resinfo.resSeq);
                return str;
            }
        }

        public static PPdbSiteInfo[] HSelectHaveProtein(this IList<PPdbSiteInfo> siteinfos)
        {
            List<PPdbSiteInfo> sele = new List<PPdbSiteInfo>();
            HashSet<string> ress = new HashSet<string>(new string[]
            {
                "GLY",
                "ALA", "VAL", "PHE", "PRO", "LEU", "ILE",
                "ARG", "ASP", "GLU", "SER", "CYS", "ASN", "GLN", "HIS",
                "THR",
                "LYS", "TYR", "MET", "TRP",
            });
            foreach(var siteinfo in siteinfos)
            {
                bool bHasProtein = false;
                foreach(var resinfo in siteinfo.resinfos)
                {
                    if(ress.Contains(resinfo.resName))
                    {
                        bHasProtein=true;
                        continue;
                    }
                    switch(resinfo.resName)
                    {
                        case "HOH": continue;
                        case "PG4": continue; // ??
                        case "GOL": continue; // glycerol 
                        default:
                            continue;
                    }
                }
                if(bHasProtein == true)
                    sele.Add(siteinfo);
            }
            return sele.ToArray();
        }
        public static ResInfo[] HListResInfo(this IList<PPdbSiteInfo> siteinfos)
        {
            List<ResInfo> resinfos = new List<ResInfo>();
            foreach(var siteinfo in siteinfos)
            {
                foreach(var resinfo in siteinfo.resinfos)
                {
                    resinfos.Add(resinfo);
                }
            }
            return resinfos.ToArray();
        }

        public static string[] HListSiteID(this IList<Site> sites)
        {
            HashSet<string> IDs = new HashSet<string>();
            foreach(var site in sites)
                IDs.Add(site.siteID);
            return IDs.ToArray();
        }
        public static Site[] HSelectBySiteID(this IList<Site> sites, string siteID)
        {
            Tuple<int,Site>[] sele = new Tuple<int,Site>[0];
            foreach(var site in sites)
            {
                if(siteID != site.siteID)
                    continue;
                sele = sele.HAdd(new Tuple<int, Site>(site.seqNum, site));
            }
            int[] idxSorted = sele.HListItem1().HIdxSorted();
            sele = sele.HSelectByIndex(idxSorted);
            return sele.HListItem2().ToArray();
        }
        public static PPdbSiteInfo[] GetSiteInfos(this IList<Site> sites)
        {
            /// SITE     1 AC1  3 HIS A  94  HIS A  96  HIS A 119                               
            /// SITE     1 AC2  5 ASN A  62  GLY A  63  HIS A  64  HOH A 328                    
            /// SITE     2 AC2  5 HOH A 634                                                     
            //HDebug.ToDo();
            PPdbSiteInfo[] siteinfos = new PPdbSiteInfo[0];
            string[] IDs = sites.HListSiteID();
            foreach(var ID in IDs)
            {
                Site[] lsites = sites.HSelectBySiteID(ID);
                ResInfo[] resinfos = new ResInfo[0];
                foreach(Site lsite in lsites)
                {
                    HDebug.Assert(lsites[0].numRes == lsite.numRes);
                    if(lsite.resName1.Trim().Length != 0) resinfos=resinfos.HAdd(new ResInfo{resName=lsite.resName1, chainID=lsite.chainID1, resSeq=lsite.seq1, iCode=lsite.iCode1});
                    if(lsite.resName2.Trim().Length != 0) resinfos=resinfos.HAdd(new ResInfo{resName=lsite.resName2, chainID=lsite.chainID2, resSeq=lsite.seq2, iCode=lsite.iCode2});
                    if(lsite.resName3.Trim().Length != 0) resinfos=resinfos.HAdd(new ResInfo{resName=lsite.resName3, chainID=lsite.chainID3, resSeq=lsite.seq3, iCode=lsite.iCode3});
                    if(lsite.resName4.Trim().Length != 0) resinfos=resinfos.HAdd(new ResInfo{resName=lsite.resName4, chainID=lsite.chainID4, resSeq=lsite.seq4, iCode=lsite.iCode4});
                }
                HDebug.Assert(lsites[0].numRes == resinfos.Length);

                siteinfos = siteinfos.HAdd(new PPdbSiteInfo
                {
                    siteID   = ID,
                    resinfos = resinfos
                });
            }

            return siteinfos;
        }
        public static PPdbSiteInfo[] GetSiteInfos(this Pdb pdb)
        {
            return pdb.sites.GetSiteInfos();
        }
    }
}
