﻿using System;
using System.Collections.Generic;
using TdqqClient.Models;
using TdqqClient.Services.Database;

namespace TdqqClient.Services.Export
{
    /// <summary>
    /// 导出的基类
    /// </summary>
    class ExportBase
    {
        protected  string PersonDatabase;
        protected  string SelectFeature;
        protected  string BasicDatabase;
        private IDatabaseService _pDatabaseService;


        public ExportBase(string personDatabase, string selectFeature, string basicDatabase)
        {
            PersonDatabase = personDatabase;
            SelectFeature = selectFeature;
            BasicDatabase = basicDatabase;
        }

       

        #region 承包方信息
        /// <summary>
        /// 获取所有承包方信息
        /// </summary>
        /// <param name="isValid">是否有效</param>
        /// <returns>承包方集合</returns>
        protected List<CbfModel> Cbfs(bool isValid)
        {
            var cbfModels = new List<CbfModel>();
            var farmers = Cbfbms(isValid);
            _pDatabaseService = new MsAccessDatabase(BasicDatabase);
            foreach (var farmer in farmers)
            {
                var cbfModel = Cbf(farmer.Cbfbm);
                cbfModel.Cbfbm = farmer.Cbfbm;
                cbfModel.Cbfmc = farmer.Cbfmc;
                cbfModels.Add(cbfModel);
            }
            return cbfModels;
        }


        /// <summary>
        /// 获取单个承包信息
        /// </summary>
        /// <param name="cbfbm">承包方编码</param>
        /// <returns>承包方对象</returns>
        protected CbfModel Cbf(string cbfbm)
        {
            var sqlString =
                    string.Format(
                        "Select CBFLX,CYXB,CBFZJLX,CBFZJHM,CBFDZ,YZBM,LXDH,CBFCYSL From {0} where CBFBM='{1}' ",
                        "CBF", cbfbm);
            _pDatabaseService = new MsAccessDatabase(BasicDatabase);
            var dt = _pDatabaseService.Query(sqlString);
            if (dt == null || dt.Rows.Count != 1) return new CbfModel();
            return new CbfModel()
            {
                Cbflx = dt.Rows[0][0].ToString().Trim(),
                Cyxb = dt.Rows[0][1].ToString().Trim(),
                Cbfzjlx = dt.Rows[0][2].ToString().Trim(),
                Cbfzjhm = dt.Rows[0][3].ToString().Trim(),
                Cbfdz = dt.Rows[0][4].ToString().Trim(),
                Yzbm = dt.Rows[0][5].ToString().Trim(),
                Lxdh = dt.Rows[0][6].ToString().Trim(),
                Cbfcysl = Convert.ToInt32(dt.Rows[0][7].ToString().Trim())
            };

        }
        #endregion

        #region 其他信息
        /// <summary>
        /// 根据承包方编码获取该农户的家庭成员信息
        /// </summary>
        /// <param name="cbfbm">承包方编码</param>
        /// <returns>返回所有家庭成员信息，如果未找到返回null</returns>
        protected List<CbfjtcyModel> Cbfjtcys(string cbfbm)
        {
           _pDatabaseService=new MsAccessDatabase(BasicDatabase);
            var sqlString =
                string.Format("Select CBFBM,CBFMC,CYXB,CYXM,CYZJHM,CYZJLX,CYBZ,YHZGX,CYSZC,YZBM,LXDH,SFGYR" +
                              "From {0} where CBFBM='{1}' order by YHZGX", "CBF_JTCY", cbfbm);
            var dt = _pDatabaseService.Query(sqlString);
            if (dt == null || dt.Rows.Count == 0) return null;
            List<CbfjtcyModel> cbfjtcys=new List<CbfjtcyModel>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cbfjtcys.Add(new CbfjtcyModel()
                {
                    Cbfbm = dt.Rows[i][0].ToString().Trim(),
                    Cbfmc = dt.Rows[i][1].ToString().Trim(),
                    Cyxb = dt.Rows[i][2].ToString().Trim(),
                    Cyxm = dt.Rows[i][3].ToString().Trim(),
                    Cyzjhm = dt.Rows[i][4].ToString().Trim(),
                    Cyzjlx = dt.Rows[i][5].ToString().Trim(),
                    Cybz = dt.Rows[i][6].ToString().Trim(),
                    Yhzgx = dt.Rows[i][7].ToString().Trim(),
                    Cyszc = dt.Rows[i][8].ToString().Trim(),
                    Lxdh = dt.Rows[i][9].ToString().Trim(),
                    Sfgyr = dt.Rows[i][10].ToString().Trim(),
                });
            }
            return cbfjtcys;
        }

        /// <summary>
        /// 获取发包方信息
        /// </summary>
        /// <returns></returns>
        protected FbfModel Fbf()
        {
            _pDatabaseService=new MsAccessDatabase(BasicDatabase);
            var sqlString =
                string.Format("Select FBFMC,FBFBM,FBFFZRXM,FZRZJLX,FZRZJHM,LXDH,FBFDZ,YZBM,FBFDCY,FBFDCRQ,FBFDCJS " +
                              "From FBF");
            var dt = _pDatabaseService.Query(sqlString);
            if (dt==null||dt.Rows.Count!=1)return null;
            return new FbfModel()
            {
                Fbfmc = dt.Rows[0][0].ToString().Trim(),
                Fbfbm = dt.Rows[0][1].ToString().Trim(),
                Fbffzrxm = dt.Rows[0][2].ToString().Trim(),
                Fzrzjlx = dt.Rows[0][3].ToString().Trim(),
                Fzrzjhm = dt.Rows[0][4].ToString().Trim(),
                Lxdh = dt.Rows[0][5].ToString().Trim(),
                Fbfdz = dt.Rows[0][6].ToString().Trim(),
                Yzbm = dt.Rows[0][7].ToString().Trim(),
                Fbfdcy = dt.Rows[0][8].ToString().Trim(),
                Fbfdcrq = string.IsNullOrEmpty(dt.Rows[0][9].ToString().Trim()) ?
                    DateTime.Now : Convert.ToDateTime(dt.Rows[0][9].ToString().Trim()),
                Fbfdcjs = dt.Rows[0][10].ToString().Trim()
            };
        }

        /// <summary>
        /// 获取调查，审核相关信息
        /// </summary>
        /// <returns></returns>
        protected DcShModel DcSh()
        {
            _pDatabaseService=new MsAccessDatabase(BasicDatabase);
            var sqlString = string.Format("Select distinct CBFDCRQ,CBFDCY,CBFDCJS,GSJS,GSJSR,GSSHRQ,GSSHR From CBF");
            var dt = _pDatabaseService.Query(sqlString);
            if (dt == null || dt.Rows.Count == 0) return null;
            return new DcShModel()
            {
                Cbfdcrq = string.IsNullOrEmpty(dt.Rows[0][0].ToString().Trim())
                    ? DateTime.Now : Convert.ToDateTime(dt.Rows[0][0].ToString().Trim()),
                Cbfdcy = dt.Rows[0][1].ToString().Trim(),
                Cbfdcjs = dt.Rows[0][2].ToString().Trim(),
                Gsjs = dt.Rows[0][3].ToString().Trim(),
                Gsjsr = dt.Rows[0][4].ToString().Trim(),
                Gsshrq = string.IsNullOrEmpty(dt.Rows[0][5].ToString().Trim())?DateTime.Now:Convert.ToDateTime(dt.Rows[0][5].ToString().Trim()),
                Gsshr = dt.Rows[0][6].ToString().Trim()                
            };
        }

        protected List<FieldModel> Fields(string cbfbm)
        {
            _pDatabaseService=new MsAccessDatabase(PersonDatabase);
            var sqlString =
                string.Format(
                    "Select OBJECTID, DKMC,CBFMC,DKBM,SCMJ,HTMJ,DKDZ,DKNZ,DKXZ,DKBZ,YHTMJ,DKBZXX,ZJRXM,CBFBM,SYQXZ,DKLB,DLDJ" +
                    "TDYT,SFJBNT From {0} where CBFBM='{1}'", SelectFeature, cbfbm);
            var dt = _pDatabaseService.Query(sqlString);
            if (dt == null || dt.Rows.Count == 0) return null;
            List<FieldModel> fields=new List<FieldModel>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                fields.Add(new FieldModel()
                {
                    ObjectId = dt.Rows[i][0],
                    Dkmc = dt.Rows[i][1].ToString().Trim(),
                    Cbfmc = dt.Rows[i][2].ToString().Trim(),
                    Dkbm = dt.Rows[i][3].ToString().Trim(),
                    Scmj = string.IsNullOrEmpty(dt.Rows[i][4].ToString().Trim())
                        ?0.0:Convert.ToDouble(dt.Rows[i][4].ToString().Trim()),
                    Htmj = string.IsNullOrEmpty(dt.Rows[i][5].ToString().Trim())
                        ? 0.0 : Convert.ToDouble(dt.Rows[i][5].ToString().Trim()),
                    Dkdz = dt.Rows[i][6].ToString().Trim(),
                    Dknz = dt.Rows[i][7].ToString().Trim(),
                    Dkxz = dt.Rows[i][8].ToString().Trim(),
                    Dkbz = dt.Rows[i][9].ToString().Trim(),
                    Yhtmj = string.IsNullOrEmpty(dt.Rows[i][10].ToString().Trim())
                        ? 0.0 : Convert.ToDouble(dt.Rows[i][10].ToString().Trim()),
                    Dkbzxx = dt.Rows[i][11].ToString().Trim(),
                    Zjrxm = dt.Rows[i][12].ToString().Trim(),
                    Cbfbm = dt.Rows[i][13].ToString().Trim(),
                    Syqxz = dt.Rows[i][14].ToString().Trim(),
                    Dklb = dt.Rows[i][15].ToString().Trim(),
                    Dldj = dt.Rows[i][16].ToString().Trim(),
                    Tdyt = dt.Rows[i][17].ToString().Trim(),
                    Sfjbnt = dt.Rows[i][18].ToString().Trim()
                });
            }
            return fields;
        } 
        #endregion

        /// <summary>
        /// 获取承包方编码
        /// </summary>
        /// <param name="isValid">是否为9999开头</param>
        /// <returns>承包编码集合</returns>
        private IEnumerable<FarmerModel> Cbfbms(bool isValid)
        {
            _pDatabaseService=new MsAccessDatabase(PersonDatabase);
            string sqlString;
            if (isValid)
            {
                sqlString = string.Format("Select Distinct CBFBM,CBFMC From {0} Where CBFBM Not Like {1} order by CBFBM ",
                    SelectFeature, "99999999999999%");
            }
            else
            {
                sqlString = string.Format("Select Distinct CBFBM,CBFMC From {0} order by CBFBM ", SelectFeature); 
            }            
            var dt = _pDatabaseService.Query(sqlString);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                yield return new FarmerModel()
                {
                    Cbfbm = dt.Rows[0].ToString().Trim(),
                    Cbfmc = dt.Rows[1].ToString().Trim()
                };
            }
        }


    }
}