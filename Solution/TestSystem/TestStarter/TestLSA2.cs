﻿using System.Collections.Generic;
using System.Xml;
using Solution;

namespace TestSystem
{
    public class CTestInfoLSA : CTestInfo
    {
        public List<IPermutation> m_aPerm;
        public CTestInfoLSA(string problem, string resultPath = "") : base(problem, resultPath) { m_aPerm = new List<IPermutation>(); }
    }

    public class CTestLSA2 : ATest
    {
        string m_curOpt;
        public CTestLSA2(string path, int count, bool bm_log) : base(path, count, bm_log) { m_curOpt = ""; }

        protected override IOptions GetOptionsAlg(string path) => new CLocalSearchAlgorithm.Options(path);
        protected override string GetAlgName() => CLocalSearchAlgorithm.Name(true);
        protected override ITestInfo CreateTestInfo(string problem, string result) => new CTestInfoLSA(problem, result);
        protected override void InitLogger()
        {
            // create logger
            string pathLog = m_path + "logs\\";
            m_log = new CLogger(pathLog, $"{m_xmlName}_{GetAlgName()}_{m_curOpt}");

            // create tabler
            string pathTable = m_path + "results\\";
            string pathTemplate = m_path + "_template.xml";
            m_tbl = new CTablerExcel(pathTable, $"{m_xmlName}_{GetAlgName()}_{m_curOpt}", pathTemplate);
        }

        public override void Start()
        {
            Init();
            System.Console.WriteLine("Total test " + m_aTest.Count);
            List<IPermutation> aPermToTest = new List<IPermutation>();
            for(int iOpt = 0; iOpt < m_aOptions.Count; iOpt++)
            {
                m_curOpt = iOpt == 0 ? "A" : "B";
                InitLogger();
                var curOption = m_aOptions[iOpt];

                m_log.Msg($"Option {curOption.Name()} start", true);
                CTimer timer = new CTimer();
                var aOptStat = new List<CTestStatistic>();
                aOptStat.Add(new CTestStatistic("Avg Error, %", 5));
                aOptStat.Add(new CTestStatistic("Avg timer, %", 2));
                aOptStat.Add(new CTestStatistic("Avg cacl count, %", 3));
                
                if(m_nCount == 1)
                    m_tbl.AddRow().AddCells(CTablerExcel.Styles.eStyleSimpleBold, "Name problem", "Timer, ms", "Calc count", "Error", "Error, %", "Result", "Optimal" ,"Worst");
                foreach(ITestInfo test in m_aTest)
                {
                    m_log.Msg($"Test {test.Name()} started", true);
                    string timeLoad = timer.Stop().ToString();
                    long examVal = 0;
                    bool bExam = test.Exam(ref examVal);
                    if(!m_problem.Deserialize(test.pathProblem))
                        continue;
                    var rowHeader = m_tbl.AddRow();
                    if(m_nCount > 1)
                    {
                        rowHeader.AddCells(CTablerExcel.Styles.eStyleSimpleBold, "Name problem", test.Name(), $"Size: {m_problem.Size()}", $"Load time: {timeLoad}", "Optimal:", bExam ? examVal.ToString() : "");
                        var rowSec = m_tbl.AddRow();
                        rowSec.AddCells(CTablerExcel.Styles.eStyleSimpleBold, "Iteration", "Timer, ms", "Calc count", "Error", "Error, %", "Result");
                    }
                    IDelayedRow row = new CDelayedRow(m_tbl);
                    for(int i = 0; i < m_nCount; i++)
                    {
                        timer.Reset();
                        IAlgorithm ALG = new CLocalSearchAlgorithm(m_problem);
                        EnableLog(m_problem, ALG);
                        timer.Reset();

                        // use single permutation for one test in all options
                        {
                            CTestInfoLSA t = (CTestInfoLSA)test;
                            if(t.m_aPerm.Count <= i)
                                t.m_aPerm.Add(m_problem.GetRandomPermutation());
                            IPermutation p = t.m_aPerm[i];
                            ((CLocalSearchAlgorithm.Options)curOption).m_p = p.Clone();
                        }

                        // start alg with permutation from test
                        ALG.Start(curOption);

                        long timerAlg = timer.Stop();
                        long calcCount = ALG.GetCalcCount();
                        long curRes = ALG.GetResultValue();
                        long resultValue = curRes;
                        long resultBest = 0;
                        if(resultBest == 0 || resultBest > curRes)
                            resultBest = curRes;

                        m_log.Msg($"Problem {test.Name()}; iteration: {i} done", true);
                        m_log.Msg($"Problem {test.Name()}; iteration: {i}, log:{ALG})");
                        if(m_nCount == 1)
                        {
                            if(bExam)
                            {
                                string errStr = $"=RC6-R{rowHeader.GetIndex()}C";
                                string errPersentStr = $"=100*(RC6-R{rowHeader.GetIndex()}C)/(=R{rowHeader.GetIndex()}C[1]-R{rowHeader.GetIndex()}C)";
                                long nRow = row.AddRow(resultValue, test.Name(), timerAlg.ToString(), calcCount.ToString(), errStr, errPersentStr, resultValue.ToString(), examVal.ToString());
                                if(aOptStat != null)
                                {
                                    foreach(var optStat in aOptStat)
                                        optStat.AddStat("-", m_problem.Size(), nRow);
                                }
                            }
                            else
                                row.AddRow(-1, i.ToString(), timerAlg.ToString(), calcCount.ToString(), "-", "-", resultValue.ToString(), "-");
                        }
                        else
                        {
                            string errStr = $"=RC6-R{rowHeader.GetIndex()}C6";
                            string errPersentStr = $"=100*(RC6-R{rowHeader.GetIndex()}C6)/(R{rowHeader.GetIndex()}C[1]-R{rowHeader.GetIndex()}C6)";
                            long nRow = row.AddRow(resultValue,i.ToString(), timerAlg.ToString(), calcCount.ToString(), errStr, errPersentStr, resultValue.ToString());
                            if(aOptStat != null)
                            {
                                foreach(var optStat in aOptStat)
                                    optStat.AddStat("-", m_problem.Size(), nRow);
                            }
                        }
                    }
                    row.Release();
                    if(m_nCount > 1)
                    {
                        m_tbl.AddRow();
                        m_tbl.AddRow();
                    }
                }
                foreach(var optStat in aOptStat)
                    optStat.ReleaseOptStat(m_tbl);
                m_log.Close();
                m_tbl.Close();
            }
        }
    }
}